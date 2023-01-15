using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
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
            return View("NotFound");
        }
    }
}
