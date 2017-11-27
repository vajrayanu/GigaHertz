using GH.DAL.SQLDAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GH.DAL.Model;
using System.Collections.Generic;

namespace GH.Test
{
    
    
    /// <summary>
    ///This is a test class for ProductManagerTest and is intended
    ///to contain all ProductManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductManagerTest
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
        ///A test for FulltextDearch
        ///</summary>
        [TestMethod()]
        public void FulltextDearchTest()
        {
            string searching = "3M";
            List<Product> expected = null; // TODO: Initialize to an appropriate value
            List<Product> actual;
            actual = ProductManager.FulltextSearch(searching);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
