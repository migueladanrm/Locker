using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locker
{
    /// <summary>
    /// Metadatos de archivo.
    /// </summary>
    [Serializable]
    public class LockerFileMetadata
    {
        private string fileName;
        private long fileLength;
        private DateTime creationDateTime;

        public LockerFileMetadata()
        {

        }

        public string FileName => fileName;
        public long FileLength => fileLength;
        public DateTime CreationDateTime => creationDateTime;

    }
}