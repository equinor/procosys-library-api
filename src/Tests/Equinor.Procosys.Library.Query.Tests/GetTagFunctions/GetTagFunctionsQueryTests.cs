using System;
using Equinor.Procosys.Library.Query.GetTagFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetTagFunctions
{
    [TestClass]
    public class GetTagFunctionsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetTagFunctionsQuery("PCS$TESTPLANT", "A");

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
            Assert.AreEqual("A", dut.RegisterCode);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetTagFunctionsQuery(null, "A"));
    }
}
