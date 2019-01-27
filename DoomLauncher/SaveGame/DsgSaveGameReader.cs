using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoomLauncher.SaveGame
{
    public class DsgSaveGameReader : ISaveGameReader
    {
        private readonly string m_file;

        public DsgSaveGameReader(string file)
        {
            m_file = file;
        }

        public string GetName()
        {
            try
            {
                using (var stream = File.OpenRead(m_file))
                {
                    byte[] data = new byte[24];
                    int read = stream.Read(data, 0, data.Length);

                    if (read != -1)
                    {
                        int index = data.ToList().IndexOf(0);
                        if (index == -1)
                            index = data.Length;
                        if (index > 0)
                            return Encoding.UTF8.GetString(data.Take(index).ToArray());
                    }
                }
            }
            catch
            {
                //continue to return the filename if we fail for any reason
            }

            return Path.GetFileName(m_file);
        }
    }
}
