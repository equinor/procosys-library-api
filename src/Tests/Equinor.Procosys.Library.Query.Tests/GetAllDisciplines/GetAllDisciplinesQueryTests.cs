using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Library.Query.GetAllDisciplines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Query.Tests.GetAllDisciplines
{
    [TestClass]
    public class GetAllDisciplinesQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var classifications = new List<string> { "ClassA", "ClassB" };

            var dut = new GetAllDisciplinesQuery("PCS$TESTPLANT", classifications);

            Assert.AreEqual("PCS$TESTPLANT", dut.Plant);
            Assert.AreEqual(classifications, dut.Classifications);
        }

        [TestMethod]
        public void Constructor_ThrowsException_WhenNoPlantIsGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new GetAllDisciplinesQuery(null, null));

        [TestMethod]
        public void Constructor_CreatesEmptyList_WhenNoClassificationsAreGiven()
        {
            var dut = new GetAllDisciplinesQuery("PCS$TESTPLANT", null);

            Assert.IsNotNull(dut.Classifications);
            Assert.AreEqual(0, dut.Classifications.Count());
        }
    }
}
