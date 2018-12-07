using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locker.Models
{
    public class NotifyProgressEventArgs : EventArgs
    {
        public NotifyProgressEventArgs(Mode mode, long current, long total)
        {
            Mode = mode;
            Current = current;
            Total = total;
        }

        public Mode Mode { get; private set; }
        public long Current { get; set; }
        public long Total { get; private set; }
    }
}