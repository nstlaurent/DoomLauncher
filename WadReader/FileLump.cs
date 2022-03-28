using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WadReader
{
    public class FileLump
    {
        private string m_name;

        public int Length => m_filelump.size;

        public FileLump(Stream stream)
        {
            m_filelump = Util.ReadStuctureFromStream<filelump_t>(stream);
        }

        public FileLump(string name)
        {
            m_name = name;
        }

        public string Name
        {
            get
            {
                if (m_name == null)
                {
                    int index = Array.IndexOf(m_filelump.name, (byte)0);
                    if (index > 8 || index < 0)
                        index = 8;
                    m_name = System.Text.Encoding.UTF8.GetString(m_filelump.name, 0, index);
                }

                return m_name;
            }
        }

        public byte[] ReadData(FileStream fs)
        {
            fs.Seek(m_filelump.filepos, SeekOrigin.Begin);

            byte[] ret = new byte[m_filelump.size];
            fs.Read(ret, 0, ret.Length);

            return ret;
        }

        public void ReadData(FileStream fs, byte[] data, int offset, int length)
        {
            fs.Seek(m_filelump.filepos, SeekOrigin.Begin);
            fs.Read(data, offset, length);
        }

        public static int FileLumpByteSize
        {
            get { return Marshal.SizeOf(typeof(filelump_t)); }
        }

        private readonly filelump_t m_filelump;

        private struct filelump_t
        {
            public filelump_t(int i) //useless constructor, removes annoying unused variable warnings
            {
                filepos = size = i;
                name = new byte[8];
            }

            public int filepos;
            public int size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] name;
        }
    }
}
