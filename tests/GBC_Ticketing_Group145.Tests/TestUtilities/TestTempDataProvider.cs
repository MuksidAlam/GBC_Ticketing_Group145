using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GBC_Ticketing_Group145.Tests.TestUtilities
{
    // Minimal ITempDataProvider used for tests to avoid null TempData
    public class TestTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(Microsoft.AspNetCore.Http.HttpContext context)
        {
            return new Dictionary<string, object>();
        }

        public void SaveTempData(Microsoft.AspNetCore.Http.HttpContext context, IDictionary<string, object> values)
        {
            // No-op
        }
    }
}
