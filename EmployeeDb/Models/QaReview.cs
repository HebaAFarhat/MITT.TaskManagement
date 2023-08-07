﻿namespace MITT.EmployeeDb.Models
{
    public partial class QaReview : BaseEntity
    {
        private QaReview(AssignedQaTask assigned, List<ReviewFinding> findings)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            AssignedQaTask = assigned;
            Findings = findings;
        }

        public static QaReview Create(AssignedQaTask assigned, List<ReviewFinding> findings) => new(assigned, findings);

        public Guid AssignedQaTaskId { get; set; }
        public List<ReviewFinding> Findings { get; set; }

        public string FilePath { get; set; }

        public virtual AssignedQaTask AssignedQaTask { get; set; }
    }
}