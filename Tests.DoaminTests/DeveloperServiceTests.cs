using Microsoft.EntityFrameworkCore;
using MITT.API.Controllers;
using MITT.EmployeeDb.Models;
using MITT.EmployeeDb;
using MITT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITT.Services.Abstracts;

namespace TestUnit;

[TestFixture]
internal class DeveloperServiceTests
{
    private DbContextOptions<ManagementDb> _options;
    private ManagementDb _dbContext;
    private DeveloperController _controller;

    private readonly List<Developer> SeedData = new()
    {
        Developer.Create("name1", "nickName1", "email1", "phone1", "pin1", DeveloperType.Rv),
        Developer.Create("name2", "nickName2", "email2", "phone2", "pin2", DeveloperType.Be),
    };

    [SetUp]
    public void Setup()
    {
        // Set up an in-memory database for testing
        _options = new DbContextOptionsBuilder<ManagementDb>()
            .UseInMemoryDatabase(databaseName: $"{nameof(ManagementDb)}-Test")
            .Options;
        _dbContext = new ManagementDb(_options);

        // Seed test data
        _dbContext.Developers.AddRange(SeedData);
        _dbContext.SaveChanges();

        _controller = new DeveloperController(new DeveloperService(_dbContext));
    }

    [Test]
    public void GetDevelopersReturnValueOrEmptyList()
    {
        // Act
        var result = _controller.Developers().Result;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<DeveloperVm>>());
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0].FullName, Is.EqualTo("name2"));
            Assert.That(result[1].FullName, Is.EqualTo("name1"));
            Assert.That(result[0].NickName, Is.EqualTo("nickName2"));
            Assert.That(result[1].NickName, Is.EqualTo("nickName1"));
        });
    }

    [Test]
    public void Add_ValidAddOrUpdateDeveloper()
    {
        // Arrange
        var newProject = new DeveloperDto
        {
            FullName = "FullName",
            NickName = "NickName",
            Email = "Email",
            Phone = "Phone",
            Pin = "Pin",
            Type = DeveloperType.Qa
        };

        // Act
        _controller.AddOrUpdateDeveloper(newProject).Wait();
        var result = _dbContext.Developers.FirstOrDefault(e => e.Type == newProject.Type);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.FullName, Is.EqualTo(newProject.FullName));
            Assert.That(result.Type, Is.EqualTo(newProject.Type));
        });
    }

    //[Test]
    //public void Add_FailedAddOrUpdateProjectWhenBankAndTypeDuplicate()
    //{
    //    // Arrange
    //    var newProject = new DeveloperDto
    //    {
    //    };

    // // Act var dataResult = _controller.AddOrUpdateDeveloper(newProject).Result;

    //    // Assert
    //    Assert.That(dataResult, Is.Not.Null);
    //    Assert.That(dataResult, Is.InstanceOf<OperationResult>());
    //    Assert.That(dataResult.Type, Is.EqualTo(OperationResult.ResultType.Failure));
    //}

    [TearDown]
    public void TearDown()
    {
        // Clean up the in-memory database after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}