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
            //throw new Exception("Exception in details");
            Employee employee = _employeeRepository.GetEmployee(id);
            if (employee == null)
            {
                return View("EmployeeNotFound",id);
            }
            return View(employee);
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
                    uniqueName = UploadNewPhoto(model);
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

        [HttpGet]
        public ViewResult Edit(int Id)
        {
            Employee employee = _employeeRepository.GetEmployee(Id);
            EditViewModel editViewModel = new EditViewModel
            {
                Id = employee.Id,
                Department = employee.Department,
                Email = employee.Email,
                Name = employee.Name,
                PhotoPath = employee.PhotoPath
            };
            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Department = model.Department;
                employee.Email = model.Email;
                employee.Name = model.Name;
                if (model.Photo != null)
                {
                    employee.PhotoPath = UploadNewPhoto(model);
                    if (model.PhotoPath != null)
                    {
                        System.IO.File.Delete(Path.Combine(_env.WebRootPath, "Images", model.PhotoPath));
                    }
                }
                

                employee = _employeeRepository.Update(employee);
                return RedirectToAction("Details", new { id = employee.Id });
            }
            return View();
        }

        private string UploadNewPhoto(CreateViewModel model)
        {
            string uniqueName;
            string serverPath = Path.Combine(_env.WebRootPath, "Images");
            uniqueName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(serverPath, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(stream);
            }
            
            return uniqueName;
        }
    }
}
