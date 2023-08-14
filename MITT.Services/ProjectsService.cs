using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;
using MITT.Services.Helpers;

namespace MITT.Services;

public class ProjectsService : ManagementService<Project>, IProjectsService
{
    private readonly ManagementDb _managementDb;

    public ProjectsService(ManagementDb managementDb) : base(managementDb) => _managementDb = managementDb;

    public async Task<List<ProjectVm>> Projects(CancellationToken cancellationToken = default)
    {
        var projectList = new List<ProjectVm>();

        var projects = await List(cancellationToken: cancellationToken);

        foreach (var project in projects)
        {
            var managerList = new List<ProjectManagerVm>();

            var managers = await _managementDb.AssignedManagers
                .Include(x => x.ProjectManager)
                .Where(x => x.ProjectId == project.Id)
                .ToListAsync(cancellationToken);

            foreach (var manager in managers) managerList.Add(new ProjectManagerVm
            {
                Id = manager.Id.ToString(),
                FullName = manager.ProjectManager.FullName,
                NickName = manager.ProjectManager.NickName,
                Email = manager.ProjectManager.Email,
                Phone = manager.ProjectManager.Phone,
                ActiveTasks = await ManagerActiveTasks(manager.Id, cancellationToken)
            });

            projectList.Add(new ProjectVm
            {
                Id = project.Id.ToString(),
                Name = project.Name,
                Description = project.Description,
                ProjectType = project.ProjectType,
                Managers = managerList
            });
        }

        return projectList
            .OrderBy(x => (int)x.ProjectType)
            .ThenByDescending(x => x.Managers.Count)
            .ToList();
    }

    public async Task<OperationResult> AddOrUpdateProject(ProjectDto projectDto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(projectDto.Id))
        {
            var entity = Project.Create(projectDto.Name, projectDto.Description, projectDto.ProjectType);

            await Add(entity, cancellationToken);

            return OperationResult.Valid();
        }

        var project = await _managementDb.Projects.FirstOrDefaultAsync(x => x.Id == Guid.Parse(projectDto.Id), cancellationToken);

        if (project is null) throw new Exception($"invalid_{project}_id!!");

        project.Update(projectDto?.Name, projectDto?.Description, projectDto.ProjectType);

        await Update(project, cancellationToken);
        return OperationResult.Valid();
    }

    public async Task<List<ProjectDto>> ProjectsToAssign(string managerId, CancellationToken cancellationToken = default)
    {
        List<ProjectDto> projectDtos = await _managementDb.Projects
                .Include(x => x.AssignedManagers)
                .Where(x => !x.AssignedManagers.Any(x => x.ProjectManagerId == Guid.Parse(managerId)))
                .Select(project => new ProjectDto
                {
                    Id = project.Id.ToString(),
                    Name = $"{project.ProjectType}  ==>  {project.Name}",
                    Description = project.Description,
                    ProjectType = project.ProjectType,
                })
                .ToListAsync(cancellationToken);
        return projectDtos;
    }

    private async Task<int> ManagerActiveTasks(Guid managerId, CancellationToken cancellationToken = default) => await _managementDb.Tasks
        .Where(x => x.AssignedManagerId == managerId && x.TaskState == TaskState.Pending)
        .CountAsync(cancellationToken);
}