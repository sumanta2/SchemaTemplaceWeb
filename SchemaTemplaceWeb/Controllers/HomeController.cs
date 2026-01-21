using Microsoft.AspNetCore.Mvc;
using SchemaTemplaceWeb.Models;
using SchemaTemplateLib.DataModel;
using SchemaTemplateLib.Interfaces;
using System.Diagnostics;

namespace SchemaTemplaceWeb.Controllers
{
    public class HomeController : Controller
    {

        private readonly IExposeMethods _exposeMethdos;

        public HomeController(IExposeMethods exposeMethods)
        {
            _exposeMethdos = exposeMethods;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string procedureName)
        {
			// Fetch validation parameters for the procedure
			List<ProcedureParam> parameters = _exposeMethdos.GetProcedureParams(procedureName);

            // If no parameters are required, generate the Excel directly
            if (parameters == null || parameters.Count == 0)
            {
                // Redirect to the Procedure Controller to handle the execution
                return RedirectToAction("Execute", "Procedure", new { procedureName = procedureName });
            }

            var model = new ProcedureInputViewModel
            {
                ProcedureName = procedureName,
                Parameters = parameters
            };

            // Navigate to the parameter input view
            return View("ProcedureInputs", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
