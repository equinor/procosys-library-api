using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.Query.GetTagFunctions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.Tests.GetTagFunctions
{
    [TestClass]
    public class GetTagFunctionsQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private const string RegisterCode = "A";
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private Mock<IBearerTokenApiClient> _clientMock;
        private GetTagFunctionsQueryHandler _dut;
        private GetTagFunctionsQuery _request;

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

            _request = new GetTagFunctionsQuery(Plant, RegisterCode);

            var url = $"{options.BaseAddress}Library/TagFunctions" +
                $"?plantId={_request.Plant}" +
                $"&registerCode={_request.RegisterCode}" +
                $"&api-version={options.ApiVersion}";

           var tagFunctions = new List<MainApiTagFunction>
                {
                    new MainApiTagFunction
                    {
                        Code = "CodeA",
                        Description = "DescriptionA"
                    },
                    new MainApiTagFunction
                    {
                        Code = "CodeB",
                        Description = "DescriptionB"
                    }
                };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiTagFunction>>(url))
                .Returns(Task.FromResult(tagFunctions));
            _dut = new GetTagFunctionsQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
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
                .Setup(x => x.QueryAndDeserialize<List<MainApiTagFunction>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiTagFunction>>(null));

            var dut = new GetTagFunctionsQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
