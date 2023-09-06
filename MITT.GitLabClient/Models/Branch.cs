using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITT.GitLabClient.Models
{
    public partial class Branch
    {
        public string Name { get; set; }
        public Commit Commit { get; set; }
        public bool Merged { get; set; }
        public bool Protected { get; set; }
        public bool DevelopersCanPush { get; set; }
        public bool DevelopersCanMerge { get; set; }
        public bool CanPush { get; set; }
        public bool Default { get; set; }
        public Uri WebUrl { get; set; }
    }

    public partial class Commit
    {
        public string Id { get; set; }
        public string ShortId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<string> ParentIds { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public DateTimeOffset AuthoredDate { get; set; }
        public string CommitterName { get; set; }
        public string CommitterEmail { get; set; }
        public DateTimeOffset CommittedDate { get; set; }
        public Trailers Trailers { get; set; }
        public Uri WebUrl { get; set; }
    }

    public partial class Trailers
    {
    }
}