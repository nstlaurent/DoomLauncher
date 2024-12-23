using SharpCompress.Common;
using System;
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

        void ExtractToFileForceOverwrite(string file);

        bool ExtractRequired { get; }
        bool IsDirectory { get; }
        string GetNameWithoutExtension();
    }

    public abstract class AbstractArchiveEntry : IArchiveEntry
    {
        public abstract long Length { get; }
        public abstract string Name { get; }
        public abstract string FullName { get; }
        public abstract bool ExtractRequired { get; }
        public abstract bool IsDirectory { get; }

        public abstract void ExtractToFile(string file, bool overwrite = false);

        public void ExtractToFileForceOverwrite(string file)
        {
            try
            {
                ExtractToFile(file, true);
            }
            catch (Exception ex)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        // Sometimes the read only flag can be set and the file can't be overwritten. This is our temp directory anyway so turn it off.
                        File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                        ExtractToFile(file, true);
                    }
                }
                catch
                {
                    throw ex;
                }
            }
        }

        public abstract string GetNameWithoutExtension();
        public abstract void Read(byte[] buffer, int offset, int length);
    }
}
