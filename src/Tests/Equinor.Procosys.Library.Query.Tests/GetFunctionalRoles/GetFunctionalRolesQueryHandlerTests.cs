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
    public class GetFunctionalRolesQueryHandlerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private const string Classification = "NOTIFICATION";

        private PersonInFunctionalRole _personInFunctionalRole;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private Mock<IBearerTokenApiClient> _clientMock;
        private GetFunctionalRolesQueryHandler _dut;
        private GetFunctionalRolesQuery _request;
        private List<MainApiFunctionalRole> _functionalRoles;

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

            _request = new GetFunctionalRolesQuery(Plant, Classification);

            var url = $"{options.BaseAddress}Library/FunctionalRoles" +
                      $"?plantId={_request.Plant}" +
                      $"&classification={_request.Classification}" +
                      $"&api-version={options.ApiVersion}";

            _functionalRoles = new List<MainApiFunctionalRole>
            {
                new()
                {
                    ProCoSysGuid = Guid.NewGuid(),
                    Code = "CodeA",
                    Description = "DescriptionA",
                    Email = "Test1@email.com",
                    InformationEmail = "TestInfo1@email.com",
                    UsePersonalEmail = true,
                    Persons = persons
                },
                new()
                {
                    ProCoSysGuid = Guid.NewGuid(),
                    Code = "CodeB",
                    Description = "DescriptionB",
                    Email = "Test2@email.com",
                    InformationEmail = "TestInfo2@email.com",
                    UsePersonalEmail = false,
                    Persons = new List<PersonInFunctionalRole>()
                }
            };

            _clientMock = new Mock<IBearerTokenApiClient>();
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiFunctionalRole>>(url))
                .Returns(Task.FromResult(_functionalRoles));
            _dut = new GetFunctionalRolesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);
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

            AssertFunctionRole(_functionalRoles.ElementAt(0), result.Data.ElementAt(0));
            AssertFunctionRole(_functionalRoles.ElementAt(1), result.Data.ElementAt(1));
        }

        [TestMethod]
        public async Task Handle_ReturnsEmptyList_IfNoElementsAreFound()
        {
            _clientMock
                .Setup(x => x.QueryAndDeserializeAsync<List<MainApiFunctionalRole>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<MainApiFunctionalRole>>(null));

            var dut = new GetFunctionalRolesQueryHandler(_clientMock.Object, _optionsMonitorMock.Object);

            var result = await dut.Handle(_request, default);

            Assert.AreEqual(0, result.Data.Count());
        }

        private void AssertFunctionRole(MainApiFunctionalRole mainApiFunctionalRole, FunctionalRoleDto functionalRoleDto)
        {
            Assert.AreEqual(mainApiFunctionalRole.ProCoSysGuid, functionalRoleDto.ProCoSysGuid);
            Assert.AreEqual(mainApiFunctionalRole.Code, functionalRoleDto.Code);
            Assert.AreEqual(mainApiFunctionalRole.Description, functionalRoleDto.Description);
            Assert.AreEqual(mainApiFunctionalRole.Email, functionalRoleDto.Email);
            Assert.AreEqual(mainApiFunctionalRole.InformationEmail, functionalRoleDto.InformationEmail);
            Assert.AreEqual(mainApiFunctionalRole.UsePersonalEmail, functionalRoleDto.UsePersonalEmail);
            Assert.AreEqual(mainApiFunctionalRole.Persons, functionalRoleDto.Persons);
        }
    }
}
