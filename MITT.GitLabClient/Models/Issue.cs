using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITT.GitLabClient.Models;

public partial class Issue
{
    public long Id { get; set; }
    public long Iid { get; set; }
    public long ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string State { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public object ClosedAt { get; set; }
    public object ClosedBy { get; set; }
    public List<string> Labels { get; set; }
    public object Milestone { get; set; }
    public List<Assignee> Assignees { get; set; }
    public Assignee Author { get; set; }
    public string Type { get; set; }
    public Assignee Assignee { get; set; }
    public long UserNotesCount { get; set; }
    public long MergeRequestsCount { get; set; }
    public long Upvotes { get; set; }
    public long Downvotes { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public bool Confidential { get; set; }
    public object DiscussionLocked { get; set; }
    public string IssueType { get; set; }
    public Uri WebUrl { get; set; }
    public TimeStats TimeStats { get; set; }
    public TaskCompletionStatus TaskCompletionStatus { get; set; }
    public long BlockingIssuesCount { get; set; }
    public bool HasTasks { get; set; }
    public string TaskStatus { get; set; }
    public Links Links { get; set; }
    public References References { get; set; }
    public string Severity { get; set; }
    public object MovedToId { get; set; }
    public object ServiceDeskReplyTo { get; set; }
}

public partial class Assignee
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public Uri AvatarUrl { get; set; }
    public Uri WebUrl { get; set; }
}

public partial class Links
{
    public Uri Self { get; set; }
    public Uri Notes { get; set; }
    public Uri AwardEmoji { get; set; }
    public Uri Project { get; set; }
    public object ClosedAsDuplicateOf { get; set; }
}

public partial class References
{
    public string Short { get; set; }
    public string Relative { get; set; }
    public string Full { get; set; }
}

public partial class TaskCompletionStatus
{
    public long Count { get; set; }
    public long CompletedCount { get; set; }
}

public partial class TimeStats
{
    public long TimeEstimate { get; set; }
    public long TotalTimeSpent { get; set; }
    public object HumanTimeEstimate { get; set; }
    public object HumanTotalTimeSpent { get; set; }
}