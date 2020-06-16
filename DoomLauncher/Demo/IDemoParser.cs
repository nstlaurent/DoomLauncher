namespace DoomLauncher.Demo
{
    public interface IDemoParser
    {
        bool CanParse();
        string[] GetRequiredFiles();    
    }
}
