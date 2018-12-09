using System;
using System.Security.Cryptography;

namespace Locker.Crypto
{
    /// <summary>
    /// Utilidades criptográficas.
    /// </summary>
    public static class CryptoUtils
    {
        /// <summary>
        /// Crea bytes de "sal" a partir de un generador de números aleatorios.
        /// </summary>
        /// <param name="size">Cantidad de bytes.</param>
        /// <param name="iterations">Número de iteraciones.</param>
        /// <returns>Bytes aleatorios generados.</returns>
        public static byte[] GenerateRandomSalt(int size, int iterations = 1)
        {
            if (0 >= size)
                throw new ArgumentOutOfRangeException($"Parameter '{nameof(size)}' must be greater than 0.");

            var bytes = new byte[size];

            using (var rng = new RNGCryptoServiceProvider()) {
                for (int i = 0; i < iterations; i++)
                    rng.GetBytes(bytes);
            }

            return bytes;
        }
    }
}