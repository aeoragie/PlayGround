namespace Generator.Database.Configuration
{
    public class CodeGenerationSettings
    {
        public string CommonPath { get; set; } = string.Empty;
        public Dictionary<string, DatabaseOptions> Databases { get; set; } = new();
    }
}
