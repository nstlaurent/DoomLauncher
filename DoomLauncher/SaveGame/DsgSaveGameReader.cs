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
                    byte[] data = new byte[32];
                    int offs = 0, read = 0;

                    do
                    {
                        read = stream.Read(data, offs, 1);
                    }
                    while (offs < data.Length && read != -1 && data[offs++] != 0);
                    
                    if (offs > 0)
                        return Encoding.UTF8.GetString(data.Take(offs-1).ToArray());
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
