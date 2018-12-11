using Locker.Crypto;
using System;
using System.IO;

namespace Locker
{
    /// <summary>
    /// Archivo cifrado.
    /// </summary>
    public class LockerFile
    {
        private LockerFileMetadata metadata;
        private FileStream payload;

        /// <summary>
        /// Inicializa una instancia de <see cref="LockerFile"/>.
        /// </summary>
        /// <param name="source">Secuencia de archivo de origen.</param>
        public LockerFile(FileStream source)
        {
            if (source.Length < Constants.LOCKER_FILE_HEADER_SIZE || !CheckFileSignature(source, false))
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

        /// <summary>
        /// Crea una copia desencriptada del archivo.
        /// </summary>
        /// <param name="key">Clave de encriptado.</param>
        /// <param name="destination">Destino de archivo.</param>
        /// <param name="progressChangedListener">Observador de evento de cambio de progreso.</param>
        public void DecryptPayload(string key, FileStream destination = null, Action<ProgressChangedEventArgs> progressChangedListener = null)
        {
            if (!metadata.HashId.Equals(GenerateLockerFileHashId(metadata, key)))
                throw new LockerPayloadDecryptException();

            if (destination == null)
                destination = new FileStream($@"{payload.Name.Substring(0, payload.Name.LastIndexOf('\\'))}\{metadata.FileName}", FileMode.Create);

            using (destination) {
                var fet = new FileEncryptionTool(progressChangedListener);
                fet.DecryptFile(payload, destination, key);
            }
        }

        #region Métodos estáticos

        /// <summary>
        /// Verifica la firma del archivo.
        /// </summary>
        /// <param name="path">Ubicación del archivo.</param>
        /// <returns>Resultado de validación de firma del archivo.</returns>
        public static bool CheckFileSignature(string path, bool dispose) => CheckFileSignature(new FileStream(path, FileMode.Open), dispose);

        /// <summary>
        /// Verifica la firma del archivo.
        /// </summary>
        /// <param name="signature">Bytes de firma de archivo.</param>
        /// <returns>Resultado de validación de firma de archivo.</returns>
        public static bool CheckFileSignature(byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            if (signature.Length < Constants.LOCKER_FILE_SIGNATURE_SIZE)
                throw new ArgumentException();
            return signature[0] == 0x4C && signature[1] == 0x4F && signature[2] == 0x43 && signature[3] == 0x4B && signature[4] == 0x45 && signature[5] == 0x52;
        }

        /// <summary>
        /// Verifica la firma del archivo.
        /// </summary>
        /// <param name="source">Secuencia de datos del archivo.</param>
        /// <returns>Resultado de validación de la firma del archivo.</returns>
        public static bool CheckFileSignature(FileStream source, bool dispose)
        {
            byte[] bytes = new byte[Constants.LOCKER_FILE_SIGNATURE_SIZE];
            source.Read(bytes, 0, Constants.LOCKER_FILE_SIGNATURE_SIZE);

            if (dispose)
                source.Dispose();

            return CheckFileSignature(bytes);
        }

        /// <summary>
        /// Genera un identificador hash de archivo a partir de los metadatos y una clave.
        /// </summary>
        /// <param name="metadata">Metadatos de archivo.</param>
        /// <param name="key">Clave de encriptado.</param>
        /// <returns>Identificador hash.</returns>
        public static string GenerateLockerFileHashId(LockerFileMetadata metadata, string key)
            => HashUtils.HashString($"{metadata.CreationDateTime.ToString("o")}|{metadata.FileLength}|{metadata.FileName}", key);

        #endregion
    }
}