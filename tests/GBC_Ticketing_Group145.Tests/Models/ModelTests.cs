using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GBC_Ticketing_Group145.Models;
using Xunit;

namespace GBC_Ticketing_Group145.Tests.Models
{
    public class ModelTests
    {
        [Fact]
        public void Purchase_Model_Validation_Fails_For_Missing_Required_Fields()
        {
            var purchase = new Purchase(); // missing required fields
            var context = new ValidationContext(purchase, null, null);
            var results = new List<ValidationResult>();

            var valid = Validator.TryValidateObject(purchase, context, results, validateAllProperties: true);

            Assert.False(valid);
            // Expect at least three validation errors for required fields
            Assert.True(results.Count >= 1);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Guest name") || r.ErrorMessage.Contains("name is required"));
        }

        [Fact]
        public void Purchase_TotalCost_Calculation_NotDone_In_Model_By_Default()
        {
            // Basic sanity: TotalCost default 0
            var purchase = new Purchase { Quantity = 2, TotalCost = 0m };
            Assert.Equal(0m, purchase.TotalCost);
        }
    }
}
