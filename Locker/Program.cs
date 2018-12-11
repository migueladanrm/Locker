using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using Locker.Crypto;

namespace Locker
{
    class Program
    {
        private const string ARG_FIRST_ENCRYPT = "-e";
        private const string ARG_FIRST_DECRYPT = "-d";
        private const string ARG_FIRST_HELP = "-?";

        /// <summary>
        /// Punto de entrada del programa.
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos.</param>
        static void Main(string[] args)
        {
            WriteTitle("Locker: Herramienta de cifrado de archivos");
            if (0 < args.Length) {
                switch (args[0]) {
                    default:
                    case ARG_FIRST_HELP:
                        WriteHelp();
                        break;
                    case ARG_FIRST_DECRYPT:
                        ModeDecrypt(args);
                        break;
                    case ARG_FIRST_ENCRYPT:
                        ModeEncrypt(args);
                        break;
                }
            } else WriteHelp();

#if DEBUG
            ReadKey();
#endif
        }

        private static void OnProgressUpdate(NotifyProgressEventArgs e)
        {
            if (e.Total < e.CurrentProgress)
                e.CurrentProgress = e.Total;

            Write($"\rProcesando archivo: {Math.Round((100.0 / e.Total) * e.CurrentProgress, 2)}% | {e.CurrentProgress}/{e.Total} bytes  ");

            if (e.CurrentProgress == e.Total)
                WriteLine();

        }

        static void ModeDecrypt(string[] args)
        {
            string sourceFile = args[1];
            string destinationFile = args[2];

            WriteLine("Se ha seleccionado el modo DESENCRIPTAR para el siguiente archivo:");
            WriteFileOverview(sourceFile);
            Write("Clave: ");

            string key = RequestPassword();
            WriteLine();

            var lockerFile = new LockerFile(new FileStream(sourceFile, FileMode.Open));
            lockerFile.DecryptPayload(key, new FileStream(destinationFile, FileMode.Create), OnProgressUpdate);

            WriteLine("\nSe ha completado el desencriptado del archivo.");
        }

        private static void ModeEncrypt(string[] args)
        {
            string sourceFile = args[1];
            string destination = null;

            if (args.Length > 3)
                destination = args[2];

            WriteLine("Se ha seleccionado el modo ENCRIPTAR para el siguiente archivo:");
            WriteFileOverview(sourceFile);
            Write("Clave de cifrado: ");

            string password = RequestPassword();
            WriteLine();

            LockerFileFactory.CreateLockerFile(sourceFile, password, destination != null ? new FileStream(destination, FileMode.Create) : null, OnProgressUpdate);

            WriteLine("\nSe ha completado el cifrado del archivo.");
        }

        private static void WriteHelp()
        {
            Write("LOCKER: INSTRUCCIONES DE USO\n\nlocker [-?|-d|-e] \"<source>\" \"<destination>\"");
        }


        /// <summary>
        /// Solicita y oculta la escritura de una frase secreta.
        /// </summary>
        /// <returns>Frase secreta.</returns>
        static string RequestPassword()
        {
            string password = string.Empty;
            do {
                ConsoleKeyInfo key = ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter) {
                    password += key.KeyChar;
                    Write("*");
                } else {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0) {
                        password = password.Substring(0, password.Length - 1);
                        Write("\b \b");
                    } else if (key.Key == ConsoleKey.Enter)
                        break;
                }
            } while (true);

            return password;
        }

        /// <summary>
        /// Escribe una línea de texto de título adornado.
        /// </summary>
        /// <param name="str">Cadena de texto que corresponde al título.</param>
        static void WriteTitle(string str)
        {
            str = $"=====| {str} |";
            while (str.Length < BufferWidth - 1)
                str += "=";

            WriteLine(str);
        }

        /// <summary>
        /// Escribe información general del archivo seleccionado.
        /// </summary>
        /// <param name="path">Ruta del archivo seleccionado.</param>
        static void WriteFileOverview(string path)
        {
            var fi = new FileInfo(path);
            WriteLine($"\n\tINFORMACIÓN DE ARCHIVO\n\tArchivo:\t{fi.Name}\n\tUbicación:\t{fi.Directory}\n\tTamaño:\t\t{fi.Length} bytes\n");
        }
    }
}