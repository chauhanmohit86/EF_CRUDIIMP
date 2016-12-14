using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRUD.Models;
using CRUD.Models;

namespace TestApplication.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult GetEmployees()
        {
            return View("Index", EmployeeStore.FetchEmployees());
        }

        public ActionResult GetEmployee(int id)
        {
            EmployeeVM emp = EmployeeStore.FetchEmployees().FirstOrDefault(x => x.EmpId == id);
            return View("EmployeeDetail", emp);
        }

        [HttpPost]
        public ActionResult SetEmployee(EmployeeVM emp)
        {
            EmployeeStore.SetEmployee(emp);
            return View("EmployeeDetail", emp);
        }

        public ActionResult AddEmployee()
        {
            return View("AddEmployee");
        }

        [HttpPost]
        public ActionResult AddEmployee(EmployeeVM emp)
        {
            EmployeeStore.AddEmployee(emp);
            return View("EmployeeDetail", emp);
        }

        [HttpPost]
        public void DeleteEmployee(long[] empIds)
        {
            EmployeeStore.DeleteEmployee(empIds);
        }
    }
}