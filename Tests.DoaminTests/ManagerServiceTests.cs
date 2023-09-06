using MITT.API.Controllers;
using MITT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITT.Services.Abstracts;
using MITT.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace TestUnit;

[TestFixture, Order(2)]
internal class ManagerServiceTests : BaseTestsService
{
    [Test]
    public void GetManagersReturnValue()
    {
        // Act
        var controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
        var result = controller.Managers().Result;

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
        var controller = new ProjectsController(new ProjectsService(_dbContext), new ManagerService(_dbContext));
        var valueResult = controller.AddOrUpdateManager(newManager).Result;
        var result = _dbContext.Managers.FirstOrDefault(e => e.NickName == "absy");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(valueResult, Is.TypeOf<OperationResult>());
            Assert.That(valueResult.Type, Is.EqualTo(OperationResult.ResultType.Success));
            Assert.That(result.NickName, Is.EqualTo(newManager.NickName));
            Assert.That(result.Phone, Is.EqualTo(newManager.Phone));
        });
    }
}