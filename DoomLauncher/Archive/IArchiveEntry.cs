using System.IO;

namespace DoomLauncher
{
    public interface IArchiveEntry
    {
        long Length { get; }
        void Read(byte[] buffer, int offset, int length);
        string Name { get; }
        string FullName { get; }
        void ExtractToFile(string file, bool overwrite = false);
        bool ExtractRequired { get; }
        bool IsDirectory { get; }
        string GetNameWithoutExtension();
    }
}
