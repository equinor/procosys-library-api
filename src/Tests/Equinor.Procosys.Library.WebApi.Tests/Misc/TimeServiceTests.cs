using System;
using Equinor.Procosys.Library.WebApi.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.WebApi.Tests.Misc
{
    [TestClass]
    public class TimeServiceTests
    {
        [TestMethod]
        public void ReturnsTimeAsUtc()
        {
            var dut = new TimeService();
            
            var time = dut.GetCurrentTimeUtc();
            
            Assert.AreEqual(DateTimeKind.Utc, time.Kind);
        }
    }
}
