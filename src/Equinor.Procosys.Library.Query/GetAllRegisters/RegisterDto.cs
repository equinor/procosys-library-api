namespace Equinor.Procosys.Library.Query.GetAllRegisters
{
    public class RegisterDto
    {
        public RegisterDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; }
        public string Description { get; }
    }
}
