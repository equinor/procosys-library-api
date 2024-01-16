using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class FunctionalRoleDto
    {
        public FunctionalRoleDto(
            Guid proCoSysGuid,
            string code,
            string description,
            string email,
            string informationEmail,
            bool? usePersonalEmail,
            IEnumerable<PersonInFunctionalRole> persons)
        {
            ProCoSysGuid = proCoSysGuid;
            Code = code;
            Description = description;
            Email = email;
            InformationEmail = informationEmail;
            UsePersonalEmail = usePersonalEmail;
            Persons = persons;
        }

        public Guid ProCoSysGuid { get; }
        public string Code { get; }
        public string Description { get; }
        public string Email { get; }
        public string InformationEmail { get; }
        public bool? UsePersonalEmail { get; }
        public IEnumerable<PersonInFunctionalRole> Persons { get; }
    }
}
