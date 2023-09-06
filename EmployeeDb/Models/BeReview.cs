using Ardalis.GuardClauses;

namespace MITT.EmployeeDb.Models
{
    public partial class BeReview : BaseEntity
    {
        private BeReview(AssignedBeTask assigned, List<ReviewFinding> findings)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            AssignedBeTask = assigned;
            Findings = findings;
        }

        public static BeReview Create(AssignedBeTask assigned, List<ReviewFinding> findings)
        {
            findings = Guard.Against.Null(findings, nameof(findings), "Findings_cannot_be_null");
            
            return new(assigned, findings);
        }

        public Guid AssignedBeTaskId { get; set; }
        public List<ReviewFinding> Findings { get; set; }
        public string FilePath { get; set; }

        public virtual AssignedBeTask AssignedBeTask { get; set; }
    }

    public class ReviewFinding
    {
        public string Location { get; set; }
        public string Scope { get; set; }
        public string Discription { get; set; }
    }
}