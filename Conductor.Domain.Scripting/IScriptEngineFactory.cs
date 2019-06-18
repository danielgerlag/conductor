using Microsoft.Scripting.Hosting;

namespace Conductor.Domain.Scripting
{
    public interface IScriptEngineFactory
    {
        ScriptEngine GetEngine(string contentType);
    }
}