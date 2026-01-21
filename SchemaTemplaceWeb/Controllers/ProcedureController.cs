using Microsoft.AspNetCore.Mvc;
using SchemaTemplateLib.Interfaces;

namespace SchemaTemplaceWeb.Controllers
{
    public class ProcedureController : Controller
    {
        private readonly IExposeMethods _exposeMethods;

        public ProcedureController(IExposeMethods exposeMethods)
        {
            _exposeMethods = exposeMethods;
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Execute(string procedureName)
        {
            if (string.IsNullOrEmpty(procedureName))
            {
                return BadRequest("Procedure Name is missing.");
            }

            // Extract Procedure Parameters if it's a POST request with Form data
            var parameterValues = new Dictionary<string, string>();
            if (Request.HasFormContentType)
            {
                foreach (var key in Request.Form.Keys)
                {
                    if (key != "ProcedureName" && key != "__RequestVerificationToken")
                    {
                        parameterValues[key] = Request.Form[key]!;
                    }
                }
            }

            var (stream, fileName) = _exposeMethods.GenerateExcelTemplate(procedureName, parameterValues);

			// Handle cases where no data was found
			if (stream == null)
			{
				return NotFound("No data found for the given procedure.");
			}
			// Return the file stream. The browser will handle the "Download" behavior.
			return File(
				stream,
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				fileName
			);
        }
    }
}
