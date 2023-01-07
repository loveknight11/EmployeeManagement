using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
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
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee = _employeeRepository.AddEmployee(employee);
                return RedirectToAction("Details", new { id = employee.Id });
            }
            return View();
        }
    }
}
