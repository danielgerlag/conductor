using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Conductor.Models
{
    public class PendingActivity
    {
        public string Token { get; set; }
        public string ActivityName { get; set; }
        public object Parameters { get; set; }
        public DateTime TokenExpiry { get; set; }

    }
}
