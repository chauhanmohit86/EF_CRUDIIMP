using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccessLayer;

namespace CRUD.Models
{
    public class DepartmentStore
    {
        /// <summary>
        /// Fetch the available Departments
        /// </summary>
        /// <returns></returns>
        internal static List<DepartmentVM> FetchDepartments()
        {
            var departments = DataRepository.DemoEntitiesContext.Departments;
            var employees = DataRepository.DemoEntitiesContext.Employees;

            var availableDepts = new List<DepartmentVM>();
            foreach (var dept in departments)
            {
                var department = new DepartmentVM() { Id = dept.Id, Name = dept.Name };
                if (dept.EmployeeDepartments.Any())
                {
                    var empIds = dept.EmployeeDepartments.Select(ed => ed.EmployeeId).ToList();
                    department.EmployeeList =
                        employees.Where(
                            emp => empIds.Contains(emp.Id))
                            .Select(emp => emp.Name).ToList();
                }
                else
                {
                    department.EmployeeList = new List<string>();
                }
                availableDepts.Add(department);
            }
            return availableDepts;
        }

        /// <summary>
        /// Add a department
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        internal static bool AddDepartment(DepartmentVM department)
        {
            try
            {
                DataRepository.DemoEntitiesContext.Departments.Add(new Department()
                {
                    Name = department.Name
                });
                return DataRepository.DemoEntitiesContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modify/Edit the department
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        internal static bool SetDepartment(DepartmentVM department)
        {
            try
            {
                if (DataRepository.DemoEntitiesContext.Departments.Any(d => d.Id == department.Id))
                {
                    Department dep = DataRepository.DemoEntitiesContext.Departments.First(d => d.Id == department.Id);
                    dep.Name = department.Name;
                    return DataRepository.DemoEntitiesContext.SaveChanges() > 0;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This function deletes the Departments
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        internal static bool DeleteDepartments(long[] ids)
        {
            try
            {
                var departments = DataRepository.DemoEntitiesContext.Departments.Where(dep => ids.Contains(dep.Id));
                foreach (var department in departments)
                {
                    //If there are associations then delete the associations first
                    if (department.EmployeeDepartments.Any(associatedEmp => associatedEmp.DepartmentId == department.Id))
                    {
                        var associations =
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.Where(
                                associatedEmp => associatedEmp.DepartmentId == department.Id).ToList();

                        foreach (var assoc in associations)
                        {
                            DataRepository.DemoEntitiesContext.EmployeeDepartments.Remove(assoc);
                        }
                    }
                    DataRepository.DemoEntitiesContext.Departments.Remove(department);
                }
                return DataRepository.DemoEntitiesContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Department View Model
    /// </summary>
    public class DepartmentVM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<string> EmployeeList { get; set; }
    }
}
