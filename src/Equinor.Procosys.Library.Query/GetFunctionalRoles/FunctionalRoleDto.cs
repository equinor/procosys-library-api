using System.Collections.Generic;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class FunctionalRoleDto
    {
        public FunctionalRoleDto(
            string code,
            string description,
            string email,
            string informationEmail,
            bool? usePersonalEmail,
            IEnumerable<PersonInFunctionalRole> persons)
        {
            Code = code;
            Description = description;
            Email = email;
            InformationEmail = informationEmail;
            UsePersonalEmail = usePersonalEmail;
            Persons = persons;
        }

        public string Code { get; }
        public string Description { get; }
        public string Email { get; }
        public string InformationEmail { get; }
        public bool? UsePersonalEmail { get; }
        public IEnumerable<PersonInFunctionalRole> Persons { get; }
    }
}
