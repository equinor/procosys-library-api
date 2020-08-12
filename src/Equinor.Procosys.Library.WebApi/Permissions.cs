namespace Equinor.Procosys.Library.WebApi
{
    public class Permissions
    {
        public static readonly string LIBRARY_GENERAL = "LIBRARY_GENERAL";
        public static readonly string LIBRARY_GENERAL_READ = $"{LIBRARY_GENERAL}/READ";
        public static readonly string LIBRARY_GENERAL_WRITE = $"{LIBRARY_GENERAL}/WRITE";
    }
}
