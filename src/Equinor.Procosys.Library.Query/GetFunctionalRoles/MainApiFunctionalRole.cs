using System.Collections.Generic;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class MainApiFunctionalRole
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public IEnumerable<PersonsInFunctionalRole> Persons { get; set; }
    }
}
