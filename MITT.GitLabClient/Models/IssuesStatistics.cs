using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITT.GitLabClient.Models
{
    public partial class IssuesStatistics
    {
        public Statistics Statistics { get; set; }
    }

    public partial class Statistics
    {
        public Counts Counts { get; set; }
    }

    public partial class Counts
    {
        public long All { get; set; }
        public long Closed { get; set; }
        public long Opened { get; set; }
    }
}