namespace Equinor.Procosys.Library.WebApi.Misc
{
    public class KeyVaultSettings
    {
        public bool Enabled { get; set; } = true;
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Uri { get; set; }
    }
}
