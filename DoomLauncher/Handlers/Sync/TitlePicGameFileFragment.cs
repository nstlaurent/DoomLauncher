using DoomLauncher.Interfaces;
using System.Text.RegularExpressions;
using WadReader;
using System.Drawing;
using System;
using System.IO;

namespace DoomLauncher.Handlers.Sync
{
    public class TitlePicFileFragment : IGameFileFragment
    {
        private const string DefaultTitlepicName = "TITLEPIC";
        private const string AltTitlepicName = "TITLE";
        private static readonly Regex TitlePageRegex = new Regex(@"titlepage\s*=\s*""([^""]*)""");
        private readonly Palette m_palette;
        private readonly Palette m_altPalette;

        public TitlePicFileFragment(Palette doomPalette, Palette hexenPalette)
        {
            m_palette = doomPalette;
            m_altPalette = hexenPalette;
        }

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            string titlepicName = DefaultTitlepicName;
            if (GetTitlepicNameFromMapInfo(mapInfoData, out string newTitlepicName))
                titlepicName = newTitlepicName;

            if (!TitlePicUtil.GetEntry(reader, titlepicName, out IArchiveEntry entry))
            {
                if (!TitlePicUtil.GetEntry(reader, AltTitlepicName, out entry))
                    return SyncResult.EMPTY;
            }

            Palette palette = GetPaletteOrDefault(file, reader);
            if (!TitlePicUtil.ConvertToImage(entry.ReadEntry(), palette, out Image image))
            {
                return SyncResult.FailedTitlePicFile(file);
            }

            return SyncResult.TitlePic(file, image);
        }

        private bool GetTitlepicNameFromMapInfo(string[] mapInfoData, out string newTitlepicName)
        {
            newTitlepicName = string.Empty;
            foreach (string data in mapInfoData)
            {
                Match match = TitlePageRegex.Match(data);
                if (!match.Success)
                    continue;

                newTitlepicName = match.Groups[1].Value;
                return true;

            }

            return false;
        }

        private Palette GetPaletteOrDefault(IGameFile gameFile, IArchiveReader reader)
        {
            if (!TitlePicUtil.FindPalette(reader, out IArchiveEntry paletteEntry))
            {
                if (Path.GetFileNameWithoutExtension(gameFile.FileNameNoPath).Equals("hexdd", StringComparison.OrdinalIgnoreCase))
                    return m_altPalette;

                return m_palette;
            }

            Palette palette = Palette.From(paletteEntry.ReadEntry());
            if (palette != null)
                return palette;

            return m_palette;
        }
    }
}