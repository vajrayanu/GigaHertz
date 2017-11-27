using GH.DAL.SQLDAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GH.DAL.Model;
using System.Collections.Generic;

namespace GH.Test
{
    
    
    /// <summary>
    ///This is a test class for CustomerManagerTest and is intended
    ///to contain all CustomerManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomerManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetCustomers
        ///</summary>
        [TestMethod()]
        public void GetCustomersTest()
        {
            //CustomerManager target = new CustomerManager(); // TODO: Initialize to an appropriate value

            //List<Customer> actual = null; // TODO: Initialize to an appropriate value
          
            //actual = CustomerManager.GetCustomers(0,10);

            //Assert.IsNotNull( actual);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            //CustomerManager target = new CustomerManager(); // TODO: Initialize to an appropriate value
            //Customer model = new Customer();
            ////model.pkCustomerId = Guid.NewGuid();
            //model.sFirstName = "ลูกค้า 1";
            //model.dtDateAdd = DateTime.Now;

            //target.Create(model);
            //Assert.IsNotNull(model.sFirstName, "ลูกค้า 1");
        }

        /// <summary>
        ///A test for GetById
        ///</summary>
        [TestMethod()]
        public void GetByIdTest()
        {
           
        }


       
    }
}
