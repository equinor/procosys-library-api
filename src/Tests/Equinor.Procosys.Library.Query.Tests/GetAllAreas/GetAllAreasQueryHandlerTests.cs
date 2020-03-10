using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.Query.GetAllAreas;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.Tests.GetAllAreas
{
    [TestClass]
    public class GetAllAreasQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IBearerTokenApiClient> _clientMock;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;

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

            var url = $"{options.BaseAddress}Library/Areas" +
                $"?plantId={Plant}" +
                $"&api-version={options.ApiVersion}";

           var  areas = new List<MainApiArea>
                {
                    new MainApiArea
                    {
                        Code = "CodeA",
                        Description = "DescriptionA"
                    },
                    new MainApiArea
                    {
                        Code = "CodeB",
                        Description = "DescriptionB"
                    }
                };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiArea>>(url))
                .Returns(Task.FromResult(areas));
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            var dut = new GetAllAreasQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
            var request = new GetAllAreasQuery(Plant);

            var result = await dut.Handle(request, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfElements()
        {
            var dut = new GetAllAreasQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
            var request = new GetAllAreasQuery(Plant);

            var result = await dut.Handle(request, default);

            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ProjectsElementsCorrectly()
        {
            var dut = new GetAllAreasQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
            var request = new GetAllAreasQuery(Plant);

            var result = await dut.Handle(request, default);

            Assert.AreEqual("CodeA", result.Data.ElementAt(0).Code);
            Assert.AreEqual("DescriptionA", result.Data.ElementAt(0).Description);
            Assert.AreEqual("CodeB", result.Data.ElementAt(1).Code);
            Assert.AreEqual("DescriptionB", result.Data.ElementAt(1).Description);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_IfNoElementsAreFound()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiArea>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiArea>>(null));

            var dut = new GetAllAreasQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
            var request = new GetAllAreasQuery(Plant);

            var result = await dut.Handle(request, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
