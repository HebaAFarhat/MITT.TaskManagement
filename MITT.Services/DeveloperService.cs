using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;
using MITT.Services.Helpers;
using System.Linq.Expressions;

namespace MITT.Services;

public class DeveloperService : ManagementService<Developer>, IDeveloperService
{
    private readonly ManagementDb _managementDb;

    public DeveloperService(ManagementDb managementDb) : base(managementDb) => _managementDb = managementDb;

    public async Task<List<DeveloperVm>> Developers(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var content = new List<DeveloperVm>();

        Expression<Func<Developer, bool>> predicate = x => activeOnly ?
            x.ActiveState == ActiveState.Active :
            x != null;

        var developers = await _managementDb.Developers
            .OrderBy(x => x.Type == DeveloperType.Rv)
            .ThenBy(x => x.Type == DeveloperType.Be)
            .Where(predicate)
            .ToListAsync(cancellationToken);

        foreach (var developer in developers) content.Add(new DeveloperVm
        {
            Id = developer.Id.ToString(),
            FullName = developer.FullName,
            NickName = developer.NickName,
            Email = developer.Email,
            Phone = developer.Phone,
            Pin = developer.Pin,
            Type = developer.Type,
            ActiveState = developer.ActiveState,
            Tasks = await AssignedTaskToDeveloper(developer, activeOnly, cancellationToken)
        });

        return content.OrderByDescending(x => x.Tasks.Count).ToList();
    }

    public async Task<List<DeveloperVm>> Developers(string taskId, CancellationToken cancellationToken = default)
    {
        var content = new List<DeveloperVm>();

        var assignedDeveloperIds = await _managementDb.AssignedBetasks
            .Where(x => x.DevTaskId == Guid.Parse(taskId))
            .Select(x => x.DeveloperId)
            .ToListAsync(cancellationToken);

        var developers = await _managementDb.Developers
            .Where(x => x.ActiveState == ActiveState.Active && !assignedDeveloperIds.Contains(x.Id) && x.Type == DeveloperType.Be || x.Type == DeveloperType.Qa)
            .ToListAsync(cancellationToken);

        foreach (var developer in developers) content.Add(new DeveloperVm
        {
            Id = developer.Id.ToString(),
            FullName = developer.FullName,
            NickName = developer.NickName,
            Email = developer.Email,
            Phone = developer.Phone,
            Pin = developer.Pin,
            Type = developer.Type,
            ActiveState = developer.ActiveState,
            Tasks = await AssignedTaskToDeveloper(developer, true, cancellationToken)
        });

        return content;
    }

    public async Task<OperationResult> AddOrUpdateDeveloper(DeveloperDto developerDto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(developerDto.Id))
        {
            var dev = await _managementDb.Developers.FirstOrDefaultAsync(x => x.Email == developerDto.Email, cancellationToken);
            if (dev is not null) return OperationResult.UnValid("this Email is already been taken");

            var entity = Developer.Create(developerDto.FullName, developerDto.NickName, developerDto.Email, developerDto.Phone, developerDto.Pin, developerDto.Type);

            await Add(entity, cancellationToken);

            return OperationResult.Valid();
        }

        var developer = await _managementDb.Developers.FirstOrDefaultAsync(x => x.Id == Guid.Parse(developerDto.Id), cancellationToken);

        if (!developer.Email.Equals(developerDto.Email))
        {
            var dev = await _managementDb.Developers.FirstOrDefaultAsync(x => x.Email == developerDto.Email, cancellationToken);
            if (dev is not null) return OperationResult.UnValid("this Email is already been taken");
        }

        if (developer is null) throw new Exception($"invalid_{developer}_id!!");

        developer.Update(developerDto.FullName, developerDto.Phone, developerDto.Phone, developerDto.Email, developerDto.Type);

        await Update(developer, cancellationToken);
        return OperationResult.Valid();
    }

    private async Task<List<DeveloperTaskVm>> AssignedTaskToDeveloper(Developer developer, bool activeOnly, CancellationToken cancellationToken = default)
    {
        var list = new List<DeveloperTaskVm>();

        switch (developer.Type)
        {
            case DeveloperType.Be:
                Expression<Func<AssignedBeTask, bool>> bePredicate = x => activeOnly ?
                    x.DevTask.TaskState == TaskState.Pending && x.Developer == developer :
                    x.Developer == developer;

                var beTasks = await _managementDb.AssignedBetasks
                    .Where(bePredicate)
                    .Include(x => x.DevTask)
                    .ThenInclude(x => x.AssignedManager)
                    .ThenInclude(x => x.ProjectManager)
                    .ToListAsync(cancellationToken);

                list.AddRange(beTasks.Select(beTask => new DeveloperTaskVm
                {
                    Name = beTask.DevTask.Name,
                    Description = beTask.DevTask.Description,
                    AssignedManagerId = beTask.DevTask.AssignedManagerId.ToString(),
                    AssignedManagerName = beTask.DevTask.AssignedManager.ProjectManager.NickName,
                    StartDate = beTask.DevTask.StartDate,
                    EndDate = beTask.DevTask.EndDate,
                    Requirements = beTask.DevTask.Requirements,
                    ImplementationType = beTask.DevTask.ImplementationType,
                    TaskState = beTask.DevTask.TaskState,
                    CompletionMessage = beTask.DevTask.CompletionMessage
                }));
                break;

            case DeveloperType.Qa:
                Expression<Func<AssignedQaTask, bool>> qaPredicate = x => activeOnly ?
                    x.DevTask.TaskState == TaskState.Pending && x.Developer == developer :
                    x.Developer == developer;
                var qaTasks = await _managementDb.AssignedQatasks
                    .Where(qaPredicate)
                    .Include(x => x.DevTask)
                    .ThenInclude(x => x.AssignedManager)
                    .ThenInclude(x => x.ProjectManager)
                    .ToListAsync(cancellationToken);

                list.AddRange(qaTasks.Select(x => new DeveloperTaskVm
                {
                    Name = x.DevTask.Name,
                    Description = x.DevTask.Description,
                    AssignedManagerId = x.DevTask.AssignedManagerId.ToString(),
                    AssignedManagerName = x.DevTask.AssignedManager.ProjectManager.NickName,
                    StartDate = x.DevTask.StartDate,
                    EndDate = x.DevTask.EndDate,
                    Requirements = x.DevTask.Requirements,
                    ImplementationType = x.DevTask.ImplementationType,
                    TaskState = x.DevTask.TaskState,
                    CompletionMessage = x.DevTask.CompletionMessage
                }));

                break;
        }
        return list.OrderBy(x => x.TaskState).ToList();
    }

    public async Task<OperationResult> ChangeState(string developerId, CancellationToken cancellationToken = default)
    {
        var developer = await _managementDb.Developers.FirstOrDefaultAsync(x => x.Id == Guid.Parse(developerId), cancellationToken) ?? throw new Exception();
        if (developer.ActiveState == ActiveState.Active) developer.ActiveState = ActiveState.Inactive;
        else developer.ActiveState = ActiveState.Active;

        await Update(developer, cancellationToken);
        return OperationResult.Valid();
    }
}