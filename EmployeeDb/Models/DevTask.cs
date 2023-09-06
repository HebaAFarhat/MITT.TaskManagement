using Ardalis.GuardClauses;

namespace MITT.EmployeeDb.Models
{
    public partial class DevTask : BaseEntity
    {
        public DevTask()
        {
            AssignedBetasks = new HashSet<AssignedBeTask>();
            AssignedQatasks = new HashSet<AssignedQaTask>();
        }

        public string SeqNo { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> Requirements { get; private set; }
        public ImplementationType ImplementationType { get; private set; }
        public TaskState TaskState { get; set; }
        public string CompletionMessage { get; set; }
        public string CommitTag { get; set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string MainBranch { get; set; }
        public string MergeBranch { get; set; }
        public Guid? AssignedManagerId { get; private set; }

        public virtual AssignedManager AssignedManager { get; set; }
        public virtual ICollection<AssignedBeTask> AssignedBetasks { get; private set; }
        public virtual ICollection<AssignedQaTask> AssignedQatasks { get; private set; }

        public static DevTask Create(string seqNo,
                                     string name,
                                     string description,
                                     string mainBranch,
                                     string mergeBranch,
                                     DateTime startDate,
                                     DateTime endDate,
                                     ImplementationType implementationType,
                                     List<string> requirements,
                                     Guid assignedManagerId)
        {
             Guard.Against.EnumOutOfRange(implementationType,nameof(implementationType),"Enum_out_of_range");
             Guard.Against.NullOrEmpty(name, nameof(name), "Name_cannot_be_null_or_empty");
             Guard.Against.Null(description, nameof(description), "Description_cannot_be_null");
             Guard.Against.NullOrEmpty(mainBranch, nameof(mainBranch), "MainBranch_cannot_be_null_or_empty");
             Guard.Against.NullOrEmpty(mergeBranch, nameof(mergeBranch), "MergeBranch_cannot_be_null_or_empty");
             Guard.Against.Null(requirements,nameof(requirements),"Requirements_cannot_be_null");
                
            return new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                SeqNo = seqNo,
                Name = name,
                Description = description,
                MainBranch = mainBranch,
                MergeBranch = mergeBranch,
                StartDate = startDate,
                EndDate = endDate,
                ImplementationType = implementationType,
                Requirements = requirements,
                AssignedManagerId = assignedManagerId,
                TaskState = TaskState.Pending,
            };
        }

        public void AddQaDevelopers(List<Developer> developers)
        {
            developers.ForEach(developer => AssignedQatasks.Add(AssignedQaTask.Create(this, developer)));
        }

        public void AddBeDevelopers(List<Developer> developers)
        {
            developers.ForEach(developer => AssignedBetasks.Add(AssignedBeTask.Create(this, developer)));
        }
    }

    public enum TaskState
    {
        Pending = 1, Completed, Canceled
    }

    public enum ImplementationType
    {
        Implementation = 1, Refactoring
    }
}