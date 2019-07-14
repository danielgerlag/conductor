using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conductor.Models
{
    public class DiagnosticInfo
    {
        public DateTime StartTime { get; set; }
        public string MachineName { get; set; }
        public string Version { get; set; }
        public string OSVersion { get; set; }
        public long WorkingSet { get; set; }
    }
}
