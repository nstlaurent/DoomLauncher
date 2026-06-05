using System.IO;

namespace DoomLauncher.Handlers
{
    public static class ExceptionExtensions
    {
        const int ERROR_SHARING_VIOLATION = 0x20;
        const int ERROR_LOCK_VIOLATION = 0x21;

        public static bool IsInUse(this IOException ex)
        {
            var result = ex.HResult & 0xFFFF;
            return result == ERROR_SHARING_VIOLATION || result == ERROR_LOCK_VIOLATION;
        }
    }
}
