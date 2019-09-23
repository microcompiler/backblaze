using System;

namespace Bytewizer.Backblaze.Agent.Console
{
    /// <summary>
    /// A program abstraction.
    /// </summary>
    public interface IConsoleHost
    {
        /// <summary>
        /// The programs configured services.
        /// </summary>
        //IServiceProvider Services { get; }

        void Run();

        //void Run<TStartup>() where TStartup : Startup, new();

        //T GetService<T>();

        //object GetService(Type type);
    }
}
