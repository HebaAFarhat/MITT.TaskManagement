using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MITT.API.Controllers;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services;
using MITT.Services.Abstracts;
using Moq;

namespace TestUnit;

[TestFixture, Order(1)]
public class ProjectServiceTests : BaseTestsService
{
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
        var controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
        controller.AddOrUpdateProject(newProject).Wait();
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
    public void Add_ValidAssignManagersToProject()
    {
        // Arrange
        AssignManagersDto assignManagersDto = new() { ManagerIds = new List<string>() };
        var newManager = new ProjectManagerDto
        {
            FullName = "abdalsalam aweid",
            NickName = "absy",
            Email = "a.aweid@masarat.ly",
            Phone = "0924228018"
        };

        // Act
        var _controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
        var valueResult = _controller.AddOrUpdateManager(newManager).Result;

        var manager = _dbContext.Managers.FirstOrDefault(e => e.NickName == "absy") ?? throw new Exception("manager returned null!!");
        Project project = Project.Create("name", "nickname", ProjectType.MB, Bank.Ismali);
        _dbContext.AssignedManagers.Add(AssignedManager.Create(project, manager));
        _dbContext.SaveChanges();

        assignManagersDto.ManagerIds.Add(manager.Id.ToString());
        assignManagersDto.ProjectId = project.Id.ToString();

        var controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
        var result = controller.AssignManagersToProject(assignManagersDto).Result;

        // Assert
        Assert.That(manager, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo(OperationResult.ResultType.Success));
        Assert.Multiple(() =>
        {
            Assert.That(project, Is.Not.Null);

            Assert.That(result, Is.InstanceOf<OperationResult>());
        });
    }

    [Test]
    public void Add_FailedAddOrUpdateProjectWhenBankAndTypeDuplicate()
    {
        // Arrange
        var firstProject = new ProjectDto
        {
            Bank = Bank.Wahda,
            ProjectType = ProjectType.MB,
            Description = "Test",
            Name = "TestName"
        };
        var secondProject = new ProjectDto
        {
            Bank = Bank.Wahda,
            ProjectType = ProjectType.MB,
            Description = "Test",
            Name = "TestName"
        };

        // Act
        var controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
        var firstDataResult = controller.AddOrUpdateProject(firstProject).Result;
        var secondDataResult = controller.AddOrUpdateProject(secondProject).Result;

        // Assert
        Assert.That(firstDataResult, Is.Not.Null);
        Assert.That(firstDataResult, Is.InstanceOf<OperationResult>());
        Assert.That(firstDataResult.Type, Is.EqualTo(OperationResult.ResultType.Success));

        Assert.That(secondDataResult, Is.Not.Null);
        Assert.That(secondDataResult, Is.InstanceOf<OperationResult>());
        Assert.That(secondDataResult.Type, Is.EqualTo(OperationResult.ResultType.Failure));
    }
}