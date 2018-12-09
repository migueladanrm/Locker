using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Locker
{
    /// <summary>
    /// Metadatos de fichero.
    /// </summary>
    [Serializable]
    public class LockerFileMetadata
    {
        private string hashId;
        private string fileName;
        private long fileLength;
        private DateTime creationDateTime;

        /// <summary>
        /// Inicializa una instancia de <see cref="LockerFileMetadata"/>.
        /// </summary>
        public LockerFileMetadata()
        {

        }

        /// <summary>
        /// Inicializa una instancia de <see cref="LockerFileMetadata"/>.
        /// </summary>
        /// <param name="hashId">Identificador autogenerado de fichero.</param>
        /// <param name="fileName">Nombre de archivo.</param>
        /// <param name="fileLength">Longitud del fichero.</param>
        /// <param name="creationDateTime">Fecha y hora de creación del fichero.</param>
        internal LockerFileMetadata(string hashId, string fileName, long fileLength, DateTime creationDateTime)
        {
            this.hashId = hashId;
            this.fileName = fileName;
            this.fileLength = fileLength;
            this.creationDateTime = creationDateTime;
        }

        /// <summary>
        /// Identidad única del fichero.
        /// </summary>
        public string HashId => hashId;

        /// <summary>
        /// Nombre del fichero.
        /// </summary>
        public string FileName => fileName;

        /// <summary>
        /// Longitud del fichero.
        /// </summary>
        public long FileLength => fileLength;

        /// <summary>
        /// Fecha y hora de creación del fichero.
        /// </summary>
        public DateTime CreationDateTime => creationDateTime;

        public override string ToString()
        {
            return $"LockerFileMetadata: {hashId}|{fileName}|{fileLength}|{creationDateTime.ToString("o")}";
        }

        #region Métodos estáticos

        public static LockerFileMetadata ReadMetadata(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            var bf = new BinaryFormatter();
            var meta = bf.Deserialize(ms) as LockerFileMetadata;

            return meta;
        }

        #endregion

    }
}