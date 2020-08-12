﻿using System.Threading;
using Equinor.Procosys.Library.Infrastructure.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Library.Infrastructure.Tests.Caching
{
    [TestClass]
    public class CacheManagerTests
    {
        private CacheManager _dut;

        [TestInitialize]
        public void Setup() => _dut = new CacheManager();

        [TestMethod]
        public void GetOrCreate_ShouldReturnCachedValue()
        {
            // Act
            var result = _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            
            // Assert
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void GetOrCreate_ShouldReuseCachedValue_BeforeExpirationExpired()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);

            // Act
            var result = _dut.GetOrCreate("A", () => "C", CacheDuration.Minutes, 2);
            
            // Assert
            Assert.AreEqual("B", result);
            result = _dut.Get<string>("A");
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void GetOrCreate_ShouldReplaceCachedValue_AfterExpirationExpired()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "C", CacheDuration.Seconds, 1);
            var MillisecondsTimeout = 2000;
            Thread.Sleep(MillisecondsTimeout);
            
            // Act
            var result = _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            
            // Assert
            Assert.AreEqual("B", result);
            result = _dut.Get<string>("A");
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void Get_ShouldReturnCachedValue_WhenKnownKey()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            
            // Act
            var result = _dut.Get<string>("A");
            
            // Assert
            Assert.AreEqual("B", result);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenUnknownKey()
        {
            // Act
            var result = _dut.Get<string>("A");
            
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveKnownKey()
        {
            // Arrange
            _dut.GetOrCreate("A", () => "B", CacheDuration.Minutes, 2);
            _dut.Get<string>("A");
            var result = _dut.Get<string>("A");
            Assert.AreEqual("B", result);
            _dut.Remove("A");
            
            // Act
            result = _dut.Get<string>("A");
            
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldDoNothing_WhenRemoveUnknownKey() => _dut.Remove("A");
    }
}
