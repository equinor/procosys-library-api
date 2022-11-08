using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Domain.Time;
using Equinor.Procosys.Library.Infrastructure.Caching;
using Equinor.Procosys.Library.Test.Common;
using Equinor.Procosys.Library.WebApi.Authorizations;
using Equinor.Procosys.Library.WebApi.Caches;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Library.WebApi.Tests.Caches
{
    [TestClass]
    public class PermissionCacheTests
    {
        private PermissionCache _dut;
        private readonly Guid Oid = new Guid("{3BFB54C7-91E2-422E-833F-951AD07FE37F}");
        private Mock<IPermissionApiService> _permissionApiServiceMock;
        private readonly string TestPlant = "TestPlant";
        private readonly string Permission1 = "A";
        private readonly string Permission2 = "B";

        [TestInitialize]
        public void Setup()
        {
            TimeService.SetProvider(new ManualTimeProvider(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            _permissionApiServiceMock = new Mock<IPermissionApiService>();
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission1, Permission2}));

            var optionsMock = new Mock<IOptionsMonitor<CacheOptions>>();
            optionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new CacheOptions());

            _dut = new PermissionCache(new CacheManager(), _permissionApiServiceMock.Object, optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPermissionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertPermissions(result);
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetPermissionsForUserAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetPermissionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertPermissions(result);
            // since GetPermissionsForUserOidAsync has been called twice, but GetPermissionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public void ClearAll_ShouldClearAllPermissionCaches()
        {
            // Arrange
            var cacheManagerMock = new Mock<ICacheManager>();
            var dut = new PermissionCache(
                cacheManagerMock.Object,
                _permissionApiServiceMock.Object,
                new Mock<IOptionsMonitor<CacheOptions>>().Object);
            // Act
            dut.ClearAll(TestPlant, Oid);

            // Assert
            cacheManagerMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(1));
        }

        private void AssertPermissions(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Permission1, result.First());
            Assert.AreEqual(Permission2, result.Last());
        }
    }
}
