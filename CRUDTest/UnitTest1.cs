using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using CRUD.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataAccessLayer;

namespace CRUDTest
{
    [TestClass]
    public class UnitTest1
    {

        private Mock<DbSet<Employee>> employeeSet;
        private Mock<DbSet<Department>> departmentSet;
        private Mock<DemoEntities> context;

        /// <summary>
        /// Generic method to mock the DbSet in the Context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        private static Mock<DbSet<T>> GetDbSetMock<T>(IEnumerable<T> items = null) where T : class
        {
            if (items == null)
            {
                items = new T[0];
            }

            var dbSetMock = new Mock<DbSet<T>>();
            var q = dbSetMock.As<IQueryable<T>>();

            q.Setup(x => x.GetEnumerator()).Returns(items.GetEnumerator);
            q.Setup(r => r.Provider).Returns(items.AsQueryable().Provider);
            q.Setup(r => r.ElementType).Returns(items.AsQueryable().ElementType);
            q.Setup(r => r.Expression).Returns(items.AsQueryable().Expression);

            return dbSetMock;
        }

        /// <summary>
        /// Initialize the context and setup data
        /// </summary>
        [TestInitializeAttribute()]
        public void TestInit()
        {
            context = new Mock<DemoEntities>();

            var departments = new List<Department>()
            {
                new Department() {Name = "HR", Id = 1},
                new Department() {Name = "Operations", Id = 2}
            };

            //Set up departments
            context.Setup(x => x.Departments).Returns(GetDbSetMock(departments).Object);

            var employeeList = new List<Employee>()
            {
               new Employee() {
                Name = "Alex",
                Salary = 12345.00m,
                Id = 1,
                EmployeeDepartments = new Collection<EmployeeDepartment>() { new EmployeeDepartment() { SequenceNo = 1, EmployeeId = 1, DepartmentId = 1 } }
            },
            new Employee()
            {
                Name = "John",
                Salary = 12545.00m,
                Id = 2,
                EmployeeDepartments = new Collection<EmployeeDepartment>() { new EmployeeDepartment() { SequenceNo = 2, EmployeeId = 2, DepartmentId = 2 } }
            }
            };

            //Set up Employee List
            context.Setup(x => x.Employees).Returns(GetDbSetMock(employeeList).Object);

            var empDeptList = new List<EmployeeDepartment>()
            {
                new EmployeeDepartment() {DepartmentId = 1, EmployeeId = 1, SequenceNo = 1},
                new EmployeeDepartment() {DepartmentId = 2, EmployeeId = 2, SequenceNo = 2}
            };

            //Set up EmployeeDepartments
            context.Setup(x => x.EmployeeDepartments).Returns(GetDbSetMock(empDeptList).Object);

            context.Object.SaveChanges();
        }

        /// <summary>
        /// Test get Employee List
        /// </summary>
        [TestMethod]
        public void FetchEmployeesTest()
        {
            DataRepository.DemoEntitiesContext = context.Object;

            var empList = EmployeeStore.FetchEmployees();
            Assert.IsNotNull(empList, "No employees found");
        }

        /// <summary>
        /// Test - update an employee
        /// </summary>
        [TestMethod]
        public void UpdateEmployeeTest()
        {
            DataRepository.DemoEntitiesContext = context.Object;

            var isUpdated = EmployeeStore.SetEmployee(new EmployeeVM() { DepartmentId = 1, EmpId = 1, Name = "Julia", Salary = 12343.00m });
            var empList = EmployeeStore.FetchEmployees();
            Assert.AreEqual(empList.Any(emp => emp.Name == "Julia"), true);
        }
    }
}
