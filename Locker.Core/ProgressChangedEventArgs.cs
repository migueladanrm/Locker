using Locker.Crypto;
using System;

namespace Locker
{
    /// <summary>
    /// Proporciona datos para el evento <see cref="FileEncryptionTool.ProgressChanged"/>.
    /// </summary>
    public class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Inicializa una instancia de <see cref="ProgressChangedEventArgs"/>.
        /// </summary>
        /// <param name="currentProgress">Progreso actual.</param>
        /// <param name="total">Total.</param>
        public ProgressChangedEventArgs(long currentProgress, long total)
        {
            CurrentProgress = currentProgress;
            Total = total;
        }

        /// <summary>
        /// Progreso actual.
        /// </summary>
        public long CurrentProgress { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public long Total { get; private set; }
    }
}