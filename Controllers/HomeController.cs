using EmployeeManagement.Models;
using EmployeeManagement.ViewModels.Home;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private IWebHostEnvironment _env;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment env)
        {
            _employeeRepository = employeeRepository;
            _env = env;
        }
        public ViewResult Details(int id)
        {
            return View(_employeeRepository.GetEmployee(id));
        }

        public ViewResult Index()
        {
            return View(_employeeRepository.GetAllEmployees());
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueName = null;
                if (model.Photo != null)
                {
                    string serverPath = Path.Combine(_env.WebRootPath, "Images");
                    uniqueName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(serverPath, uniqueName);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Employee employee = new Employee() { 
                Department = model.Department,
                Email = model.Email,
                Name = model.Name,
                PhotoPath = uniqueName
                };
                employee = _employeeRepository.AddEmployee(employee);
                return RedirectToAction("Details", new { id = employee.Id });
            }
            return View();
        }
    }
}
