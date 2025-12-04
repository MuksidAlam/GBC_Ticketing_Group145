using QRCoder;

namespace GBC_Ticketing_Group145.Services
{
    public interface IQRCodeService
    {
        string GenerateQRCode(string data);
    }

    public class QRCodeService : IQRCodeService
    {
        public string GenerateQRCode(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return Convert.ToBase64String(qrCodeImage);
        }
    }
}
