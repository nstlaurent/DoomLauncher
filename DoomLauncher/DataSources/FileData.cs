using DoomLauncher.Interfaces;
using System;

namespace DoomLauncher
{
    public class FileData : IFileData
    {
        public FileData()
        {
            FileOrder = int.MaxValue;
            DateCreated = DateTime.Now;
        }

        public int? FileID { get; set; }
        public int GameFileID { get; set; }
        public string FileName { get; set; }
        public DateTime DateCreated { get; set; }
        public FileType FileTypeID { get; set; }
        public int SourcePortID { get; set; }
        public string Description { get; set; }
        public string OriginalFileName { get; set; }
        public string OriginalFilePath { get; set; }
        public int FileOrder { get; set; }

        public virtual bool IsUrl { get { return false; } }
    }
}
