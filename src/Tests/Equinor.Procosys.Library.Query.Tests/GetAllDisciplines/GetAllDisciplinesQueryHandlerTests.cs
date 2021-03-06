﻿using System.Collections.Generic;
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
        private GetAllDisciplinesQuery _requestWithClassifications;
        private List<MainApiDiscipline> _disciplines;
        private string _urlWithClassifications;
        private string _urlWithoutClassifications;

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

            _requestWithClassifications = new GetAllDisciplinesQuery(Plant, classifications);

            _urlWithClassifications = $"{options.BaseAddress}Library/Disciplines" +
                $"?plantId={Plant}" +
                string.Join("", classifications
                    .Where(c => c != null)
                    .Select(c => $"&classifications={c.ToUpper()}")) +
                $"&api-version={options.ApiVersion}";

            _urlWithoutClassifications = $"{options.BaseAddress}Library/Disciplines" +
                $"?plantId={Plant}" +
                $"&api-version={options.ApiVersion}";

            _disciplines = new List<MainApiDiscipline>
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
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiDiscipline>>(_urlWithClassifications))
                .Returns(Task.FromResult(_disciplines));

            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_requestWithClassifications, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfElements()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiDiscipline>>(_urlWithClassifications))
                .Returns(Task.FromResult(_disciplines));

            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_requestWithClassifications, default);

            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ReturnsAllElements_WhenNoClassificationsAreGiven()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiDiscipline>>(_urlWithoutClassifications))
                .Returns(Task.FromResult(_disciplines));

            var request = new GetAllDisciplinesQuery(Plant, null);

            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(request, default);

            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ProjectsElementsCorrectly()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiDiscipline>>(_urlWithClassifications))
                .Returns(Task.FromResult(_disciplines));

            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_requestWithClassifications, default);

            Assert.AreEqual("CodeA", result.Data.ElementAt(0).Code);
            Assert.AreEqual("DescriptionA", result.Data.ElementAt(0).Description);
            Assert.AreEqual("CodeB", result.Data.ElementAt(1).Code);
            Assert.AreEqual("DescriptionB", result.Data.ElementAt(1).Description);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_IfNoElementsAreFound()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiDiscipline>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiDiscipline>>(null));

            var dut = new GetAllDisciplinesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_requestWithClassifications, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
