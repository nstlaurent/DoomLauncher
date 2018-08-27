using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class FileData : IFileData
    {
        public FileData()
        {
            FileOrder = int.MaxValue;
            DateCreated = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
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
