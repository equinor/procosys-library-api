using System.Collections.Generic;
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
        public void Constructor_CreatesEmptyList_WhenNoClassificationsAreGiven()
        {
            var dut = new GetAllDisciplinesQuery("PCS$TESTPLANT", null);

            Assert.IsNotNull(dut.Classifications);
        }
    }
}
