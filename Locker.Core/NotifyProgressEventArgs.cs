using System;

namespace Locker
{
    public class NotifyProgressEventArgs : EventArgs
    {
        public NotifyProgressEventArgs(long currentProgress, long total)
        {
            CurrentProgress = currentProgress;
            Total = total;
        }

        public long CurrentProgress { get; set; }
        public long Total { get; private set; }
    }
}