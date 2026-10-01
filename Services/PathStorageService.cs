using System.Text.Json;
using PathsEnvariament.Configuration;
using PathsEnvariament.Models;

namespace PathsEnvariament.Services;

public static class PathStorageService
{
    public static string GetStoragePath()
    {
        return AppConfig.StoragePath;
    }

    public static List<SavedPath> Load(string storagePath)
    {
        if (!File.Exists(storagePath))
        {
            return [];
        }

        try
        {
            var json = File.ReadAllText(storagePath);
            return JsonSerializer.Deserialize<List<SavedPath>>(json) ?? [];
        }
        catch (JsonException)
        {
            ConsoleUi.WriteError($"Opslagbestand is ongeldig: {storagePath}");
            return [];
        }
    }

    public static void Save(string storagePath, List<SavedPath> paths)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(storagePath, JsonSerializer.Serialize(paths, options));
    }
}
