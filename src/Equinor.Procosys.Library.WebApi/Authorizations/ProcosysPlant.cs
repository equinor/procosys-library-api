using System.Diagnostics;

namespace Equinor.Procosys.Library.WebApi.Authorizations
{
    [DebuggerDisplay("{Title} ({Id})")]
    public class ProcosysPlant
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
