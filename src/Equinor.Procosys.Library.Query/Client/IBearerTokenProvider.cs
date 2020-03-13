using System.Threading.Tasks;

namespace Equinor.Procosys.Library.Query.Client
{
    public interface IBearerTokenProvider
    {
        Task<string> GetBearerTokenAsync();
    }
}
