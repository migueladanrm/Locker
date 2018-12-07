using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using Locker.Models;

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
            if (e.Total < e.Current)
                e.Current = e.Total;

            Write($"\r{(e.Mode == Mode.Decrypt ? "Desencriptando" : "Encriptando")} archivo: {Math.Round((100.0 / e.Total) * e.Current, 2)}% | {e.Current}/{e.Total} bytes  ");

            if (e.Current == e.Total)
                WriteLine();

        }

        private static void ModeEncrypt(string[] args)
        {
            string sourceFile = args[1];
            string destination = args[2];

            WriteLine("Se ha seleccionado el modo cifrar para el siguiente archivo:");
            WriteFileOverview(sourceFile);
            Write("Clave de cifrado: ");

            string password = RequestPassword();
            WriteLine();

            using (var source = new FileStream(sourceFile, FileMode.Open))
            using (var target = new FileStream(destination, FileMode.Create)) {
                var fet = new FileEncryptionTool(OnProgressUpdate);
                fet.EncryptFile(source, target, password);
            }

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

        static void WriteTitle(string str)
        {
            str = $"=====| {str} |";
            while (str.Length < BufferWidth - 1)
                str += "=";

            WriteLine(str);
        }

        static void WriteFileOverview(string path)
        {
            var fi = new FileInfo(path);

            WriteLine($"\n\tINFORMACIÓN DE ARCHIVO\n\tArchivo:\t{fi.Name}\n\tUbicación:\t{fi.Directory}\n\tTamaño:\t\t{fi.Length} bytes\n");
        }
    }
}