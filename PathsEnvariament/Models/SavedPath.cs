namespace PathsEnvariament.Models;

public sealed class SavedPath
{
    public SavedPath(string name, string path)
    {
        Name = NormalizeName(name);
        Path = path.Trim().Trim('"');
    }

    public string Name { get; set; }
    public string Path { get; set; }

    public static string NormalizeName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().Trim('"').TrimStart('$');
    }
}
