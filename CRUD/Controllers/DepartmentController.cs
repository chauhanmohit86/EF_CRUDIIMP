using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CRUD.Models;
using CRUD.Models;

namespace CRUD.Controllers
{
    public class DepartmentController : Controller
    {
        //
        // GET: /Department/

        public ActionResult GetDepartments()
        {
            return View("Index", DepartmentStore.FetchDepartments());
        }

        /// <summary>
        /// LOad the departments in json format
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadDepartments()
        {
            return Json(DepartmentStore.FetchDepartments(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetch details of a department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetDepartment(int id)
        {
            DepartmentVM dep = DepartmentStore.FetchDepartments().FirstOrDefault(x => x.Id == id);
            return View("DepartmentDetail", dep);
        }

        /// <summary>
        /// Get the add department view
        /// </summary>
        /// <returns></returns>
        public ActionResult AddDepartment()
        {
            return View("AddDepartment");
        }

        /// <summary>
        /// Add a department
        /// </summary>
        /// <param name="dep"></param>
        /// <returns></returns>
        [HttpPost]
        public bool AddDepartment(DepartmentVM dep)
        {
            return DepartmentStore.AddDepartment(dep);
        }

        /// <summary>
        /// Set the Department
        /// </summary>
        /// <param name="dep"></param>
        /// <returns></returns>
        [HttpPost]
        public bool SetDepartment(DepartmentVM dep)
        {
            return DepartmentStore.SetDepartment(dep);
        }

        /// <summary>
        /// Delete the selected departments
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteDepartments(long[] ids)
        {
            return DepartmentStore.DeleteDepartments(ids);
        }
    }
}
