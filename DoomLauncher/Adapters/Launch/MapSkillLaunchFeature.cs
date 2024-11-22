using DoomLauncher.Interfaces;
using DoomLauncher.SourcePort;
using System.Text;

namespace DoomLauncher.Adapters.Launch
{
    public class MapSkillLaunchFeature : LaunchFeature
    {

        private readonly string _map;
        private readonly string _skill;

        public MapSkillLaunchFeature(string map, string skill)
        {
            _map = map;
            _skill = skill;
        }

        public LaunchParameters CreateParam(ISourcePortData sourcePort, IGameFile gameFile)
        {
            LaunchParameters result = LaunchParameters.EMPTY;

            if (_map != null)
            {
                var warpParam = LaunchParameters.Param(sourcePort.GetFlavor().WarpParameter(new SpData(_map)));
                result = warpParam;

                if (_skill != null)
                {
                    var skillParam = LaunchParameters.Param(sourcePort.GetFlavor().SkillParameter(new SpData(_skill)));
                    result = warpParam.Combine(skillParam);
                }
            }

            return result;
        }
    }
}
