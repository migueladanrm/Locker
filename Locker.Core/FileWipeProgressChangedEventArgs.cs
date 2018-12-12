using System;

namespace Locker
{
    /// <summary>
    /// Proporciona datos para el evento <see cref="FileWipeTool.ProgressChanged"/>.
    /// </summary>
    public class FileWipeProgressChangedEventArgs : EventArgs
    {
        public FileWipeProgressChangedEventArgs(int currentIteration, int totalIterations, long fileCurrentWriteLength, long fileLength)
        {
            CurrentIteration = currentIteration;
            TotalIterations = totalIterations;
            FileCurrentWriteLength = fileCurrentWriteLength;
            FileLength = fileLength;
        }

        public int CurrentIteration { get; private set; }
        public int TotalIterations { get; private set; }
        public long FileCurrentWriteLength { get; private set; }
        public long FileLength { get; private set; }
    }
}