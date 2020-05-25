namespace Equinor.Procosys.Library.Query.GetAllResponsibles
{
    public class ResponsibleDto
    {
        public ResponsibleDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; }
        public string Description { get; }
    }
}
