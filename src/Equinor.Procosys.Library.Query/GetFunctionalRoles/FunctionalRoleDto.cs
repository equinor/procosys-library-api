using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class FunctionalRoleDto
    {
        public FunctionalRoleDto(
            string code,
            Guid proCoSysGuid,
            string description,
            string email,
            string informationEmail,
            bool? usePersonalEmail,
            IEnumerable<PersonInFunctionalRole> persons)
        {
            Code = code;
            ProCoSysGuid = proCoSysGuid;
            Description = description;
            Email = email;
            InformationEmail = informationEmail;
            UsePersonalEmail = usePersonalEmail;
            Persons = persons;
        }

        public string Code { get; }
        public Guid ProCoSysGuid { get; }
        public string Description { get; }
        public string Email { get; }
        public string InformationEmail { get; }
        public bool? UsePersonalEmail { get; }
        public IEnumerable<PersonInFunctionalRole> Persons { get; }
    }
}
