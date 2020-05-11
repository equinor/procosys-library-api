namespace Equinor.Procosys.Library.Query.GetTagFunctions
{
    public class TagFunctionDto
    {
        public TagFunctionDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; }
        public string Description { get; }
    }
}
