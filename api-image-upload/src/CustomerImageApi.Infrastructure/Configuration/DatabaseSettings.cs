namespace CustomerImageApi.Infrastructure.Configuration;

public class DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";
    
    public bool AutoMigrateOnStartup { get; set; } = true;
    public bool CreateDatabaseIfNotExists { get; set; } = true;
    public bool ThrowOnMigrationError { get; set; } = false;
}