namespace SmartPOS.Services;

/// <summary>
/// Interface for data seeding services
/// </summary>
public interface IDataSeedingService
{
    /// <summary>
    /// Seeds initial data like default roles and admin user
    /// </summary>
    Task SeedInitialDataAsync();
    
    /// <summary>
    /// Seeds sample data for development/testing
    /// </summary>
    Task SeedSampleDataAsync();
}