using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoomLauncher
{
    public class NewFileDetector : INewFileDetector
    {
        public NewFileDetector(string[] extensions, string directory)
        {
            Directory = directory;
            Extenstions = extensions;
            ScanSubDirectories = false;
        }

        public NewFileDetector(string[] extensions, string directory, bool scanSubDirectories)
        {
            Directory = directory;
            Extenstions = extensions;
            ScanSubDirectories = scanSubDirectories;
        }

        public void StartDetection()
        {
            BaseFiles = GetFiles(Directory);
        }

        public string[] GetNewFiles()
        {
            if (BaseFiles != null && Directory != null)
            {
                string[] baseFiles = BaseFiles.Select(x => x.FullName).ToArray();
                string[] currentFiles = GetFiles(Directory).Select(x => x.FullName).ToArray();

                return currentFiles.Except(baseFiles).ToArray();
            }

            return new string[] { };
        }

        public string[] GetModifiedFiles()
        {
            if (BaseFiles != null && Directory != null)
            {
                FileInfo[] currentFiles = GetFiles(Directory);
                var joinedFiles = from file in BaseFiles
                                  join currentFile in currentFiles
                                  on file.FullName equals currentFile.FullName
                                  select new { OriginalFile = file, CurrentFile = currentFile };

                return (from file in joinedFiles 
                        where file.OriginalFile.LastWriteTime != file.CurrentFile.LastWriteTime ||
                        file.OriginalFile.Length != file.CurrentFile.Length
                        select file.CurrentFile.FullName).ToArray();
            }

            return new string[] { };
        }

        private FileInfo[] GetFiles(string directory)
        {
            try
            {
                if (ScanSubDirectories)
                {
                    List<FileInfo> files = new List<FileInfo>();
                    DirectoryInfo dir = new DirectoryInfo(directory);
                    DirectoryInfo[] subDirs = dir.GetDirectories();

                    files.AddRange(GetFilesBase(directory));
                    Array.ForEach(subDirs, x => files.AddRange(GetFiles(x.FullName)));

                    return files.ToArray();
                }
                else
                {
                    return GetFilesBase(directory);
                }
            }
            catch
            {
                return new FileInfo[] { };
            }
        }

        private FileInfo[] GetFilesBase(string directory)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                return di.GetFiles().Where(x => Extenstions.Contains(x.Extension)).ToArray();
            }
            catch
            {
                return new FileInfo[] { };
            }
        }

        public bool ScanSubDirectories { get; set; }
        public string Directory { get; set; }
        private FileInfo[] BaseFiles { get; set; }
        public string[] Extenstions { get; set; }
    }
}
