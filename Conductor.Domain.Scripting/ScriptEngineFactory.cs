using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace Conductor.Domain.Scripting
{
    class ScriptEngineFactory : IScriptEngineFactory
    {
        public ScriptEngine GetEngine(string contentType)
        {
            return IronPython.Hosting.Python.CreateEngine();
        }
    }
}
