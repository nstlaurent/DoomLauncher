using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DoomLauncher
{
    public class ZdlParser
    {
        private static string s_regexFull = @"{0}=.*";
        private static string s_regex = @"{0}=";

        private readonly List<string> m_errors = new List<string>();

        private readonly IEnumerable<ISourcePortData> m_sourcePorts;
        private readonly IEnumerable<IIWadData> m_iwads;

        public ZdlParser(IEnumerable<ISourcePortData> sourcePorts, IEnumerable<IIWadData> iwads)
        {
            m_sourcePorts = sourcePorts;
            m_iwads = iwads;
        }

        public string[] Errors { get { return m_errors.ToArray(); } }

        public IGameFile[] Parse(string fileText)
        {
            m_errors.Clear();

            List<IGameFile> gameFiles = new List<IGameFile>();
            fileText = fileText.Replace("\r\n", "\n"); ;

            if (!fileText.StartsWith("[zdl.save]"))
            {
                m_errors.Add("Not a valid zdl file");
                return new IGameFile[] {};
            }

            string zdlFilePath = null;
            int zdlFileNum = 0;

            do
            {
                zdlFilePath = FindValue(fileText, string.Format("file{0}", zdlFileNum), s_regexFull).Trim();
                zdlFileNum++;

                if (!string.IsNullOrEmpty(zdlFilePath))
                {
                    GameFile gameFile = new GameFile();
                    gameFile.FileName = zdlFilePath;
                    gameFiles.Add(gameFile);
                }

            } while (!string.IsNullOrEmpty(zdlFilePath));

            if (gameFiles.Count > 0)
            {
                string skill = FindValue(fileText, "skill", s_regexFull);
                string port = FindValue(fileText, "port", s_regexFull);
                string warp = FindValue(fileText, "warp", s_regexFull);
                string iwad = FindValue(fileText, "iwad", s_regexFull);
                string parameters = FindValue(fileText, "extra", s_regexFull);

                IGameFile gameFile = gameFiles[0];
                gameFile.SettingsSkill = skill;
                gameFile.SettingsMap = warp;
                gameFile.Map = warp;
                gameFile.SettingsExtraParams = parameters;
                gameFile.SourcePortID = GetSourcePort(port);
                gameFile.IWadID = GetIWad(iwad);
            }
            else
            {
                m_errors.Add("Did not contain any files (e.g. file=0)");
            }

            return gameFiles.ToArray();
        }

        private int? GetSourcePort(string port)
        {
            ISourcePortData sp = m_sourcePorts.FirstOrDefault(x => x.Name.Equals(port, StringComparison.InvariantCultureIgnoreCase));

            if (sp == null)
                m_errors.Add(string.Format("Could not find Source Port - {0}", port));

            if (sp != null)
                return sp.SourcePortID;

            return null;
        }

        private int? GetIWad(string iwad)
        {
            iwad = Path.GetFileNameWithoutExtension(iwad);
            IIWadData data = m_iwads.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FileName).Equals(iwad, StringComparison.InvariantCultureIgnoreCase));

            if (data == null)
                m_errors.Add(string.Format("Could not find IWAD - {0}", iwad));

            if (data != null)
                return data.IWadID;

            return null;
        }

        private string FindValue(string text, string category, string regexFull)
        {
            Match m = new Regex(string.Format(regexFull, category), RegexOptions.Multiline).Match(text);

            if (m.Success)
                return m.Value.Substring((new Regex(string.Format(s_regex, category)).Match(m.Value).Value.Length));

            return string.Empty;
        }
    }
}
