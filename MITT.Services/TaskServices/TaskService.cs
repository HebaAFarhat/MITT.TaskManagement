// Ignore Spelling: Dto

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;
using MITT.Services.Helpers;

namespace MITT.Services.TaskServices;

public class TaskService : ManagementService<DevTask>, ITaskService
{
    private readonly ManagementDb _managementDb;

    public TaskService(ManagementDb managementDb) : base(managementDb) => _managementDb = managementDb;

    public async Task<List<TaskVm>> Tasks(string projectId, string developerId, TaskState? taskState, CancellationToken cancellationToken = default)
    {
        var list = new List<TaskVm>();

        List<DevTask> tasks = await GetTasks(projectId, developerId, taskState, cancellationToken);

        foreach (var task in tasks) list.Add(new TaskVm
        {
            Id = task.Id.ToString(),
            SeqNo = task.SeqNo,
            AssignedManagerId = task.AssignedManagerId.ToString(),
            AssignedManagerName = task.AssignedManager.ProjectManager.FullName,
            CompletionMessage = task.CompletionMessage ?? string.Empty,
            Name = task.Name,
            Description = task.Description,
            MainBranch = task.MainBranch,
            MergeBranch = task.MergeBranch,
            CommitTag = task.CommitTag,
            ImplementationType = task.ImplementationType,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Requirements = task.Requirements,
            TaskState = task.TaskState,
            AssignedProjectId = task.AssignedManager.ProjectId.ToString(),
            AssignedProjectName = task.AssignedManager.Project.Name,
            TimeLeft = string.Empty,
            Progress = string.Empty,
            TimeAllow = 0,
            AssignedBeDevs = await GetAssignedBeDevs(task.Id),
            AssignedQaDevs = await GetAssignedQaDevs(task),
        });

        return list.OrderBy(x => x.TaskState)
            .ToList();
    }

    public async Task<List<TaskNamesVm>> TaskNames(CancellationToken cancellationToken = default) => await _managementDb.Tasks
        .Include(x => x.AssignedManager)
        .ThenInclude(x => x.Project)
        .Where(x => x.TaskState == TaskState.Pending)
        .Select(x => new TaskNamesVm
        {
            Id = x.Id.ToString(),
            SeqNo = x.SeqNo,
            Name = $"{x.AssignedManager.Project.ProjectType}  ==>  {x.AssignedManager.Project.Name}  ==>  {x.Name}",
            MainBranch = x.MainBranch,
            MergeBranch = x.MergeBranch,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            ProjectType = x.AssignedManager.Project.ProjectType,
            ImplementationType = x.ImplementationType
        }).ToListAsync(cancellationToken);

    public async Task<OperationResult> AddTask(TaskDto taskDto, CancellationToken cancellationToken = default)
    {
        var manager = await _managementDb.AssignedManagers
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(taskDto.AssignedManagerId), cancellationToken) ?? throw new Exception($"invalid_manager_id!!");

        var seqNo = await _managementDb.GenerateSequence(manager.Project.ProjectType);

        var entity = DevTask.Create(seqNo,
                                    taskDto.Name,
                                    taskDto.Description,
                                    taskDto.MainBranch,
                                    taskDto.MergeBranch,
                                    taskDto.StartDate ?? DateTime.Now,
                                    taskDto.EndDate,
                                    taskDto.ImplementationType,
                                    taskDto.Requirements,
                                    manager.Id);

        await Add(entity, cancellationToken);

        return OperationResult.Valid();
    }

    public async Task<OperationResult> CompleteTask(CompleteTaskDto completeTaskDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviewer = await _managementDb.Developers.FirstOrDefaultAsync(x => /*x.Id == Guid.Parse(cancelTaskDto.ReviewerId) &&*/ x.Type == DeveloperType.Rv, cancellationToken) ?? throw new Exception("invalid_reviewer_id!!");
            var task = await _managementDb.Tasks.FirstOrDefaultAsync(x => x.Id == Guid.Parse(completeTaskDto.TaskId) && x.TaskState == TaskState.Pending, cancellationToken) ?? throw new Exception("invalid_task_id!!");

            task.CompletionMessage = completeTaskDto.Message ?? TaskState.Completed.ToString();
            task.CommitTag = completeTaskDto.CommitTag;
            task.TaskState = TaskState.Completed;

            await Update(task, cancellationToken);

            return OperationResult.Valid();
        }
        catch (Exception e)
        {
            return OperationResult.UnValid(messages: new string[] { e.Message });
        }
    }

    public async Task<OperationResult> CancelTask(CancelTaskDto cancelTaskDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviewer = await _managementDb.Developers.FirstOrDefaultAsync(x => /*x.Id == Guid.Parse(cancelTaskDto.ReviewerId) &&*/ x.Type == DeveloperType.Rv, cancellationToken) ?? throw new Exception("invalid_reviewer_id!!");
            var task = await _managementDb.Tasks.FirstOrDefaultAsync(x => x.Id == Guid.Parse(cancelTaskDto.TaskId) && x.TaskState == TaskState.Pending, cancellationToken) ?? throw new Exception("invalid_task_id!!");

            task.CompletionMessage = cancelTaskDto.Message ?? TaskState.Completed.ToString();
            task.TaskState = TaskState.Canceled;

            await Update(task, cancellationToken);

            return OperationResult.Valid();
        }
        catch (Exception e)
        {
            return OperationResult.UnValid(messages: new string[] { e.Message });
        }
    }

    #region helpers

    private async Task<List<DevTask>> GetTasks(string projectId, string developerId, TaskState? taskState, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(projectId, out var project))
        {
            return await _managementDb.Tasks
                .Include(x => x.AssignedManager)
                .ThenInclude(x => x.ProjectManager)
                .Include(x => x.AssignedManager)
                .ThenInclude(x => x.Project)
                .Where(x => x.AssignedManager.ProjectId == project && taskState != null ? x.TaskState == taskState : x != null)
                .ToListAsync(cancellationToken);
        }

        if (Guid.TryParse(developerId, out var developer))
        {
            var dev = await _managementDb.Developers.FirstOrDefaultAsync(x => x.Id == developer, cancellationToken);

            return dev.Type == DeveloperType.Be ?
             await _managementDb.Tasks
                .Include(x => x.AssignedManager)
                .ThenInclude(x => x.ProjectManager)
                .Include(x => x.AssignedManager)
                .ThenInclude(x => x.Project)
                .Where(devTask => devTask.AssignedBetasks.Any(assignedDevTask => assignedDevTask.DeveloperId == developer) && devTask.TaskState == TaskState.Pending)
                .ToListAsync(cancellationToken) :
                 await _managementDb.Tasks
                .Include(x => x.AssignedManager)
                .ThenInclude(x => x.ProjectManager)
                .Include(x => x.AssignedManager)
                .ThenInclude(x => x.Project)
                .Where(devTask => devTask.AssignedQatasks.Any(assignedDevTask => assignedDevTask.DeveloperId == developer) && devTask.TaskState == TaskState.Pending)
                .ToListAsync(cancellationToken);
        }

        return await _managementDb.Tasks
            .Include(x => x.AssignedManager)
            .ThenInclude(x => x.ProjectManager)
            .Include(x => x.AssignedManager)
            .ThenInclude(x => x.Project)
            .Where(x => taskState != null ? x.TaskState == taskState : x != null)
            .ToListAsync(cancellationToken);
    }

    private async Task<List<AssignedDevVm>> GetAssignedBeDevs(Guid devTaskId)
    {
        var list = new List<AssignedDevVm>();

        var tasks = await _managementDb.AssignedBetasks
            .Where(x => x.DevTaskId == devTaskId)
            .Include(x => x.Developer)
            .ToListAsync();

        foreach (var task in tasks) list.Add(new AssignedDevVm
        {
            AssignedTaskId = task.Id.ToString(),
            DevId = task.DeveloperId.ToString(),
            Name = task.Developer.FullName,
            Email = task.Developer.Email,
            Phone = task.Developer.Phone,
            Reviews = task.BeReviews.Select(x => new ReviewVm
            {
                ReviewDate = x.CreatedAt,
                Findings = x.Findings
            }).ToList()
        });

        return list;
    }

    private async Task<List<AssignedDevVm>> GetAssignedQaDevs(DevTask devTask)
    {
        var list = new List<AssignedDevVm>();

        var tasks = await _managementDb.AssignedQatasks
            .Where(x => x.DevTaskId == devTask.Id)
            .Include(x => x.Developer)
            .ToListAsync();

        foreach (var task in tasks) list.Add(new AssignedDevVm
        {
            AssignedTaskId = task.Id.ToString(),
            DevId = task.DeveloperId.ToString(),
            Name = task.Developer.FullName,
            Email = task.Developer.Email,
            Phone = task.Developer.Phone,
            Reviews = task.QaReviews.Select(x => new ReviewVm
            {
                ReviewDate = x.CreatedAt,
                Findings = x.Findings
            }).ToList()
        });

        return list;
    }

    #endregion helpers
}