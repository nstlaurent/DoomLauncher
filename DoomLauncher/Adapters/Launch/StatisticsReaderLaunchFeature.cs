using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;

namespace DoomLauncher.Adapters.Launch
{
    public class StatisticsReaderLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            IStatisticsReader statsReader = sourcePort.GetFlavor().CreateStatisticsReader(gameFile, Array.Empty<IStatsData>());
            return LaunchParameters.Param(statsReader?.LaunchParameter);
        }
    }
}
