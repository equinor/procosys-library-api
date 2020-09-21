using System.Collections.Generic;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class FunctionalRoleDto
    {
        public FunctionalRoleDto(
            string code,
            string description,
            IEnumerable<PersonsInFunctionalRole> persons)
        {
            Code = code;
            Description = description;
            Persons = persons;
        }

        public string Code { get; }
        public string Description { get; }
        public IEnumerable<PersonsInFunctionalRole> Persons { get; }
    }
}
