using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conductor.Auth
{
    public static class Permissions
    {
        public const string Admin = "conductor:admin";
        public const string Viewer = "conductor:viewer";
        public const string Controller = "conductor:controller";
        public const string Author = "conductor:author";
        public const string Worker = "conductor:worker";
    }
}
