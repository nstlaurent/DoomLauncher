using DoomLauncher.Interfaces;
using DoomLauncher.SaveGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoomLauncher.SourcePort
{
    public class GenericSourcePort : ISourcePort
    {
        protected readonly ISourcePortData m_sourcePortData;

        public GenericSourcePort(ISourcePortData sourcePortData)
        {
            m_sourcePortData = sourcePortData;
        }

        public virtual string IwadParameter(SpData data)
        {
            return string.Format(" -iwad \"{0}\"", data.Value);
        }

        public virtual string FileParameter(SpData data)
        {
            if (!string.IsNullOrEmpty(m_sourcePortData.FileOption))
                return string.Concat(" ", m_sourcePortData.FileOption, " ");
            return string.Empty;
        }

        public virtual string WarpParameter(SpData data)
        {
            return BuildWarpParameter(data.Value);
        }

        public virtual string SkillParameter(SpData data)
        {
            return string.Format(" -skill {0}", data.Value);
        }

        public virtual string RecordParameter(SpData data)
        {
            return string.Format(" -record \"{0}\"", data.Value);
        }

        public virtual string PlayDemoParameter(SpData data)
        {
            return string.Format(" -playdemo \"{0}\"", data.Value);
        }

        public static string BuildWarpParameter(string map)
        {
            if (Regex.IsMatch(map, @"^E\dM\d$") || Regex.IsMatch(map, @"^MAP\d\d$"))
                return BuildWarpLegacy(map);

            return string.Format(" +map {0}", map);
        }

        public virtual bool Supported()
        {
            return true;
        }

        public virtual bool StatisticsSupported()
        {
            return false;
        }

        public virtual IStatisticsReader CreateStatisticsReader(IGameFile gameFile, IEnumerable<IStatsData> existingStats)
        {
            return null;
        }

        public virtual ISaveGameReader CreateSaveGameReader(FileInfo file)
        {
            if (file.Extension.Equals(".zds", StringComparison.InvariantCultureIgnoreCase))
                return new ZDoomSaveGameReader(file.FullName);
            else if (file.Extension.Equals(".dsg", StringComparison.InvariantCultureIgnoreCase))
                return new DsgSaveGameReader(file.FullName);

            return null;
        }

        private static string BuildWarpLegacy(string map)
        {
            List<string> numbers = new List<string>();
            StringBuilder num = new StringBuilder();

            for (int i = 0; i < map.Length; i++)
            {
                if (char.IsDigit(map[i]))
                {
                    num.Append(map[i]);
                }
                else
                {
                    if (num.Length > 0) numbers.Add(num.ToString());
                    num.Clear();
                }
            }

            if (num.Length > 0)
                numbers.Add(num.ToString());

            StringBuilder sb = new StringBuilder();

            foreach (string number in numbers)
            {
                sb.Append(Convert.ToInt32(number));
                sb.Append(' ');
            }

            if (numbers.Any())
                sb.Remove(sb.Length - 1, 1);

            return string.Format(" -warp {0}", sb.ToString());
        }
    }
}
