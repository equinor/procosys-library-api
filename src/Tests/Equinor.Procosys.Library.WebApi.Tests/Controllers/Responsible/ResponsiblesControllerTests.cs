using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.GetAllResponsibles;
using Equinor.Procosys.Library.WebApi.Controllers.Responsible;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.WebApi.Tests.Controllers
{
    [TestClass]
    public class ResponsiblesControllerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void Setup() => _mediatorMock = new Mock<IMediator>();

        [TestMethod]
        public async Task GetAllResponsiblesAsync_Returns200Ok()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetAllResponsiblesQuery>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.FromResult(
                        (Result<IEnumerable<ResponsibleDto>>)new SuccessResult<IEnumerable<ResponsibleDto>>(
                            new List<ResponsibleDto>
                                {
                                    new ResponsibleDto("CodeA", "DescriptionA"),
                                    new ResponsibleDto("CodeB", "DescriptionB"),
                                })));

            var dut = new ResponsiblesController(_mediatorMock.Object);

            var result = await dut.GetAllResponsiblesAsync(Plant);

            Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result.Result).StatusCode);
        }

        [TestMethod]
        public async Task GetAllResponsiblesAsync_ReturnsArrayOfElements()
        {
            var responsibles = new List<ResponsibleDto>
                {
                    new ResponsibleDto("CodeA", "DescriptionA"),
                    new ResponsibleDto("CodeB", "DescriptionB"),
                };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetAllResponsiblesQuery>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.FromResult(
                        (Result<IEnumerable<ResponsibleDto>>)new SuccessResult<IEnumerable<ResponsibleDto>>(responsibles)));

            var dut = new ResponsiblesController(_mediatorMock.Object);

            var result = await dut.GetAllResponsiblesAsync(Plant);

            var items = (IEnumerable<ResponsibleDto>)((OkObjectResult)result.Result).Value;
            Assert.AreEqual(2, items.Count());
            Assert.AreEqual(responsibles[0], items.ElementAt(0));
            Assert.AreEqual(responsibles[1], items.ElementAt(1));
        }
    }
}
