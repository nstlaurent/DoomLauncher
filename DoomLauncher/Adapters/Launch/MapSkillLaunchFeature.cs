using DoomLauncher.SourcePort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public LaunchResult CreateParam(ISourcePort sourcePort)
        {
            var sb = new StringBuilder();

            if (_map != null)
            {
                sb.Append(sourcePort.WarpParameter(new SpData(_map)));

                if (_skill != null)
                    sb.Append(sourcePort.SkillParameter(new SpData(_skill)));
            }

            return LaunchResult.Success(sb.ToString());
        }
    }
}
