using CustomerImageApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApi.API.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Automatically applies any pending database migrations on application startup.
    /// This ensures the database is always up-to-date without manual intervention.
    /// Uses proper migration-based database creation to avoid migration history issues.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="logger">Optional logger for migration information</param>
    /// <returns>The web application for method chaining</returns>
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app, ILogger? logger = null)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var config = app.Configuration;
        var autoMigrate = config.GetValue<bool>("DatabaseSettings:AutoMigrateOnStartup", true);
        var throwOnError = config.GetValue<bool>("DatabaseSettings:ThrowOnMigrationError", false);
        
        if (!autoMigrate)
        {
            logger?.LogInformation("Auto-migration is disabled in configuration. Skipping database migration.");
            return app;
        }
        
        try
        {
            logger?.LogInformation("Starting automatic database migration process...");
            
            // Check if database exists, but don't create it yet
            var databaseExists = await DatabaseExistsAsync(context, logger);
            
            if (!databaseExists)
            {
                logger?.LogInformation("Database does not exist. It will be created through migrations.");
            }
            
            // Get migration information
            var allMigrations = context.Database.GetMigrations();
            logger?.LogDebug("Found {Count} total migrations in assembly: {Migrations}", 
                allMigrations.Count(), 
                string.Join(", ", allMigrations));
            
            // Get applied migrations (will be empty if database doesn't exist)
            var appliedMigrations = await GetAppliedMigrationsAsync(context, logger);
            logger?.LogDebug("Found {Count} already applied migrations: {AppliedMigrations}", 
                appliedMigrations.Count(), 
                string.Join(", ", appliedMigrations));
            
            // Get pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                logger?.LogInformation("Found {Count} pending migrations that need to be applied: {Migrations}", 
                    pendingMigrations.Count(), 
                    string.Join(", ", pendingMigrations));
                
                // Apply migrations - this will create the database if it doesn't exist
                // and properly record all migrations in the history table
                await context.Database.MigrateAsync();
                
                logger?.LogInformation("? Database migrations applied successfully!");
                
                // Verify final state
                var finalAppliedMigrations = await context.Database.GetAppliedMigrationsAsync();
                logger?.LogInformation("Total applied migrations after update: {Count}", finalAppliedMigrations.Count());
            }
            else
            {
                logger?.LogInformation("? Database is up-to-date. No pending migrations found.");
                
                if (appliedMigrations.Any())
                {
                    logger?.LogDebug("Database has {Count} migrations already applied: {AppliedMigrations}", 
                        appliedMigrations.Count(), 
                        string.Join(", ", appliedMigrations.Take(5)) + (appliedMigrations.Count() > 5 ? "..." : ""));
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "? An error occurred while migrating the database: {Message}", ex.Message);
            
            if (throwOnError || app.Environment.IsProduction())
            {
                logger?.LogCritical("Migration failed and ThrowOnMigrationError is enabled. Application will terminate.");
                throw;
            }
            
            logger?.LogWarning("??  Migration failed but application will continue (non-production environment)...");
        }
        
        return app;
    }
    
    /// <summary>
    /// Checks if the database exists without creating it
    /// </summary>
    private static async Task<bool> DatabaseExistsAsync(ApplicationDbContext context, ILogger? logger)
    {
        try
        {
            logger?.LogDebug("Checking if database exists...");
            
            // Try to connect to the database
            var canConnect = await context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                logger?.LogDebug("Database connection successful - database exists.");
                return true;
            }
            else
            {
                logger?.LogDebug("Cannot connect to database - database does not exist.");
                return false;
            }
        }
        catch (Exception ex)
        {
            logger?.LogDebug("Error checking database existence: {Message}", ex.Message);
            return false;
        }
    }
    
    /// <summary>
    /// Safely gets applied migrations, handling the case where database doesn't exist
    /// </summary>
    private static async Task<IEnumerable<string>> GetAppliedMigrationsAsync(ApplicationDbContext context, ILogger? logger)
    {
        try
        {
            return await context.Database.GetAppliedMigrationsAsync();
        }
        catch (Exception ex)
        {
            logger?.LogDebug("Could not get applied migrations (likely database doesn't exist): {Message}", ex.Message);
            return Enumerable.Empty<string>();
        }
    }
    
    /// <summary>
    /// Alternative method for environments where you want more control
    /// </summary>
    public static async Task<WebApplication> MigrateDatabaseWithOptionsAsync(
        this WebApplication app, 
        Action<DatabaseMigrationOptions>? configure = null)
    {
        var options = new DatabaseMigrationOptions();
        configure?.Invoke(options);
        
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            if (options.ApplyMigrations)
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully!");
                }
            }
            
            if (options.SeedData != null)
            {
                logger.LogInformation("Seeding database...");
                await options.SeedData(context);
                logger.LogInformation("Database seeding completed!");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed");
            if (options.ThrowOnError)
                throw;
        }
        
        return app;
    }
}

public class DatabaseMigrationOptions
{
    public bool ApplyMigrations { get; set; } = true;
    public bool ThrowOnError { get; set; } = false;
    public Func<ApplicationDbContext, Task>? SeedData { get; set; }
}