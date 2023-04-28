namespace DoomLauncher
{
    public class PreviewImage
    {
        public PreviewImage(string path, string title)
        {
            Path = path;
            Title = title;
        }

        public string Path { get; set; }
        public string Title { get; set; }
    }
}
