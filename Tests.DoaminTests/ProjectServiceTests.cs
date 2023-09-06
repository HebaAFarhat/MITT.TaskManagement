using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MITT.API.Controllers;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services;
using MITT.Services.Abstracts;
using Moq;

namespace TestUnit;

[TestFixture]
public class ProjectServiceTests
{
    private DbContextOptions<ManagementDb> _options;
    private ManagementDb _dbContext;
    private ProjectsController _controller;

    private readonly List<Project> SeedData = new()
    {
        Project.Create(Bank.Jumhoria.ToString(), ProjectType.OT.ToString(), ProjectType.OT, Bank.Jumhoria),
        Project.Create(Bank.Wahda.ToString(), ProjectType.MB.ToString(), ProjectType.MB, Bank.Wahda),
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
        _dbContext.Projects.AddRange(SeedData);
        _dbContext.SaveChanges();

        _controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
    }

    [Test]
    public void GetProjectsReturnValueOrEmptyList()
    {
        // Act
        var result = _controller.Projects().Result;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<ProjectVm>>());
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Name, Is.EqualTo(Bank.Wahda.ToString()));
            Assert.That(result[1].Name, Is.EqualTo(Bank.Jumhoria.ToString()));
        });
    }

    [Test]
    public void Add_ValidAddOrUpdateProject()
    {
        // Arrange
        var newProject = new ProjectDto
        {
            Bank = Bank.Sahara,
            ProjectType = ProjectType.MB,
            Description = "Test",
            Name = "TestName"
        };

        // Act
        _controller.AddOrUpdateProject(newProject).Wait();
        var result = _dbContext.Projects.FirstOrDefault(e => e.Bank == newProject.Bank);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Bank, Is.EqualTo(newProject.Bank));
            Assert.That(result.ProjectType, Is.EqualTo(newProject.ProjectType));
        });
    }

    [Test]
    public void Add_FailedAddOrUpdateProjectWhenBankAndTypeDuplicate()
    {
        // Arrange
        var newProject = new ProjectDto
        {
            Bank = Bank.Wahda,
            ProjectType = ProjectType.MB,
            Description = "Test",
            Name = "TestName"
        };

        // Act
        var dataResult = _controller.AddOrUpdateProject(newProject).Result;

        // Assert
        Assert.That(dataResult, Is.Not.Null);
        Assert.That(dataResult, Is.InstanceOf<OperationResult>());
        Assert.That(dataResult.Type, Is.EqualTo(OperationResult.ResultType.Failure));
    }

    [Test]
    public void GetManagersReturnValue()
    {
        // Act
        var result = _controller.Managers().Result;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<ProjectManagerVm>>());
        Assert.That(result, Has.Count.AtMost(1));
    }

    [Test]
    public void Add_ValidAddOrUpdateManager()
    {
        // Arrange
        var newManager = new ProjectManagerDto
        {
            FullName = "abdalsalam aweid",
            NickName = "absy",
            Email = "a.aweid@masarat.ly",
            Phone = "0924228018"
        };

        // Act
        var valueResult = _controller.AddOrUpdateManager(newManager).Result;
        var result = _dbContext.Managers.FirstOrDefault(e => e.NickName == "absy");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(valueResult, Is.TypeOf<OperationResult>());
            Assert.That(result.NickName, Is.EqualTo(newManager.NickName));
            Assert.That(result.Phone, Is.EqualTo(newManager.Phone));
        });
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the in-memory database after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}