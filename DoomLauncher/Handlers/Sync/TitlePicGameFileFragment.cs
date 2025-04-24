using DoomLauncher.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WadReader;
using System.Drawing;

namespace DoomLauncher.Handlers.Sync
{
    public class TitlePicFileFragment : IGameFileFragment
    {
        private const string DefaultTitlepicName = "TITLEPIC";
        private static readonly Regex TitlePageRegex = new Regex(@"titlepage\s*=\s*""([^""]*)""");
        private readonly Palette m_palette;

        public TitlePicFileFragment(Palette palette)
        {
            m_palette = palette;
        }

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            var m_titlepics = new Dictionary<IGameFile, Image>();

            string titlepicName = DefaultTitlepicName;
            if (GetTitlepicNameFromMapInfo(mapInfoData, out string newTitlepicName))
                titlepicName = newTitlepicName;

            if (!TitlePicUtil.GetEntry(reader, titlepicName, out IArchiveEntry entry))
                return SyncResult.EMPTY;

            Palette palette = GetPaletteOrDefault(reader);
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

        private Palette GetPaletteOrDefault(IArchiveReader reader)
        {
            if (!TitlePicUtil.FindPalette(reader, out IArchiveEntry paletteEntry))
                return m_palette;

            Palette palette = Palette.From(paletteEntry.ReadEntry());
            if (palette != null)
                return palette;

            return m_palette;
        }
    }
}