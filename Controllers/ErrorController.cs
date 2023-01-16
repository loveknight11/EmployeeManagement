using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("/error/{statuscode}")]
        public IActionResult Index(int statuscode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statuscode)
            {

                case 404:
                    ViewBag.Error = "Can't find what you are asking for";
                    ViewBag.Qs = statusCodeResult.OriginalQueryString;
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    logger.LogWarning($"Path error : {statusCodeResult.OriginalPath} " +
                        $"and Query String Error : {statusCodeResult.OriginalQueryString}");
                    break;
                default:
                    break;
            }

            return View("NotFound");
        }

        [Route("error")]
        public IActionResult ExceptionHandler()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //log exception
            logger.LogError($"*********** Error in {ex.Path} caused error : {ex.Error} **************");
            return View("NotFound");
        }
    }
}
