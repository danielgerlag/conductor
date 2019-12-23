using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conductor.Auth
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Viewer = "Viewer";
        public const string Controller = "Controller";
        public const string Author = "Author";

        public const string ControllerOrViewer = "Controller,Viewer";
    }
}
