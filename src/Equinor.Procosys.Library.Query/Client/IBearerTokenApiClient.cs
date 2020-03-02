using System.Threading.Tasks;

namespace Equinor.Procosys.Library.Query.Client
{
    public interface IBearerTokenApiClient
    {
        Task<T> QueryAndDeserialize<T>(string url);
    }
}
