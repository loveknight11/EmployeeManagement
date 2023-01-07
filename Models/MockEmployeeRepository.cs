using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        List<Employee> _employeeList = new List<Employee>();
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>() { 
            new Employee(){ Department = Dept.IT, Email = "a@a.a", Id = 1, Name = "a"},
            new Employee(){ Department = Dept.HR, Email = "b@bb", Id = 2, Name = "b"},
            new Employee(){ Department = Dept.IT, Email = "c@bb", Id = 3, Name = "c"},
            new Employee(){ Department = Dept.HR, Email = "d@bb", Id = 4, Name = "d"}
            };
        }

        public Employee AddEmployee(Employee employee)
        {
            employee.Id = _employeeList.Max(x => x.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public List<Employee> GetAllEmployees()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(x => x.Id == Id);
        }
    }
}
