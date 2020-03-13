using System;
using Equinor.Procosys.Library.Query.GetAllAreas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetAllAreas
{
    [TestClass]
    public class GetAllAreasQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetAllAreasQuery("PCS$TESTPLANT");

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetAllAreasQuery(null));
    }
}
