using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace Locker
{
    class Program
    {
        private const string ARG_FIRST_DECRYPT = "-d";
        private const string ARG_FIRST_ENCRYPT = "-e";
        private const string ARG_FIRST_SDELETE = "-sd";
        private const string ARG_FIRST_HELP = "-?";

        /// <summary>
        /// Punto de entrada del programa.
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos.</param>
        static void Main(string[] args)
        {
            if (0 < args.Length) {
                switch (args[0]) {
                    default:
                    case ARG_FIRST_HELP:
                        WriteHelp();
                        break;
                    case ARG_FIRST_DECRYPT:
                        OptionDecrypt(args.Skip(1).ToArray());
                        break;
                    case ARG_FIRST_ENCRYPT:
                        OptionEncrypt(args.Skip(1).ToArray());
                        break;
                    case ARG_FIRST_SDELETE:
                        OptionSecureDelete(args.Skip(1).ToArray());
                        break;
                }
            } else WriteHelp();

#if DEBUG
            ReadKey();
#endif
        }

        private static void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (e.Total < e.CurrentProgress)
                e.CurrentProgress = e.Total;

            Write($"\rProcesando archivo... {Math.Round(100.0 / e.Total * e.CurrentProgress, 2)}% | {e.CurrentProgress}/{e.Total} bytes  ");

            if (e.CurrentProgress == e.Total)
                WriteLine();
        }

        #region Modo: Desencriptar

        static void OptionDecrypt(string[] args)
        {
            if (args.Length < 1) {
                WriteLine("Error: No se ha proporcionado un archivo de origen.");
                return;
            }

            var sourceFile = new FileInfo(args[0]);
            if (!sourceFile.Exists) {
                WriteLine("Error: No se ha encontrado el archivo de origen especificado.");
                return;
            }

            if (!LockerFile.CheckFileSignature(path: sourceFile.FullName, dispose: true)) {
                WriteLine("Error: El archivo especificado está dañado o no tiene formato de archivo Locker.");
                return;
            }

            FileStream destinationFile = null;
            if (args.Length > 1)
                destinationFile = new FileStream(args[1], FileMode.Create);

            try {
                WriteLine("Se ha seleccionado el siguiente fichero para DESENCRIPTAR:");

                var lf = new LockerFile(new FileStream(sourceFile.FullName, FileMode.Open));
                var lfmeta = lf.Metadata;

                WriteLine($"\n\tINFORMACIÓN DE ARCHIVO\n\tPaquete:\t{sourceFile.Name}\n\tIdentificador:\t{lfmeta.HashId}\n\tContenido:\t{lfmeta.FileName}\n\tTamaño:\t\t{lfmeta.FileLength} bytes\n\tCreación:\t{lfmeta.CreationDateTime}\n");

                Write("Clave: ");
                string key = RequestPassword();
                WriteLine();

                lf.DecryptPayload(key, null, OnProgressChanged);

                WriteLine("\nSe ha desencriptado el archivo correctamente.");
            } catch (LockerPayloadDecryptException) {
                WriteLine("Error: La clave introducida es incorrecta.");
            } catch (LockerFileFormatException) {
                WriteLine("Error: El formato del archivo es incorrecto.");
            } catch {
                WriteLine("Ha ocurrido un error inesperado.");
            }
        }

        #endregion

        #region Modo: Encriptar

        private static void OptionEncrypt(string[] args)
        {
            if (args.Length < 1) {
                WriteLine("Error: No se han proporcionado suficientes parámetros.");
                return;
            }

            var sourceFile = new FileInfo(args[0]);
            if (!sourceFile.Exists) {
                WriteLine("Error: No se ha encontrado el archivo de origen especificado.");
                return;
            }

            FileStream destinationFile = null;
            if (args.Length > 1)
                destinationFile = new FileStream(args[1], FileMode.Create);

            WriteLine("Se ha seleccionado el siguiente archivo para ENCRIPTAR:");
            WriteFileOverview(sourceFile);
            var key = string.Empty;

            while (true) {
                Write("\rClave de cifrado: ");
                key = RequestPassword();
                if (key.Length < 1 || key.Length > 32)
                    WriteLine("\rLa clave debe comprender entre 1 a 32 caracteres. Inténtelo de nuevo.");
                else {
                    WriteLine();
                    break;
                }
            }

            LockerFileFactory.CreateLockerFile(sourceFile, destinationFile, key, OnProgressChanged);

            WriteLine("El archivo se ha encriptado correctamente.");
        }

        #endregion

        static void OnFileWipeProgressChanged(FileWipeProgressChangedEventArgs e)
        {
            Write($"\rSobreescribiendo archivo... {e.CurrentIteration + 1}/{e.TotalIterations} iteraciones | {Math.Round(100.0 / e.FileLength * e.FileCurrentWriteLength, 2)}%   ");
        }

        static void OptionSecureDelete(string[] args)
        {
            var targetFile = args[0];
            var iterations = int.Parse(args[2]);

            WriteLine("Inicializando sobreescritura y eliminación de archivo...");

            var fwt = new FileWipeTool(OnFileWipeProgressChanged);
            fwt.WipeFile(targetFile, iterations);

            WriteLine("\nEl archivo se ha eliminado de forma segura.");
        }

        private static void WriteHelp()
        {
            Write("LOCKER: INSTRUCCIONES DE USO\n\nlocker [-?|-d|-e] \"<source>\" \"<destination>\"");
        }

        #region Métodos auxiliares

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
        /// <param name="fileInfo">Información de archivo.</param>
        static void WriteFileOverview(FileInfo fileInfo) => WriteFileOverview(fileInfo.FullName);

        /// <summary>
        /// Escribe información general del archivo seleccionado.
        /// </summary>
        /// <param name="path">Ruta del archivo seleccionado.</param>
        static void WriteFileOverview(string path)
        {
            var fi = new FileInfo(path);
            WriteLine($"\n\tINFORMACIÓN DE ARCHIVO\n\tArchivo:\t{fi.Name}\n\tUbicación:\t{fi.Directory}\n\tTamaño:\t\t{fi.Length} bytes\n");
        }

        #endregion
    }
}