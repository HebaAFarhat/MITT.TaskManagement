using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;
using MITT.Services.Helpers;
using System.Collections.Generic;

namespace MITT.Services;

public class ManagerService : ManagementService<Manager>, IManagerService
{
    private readonly ManagementDb _managementDb;

    public ManagerService(ManagementDb managementDb) : base(managementDb) => _managementDb = managementDb;

    public async Task<List<ProjectManagerVm>> Managers(ProjectTypeVm projectType, CancellationToken cancellationToken = default)
    {
        var list = new List<ProjectManagerVm>();

        var type = Parse(projectType);

        var managers = projectType == ProjectTypeVm.All ?
            await _managementDb.Managers.Include(x => x.AssignedManagers).ThenInclude(x => x.Project).Select(x => new AssignedManager
            {
                Id = x.Id,
                ProjectManager = x,
            }).ToListAsync(cancellationToken) :
            await _managementDb.AssignedManagers
            .Include(x => x.ProjectManager)
            .Include(x => x.Project)
            .Where(x => x.Project.ProjectType == type)
            .ToListAsync(cancellationToken);

        foreach (var manager in managers) list.Add(new ProjectManagerVm
        {
            Id = manager.Id.ToString(),
            FullName = manager.ProjectManager.FullName,
            NickName = manager.ProjectManager.NickName,
            Phone = manager.ProjectManager.Phone,
            Email = manager.ProjectManager.Email,
            ActiveState = manager.ProjectManager.ActiveState,
            ActiveTasks = await GetAssignedTasks(manager.ProjectManager.Id, cancellationToken)
        });

        return list.OrderByDescending(x => x.ActiveTasks).ToList();
    }

    public async Task<ManagersByProjectVm> Managers(string projectId, CancellationToken cancellationToken = default)
    {
        var assignedManagers = new List<ProjectManagerVm>();
        var freeManagers = new List<ProjectManagerVm>();

        var assignedManagersData = await _managementDb.AssignedManagers
            .Include(x => x.ProjectManager)
            .Where(x => x.ProjectId == Guid.Parse(projectId))
            .ToListAsync(cancellationToken);

        foreach (var manager in assignedManagersData) assignedManagers.Add(new ProjectManagerVm
        {
            Id = manager.Id.ToString(),
            FullName = manager.ProjectManager.FullName,
            NickName = manager.ProjectManager.NickName,
            Phone = manager.ProjectManager.Phone,
            Email = manager.ProjectManager.Email,
            ActiveState = manager.ProjectManager.ActiveState,
            ActiveTasks = await GetAssignedTasks(manager.ProjectManager.Id, cancellationToken)
        });

        var freeManagersData = await _managementDb.Managers
            .Where(x => x.ActiveState == ActiveState.Active)
            .ToListAsync(cancellationToken);

        foreach (var manager in freeManagersData)
        {
            if (!assignedManagers.Select(x => x.FullName).ToList().Contains(manager.FullName)) freeManagers.Add(new ProjectManagerVm
            {
                Id = manager.Id.ToString(),
                FullName = manager.FullName,
                NickName = manager.NickName,
                Phone = manager.Phone,
                Email = manager.Email,
                ActiveState = manager.ActiveState
            });
        }

        return new ManagersByProjectVm
        {
            AssignedManagers = assignedManagers.OrderByDescending(x => x.ActiveTasks).ToList(),
            FreeManagers = freeManagers.OrderByDescending(x => x.ActiveTasks).ToList()
        };
    }

    public async Task<OperationResult> AddOrUpdateManager(ProjectManagerDto projectManagerDto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(projectManagerDto.Id))
        {
            var entity = Manager.Create(projectManagerDto.FullName, projectManagerDto.NickName, projectManagerDto.Email, projectManagerDto.Phone);

            await Add(entity, cancellationToken);
            return OperationResult.Valid();
        }

        var manager = await _managementDb.Managers.FirstOrDefaultAsync(x => x.Id == Guid.Parse(projectManagerDto.Id), cancellationToken);

        if (manager is null) throw new Exception($"invalid_{manager}_id!!");

        manager.Update(projectManagerDto?.FullName, projectManagerDto?.NickName, projectManagerDto?.Phone, projectManagerDto?.Email);

        await Update(manager, cancellationToken);
        return OperationResult.Valid();
    }

    public async Task<OperationResult> AssignManagersToProject(AssignManagersDto assignManagersDto, CancellationToken cancellationToken = default)
    {
        var managerIdList = assignManagersDto.ManagerIds.Select(Guid.Parse);

        var project = await _managementDb.Projects
            .Include(x => x.AssignedManagers)
            .ThenInclude(x => x.ProjectManager)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(assignManagersDto.ProjectId), cancellationToken);

        if (project is null) throw new Exception($"{nameof(project)} is null!!");

        var managers = await ValidateManagerList(managerIdList, project, cancellationToken);

        foreach (var manager in managers) project.AssignedManagers.Add(AssignedManager.Create(project, manager));

        await Save(cancellationToken);
        return OperationResult.Valid();
    }

    public async Task<OperationResult> ChangeState(string managerId, CancellationToken cancellationToken = default)
    {
        var developer = await _managementDb.Managers.FirstOrDefaultAsync(x => x.Id == Guid.Parse(managerId), cancellationToken) ?? throw new Exception("invalid_develoepr_id_provided!!");
        if (developer.ActiveState == ActiveState.Active) developer.ActiveState = ActiveState.Inactive;
        else developer.ActiveState = ActiveState.Active;

        await Update(developer, cancellationToken);
        return OperationResult.Valid();
    }

    private async Task<List<Manager>> ValidateManagerList(IEnumerable<Guid> managerIdList, Project project, CancellationToken cancellationToken)
    {
        var managers = new List<Manager>();
        foreach (var managerId in managerIdList)
        {
            var manager = await Get(managerId, cancellationToken);
            if (manager is null) continue;

            if (!project.AssignedManagers.Any(assignedManager => managerIdList.Contains(assignedManager.ProjectManagerId))) managers.Add(manager);
        }
        return managers;
    }

    private async Task<int> GetAssignedTasks(Guid managerId, CancellationToken cancellationToken)
    {
        var assignedProjectCount = 0;
        var assignedProjects = await _managementDb.AssignedManagers
            .Where(x => x.ProjectManagerId == managerId)
            .ToListAsync(cancellationToken);

        foreach (var projectManager in assignedProjects)
        {
            var count = await _managementDb.Tasks
                .Where(devTask => devTask.AssignedManagerId == projectManager.Id && devTask.TaskState == TaskState.Pending)
                .CountAsync(cancellationToken);

            assignedProjectCount += count;
        }

        return assignedProjectCount;
    }

    private static ProjectType? Parse(ProjectTypeVm projectTypeVm) => projectTypeVm switch
    {
        ProjectTypeVm.Mb => ProjectType.MB,
        ProjectTypeVm.Py => ProjectType.PY,
        ProjectTypeVm.Wb => ProjectType.WB,
        ProjectTypeVm.Ot => ProjectType.OT,
        ProjectTypeVm.All => null,
        _ => null,
    };
}

public enum ProjectTypeVm
{
    Mb = 1, Py, Wb, Ot, All
}