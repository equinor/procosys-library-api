using System.Linq;

namespace Equinor.Procosys.Library.Infrastructure
{
    public interface IReadOnlyContext
    {
        IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class;
    }
}
