using MITT.EmployeeDb.Models;

namespace MITT.Services.Abstracts;

public interface IProjectsService
{
    Task<OperationResult> AddOrUpdateProject(ProjectDto projectDto, CancellationToken cancellationToken = default);

    Task<List<ProjectVm>> Projects(CancellationToken cancellationToken = default);

    Task<List<ProjectDto>> ProjectsToAssign(string managerId, CancellationToken cancellationToken = default);
}

public class ProjectDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectType ProjectType { get; set; }
    public Bank Bank { get; set; }
}

public class ProjectVm : ProjectDto
{
    public List<ProjectManagerVm> Managers { get; set; }
}