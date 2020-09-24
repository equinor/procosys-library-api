using System.Collections.Generic;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class MainApiFunctionalRole
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string InformationEmail { get; set; }
        public bool? UsePersonalEmail { get; set; }
        public IEnumerable<PersonInFunctionalRole> Persons { get; set; }
    }
}
