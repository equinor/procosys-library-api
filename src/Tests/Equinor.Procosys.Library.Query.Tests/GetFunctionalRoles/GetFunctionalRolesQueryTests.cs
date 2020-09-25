using System;
using Equinor.Procosys.Library.Query.GetFunctionalRoles;
using Equinor.Procosys.Library.Query.GetTagFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetFunctionalRoles
{
    [TestClass]
    public class GetFunctionalRolesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetFunctionalRolesQuery("PCS$TESTPLANT", "NOTIFICATION");

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
            Assert.AreEqual("NOTIFICATION", dut.Classification);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetTagFunctionsQuery(null, "NOTIFICATION"));
    }
}
