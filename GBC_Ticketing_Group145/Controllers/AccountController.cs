using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using GBC_Ticketing_Group145.Models;
using GBC_Ticketing_Group145.ViewModels;
using System.Text.Encodings.Web;

namespace GBC_Ticketing_Group145.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailSender _emailSender;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Event");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign selected role (only allow Attendee or Organizer for registration)
                var roleToAssign = model.SelectedRole;
                if (roleToAssign != Roles.Attendee && roleToAssign != Roles.Organizer)
                {
                    roleToAssign = Roles.Attendee; // Default to Attendee if invalid role selected
                }

                await _userManager.AddToRoleAsync(user, roleToAssign);

                _logger.LogInformation("User {Email} registered successfully with role {Role}", model.Email, roleToAssign);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Event");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Event");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        returnUrl ??= Url.Action("Index", "Event") ?? "/Event";

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Update last login time
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }

                _logger.LogInformation("User {Email} logged in", model.Email);
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            // Don't reveal that the user does not exist or is not confirmed (security best practice)
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Create the password reset callback URL
            var callbackUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new { token = token, email = model.Email },
                protocol: Request.Scheme);

            // Send email with the reset link
            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Your Password - GBC Ticketing",
                GeneratePasswordResetEmailHtml(user.FirstName, callbackUrl!));

            _logger.LogInformation("Password reset email sent to {Email}", model.Email);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token = null, string? email = null)
    {
        if (token == null || email == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid password reset token.");
        }

        var model = new ResetPasswordViewModel 
        { 
            Token = token ?? string.Empty,
            Email = email ?? string.Empty
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successful for {Email}", model.Email);
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    /// <summary>
    /// Generates a professional HTML email for password reset
    /// </summary>
    private string GeneratePasswordResetEmailHtml(string firstName, string resetUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reset Your Password</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td align=""center"" style=""padding: 40px 0;"">
                <table role=""presentation"" style=""width: 600px; border-collapse: collapse; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;"">Password Reset Request</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                Hello {HtmlEncoder.Default.Encode(firstName)},
                            </p>
                            
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                We received a request to reset your password for your GBC Ticketing account. If you didn't make this request, you can safely ignore this email.
                            </p>
                            
                            <p style=""color: #333333; font-size: 16px; line-height: 1.6; margin: 0 0 30px 0;"">
                                To reset your password, click the button below:
                            </p>
                            
                            <!-- Button -->
                            <table role=""presentation"" style=""margin: 0 auto;"">
                                <tr>
                                    <td style=""border-radius: 4px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);"">
                                        <a href=""{HtmlEncoder.Default.Encode(resetUrl)}"" 
                                           style=""display: inline-block; padding: 16px 40px; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 16px;"">
                                            Reset Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""color: #666666; font-size: 14px; line-height: 1.6; margin: 30px 0 20px 0;"">
                                Or copy and paste this link into your browser:
                            </p>
                            
                            <p style=""color: #667eea; font-size: 14px; line-height: 1.6; margin: 0 0 30px 0; word-break: break-all;"">
                                {HtmlEncoder.Default.Encode(resetUrl)}
                            </p>
                            
                            <div style=""border-top: 1px solid #eeeeee; padding-top: 20px; margin-top: 30px;"">
                                <p style=""color: #999999; font-size: 13px; line-height: 1.6; margin: 0;"">
                                    <strong>Security Note:</strong> This password reset link will expire in 24 hours for security reasons. If you didn't request this password reset, please contact our support team immediately.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-top: 1px solid #eeeeee;"">
                            <p style=""color: #666666; font-size: 14px; line-height: 1.6; margin: 0 0 10px 0;"">
                                Thank you for using GBC Ticketing!
                            </p>
                            <p style=""color: #999999; font-size: 12px; line-height: 1.6; margin: 0;"">
                                © {DateTime.Now.Year} GBC Ticketing. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
