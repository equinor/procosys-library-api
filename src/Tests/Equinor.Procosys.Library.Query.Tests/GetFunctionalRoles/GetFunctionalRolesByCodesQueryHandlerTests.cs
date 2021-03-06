﻿using System.Collections.Generic;
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
        private readonly List<string> Codes = new List<string> { "CodeA" };
        private const string Classification = "NOTIFICATION";

        private List<PersonInFunctionalRole> _persons;
        private PersonInFunctionalRole _personInFunctionalRole;
        private Mock<IOptionsMonitor<MainApiOptions>> _optionsMonitorMock;
        private Mock<IBearerTokenApiClient> _clientMock;
        private GetFunctionalRolesByCodesQueryHandler _dut;
        private GetFunctionalRolesByCodesQuery _request;

        [TestInitialize]
        public void Setup()
        {
            _personInFunctionalRole = new PersonInFunctionalRole()
            {
                AzureOid = "3BFB54C7-91E2-422E-833F-951AD07FE37F",
                Email = "email@example.com",
                FirstName = "FirstName",
                LastName = "LastName",
                UserName = "UN"
            };

            _persons = new List<PersonInFunctionalRole>
            {
                _personInFunctionalRole
            };

            var options = new MainApiOptions { ApiVersion = "1", Audience = "Aud", BaseAddress = "http://example.com/" };

            _optionsMonitorMock = new Mock<IOptionsMonitor<MainApiOptions>>();
            _optionsMonitorMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            _request = new GetFunctionalRolesByCodesQuery(Plant, Codes, Classification);

            var functionalRoleCodes = Codes.Aggregate("", (current, code) => current + $"&functionalRoleCodes={code}");

            var url = $"{options.BaseAddress}Library/FunctionalRolesByCodes" +
                      $"?plantId={_request.Plant}" +
                      functionalRoleCodes +
                      $"&classification={Classification}" +
                      $"&api-version={options.ApiVersion}";

            var functionalRoles = new List<MainApiFunctionalRole>
            {
                new MainApiFunctionalRole 
                {
                        Code = "CodeA",
                        Description = "DescriptionA",
                        Email = "Test1@email.com",
                        InformationEmail = "TestInfo1@email.com",
                        UsePersonalEmail = true,
                        Persons = _persons
                },
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

            Assert.AreEqual("CodeA", result.Data.ElementAt(0).Code);
            Assert.AreEqual("DescriptionA", result.Data.ElementAt(0).Description);
            Assert.AreEqual("Test1@email.com", result.Data.ElementAt(0).Email);
            Assert.AreEqual("TestInfo1@email.com", result.Data.ElementAt(0).InformationEmail);
            var fr1UsePersonalEmail = result.Data.ElementAt(0).UsePersonalEmail;
            Assert.IsTrue(fr1UsePersonalEmail != null && fr1UsePersonalEmail.Value);
            Assert.AreEqual(_persons, result.Data.ElementAt(0).Persons);
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
