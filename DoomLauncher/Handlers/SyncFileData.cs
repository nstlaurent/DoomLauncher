namespace DoomLauncher
{
    class SyncFileData
    {
        public SyncFileData(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; set; }
        public bool Selected { get; set; }
    }
}
