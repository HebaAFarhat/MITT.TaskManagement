using MITT.EmployeeDb.Models;

namespace MITT.Services.Abstracts;

public interface ITaskService
{
    Task<List<TaskVm>> Tasks(string projectId, string developerId, TaskState? taskState, CancellationToken cancellationToken = default);

    Task<List<TaskNamesVm>> TaskNames(CancellationToken cancellationToken = default);

    Task<OperationResult> AddTask(TaskDto taskDto, CancellationToken cancellationToken = default);

    Task<OperationResult> CompleteTask(CompleteTaskDto completeTaskDto, CancellationToken cancellationToken = default);

    Task<OperationResult> CancelTask(CancelTaskDto cancelTaskDto, CancellationToken cancellationToken = default);
}

public class CompleteTaskDto : CancelTaskDto
{
    public string CommitTag { get; set; }
}

public class CancelTaskDto
{
    public string ReviewerId { get; set; }
    public string TaskId { get; set; }
    public string Message { get; set; }
}

public class AssignTaskDto
{
    public AssignDevType AssignDevType { get; set; }
    public string TaskId { get; set; }
    public List<string> DeveloperIds { get; set; }
}

public class TaskDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string MainBranch { get; set; }
    public string MergeBranch { get; set; }
    public List<string> Requirements { get; set; }
    public string AssignedManagerId { get; set; }
    public ImplementationType ImplementationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class TaskNamesVm
{
    public string Id { get; set; }
    public string SeqNo { get; set; }
    public string Name { get; set; }
    public string MainBranch { get; set; }
    public string MergeBranch { get; set; }
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public ProjectType ProjectType { get; set; }
    public ImplementationType ImplementationType { get; set; }
}

public class TaskVm
{
    public string Id { get; set; }
    public string SeqNo { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MainBranch { get; set; }
    public string MergeBranch { get; set; }
    public string CommitTag { get; set; }
    public List<string> Requirements { get; set; }
    public string AssignedManagerId { get; set; }
    public string AssignedManagerName { get; set; }
    public string AssignedProjectId { get; set; }
    public string AssignedProjectName { get; set; }
    public ImplementationType ImplementationType { get; set; }
    public TaskState TaskState { get; set; } = TaskState.Pending;
    public string CompletionMessage { get; set; }
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
    public string TimeLeft { get; set; }
    public string Progress { get; set; }
    public int TimeAllow { get; set; }
    public List<AssignedDevVm> AssignedBeDevs { get; set; }
    public List<AssignedDevVm> AssignedQaDevs { get; set; }
}

public class AssignedDevVm
{
    public string AssignedTaskId { get; set; }
    public string DevId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public List<ReviewVm> Reviews { get; set; }
}

public class ReviewVm
{
    public DateTime ReviewDate { get; set; }
    public List<ReviewFinding> Findings { get; set; }
}

public enum AssignDevType
{
    Be = 1, Qa
}