using System;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// Represents an <see cref="IDisposable"/> base class.
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// Indicates this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Finalizer which calls <see cref="Dispose(bool)"/> with false when it has not been disabled
        /// by a proactive call to <see cref="Dispose()"/>.
        /// </summary>
        ~DisposableObject()
        {
            // Dispose once only
            if (IsDisposed)
                return;

            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Pro-actively frees resources owned by this instance.
        /// </summary>
        public void Dispose()
        {
            // Dispose once only
            if (IsDisposed)
                return;

            // Flag disposed first to help prevent external access during dispose
            IsDisposed = true;

            // Dispose
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalizer (we already cleaned-up)
                GC.SuppressFinalize(this);
            }
        }
    }
}
