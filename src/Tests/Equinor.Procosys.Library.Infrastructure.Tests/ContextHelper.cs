using Equinor.Procosys.Library.Domain;
using Equinor.Procosys.Library.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Equinor.Procosys.Library.Infrastructure.Tests
{
    public class ContextHelper
    {
        public ContextHelper()
        {
            DbOptions = new DbContextOptions<LibraryContext>();
            EventDispatcherMock = new Mock<IEventDispatcher>();
            PlantProviderMock = new Mock<IPlantProvider>();
            ContextMock = new Mock<LibraryContext>(DbOptions, EventDispatcherMock.Object, PlantProviderMock.Object);
        }

        public DbContextOptions<LibraryContext> DbOptions { get; }
        public Mock<IEventDispatcher> EventDispatcherMock { get; }
        public Mock<IPlantProvider> PlantProviderMock { get; }
        public Mock<LibraryContext> ContextMock { get; }
    }
}
