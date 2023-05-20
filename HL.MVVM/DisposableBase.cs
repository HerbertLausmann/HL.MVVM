using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.MVVM
{
    /// <summary>
    /// Represents a base class for disposable objects, providing a thread-safe implementation of the IDisposable interface.
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        public bool IsDisposed => disposedValue;

        private void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!disposedValue)
                {
                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                    if (disposing)
                    {
                        DoDispose();
                    }
                }
            }
        }

        /// <summary>
        /// Provides a method to perform custom disposal logic in derived classes. Called within a lock statement, ensuring thread-safety.
        /// </summary>
        protected abstract void DoDispose();

        /// <summary>
        /// Destructor that calls the Dispose method to release resources.
        /// </summary>
        ~DisposableBase() { Dispose(); }

        /// <summary>
        /// Disposes the object and suppresses the finalizer.
        /// </summary>
        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
