using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccessLayer;

namespace CRUD.Models
{
    public class EmployeeStore
    {
        public static List<EmployeeVM> FetchEmployees()
        {
            var departments = DataRepository.DemoEntitiesContext.Departments.ToList();
            var employees = DataRepository.DemoEntitiesContext.Employees.ToList();
            var employeeList = new List<EmployeeVM>();
            foreach (var employee in employees)
            {
                var emp = new EmployeeVM()
                {
                    EmpId = employee.Id,
                    Name = employee.Name,
                    Salary = employee.Salary
                };
                if (employee.EmployeeDepartments.Any())
                {
                    var depId = employee.EmployeeDepartments.First().DepartmentId;
                    var departmentName = depId != 0 ? departments.First(dep => dep.Id == depId).Name : string.Empty;
                    emp.DepartmentId = depId;
                    emp.DepartmentName = departmentName;
                }
                employeeList.Add(emp);
            }
            return employeeList;
        }

        /// <summary>
        /// Add an Employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static bool AddEmployee(EmployeeVM employee)
        {
            try
            {
                var newEmployee = new Employee()
                {
                    Name = employee.Name,
                    Salary = employee.Salary,
                };

                DataRepository.DemoEntitiesContext.Employees.Add(newEmployee);
                DataRepository.DemoEntitiesContext.SaveChanges();

                if (employee.DepartmentId != 0)
                {
                    DataRepository.DemoEntitiesContext.EmployeeDepartments.Add(new EmployeeDepartment()
                    {
                        EmployeeId = newEmployee.Id, //##Great feature 
                        DepartmentId = employee.DepartmentId
                    });
                    return DataRepository.DemoEntitiesContext.SaveChanges() > 0;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        /// <summary>
        /// Update an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static bool SetEmployee(EmployeeVM employee)
        {
            try
            {
                if (DataRepository.DemoEntitiesContext.Employees != null && DataRepository.DemoEntitiesContext.Employees.ToList().Any(e => e.Id == employee.EmpId))
                {
                    Employee emp = DataRepository.DemoEntitiesContext.Employees.First(e => e.Id == employee.EmpId);
                    emp.Name = employee.Name;
                    emp.Salary = employee.Salary;

                    if (DataRepository.DemoEntitiesContext.EmployeeDepartments.Any(e => e.EmployeeId == employee.EmpId))
                    {
                        var associatedDepartment =
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.First(
                                e => e.EmployeeId == employee.EmpId);
                        //If the selected departmentid is 0, then delete the Association else establish the updated one.
                        if (employee.DepartmentId == 0)
                        {
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.Remove(associatedDepartment);
                        }
                        else
                        {
                            associatedDepartment.DepartmentId = employee.DepartmentId;
                        }

                    }
                    else
                    {
                        if (employee.DepartmentId != 0)
                        {
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.Add(new EmployeeDepartment()
                            {
                                EmployeeId = employee.EmpId,
                                DepartmentId = employee.DepartmentId
                            });
                        }
                    }

                    return DataRepository.DemoEntitiesContext.SaveChanges() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        /// <summary>
        /// Remove list of employees by their employee Ids
        /// </summary>
        /// <param name="empIds"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(long[] empIds)
        {
            try
            {
                foreach (var empId in empIds)
                {
                    if (DataRepository.DemoEntitiesContext.Employees.Any(e => e.Id == empId))
                    {
                        Employee emp = DataRepository.DemoEntitiesContext.Employees.First(e => e.Id == empId);

                        if (
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.Any(
                                e => e.EmployeeId == emp.Id))
                        {
                            var associatedDepartment =
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.First(
                                e => e.EmployeeId == emp.Id);
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.Remove(associatedDepartment);
                        }

                        DataRepository.DemoEntitiesContext.Employees.Remove(emp);
                    }
                }
                return DataRepository.DemoEntitiesContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

    }

    public class EmployeeVM
    {
        public long EmpId { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }


}