using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.Query.GetFunctionalRoles;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.Tests.GetFunctionalRoles
{
    [TestClass]
    public class GetFunctionalRolesByCodesQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private readonly List<string> _codes = new() { "CodeA" };
        private const string Classification = "NOTIFICATION";

        private PersonInFunctionalRole _personInFunctionalRole;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private Mock<IBearerTokenApiClient> _clientMock;
        private GetFunctionalRolesByCodesQueryHandler _dut;
        private GetFunctionalRolesByCodesQuery _request;
        private MainApiFunctionalRole _mainApiFunctionalRole;

        [TestInitialize]
        public void Setup()
        {
            _personInFunctionalRole = new PersonInFunctionalRole
            {
                AzureOid = "3BFB54C7-91E2-422E-833F-951AD07FE37F",
                Email = "email@example.com",
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "UN"
            };

            var persons = new List<PersonInFunctionalRole>
            {
                _personInFunctionalRole
            };

            var options = new MainApiOptions { ApiVersion = "1", Audience = "Aud", BaseAddress = "http://example.com/" };

            _optionsMonitorMock = new Mock<IOptionsMonitor<MainApiOptions>>();
            _optionsMonitorMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            _request = new GetFunctionalRolesByCodesQuery(Plant, _codes, Classification);

            var functionalRoleCodes = _codes.Aggregate("", (current, code) => current + $"&functionalRoleCodes={code}");

            var url = $"{options.BaseAddress}Library/FunctionalRolesByCodes" +
                      $"?plantId={_request.Plant}" +
                      functionalRoleCodes +
                      $"&classification={Classification}" +
                      $"&api-version={options.ApiVersion}";

            _mainApiFunctionalRole = new MainApiFunctionalRole()
            {
                ProCoSysGuid = Guid.NewGuid(),
                Code = "CodeA",
                Description = "DescriptionA",
                Email = "Test1@email.com",
                InformationEmail = "TestInfo1@email.com",
                UsePersonalEmail = true,
                Persons = persons
            };
            var functionalRoles = new List<MainApiFunctionalRole>
            {
                _mainApiFunctionalRole
            };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiFunctionalRole>>(url))
                .Returns(Task.FromResult(functionalRoles));
            _dut = new GetFunctionalRolesByCodesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
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

            Assert.AreEqual(1, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ProjectsElementsCorrectly()
        {
            var result = await _dut.Handle(_request, default);

            var functionalRoleDto = result.Data.ElementAt(0);
            Assert.AreEqual(_mainApiFunctionalRole.ProCoSysGuid, functionalRoleDto.ProCoSysGuid);
            Assert.AreEqual(_mainApiFunctionalRole.Code, functionalRoleDto.Code);
            Assert.AreEqual(_mainApiFunctionalRole.Description, functionalRoleDto.Description);
            Assert.AreEqual(_mainApiFunctionalRole.Email, functionalRoleDto.Email);
            Assert.AreEqual(_mainApiFunctionalRole.InformationEmail, functionalRoleDto.InformationEmail);
            Assert.AreEqual(_mainApiFunctionalRole.UsePersonalEmail, functionalRoleDto.UsePersonalEmail);
            Assert.AreEqual(_mainApiFunctionalRole.Persons, functionalRoleDto.Persons);
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_IfNoElementsAreFound()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiFunctionalRole>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiFunctionalRole>>(null));

            var dut = new GetFunctionalRolesByCodesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
