﻿using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni

{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var dataContext = new SoftUniContext();
            var result = RemoveTown(dataContext);
            Console.WriteLine(result);
        }

        // 3.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var emploeeInfo = context.Employees
                .Select(e => new 
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary,
                })
                .OrderBy(e => e.EmployeeId)
                .ToList();
            //Guy Gilbert R Production Technician 12500.00
            StringBuilder sb = new StringBuilder();
            foreach (var employee in emploeeInfo)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            var result = sb.ToString().TrimEnd();
            
            return result;
        }

        // 4.Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary,
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();
            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 5.Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary,
                    DepartmentName = e.Department.Name,
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName);

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 6.Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Employee nakov = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            if (nakov != null) 
            {
                nakov.Address = new Address
                {
                    AddressText = "Vitoshka 15",
                    TownId = 4,
                };
            }

            context.SaveChanges();

            var addresses = context.Employees
                .Select(e => new 
                { 
                    e.Address.AddressText, 
                    e.Address.AddressId, 
                })
                .OrderByDescending(e => e.AddressId)
                .Take(10);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var currentAddress in addresses)
            {
                stringBuilder.AppendLine($"{currentAddress.AddressText}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        // 7.Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                 .Include(e => e.EmployeesProjects)
                 .ThenInclude(e => e.Project)
                 .Where(e => e.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001 
                                                       && p.Project.StartDate.Year <= 2003))
                 .Select(e => new
                 {
                     EmployeeFirstName = e.FirstName,
                     EmployeeLastName = e.LastName,
                     ManagerFirstName = e.Manager.FirstName,
                     ManagerLastName = e.Manager.LastName,
                     Projects = e.EmployeesProjects
                                 .Select(p => new 
                                 { 
                                     ProjectName = p.Project.Name,
                                     EndDate = p.Project.EndDate,
                                     StartDate = p.Project.StartDate,
                                 })
                 })
                 .Take(10)
                 .ToList();

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName}" +
                    $" - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");
                foreach (var project in employee.Projects)
                {
                    var endDate = project.EndDate.HasValue 
                        ? project.EndDate.Value
                        .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) 
                        : "not finished";
                    sb.AppendLine($"--{project.ProjectName} - " +
                        $"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}" +
                        $" - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // 8.Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(a => new 
                {
                    EmployeeCount = a.Employees.Count(),
                    EmployeeTownName = a.Town.Name,
                    a.AddressText,
                })
                .OrderByDescending(a => a.EmployeeCount)
                .ThenBy(a => a.EmployeeTownName)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.EmployeeTownName} - {address.EmployeeCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        // 09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Select(e => new Employee
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    EmployeesProjects = e.EmployeesProjects.Select(p => new EmployeeProject
                    {
                        Project = p.Project
                    })
                    .OrderBy(e => e.Project.Name)
                    .ToList(),
                })
                .FirstOrDefault(e => e.EmployeeId == 147);

            var sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            foreach (var project in employee.EmployeesProjects)
            {
                sb.AppendLine($"{project.Project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        // 10.Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    Name = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .Select(e => new Employee
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        JobTitle = e.JobTitle,
                    })
                })
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .ToList();

            var sb = new StringBuilder();
            foreach (var department in departments)
            {
                sb.AppendLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastName}");
                foreach (var employee in department.Employees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // 11.Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate,
                })
                .OrderBy(p => p.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        // 12.Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            string[] departmentsToIncreaseSalary = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };
            var employees = context.Employees
                .Where(e => departmentsToIncreaseSalary.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        // 13.Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => EF.Functions.Like(e.FirstName, "sa%"))
                .Select(e => new 
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        // 14.Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToFind = context.Projects.Find(2);
            var employeeProject = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2);

            context.EmployeesProjects
                .ToList()
                .ForEach(ep => context.EmployeesProjects.Remove(ep));

            context.Projects.Remove(projectToFind);

            context.SaveChanges();

            var projects = context.Projects.Take(10).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        // 15.Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .Include(x => x.Addresses)
                .FirstOrDefault(x => x.Name == "Seattle");

            var allAddressIds = context.Addresses
                .Select(x => x.AddressId)
                .ToList();

            var employees = context.Employees
                .Where(x => x.AddressId.HasValue && allAddressIds.Contains(x.AddressId.Value))
                .ToList();

            foreach (var employee in employees)
            {
                employee.AddressId = null;
            }

            foreach (var addressId in allAddressIds)
            {
                var address = context.Addresses
                    .FirstOrDefault(x => x.AddressId == addressId);

                context.Addresses.Remove(address);
            }

            context.SaveChanges();

            return $"{allAddressIds.Count} addresses in Seattle were deleted";
        }
    }
}
