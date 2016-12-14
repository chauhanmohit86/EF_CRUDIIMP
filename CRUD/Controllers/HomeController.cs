using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccessLayer;
using EntityState = System.Data.Entity.EntityState;

namespace TestApplication.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Default action method
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tack the changes madde to an entity
        /// </summary>
        /// <returns></returns>
        public ActionResult TrackChanges()
        {
            using (var context = new DemoEntities())
            {
                var department = context.Departments.First();
                Debug.WriteLine("Context tracking changes of {0} entity.",
                    context.ChangeTracker.Entries().Count());

                var employee = context.Employees.First();
                Debug.WriteLine("Context tracking changes of {0} entity.",
                    context.ChangeTracker.Entries().Count());

                var entry = context.Entry(employee);
                Debug.WriteLine("State : {0}", entry.State);

                employee.Name = "Change track";
                Debug.WriteLine("State : {0}", entry.State);

                entry = context.Entry(employee);
                Debug.WriteLine("State : {0}", entry.State);
            }

            return View("Index");
        }

        /// <summary>
        /// Test for persistency. Saving entity from a different context
        /// </summary>
        /// <returns></returns>
        public ActionResult TestPersistence()
        {
            Employee employee = null;
            using (var context = new DemoEntities())
            {
                employee = context.Employees.First();
                employee.Name = "Disconnected employee";

                var entry = context.Entry(employee);
                Debug.WriteLine("State : {0}", entry.State);
            }

            using (var newContext = new DemoEntities())
            {
                var entry = newContext.Entry(employee);
                Debug.WriteLine("State : {0}", entry.State);

                newContext.Employees.Attach(employee);
                Debug.WriteLine("State : {0}", entry.State);

                entry.State = EntityState.Modified;
                Debug.WriteLine("State : {0}", entry.State);


                var newEmployee = new Employee() { Name = "New Disconnected Employee", Salary = 10000.00m };
                entry = newContext.Entry(newEmployee);
                entry.State = EntityState.Added;

                newContext.SaveChanges();
            }

            return View("Index");
        }

        /// <summary>
        /// Call a stored procedure
        /// </summary>
        /// <returns></returns>
        public ActionResult CallStoredProcedure()
        {
            using (var newContext = new DemoEntities())
            {
                newContext.AddEmployee("New Emp by SP", 12345.00m);
                newContext.SaveChanges();
            }
            return RedirectToAction("GetEmployees", "Employee");
        }

        /// <summary>
        /// Call a function
        /// </summary>
        /// <returns></returns>
        public ActionResult CallFunction()
        {
            var result = new List<DepartmentEmployee_Result>();
            using (var newContext = new DemoEntities())
            {
                result = newContext.DepartmentEmployee().ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Call raw query
        /// </summary>
        /// <returns></returns>
        public ActionResult CallRawSql()
        {
            var result = new List<DepartmentEmployee_Result>();
            using (var newContext = new DemoEntities())
            {
                result =
                    newContext.Database.SqlQuery<DepartmentEmployee_Result>(
                        @"SELECT emp.Name as EmpName, dept.Name AS DeptName FROM Employee emp left JOIN EmployeeDepartment ED on emp.Id = ED.EmployeeID
								left JOIN Department dept on (ED.DepartmentId = dept.Id)").ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Async query EF6.0
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> AsyncQuery()
        {
            //var result = await RunAsyncQuery();
            var task = GetAsyncDepartments();
            Debug.WriteLine("Do some stuff here");
            await task;
            return View("Index");
        }

        //public async Task<List<Department>> RunAsyncQuery()
        //{
        //    var result = GetAsyncDepartments();
        //    Debug.WriteLine("Do some stuff here");
        //    return await result;
        //}

        private async Task<List<Department>> GetAsyncDepartments()
        {
            var depts = new List<Department>();
            using (var newContext = new DemoEntities())
            {
                depts = await newContext.Departments.ToListAsync();
            }
            Debug.WriteLine("Got departments");
            return depts;
        }

        /// <summary>
        /// Database logging
        /// </summary>
        /// <returns></returns>
        public ActionResult DatabaseLog()
        {
            using (var newContext = new DemoEntities())
            {
                newContext.Database.Log = s => Debug.WriteLine(s);
                var dept = newContext.Departments.First();
                dept.Name = "Logging Department";

                var emp = newContext.Employees.First();
                emp.Name = "Logging Employee";

                newContext.SaveChanges();
            }
            return View("Index");
        }

        /// <summary>
        /// Control Transaction
        /// </summary>
        /// <returns></returns>
        public ActionResult ControlTransaction()
        {
            using (var newContext = new DemoEntities())
            {
                newContext.Database.Log = s => Debug.WriteLine(s);
                using (DbContextTransaction dbTran = newContext.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var dept = newContext.Departments.First();
                        dept.Name = "Under Transaction Department";

                        var emp = newContext.Employees.First();
                        emp.Name = "Under Transaction Employee";

                        newContext.SaveChanges();

                        dbTran.Commit();
                    }

                    catch (Exception ex)
                    {
                        //Rollback transaction if exception occurs
                        dbTran.Rollback();
                    }
                }
            }
            return View("Index");
        }

        /// <summary>
        /// Control Transaction
        /// </summary>
        /// <returns></returns>
        public ActionResult AddRange()
        {
            using (var newContext = new DemoEntities())
            {
                newContext.Database.Log = s => Debug.WriteLine(s);

                newContext.Employees.AddRange(new List<Employee>() { 
                    new Employee() { Name = "AddRange Emp1", Salary = 12345.00m }, 
                    new Employee() { Name = "AddRange Emp2", Salary = 12545.00m } 
                });

                newContext.SaveChanges();
            }
            return View("Index");
        }
    }
}
