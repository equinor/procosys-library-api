using System;
using Equinor.Procosys.Library.Query.GetAllRegisters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetAllRegisters
{
    [TestClass]
    public class GetAllRegistersQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetAllRegistersQuery("PCS$TESTPLANT");

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetAllRegistersQuery(null));
    }
}
