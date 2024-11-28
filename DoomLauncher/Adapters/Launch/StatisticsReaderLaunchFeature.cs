using DoomLauncher.Interfaces;
using System;

namespace DoomLauncher.Adapters.Launch
{
    public class StatisticsReaderLaunchFeature : ILaunchFeature
    {
        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile, bool isGameFileIwad, LauncherPath gameFileDirectory, LauncherPath tempDirectory)
        {
            IStatisticsReader statsReader = sourcePort.GetFlavor().CreateStatisticsReader(gameFile, Array.Empty<IStatsData>());
            return LaunchParameters.Param(statsReader?.LaunchParameter);
        }
    }
}
