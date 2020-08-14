using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Library.Query.Tests.ClientTests
{
    [TestClass]
    public class BearerTokenApiClientTests
    {
        private Mock<IBearerTokenProvider> _bearerTokenProvider;
        private Mock<ILogger<BearerTokenApiClient>> _logger;

        [TestInitialize]
        public void Setup()
        {
            _bearerTokenProvider = new Mock<IBearerTokenProvider>();
            _logger = new Mock<ILogger<BearerTokenApiClient>>();
        }

        [TestMethod]
        public async Task QueryAndDeserialize_ReturnsDeserialized_Object()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.OK, "{\"Id\": 123}");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            var response = await dut.QueryAndDeserializeAsync<DummyClass>("url");

            Assert.IsNotNull(response);
            Assert.AreEqual(123, response.Id);
        }

        [TestMethod]
        public async Task QueryAndDeserializeReturnsDefaultObject_WhenRequestIsNotSuccessful()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.BadGateway, "");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            var response = await dut.QueryAndDeserializeAsync<DummyClass>("url");

            Assert.AreEqual(default, response);
        }

        [TestMethod]
        public async Task QueryAndDeserialize_ThrowsException_WhenInvalidResponseIsReceived()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.OK, "");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<JsonException>(async () => await dut.QueryAndDeserializeAsync<DummyClass>("url"));
        }

        [TestMethod]
        public async Task QueryAndDeserialize_ThrowsException_WhenNoUrl()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.OK, "");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await dut.QueryAndDeserializeAsync<DummyClass>(null));
        }

        [TestMethod]
        public async Task QueryAndDeserialize_ThrowsException_WhenUrlTooLong()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.OK, "");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.QueryAndDeserializeAsync<DummyClass>(new string('u', 2001)));
        }

        private class DummyClass
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Id { get; set; }
        }
    }
}
