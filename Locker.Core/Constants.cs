namespace Locker
{
    public static class Constants
    {
        public const int LOCKER_FILE_SIGNATURE_SIZE = 16;
        public const int LOCKER_FILE_METADATA_SIZE = 2032;
        public const int LOCKER_FILE_HEADER_SIZE = LOCKER_FILE_SIGNATURE_SIZE + LOCKER_FILE_METADATA_SIZE;
    }
}