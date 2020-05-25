using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.Query.GetAllResponsibles;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.Tests.GetAllResponsibles
{
    [TestClass]
    public class GetAllResponsiblesQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IBearerTokenApiClient> _clientMock;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private GetAllResponsiblesQueryHandler _dut;
        private GetAllResponsiblesQuery _request;

        [TestInitialize]
        public void Setup()
        {
            var options = new MainApiOptions
            {
                ApiVersion = "1",
                Audience = "Aud",
                BaseAddress = "http://example.com/"
            };

            _optionsMonitorMock = new Mock<IOptionsMonitor<MainApiOptions>>();
            _optionsMonitorMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            _request = new GetAllResponsiblesQuery(Plant);
            var url = $"{options.BaseAddress}Library/Responsibles" +
                $"?plantId={_request.Plant}" +
                $"&api-version={options.ApiVersion}";

            var responsibles = new List<MainApiResponsible>
                {
                    new MainApiResponsible
                    {
                        Code = "CodeA",
                        Description = "DescriptionA"
                    },
                    new MainApiResponsible
                    {
                        Code = "CodeB",
                        Description = "DescriptionB"
                    }
                };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiResponsible>>(url))
                .Returns(Task.FromResult(responsibles));
            _dut = new GetAllResponsiblesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            var result = await _dut.Handle(_request, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfElements()
        {
            var result = await _dut.Handle(_request, default);

            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ProjectsElementsCorrectly()
        {
            var result = await _dut.Handle(_request, default);

            Assert.AreEqual("CodeA", result.Data.ElementAt(0).Code);
            Assert.AreEqual("DescriptionA", result.Data.ElementAt(0).Description);
            Assert.AreEqual("CodeB", result.Data.ElementAt(1).Code);
            Assert.AreEqual("DescriptionB", result.Data.ElementAt(1).Description);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_IfNoElementsAreFound()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiResponsible>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiResponsible>>(null));

            var dut = new GetAllResponsiblesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
