using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Locker
{
    /// <summary>
    /// Archivo cifrado.
    /// </summary>
    public class LockerFile
    {
        /// <summary>
        /// Verifica la firma del archivo.
        /// </summary>
        /// <param name="path">Ubicación del archivo.</param>
        /// <returns>Resultado de validación de firma del archivo.</returns>
        public static bool CheckFileSignature(string path)
        {
            byte[] signature = new byte[16];
            using (var br = new BinaryReader(new FileStream(path, FileMode.Open)))
                br.Read(signature, 0, 16);

            return signature[0] == 76 && signature[1] == 79
                && signature[2] == 67 && signature[3] == 75
                && signature[4] == 69 && signature[5] == 82;
        }
    }
}