using DoomLauncher.Config;
using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System.Collections.Generic;
using System.Text;

namespace DoomLauncher.Adapters.Launch
{
    public class MapSkillLaunchFeature : ILaunchFeature
    {

        private readonly string _map;
        private readonly string _skill;

        public MapSkillLaunchFeature(string map, string skill)
        {
            _map = map;
            _skill = skill;
        }

        public LaunchParameters CreateParameter(IGameFile gameFile, IEnumerable<IGameFile> addFiles, ISourcePortData sourcePort, bool isGameFileIwad, IDirectoriesConfiguration directories)
        {
            LaunchParameters result = LaunchParameters.EMPTY;

            if (_map != null)
            {
                var warpParam = LaunchParameters.Param(sourcePort.GetFlavor().WarpParameter(new SpData(_map, gameFile, addFiles)));
                result = warpParam;

                if (_skill != null)
                {
                    var skillParam = LaunchParameters.Param(sourcePort.GetFlavor().SkillParameter(new SpData(_skill, gameFile, addFiles)));
                    result = warpParam.Combine(skillParam);
                }
            }

            return result;
        }
    }
}
