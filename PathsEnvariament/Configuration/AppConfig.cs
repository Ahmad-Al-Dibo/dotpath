namespace PathsEnvariament.Configuration;

public static class AppConfig
{
    public const string StorageFileName = "paths.json";

    public static string StorageDirectory => AppContext.BaseDirectory;

    public static string StoragePath => Path.Combine(StorageDirectory, StorageFileName);

    public static string PromptText => "";
}
