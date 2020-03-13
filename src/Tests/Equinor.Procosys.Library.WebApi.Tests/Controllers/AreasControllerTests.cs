using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.GetAllAreas;
using Equinor.Procosys.Library.WebApi.Controllers.Area;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Library.WebApi.Tests.Controllers
{
    [TestClass]
    public class AreasControllerTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void Setup() => _mediatorMock = new Mock<IMediator>();

        [TestMethod]
        public async Task GetAllAreasAsync_Returns200Ok()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetAllAreasQuery>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.FromResult(
                        (Result<IEnumerable<AreaDto>>)new SuccessResult<IEnumerable<AreaDto>>(
                            new List<AreaDto>
                                {
                                    new AreaDto("CodeA", "DescriptionA"),
                                    new AreaDto("CodeB", "DescriptionB"),
                                })));

            var dut = new AreasController(_mediatorMock.Object);

            var result = await dut.GetAllAreasAsync(Plant);

            Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result.Result).StatusCode);
        }

        [TestMethod]
        public async Task GetAllAreasAsync_ReturnsArrayOfElements()
        {
            var areas = new List<AreaDto>
                {
                    new AreaDto("CodeA", "DescriptionA"),
                    new AreaDto("CodeB", "DescriptionB"),
                };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetAllAreasQuery>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.FromResult(
                        (Result<IEnumerable<AreaDto>>)new SuccessResult<IEnumerable<AreaDto>>(areas)));

            var dut = new AreasController(_mediatorMock.Object);

            var result = await dut.GetAllAreasAsync(Plant);

            var items = (IEnumerable<AreaDto>)((OkObjectResult)result.Result).Value;
            Assert.AreEqual(2, items.Count());
            Assert.AreEqual(areas[0], items.ElementAt(0));
            Assert.AreEqual(areas[1], items.ElementAt(1));
        }
    }
}
