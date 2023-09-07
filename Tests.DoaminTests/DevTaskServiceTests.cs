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
using Microsoft.AspNetCore.Mvc;
using MITT.Services.TaskServices;

namespace TestUnit;

[TestFixture, Order(3)]
internal class DevTaskServiceTests : BaseTestsService
{
    [Test, Order(1)]
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
        var controller = new DeveloperController(new DeveloperService(_dbContext));
        controller.AddOrUpdateDeveloper(newProject).Wait();
        var result = _dbContext.Developers.FirstOrDefault(e => e.Type == newProject.Type);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.FullName, Is.EqualTo(newProject.FullName));
            Assert.That(result.Type, Is.EqualTo(newProject.Type));
        });
    }

    [Test, Order(2)]
    public void Add_ValidAddDevTask()
    {
        // Arrange
        var newTask = new TaskDto
        {
            ImplementationType = ImplementationType.Implementation,
            Name = "Test",
            Description = "Test",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now,
            MainBranch = "Test",
            MergeBranch = "Test",
            Requirements = new List<string> { "Test" }
        };

        // Act
        Manager manager = Manager.Create("name", "nickname", "email", "phone");
        _dbContext.Managers.Add(manager);
        Project project = Project.Create("name", "nickname", ProjectType.MB, Bank.Ismali);

        _dbContext.AssignedManagers.Add(AssignedManager.Create(project, manager));
        _dbContext.SaveChanges();
        newTask.AssignedManagerId = _dbContext.AssignedManagers.First().Id.ToString();

        var taskService = new TaskService(_dbContext);
        var newTaskResult = taskService.AddTask(newTask, true).Result;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(newTaskResult, Is.Not.Null);
            Assert.That(newTaskResult, Is.TypeOf<OperationResult>());
            Assert.That(newTaskResult.Type, Is.EqualTo(OperationResult.ResultType.Success));
        });
    }

    [Test, Order(3)]
    public void Add_ValidAddDevTaskReview()
    {
        // Arrange
        AddReviewDto addReviewDto = new()
        {
            AssignDevType = AssignDevType.Be,
            AssignedTaskId = "",
            Findings = new List<ReviewFinding> { new ReviewFinding() { } }
        };

        // Act
        var assignedManager = AssignedManager.Create(Project.Create("name", "description", ProjectType.OT, Bank.NorthAfrica), Manager.Create("name", "nick-name", "email", "phone"));
        _dbContext.AssignedManagers.Add(assignedManager);
        DevTask devTask = DevTask.Create("seq",
                                         "name",
                                         "desc",
                                         "main-branch",
                                         "merge-branch",
                                         DateTime.Now,
                                         DateTime.Now,
                                         ImplementationType.Implementation,
                                         new List<string>(),
                                         assignedManager.Id);
        _dbContext.Tasks.Add(devTask);
        Developer developer = Developer.Create("name", "nickName", "email", "phone", "pin", DeveloperType.Pm);
        _dbContext.Developers.Add(developer);
        _dbContext.AssignedBetasks.Add(AssignedBeTask.Create(devTask, developer));
        _dbContext.SaveChanges();

        addReviewDto.AssignedTaskId = _dbContext.AssignedBetasks.First().Id.ToString();

        var devTaskController = new TaskController(new TaskService(_dbContext), new AssignmentService(_dbContext), new ReviewService(_dbContext));
        var addReviewResult = devTaskController.AddReview(addReviewDto).Result;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addReviewResult, Is.Not.Null);
            Assert.That(addReviewResult, Is.TypeOf<OperationResult>());
            Assert.That(addReviewResult.Type, Is.EqualTo(OperationResult.ResultType.Success));
        });
    }
}