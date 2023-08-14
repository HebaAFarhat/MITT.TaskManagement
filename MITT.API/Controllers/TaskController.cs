using Microsoft.AspNetCore.Mvc;
using MITT.API.Shared;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;
using MITT.Services.TaskServices;

namespace MITT.API.Controllers;

public class TaskController : BaseController
{
    private readonly ITaskService _taskService;
    private readonly AssignmentService _taskAssignmentService;
    private readonly ReviewService _reviewService;

    public TaskController(ITaskService taskService, AssignmentService taskAssignmentService, ReviewService reviewService)
    {
        _taskService = taskService;
        _taskAssignmentService = taskAssignmentService;
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<List<TaskVm>> Tasks(string? projectId, string? developerId, TaskState? taskState, CancellationToken cancellationToken = default)
        => await _taskService.Tasks(projectId, developerId, taskState, cancellationToken);

    [HttpGet]
    public async Task<List<TaskNamesVm>> TaskNames(CancellationToken cancellationToken = default)
        => await _taskService.TaskNames(cancellationToken);

    [HttpPost]
    public async Task<OperationResult> AddTask(TaskDto taskDto, CancellationToken cancellationToken = default)
        => await _taskService.AddTask(taskDto, cancellationToken);

    [HttpPost]
    public async Task<OperationResult> AddReview(AddReviewDto addReviewDto, CancellationToken cancellationToken = default) => addReviewDto.AssignDevType switch
    {
        AssignDevType.Be => await _reviewService.AddBeReview(addReviewDto, cancellationToken),
        AssignDevType.Qa => await _reviewService.AddQaReview(addReviewDto, cancellationToken),
    };

    [HttpPost]
    public async Task<OperationResult> Complete(CompleteTaskDto completeTaskDto, CancellationToken cancellationToken = default)
        => await _taskService.CompleteTask(completeTaskDto, cancellationToken);

    [HttpPost]
    public async Task<OperationResult> Cancel(CancelTaskDto cancelTaskDto, CancellationToken cancellationToken = default)
        => await _taskService.CancelTask(cancelTaskDto, cancellationToken);

    [HttpPost]
    public async Task<OperationResult> Assign(AssignTaskDto assignTaskDto, CancellationToken cancellationToken = default) => assignTaskDto.AssignDevType switch
    {
        AssignDevType.Be => await _taskAssignmentService.AssignBETask(assignTaskDto, cancellationToken),
        AssignDevType.Qa => await _taskAssignmentService.AssignQATask(assignTaskDto, cancellationToken),
    };
}