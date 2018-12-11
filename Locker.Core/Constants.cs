namespace Locker
{
    /// <summary>
    /// Constantes.
    /// </summary>
    public static class Constants
    {
        public const string LOCKER_FILE_EXT = ".lkr";
        /// <summary>
        /// Tamaño de bytes de cabecera de archivo.
        /// </summary>
        public const int LOCKER_FILE_HEADER_SIZE = LOCKER_FILE_SIGNATURE_SIZE + LOCKER_FILE_METADATA_SIZE;

        /// <summary>
        /// Tamaño en bytes de sección de metadatos de archivo.
        /// </summary>
        public const int LOCKER_FILE_METADATA_SIZE = 2032;

        /// <summary>
        /// Tamaño en bytes de la firma de formato de archivo.
        /// </summary>
        public const int LOCKER_FILE_SIGNATURE_SIZE = 16;

    }
}