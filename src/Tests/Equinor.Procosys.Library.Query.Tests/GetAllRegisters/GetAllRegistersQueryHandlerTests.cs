using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.Query.GetAllRegisters;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.Tests.GetAllRegisters
{
    [TestClass]
    public class GetAllRegistersQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IBearerTokenApiClient> _clientMock;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private GetAllRegistersQueryHandler _dut;
        private GetAllRegistersQuery _request;

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

            _request = new GetAllRegistersQuery(Plant);
            var url = $"{options.BaseAddress}Library/Registers" +
                $"?plantId={_request.Plant}" +
                $"&api-version={options.ApiVersion}";

           var registers = new List<MainApiRegister>
                {
                    new MainApiRegister
                    {
                        Code = "CodeA",
                        Description = "DescriptionA"
                    },
                    new MainApiRegister
                    {
                        Code = "CodeB",
                        Description = "DescriptionB"
                    }
                };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiRegister>>(url))
                .Returns(Task.FromResult(registers));
            _dut = new GetAllRegistersQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
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
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiRegister>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiRegister>>(null));

            var dut = new GetAllRegistersQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
