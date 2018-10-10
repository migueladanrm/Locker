using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Herramienta de encriptación de archivos.
/// </summary>
/// <remarks>
/// Adaptado de: https://ourcodeworld.com/articles/read/471/how-to-encrypt-and-decrypt-files-using-the-aes-encryption-algorithm-in-c-sharp.
/// </remarks>
public static class FileEncryptionTool
{
    /// <summary>
    /// Desencripta un archivo cifrado con el algoritmo AES a través de una clave secreta.
    /// </summary>
    /// <param name="source">Flujo de datos de origen (encriptado).</param>
    /// <param name="destination">Flujo de datos de destino (desencriptado).</param>
    /// <param name="password">Clave secreta.</param>
    /// <param name="disposeSource">Determina si se liberará el flujo de origen al finalizar.</param>
    public static void DecryptFile(Stream source, Stream destination, string password, bool disposeSource = true)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var salt = new byte[32];
        source.Read(salt, 0, salt.Length);

        var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);

        var aes = new RijndaelManaged {
            KeySize = 256,
            BlockSize = 128,
            Padding = PaddingMode.PKCS7,
            Mode = CipherMode.CFB
        };
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        using (var cs = new CryptoStream(source, aes.CreateDecryptor(), CryptoStreamMode.Read)) {
            var buffer = new byte[1024 * 1024];
            int read;

            while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, read);
        }

        if (disposeSource)
            source.Close();
    }

    /// <summary>
    /// Encripta un archivo con el algoritmo AES a través de una clave secreta.
    /// </summary>
    /// <param name="source">Flujo de datos de origen.</param>
    /// <param name="destination">Flujo de datos de destino.</param>
    /// <param name="password">Clave secreta.</param>
    /// <param name="disposeSource">Determina si se liberará el flujo de origen al finalizar.</param>
    public static void EncryptFile(Stream source, Stream destination, string password, bool disposeSource = true)
    {
        var salt = CryptoUtils.GenerateRandomSalt(32, 10);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
        var aes = new RijndaelManaged {
            KeySize = 256,
            BlockSize = 128,
            Padding = PaddingMode.PKCS7,
            Mode = CipherMode.CFB
        };
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        destination.Write(salt, 0, salt.Length);

        using (var cs = new CryptoStream(destination, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
            var buffer = new byte[1024 * 1024];
            int read;

            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
                cs.Write(buffer, 0, read);
        }

        if (disposeSource)
            source.Close();
    }
}