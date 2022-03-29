using System;
using System.Drawing;
using System.IO;
using System.Linq;
using WadReader;

namespace DoomLauncher
{
    public static class DoomImageUtil
    {
        public const string TitlepicName = "TITLEPIC";

        public static bool FindPalette(IArchiveReader archive, out IArchiveEntry entry) =>
            GetEntry(archive, "PLAYPAL", out entry);

        public static bool GetEntry(IArchiveReader archive, string name, out IArchiveEntry entry)
        {
            if (archive.EntriesHaveExtensions)
                entry = archive.Entries.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.Name).Equals(name, StringComparison.OrdinalIgnoreCase));
            else
                entry = archive.Entries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return entry != null;
        }

        public static bool ConvertToImage(byte[] data, Palette palette, out Image image)
        {
            try
            {
                if (IsPng(data) || IsJpg(data) || IsBmp(data))
                {
                    image = new Bitmap(new MemoryStream(data), true);
                    return true;
                }

                DoomImage doomImage;
                if (PaletteReaders.LikelyFlat(data))
                    doomImage = PaletteReaders.ReadFlat(data);
                else
                    doomImage = PaletteReaders.ReadColumn(data);

                if (palette != null && doomImage != null)
                    doomImage = doomImage.PaletteToArgb(palette);

                image = doomImage?.Bitmap;
                return image != null;
            }
            catch
            {
                image = null;
                return false;
            }
        }

        public static bool IsPng(byte[] data)
        {
            return data.Length > 8 && data[0] == 137 && data[1] == 'P' && data[2] == 'N' && data[3] == 'G';
        }

        public static bool IsJpg(byte[] data)
        {
            return data.Length > 10 && data[0] == 0xFF && data[1] == 0xD8;
        }

        public static bool IsBmp(byte[] data)
        {
            return data.Length > 14 && data[0] == 'B' && data[1] == 'M';
        }
    }
}
