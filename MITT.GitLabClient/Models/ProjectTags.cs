using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITT.GitLabClient.Models
{
    public partial class ProjectTags
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public Commit Commit { get; set; }
        public Release Release { get; set; }
        public bool Protected { get; set; }
    }

    public partial class Trailers
    {
    }

    public partial class Release
    {
        public string TagName { get; set; }
        public string Description { get; set; }
    }
}