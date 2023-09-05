using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb.Models;
using MITT.EmployeeDb;

namespace TestUnit;

public enum RunType
{
    SqlServer, InMemory
}

public static class TestConfig
{
    public static RunType RunType = RunType.SqlServer;
}

[TestFixture]
public class BaseTestsService
{
    protected DbContextOptions<ManagementDb> _options;
    protected ManagementDb _dbContext;

    [SetUp]
    public void Setup()
    {
        // Set up an database for testing

        if (TestConfig.RunType == RunType.SqlServer)
        {
            _options = new DbContextOptionsBuilder<ManagementDb>()
                                        .UseSqlServer($"Data Source =.; Initial Catalog = Test-{nameof(ManagementDb)}; Trusted_Connection = true;TrustServerCertificate=True;")
                                        .Options;
            _dbContext = new ManagementDb(_options);

            var deletedResult = _dbContext.Database.EnsureDeleted();
            _dbContext.Database.Migrate();
        }
        else
        {
            _options = new DbContextOptionsBuilder<ManagementDb>()
                                    .UseInMemoryDatabase($"Test-{nameof(ManagementDb)}")
                                    .Options;
            _dbContext = new ManagementDb(_options);

            var deletedResult = _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the database after each test
        _dbContext.Database.EnsureDeletedAsync().Wait();
        _dbContext.Dispose();
    }
}