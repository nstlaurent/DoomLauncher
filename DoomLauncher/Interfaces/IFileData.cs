using System;

namespace DoomLauncher.Interfaces
{
    public interface IFileData
    {
        int? FileID { get; set; }
        int GameFileID { get; set; }
        string FileName { get; set; }
        DateTime DateCreated { get; set; }
        FileType FileTypeID { get; set; }
        int SourcePortID { get; set; }
        string Description { get; set; }
        string OriginalFileName { get; set; }
        string OriginalFilePath { get; set; }
        int FileOrder { get; set; }
        bool IsUrl { get; }
    }
}
