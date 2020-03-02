using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Library.Domain
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
