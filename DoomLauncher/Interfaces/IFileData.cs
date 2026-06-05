using System;

namespace DoomLauncher.Interfaces
{
    public interface IFileData
    {
        int? FileID { get; set; }
        int GameFileID { get; set; }
        string FileName { get; set; }
        string FullFileName { get; set; }
        DateTime DateCreated { get; set; }
        FileType FileTypeID { get; set; }
        int? SourcePortID { get; set; }
        string Description { get; set; }
        string OriginalFileName { get; set; }
        string UserTitle { get; set; }
        string UserDescription { get; set; }
        string Map { get; set; }
        int FileOrder { get; set; }
        bool IsUrl { get; }
        bool IsMain { get; set; }
        int? DerivedFromFileID { get; set; }
        string Title { get; }
        FileType DerivedFileType { get; set; }
    }
}
