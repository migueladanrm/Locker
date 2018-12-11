using Locker.Crypto;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Locker
{
    /// <summary>
    /// Herramienta de creación de ficheros cifrados.
    /// </summary>
    public static class LockerFileFactory
    {
        /// <summary>
        /// Crea un fichero cifrado con una clave especificada.
        /// </summary>
        /// <param name="sourceFile">Ubicación del archivo de origen.</param>
        /// <param name="destination">Fichero de destino.</param>
        /// <param name="key">Clave de cifrado.</param>
        /// <param name="progressChangedListener">Observador de evento de cambio de progreso.</param>
        public static void CreateLockerFile(string sourceFile, FileStream destination, string key, Action<ProgressChangedEventArgs> progressChangedListener = null)
        {
            var fi = new FileInfo(sourceFile);

            if (destination == null)
                destination = new FileStream($"{fi.Directory}\\{fi.Name.Replace(fi.Extension, Constants.LOCKER_FILE_EXT)}", FileMode.Create);

            WriteFileSignature(ref destination);
            WriteMetadata(ref destination, CreateMetadata(fi, key));

            using (var source = new FileStream(sourceFile, FileMode.Open))
            using (destination) {
                var fet = new FileEncryptionTool(progressChangedListener);
                fet.EncryptFile(source, destination, key);
            }
        }

        /// <summary>
        /// Crea los metadatos de archivo.
        /// </summary>
        /// <param name="file">Información del archivo original.</param>
        /// <param name="key">Clave de cifrado utilizada para calcular el identificador hash.</param>
        /// <returns>Metadatos de archivo.</returns>
        private static LockerFileMetadata CreateMetadata(FileInfo file, string key)
        {
            var filename = file.Name;
            var length = file.Length;
            var creation = DateTime.Now;
            var hashId = HashUtils.HashString($"{creation.ToString("o")}|{length}|{filename}", key);

            return new LockerFileMetadata(hashId, filename, length, creation);
        }

        /// <summary>
        /// Escriba la firma de formato en el inicio del archivo.
        /// </summary>
        /// <param name="stream">Secuencia de archivo de destino.</param>
        private static void WriteFileSignature(ref FileStream stream)
        {
            var signature = new byte[Constants.LOCKER_FILE_SIGNATURE_SIZE] { 0x4C, 0x4F, 0x43, 0x4B, 0x45, 0x52, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, };
            for (int i = 0; i < signature.Length; i++)
                stream.WriteByte(signature[i]);
        }

        /// <summary>
        /// Escribe los metadatos del archivo en la cabecera.
        /// </summary>
        /// <param name="stream">Secuencia de archivo de destino.</param>
        /// <param name="metadata">Metadatos de archivo.</param>
        private static void WriteMetadata(ref FileStream stream, LockerFileMetadata metadata)
        {
            var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(ms, metadata);
            var bytes = ms.ToArray();

            for (int i = 0; i < bytes.Length; i++)
                stream.WriteByte(bytes[i]);

            while (stream.Length < Constants.LOCKER_FILE_HEADER_SIZE)
                stream.WriteByte(0);
        }
    }
}