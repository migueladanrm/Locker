using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Locker.Crypto;

namespace Locker
{
    /// <summary>
    /// Archivo cifrado.
    /// </summary>
    public class LockerFile
    {
        private LockerFileMetadata metadata;
        private FileStream payload;

        public LockerFile()
        {

        }

        public LockerFile(FileStream source)
        {
            if (source.Length < Constants.LOCKER_FILE_HEADER_SIZE)
                throw new LockerFileFormatException();

            if (!CheckFileSignature(source))
                throw new LockerFileFormatException();

            var metadataBytes = new byte[Constants.LOCKER_FILE_METADATA_SIZE];
            source.Read(metadataBytes, 0, Constants.LOCKER_FILE_METADATA_SIZE);

            metadata = LockerFileMetadata.ReadMetadata(metadataBytes);

            payload = source;
        }

        /// <summary>
        /// Metadatos.
        /// </summary>
        public LockerFileMetadata Metadata => metadata;

        /// <summary>
        /// Contenido encriptado.
        /// </summary>
        public FileStream Payload => payload;

        public void DecryptPayload(string key, FileStream destination = null, Action<NotifyProgressEventArgs> progressChangedListener = null)
        {
            if (!metadata.HashId.Equals(GenerateLockerFileHashId(metadata, key)))
                throw new Exception();

            if (destination == null)
                destination = new FileStream($@"{payload.Name.Substring(0, payload.Name.LastIndexOf('\\'))}\{metadata.FileName}", FileMode.Create);

            using (destination) {
                var fet = new FileEncryptionTool(progressChangedListener);
                fet.DecryptFile(payload, destination, key);
            }
        }

        public static string GenerateLockerFileHashId(LockerFileMetadata metadata, string key)
        {
            return HashUtils.HashString($"{metadata.CreationDateTime.ToString("o")}|{metadata.FileLength}|{metadata.FileName}", key);
        }

        #region Métodos estáticos

        /// <summary>
        /// Verifica la firma del archivo.
        /// </summary>
        /// <param name="path">Ubicación del archivo.</param>
        /// <returns>Resultado de validación de firma del archivo.</returns>
        public static bool CheckFileSignature(string path) => CheckFileSignature(new FileStream(path, FileMode.Open));


        public static bool CheckFileSignature(byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            if (signature.Length < Constants.LOCKER_FILE_SIGNATURE_SIZE)
                throw new ArgumentException();

            return signature[0] == 76 && signature[1] == 79 && signature[2] == 67 && signature[3] == 75 && signature[4] == 69 && signature[5] == 82;
        }

        public static bool CheckFileSignature(FileStream source)
        {
            byte[] bytes = new byte[Constants.LOCKER_FILE_SIGNATURE_SIZE];
            source.Read(bytes, 0, Constants.LOCKER_FILE_SIGNATURE_SIZE);

            return CheckFileSignature(bytes);
        }

        #endregion
    }
}