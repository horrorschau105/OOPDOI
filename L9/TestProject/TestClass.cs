using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L9;

namespace TestProject
{
    [TestClass]
    public class TestClass
    {
        [TestMethod]
        public void TestOfTest()
        {
            Assert.AreNotEqual(5, 6);
           
        }
        [TestMethod]
        public void CanUseContainer()
        {
            SimplyContainer sc = new SimplyContainer();
            
            Assert.IsNotNull(sc);
        }
    }
}
