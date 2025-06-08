using System;
using System.Drawing;
using System.IO;
using System.Linq;
using WadReader;

namespace DoomLauncher.Handlers.Sync
{
    public static class TitlePicUtil
    {
        const int TitleWidth = 320;
        const int TitleHeight = 200;

        public static bool FindPalette(IArchiveReader archive, out IArchiveEntry entry) =>
            GetEntry(archive, "PLAYPAL", out entry);

        public static bool GetEntry(IArchiveReader archive, string name, out IArchiveEntry entry)
        {
            try
            { 
                if (archive.EntriesHaveExtensions)
                    entry = archive.Entries.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.Name).Equals(name, StringComparison.OrdinalIgnoreCase));
                else
                    entry = archive.Entries.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                entry = null;
                return false;
            }
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

                // Heretic/hexen didn't use doom image format and was a flat list of indices
                if (doomImage == null && data.Length == TitleWidth * TitleHeight)
                    doomImage = DoomImage.FromPaletteIndices(TitleWidth, TitleHeight, data.Select(x => (ushort)x).ToArray(), 0, 0);

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

        private static bool IsPng(byte[] data)
        {
            return data.Length > 8 && data[0] == 137 && data[1] == 'P' && data[2] == 'N' && data[3] == 'G';
        }

        private static bool IsJpg(byte[] data)
        {
            return data.Length > 10 && data[0] == 0xFF && data[1] == 0xD8;
        }

        private static bool IsBmp(byte[] data)
        {
            return data.Length > 14 && data[0] == 'B' && data[1] == 'M';
        }
    }
}
