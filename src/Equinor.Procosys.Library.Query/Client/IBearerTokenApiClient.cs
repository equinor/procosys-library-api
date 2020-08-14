using System.Threading.Tasks;

namespace Equinor.Procosys.Library.Query.Client
{
    public interface IBearerTokenApiClient
    {
        Task<T> QueryAndDeserializeAsync<T>(string url);
    }
}
