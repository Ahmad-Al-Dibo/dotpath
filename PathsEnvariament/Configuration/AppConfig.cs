namespace PathsEnvariament.Configuration;

public static class AppConfig
{
    public const string StorageFileName = "paths.json";
    public const int DeveloperTokenMinDelayMs = 30;
    public const int DeveloperTokenMaxDelayMs = 190;
    public const int DeveloperTokenDelayReferenceTokenCount = 32;

    public static string StorageDirectory => AppContext.BaseDirectory;

    public static string StoragePath => Path.Combine(StorageDirectory, StorageFileName);

    public static string PromptText => "";
}
