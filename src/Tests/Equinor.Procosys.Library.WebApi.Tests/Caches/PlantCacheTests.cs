using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Infrastructure.Caching;
using Equinor.Procosys.Library.WebApi.Authorizations;
using Equinor.Procosys.Library.WebApi.Caches;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Library.WebApi.Tests.Caches
{
    [TestClass]
    public class PlantCacheTests
    {
        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private Mock<IPlantApiService> _plantApiServiceMock;
        private readonly string Plant1 = "P1";
        private readonly string Plant2 = "P2";

        private PlantCache _dut;

        [TestInitialize]
        public void Setup()
        {
            var optionsMock = new Mock<IOptionsMonitor<CacheOptions>>();
            optionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new CacheOptions());

            _plantApiServiceMock = new Mock<IPlantApiService>();
            _plantApiServiceMock.Setup(p => p.GetPlantsAsync()).Returns(Task.FromResult(
                new List<ProcosysPlant> {new ProcosysPlant {Id = Plant1}, new ProcosysPlant {Id = Plant2}}));

            _dut = new PlantCache(new CacheManager(), _plantApiServiceMock.Object, optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPlantIdsForUserOid_ShouldReturnPlantIdsFromPlantApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPlantIdsForUserOidAsync(_currentUserOid);

            // Assert
            AssertPlants(result);
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetPlantsForUserOid_ShouldReturnPlantsFromCacheSecondTime()
        {
            await _dut.GetPlantIdsForUserOidAsync(_currentUserOid);

            // Act
            var result = await _dut.GetPlantIdsForUserOidAsync(_currentUserOid);

            // Assert
            AssertPlants(result);
            // since GetPlantIdsForUserOidAsync has been called twice, but GetPlantsAsync has been called once, the second Get uses cache
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnTrue_WhenKnownPlant()
        {
            // Act
            var result = await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnFalse_WhenUnknownPlant()
        {
            // Act
            var result = await _dut.IsValidPlantForUserAsync("XYZ", _currentUserOid);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnPlantIdsFromPlantApiServiceFirstTime()
        {
            // Act
            await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);

            // Assert
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnPlantsFromCache()
        {
            await _dut.IsValidPlantForUserAsync("ABC", _currentUserOid);
            // Act
            await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);

            // Assert
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Clear_ShouldForceGettingPlantsFromApiServiceAgain()
        {
            // Arrange
            var result = await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);
            Assert.IsTrue(result);
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);

            // Act
            _dut.Clear(_currentUserOid);

            // Assert
            result = await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);
            Assert.IsTrue(result);
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Exactly(2));
        }

        private void AssertPlants(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Plant1, result.First());
            Assert.AreEqual(Plant2, result.Last());
        }
    }
}
