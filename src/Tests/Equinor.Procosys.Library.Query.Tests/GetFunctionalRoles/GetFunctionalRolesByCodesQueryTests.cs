using System;
using System.Collections.Generic;
using Equinor.Procosys.Library.Query.GetFunctionalRoles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetFunctionalRoles
{
    [TestClass]
    public class GetFunctionalRolesByCodesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetFunctionalRolesByCodesQuery("PCS$TESTPLANT", new List<string>{ "NOTIFICATION-COMPANY-AUTHORIZED" });

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
            Assert.AreEqual(1, dut.Codes.Count);
            Assert.AreEqual("NOTIFICATION-COMPANY-AUTHORIZED", dut.Codes[0]);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetFunctionalRolesByCodesQuery(null, new List<string> { "NOTIFICATION-COMPANY-AUTHORIZED" }));
    }
}
