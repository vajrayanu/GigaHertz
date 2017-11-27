using GH.DAL.SQLDAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GH.DAL.Model;
using System.Collections.Generic;

namespace GH.Test
{
    
    
    /// <summary>
    ///This is a test class for ReportManagerTest and is intended
    ///to contain all ReportManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReportManagerTest
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
        ///A test for ReportRepairDay
        ///</summary>
        [TestMethod()]
        public void ReportRepairDayTest()
        {
            DateTime date1 = new DateTime(2012, 9, 25, 0, 0, 0);
            DateTime date2 = new DateTime(2012, 9, 26, 12, 0, 0);
            int result = DateTime.Compare(date1, date2);
            string relationship;

            if (result < 0)
                relationship = "is earlier than";
            else if (result == 0)
                relationship = "is the same time as";
            else
                relationship = "is later than";


            DateTime start = DateTime.Now;//.AddDays(-2);
            DateTime end = DateTime.Now.AddDays(1);
            var actual = ReportManager.ReportRepairDay(start,end);
         
        }

        /// <summary>
        ///A test for ReportRepair
        ///</summary>
        [TestMethod()]
        public void ReportRepairTest()
        {
            DateTime start = DateTime.Now.AddDays(-2);
            DateTime end = DateTime.Now.AddDays(2);
         
            var actual = ReportManager.ReportRepair(start, end);
        
        }

        /// <summary>
        ///A test for ReportRepair
        ///</summary>
        [TestMethod()]
        public void ReportRepairTest1()
        {
            DateTime start = DateTime.Now.AddDays(-26);
            DateTime end = DateTime.Now.AddDays(3);
       
            var actual = ReportManager.ReportRepair(start, end);
            var actual2 = ReportManager.ReportClaim(start, end);
        }

  

       
    }
}
