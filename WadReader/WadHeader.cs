using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace WadReader
{
    public class WadHeader
    {
        private const int s_iwad = 1145132873;
        private const int s_pwad = 1145132880;

        public WadHeader(Stream stream)
        {
            if (stream.Length < Marshal.SizeOf(typeof(wadinfo_t)))
            {
                WadType = WadType.Unknown;
            }
            else
            {
                m_stream = stream;
                m_wadinfo = Util.ReadStuctureFromStream<wadinfo_t>(m_stream);

                switch (m_wadinfo.identification)
                {
                    case s_iwad:
                        WadType = WadType.IWAD;
                        break;
                    case s_pwad:
                        WadType = WadType.PWAD;
                        break;
                    default:
                        WadType = WadType.Unknown;
                        break;
                }
            }
        }

        public List<FileLump> ReadLumps()
        {
            List<FileLump> lumps = new List<FileLump>(m_wadinfo.numlumps);

            try
            {
                m_stream.Seek(m_wadinfo.infotableofs, SeekOrigin.Begin);

                for (int i = 0; i < m_wadinfo.numlumps; i++)
                    lumps.Add(new FileLump(m_stream));
            }
            catch
            {
                //failed, nothing to do
            }

            return lumps;
        }

        public WadType WadType
        {
            get;
            private set;
        }

        private readonly wadinfo_t m_wadinfo;
        private readonly Stream m_stream;

        private struct wadinfo_t
        {
            public wadinfo_t(int i) //useless constructor, removes annoying unused variable warnings
            {
                identification = numlumps = infotableofs = i;
            }

            public int identification;
            public int numlumps;
            public int infotableofs;
        };
    }
}
