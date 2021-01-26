using DoomLauncher.Interfaces;
using DoomLauncher.SaveGame;
using System.Collections.Generic;
using System.IO;

namespace DoomLauncher.SourcePort
{
    public interface ISourcePort
    {
        string IwadParameter(SpData data);
        string FileParameter(SpData data);
        string WarpParameter(SpData data);
        string SkillParameter(SpData data);
        string RecordParameter(SpData data);
        string PlayDemoParameter(SpData data);
        string LoadSaveParameter(SpData data);

        bool Supported(); //if the exe is supported by this implementation
        bool StatisticsSupported();
        bool LoadSaveGameSupported();
        IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats);
        ISaveGameReader CreateSaveGameReader(FileInfo file);
    }
}
