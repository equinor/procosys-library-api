using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.Query.GetAllDisciplines;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.Tests.GetAllDisciplines
{
    [TestClass]
    public class GetAllDisciplinesQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IBearerTokenApiClient> _clientMock;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private GetAllDisciplinesQuery _request;

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

            var classifications = new List<string>
            {
                "ClassA",
                "ClassB"
            };

            _request = new GetAllDisciplinesQuery(Plant, classifications);

            var url = $"{options.BaseAddress}Library/Disciplines" +
                $"?plantId={Plant}" +
                string.Join("", classifications
                    .Where(c => c != null)
                    .Select(c => $"&classifications={c.ToUpper()}")) +
                $"&api-version={options.ApiVersion}";

            var  disciplines = new List<MainApiDiscipline>
                {
                    new MainApiDiscipline
                    {
                        Code = "CodeA",
                        Description = "DescriptionA"
                    },
                    new MainApiDiscipline
                    {
                        Code = "CodeB",
                        Description = "DescriptionB"
                    }
                };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiDiscipline>>(url))
                .Returns(Task.FromResult(disciplines));
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfElements()
        {
            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ProjectsElementsCorrectly()
        {
            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual("CodeA", result.Data.ElementAt(0).Code);
            Assert.AreEqual("DescriptionA", result.Data.ElementAt(0).Description);
            Assert.AreEqual("CodeB", result.Data.ElementAt(1).Code);
            Assert.AreEqual("DescriptionB", result.Data.ElementAt(1).Description);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_IfNoElementsAreFound()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserialize<List<MainApiDiscipline>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiDiscipline>>(null));

            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
