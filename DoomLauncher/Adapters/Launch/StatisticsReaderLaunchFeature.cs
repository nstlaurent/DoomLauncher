using DoomLauncher.Interfaces;
using System;

namespace DoomLauncher.Adapters.Launch
{
    public class StatisticsReaderLaunchFeature : LaunchFeature
    {
        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            IStatisticsReader statsReader = sourcePort.GetFlavor().CreateStatisticsReader(gameFile, Array.Empty<IStatsData>());
            return LaunchParameters.Param(statsReader?.LaunchParameter);
        }
    }
}
