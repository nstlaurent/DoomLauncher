using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.Demo
{
    public class CldDemoParser : IDemoParser
    {
        private enum DemoCmds
        {
            CLD_DEMOLENGTH = 242,
            CLD_DEMOVERSION,
            CLD_CVARS,
            CLD_USERINFO,
            CLD_BODYSTART,
            CLD_TICCMD,
            CLD_LOCALCOMMAND,
            CLD_DEMOEND,
            CLD_DEMOWADS,
            NUM_DEMO_COMMANDS
        };

        private readonly string m_file;
        private const int m_hdr = 0x444C435A;

        public CldDemoParser(string demofile)
        {
            m_file = demofile;
        }

        public bool CanParse()
        {
            if (File.Exists(m_file))
            {
                using (BinaryReader br = new BinaryReader(new FileStream(m_file, FileMode.Open)))
                {
                    return br.BaseStream.Length > 16 && br.ReadInt32() == m_hdr && br.ReadByte() == (byte)DemoCmds.CLD_DEMOLENGTH;
                }
            }

            return false;
        }

        public string[] GetRequiredFiles()
        {
            string[] wads = new string[] { };
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(m_file, FileMode.Open)))
                {
                    if (br.ReadInt32() == m_hdr && br.ReadByte() == (byte)DemoCmds.CLD_DEMOLENGTH)
                    {
                        bool bodyStart = false;

                        while (!bodyStart)
                        {
                            byte cmd = br.ReadByte();
                            switch ((DemoCmds)cmd)
                            {
                                case DemoCmds.CLD_DEMOVERSION:
                                    ReadVersion(br);
                                    break;

                                case DemoCmds.CLD_DEMOWADS:
                                    return ReadDemoWads(br);

                                case DemoCmds.CLD_BODYSTART:
                                    bodyStart = false;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                //bad file, do nothing
            }

            return wads;
        }

        private void ReadVersion(BinaryReader br)
        {
            br.ReadInt16(); //demo version
            ReadString(br); //string app version
            br.ReadByte();
            br.ReadInt32();
        }

        private string[] ReadDemoWads(BinaryReader br)
        {
            List<string> wads = new List<string>();
            short wadCount = br.ReadInt16();
            for (short i = 0; i < wadCount; i++)
                wads.Add(ReadString(br));
            return wads.ToArray();
        }

        private string ReadString(BinaryReader br)
        {
            List<char> data = new List<char>();

            do
            {
                data.Add((char)br.ReadByte());
            } while (data.Last() != 0);

            return new string(data.Take(data.Count - 1).ToArray());
        }
    }
}
