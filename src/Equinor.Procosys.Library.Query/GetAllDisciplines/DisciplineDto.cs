﻿namespace Equinor.Procosys.Library.Query.GetAllDisciplines
{
    public class DisciplineDto
    {
        public DisciplineDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; }
        public string Description { get; }
    }
}
