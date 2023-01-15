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
            switch (statuscode)
            {
                case 404:
                    ViewBag.Error = "Can't find what you are asking for";
                    break;
                default:
                    break;
            }
            
            return View("NotFound");
        }
    }
}
