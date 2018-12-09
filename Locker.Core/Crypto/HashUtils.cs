using System.Security.Cryptography;
using System.Text;

namespace Locker.Crypto
{
    /// <summary>
    /// Herramientas de hash.
    /// </summary>
    public static class HashUtils
    {
        /// <summary>
        /// Convierte en hash una cadena de texto utilizando un algoritmo SHA.
        /// </summary>
        /// <param name="input">Texto de entrada.</param>
        /// <returns>Cadena de texto hash.</returns>
        public static string HashString(string input)
        {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(input))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        /// <summary>
        /// Convierte en hash una cadena de texto utilizando un algoritmo SHA y una clave.
        /// </summary>
        /// <param name="input">Texto de entrada.</param>
        /// <param name="key">Clave.</param>
        /// <returns>Cadena de texto hash.</returns>
        public static string HashString(string input, string key)
        {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(input, key))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private static byte[] GetHash(string input)
        {
            var sha = HashAlgorithm.Create("SHA256");
            var bytes = Encoding.UTF8.GetBytes(input);
            return sha.ComputeHash(bytes);
        }

        private static byte[] GetHash(string input, string key)
        {
            var hmac = HMAC.Create("HMACSHA256");
            hmac.Key = Encoding.UTF8.GetBytes(key);
            var bytes = Encoding.UTF8.GetBytes(input);
            return hmac.ComputeHash(bytes);
        }

    }
}