using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITT.GitLabClient.Models
{
    public partial class AddIssue
    {
        public string Id { get; set; }
        public string ProjectId { get; set; } = "47123376";
        public string Title { get; set; }
        public string Description { get; set; }
        public string IssueType { get; set; }
        public string DueDate { get; set; }
        public long AssigneeId { get; set; }
        public bool Confidential { get; set; }
        public string Labels { get; set; }
    }
}