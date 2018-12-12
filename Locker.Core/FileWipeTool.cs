using Locker.Crypto;
using System;
using System.IO;

namespace Locker
{
    /// <summary>
    /// Herramienta de borrado seguro de archivos.
    /// </summary>
    public class FileWipeTool
    {
        public event Action<FileWipeProgressChangedEventArgs> ProgressChanged;

        public FileWipeTool()
        {

        }

        public FileWipeTool(Action<FileWipeProgressChangedEventArgs> progressChangedListener)
        {
            ProgressChanged = progressChangedListener;
        }

        public void WipeFile(string path, int iterations)
        {
            using (var fs = new FileStream(path, FileMode.Open)) {
                long originalFileLength = fs.Length;
                int i = 0;

                while (i < iterations) {
                    fs.Seek(0, SeekOrigin.Begin);

                    var msRandomData = new MemoryStream(CryptoUtils.GenerateRandomBytes(fs.Length));

                    fs.SetLength(0);

                    var buffer = new byte[1024 * 1024];
                    int read;

                    while ((read = msRandomData.Read(buffer, 0, buffer.Length)) > 0) {
                        fs.Write(buffer, 0, read);
                        ProgressChanged?.Invoke(new FileWipeProgressChangedEventArgs(i, iterations, fs.Length, originalFileLength));
                    }

                    msRandomData.Dispose();
                    i++;
                }
                fs.SetLength(0);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var rnd = new Random(DateTime.Now.Millisecond);
            File.SetCreationTime(path, new DateTime(DateTime.Now.Year + rnd.Next(1, 10), rnd.Next(1, 12), 1));
            File.Delete(path);
        }


    }
}