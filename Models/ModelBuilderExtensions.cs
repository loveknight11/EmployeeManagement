using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Department = Dept.HR,
                    Email = "2@2.com",
                    Id = 2,
                    Name = "2"
                },
                new Employee
                {
                    Department = Dept.IT,
                    Email = "1@1.com",
                    Id = 1,
                    Name = "1"
                },
                new Employee
                {
                    Department = Dept.Payroll,
                    Email = "3@3.com",
                    Id = 3,
                    Name = "3"
                }
                );
        }
    }
}
