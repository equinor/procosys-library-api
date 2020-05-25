using System;
using Equinor.Procosys.Library.Query.GetAllResponsibles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetAllResponsibles
{
    [TestClass]
    public class GetAllResponsiblesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetAllResponsiblesQuery("PCS$TESTPLANT");

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetAllResponsiblesQuery(null));
    }
}
