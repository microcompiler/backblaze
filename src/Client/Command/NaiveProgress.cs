using System;

namespace Bytewizer.Backblaze.Command
{
    /// <summary>
    /// A really naive progress class which doesn't do any surprising synchronization, threading, etc. Reports are passed directly to the ProgressChanged event.
    /// </summary>
    public class NaiveProgress<T> : IProgress<T>
    {
        /// <summary>
        /// This event is run on every invocation of Report. Events block, so be careful with performance in handlers.
        /// </summary>
        public event EventHandler<T> ProgressChanged;

        /// <summary>
        /// The default constructor. Does not subscribe anything to ProgressChanged.
        /// </summary>
        public NaiveProgress()
        { }

        /// <summary>
        /// Helper constructor which adds the given action to the ProgressChanged event.
        /// </summary>
        /// <param name="action">The action to run when a progress report is created.</param>
        public NaiveProgress(Action<T> action)
        {
            ProgressChanged += new EventHandler<T>((s, e) => { action.Invoke(e); });
        }

        /// <summary>
        /// This is called by Report to invoke the ProgressChanged event, and can be overridden to shim additional functionality.
        /// </summary>
        protected virtual void OnReport(T value)
        {
            ProgressChanged?.Invoke(this, value);
        }

        void IProgress<T>.Report(T value) => OnReport(value);
    }
}
