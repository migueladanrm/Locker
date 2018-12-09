using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Locker.Crypto;

namespace Locker
{
    public static class LockerFileFactory
    {
        public static void CreateLockerFile(string sourceFile, FileStream destination, string key, Action<NotifyProgressEventArgs> progressChangedListener)
        {
            WriteFileSignature(ref destination);
            WriteMetadata(ref destination, CreateMetadata(new FileInfo(sourceFile), key));

            using (var source = new FileStream(sourceFile, FileMode.Open))
            using (destination) {
                var fet = new FileEncryptionTool(progressChangedListener);
                fet.EncryptFile(source, destination, key);
            }
        }

        /// <summary>
        /// Escriba la firma de formato en el inicio del archivo.
        /// </summary>
        /// <param name="stream">Secuencia de archivo de destino.</param>
        private static void WriteFileSignature(ref FileStream stream)
        {
            byte[] signature = new byte[16] { 0x4c, 0x4f, 0x43, 0x4b, 0x45, 0x52, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, };
            for (int i = 0; i < signature.Length; i++)
                stream.WriteByte(signature[i]);
        }

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


        private static LockerFileMetadata CreateMetadata(FileInfo file, string key)
        {
            var filename = file.Name;
            var length = file.Length;
            var creation = DateTime.Now;
            var hashId = HashUtils.HashString($"{creation.ToString("o")}|{length}|{filename}", key);

            return new LockerFileMetadata(hashId, filename, length, creation);
        }
    }
}