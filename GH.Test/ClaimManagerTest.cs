using GH.DAL.SQLDAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GH.DAL.Model;

namespace GH.Test
{
    
    
    /// <summary>
    ///This is a test class for ClaimManagerTest and is intended
    ///to contain all ClaimManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClaimManagerTest
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
        ///A test for GetByRepairNo
        ///</summary>
        [TestMethod()]
        public void GetByRepairNoTest()
        {
            DateTime d1 = DateTime.Today;
            DateTime d2 = DateTime.Today.AddDays(-3);
            double minutes = d2.Subtract(d1).TotalDays;

            double dtest = d2.CompareTo(d1);


            //string repairNo = "P5510055";
            //Claim expected = null; // TODO: Initialize to an appropriate value
            //Claim actual;
            //actual = ClaimManager.GetByRepairNo(repairNo);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
