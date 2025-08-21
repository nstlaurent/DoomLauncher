using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WadReader;

namespace DoomLauncher.Handlers.Sync
{
    public class TitlePicSyncAction : ISyncAction
    {
        private const string DoomTitlepicName = "TITLEPIC";
        private const string HereticHexenTitlepicName = "TITLE";
        private static readonly Regex TitlePageRegex = new Regex(@"titlepage\s*=\s*""([^""]*)""");
        private readonly Palette m_doomPalette;
        private readonly Palette m_hexenPalette;
        private readonly Palette m_hereticPalette;
        private readonly IDataSourceAdapter m_database;

        public TitlePicSyncAction(IDataSourceAdapter database, Palette doomPalette, Palette hexenPalette, Palette hereticPalette)
        {
            m_doomPalette = doomPalette;
            m_hexenPalette = hexenPalette;
            m_hereticPalette = hereticPalette;
            m_database = database;
        }

        public SyncResult ApplyToGameFile(IGameFile file, IArchiveReader reader, string[] mapInfoData)
        {
            string titlepicName = DoomTitlepicName;

            if (GetTitlepicNameFromMapInfo(mapInfoData, out string newTitlepicName))
                titlepicName = newTitlepicName;

            Palette palette = null;
            if (!TitlePicUtil.GetEntry(reader, titlepicName, out IArchiveEntry entry))
            {
                if (TitlePicUtil.GetEntry(reader, HereticHexenTitlepicName, out entry))
                {
                    var iwadFileName = m_database.GetIWads().FirstOrDefault(iw => iw.IWadID == file.IWadID)?.FileNameBase;

                    if (iwadFileName != null && iwadFileName.Equals("hexen", StringComparison.OrdinalIgnoreCase))
                        palette = m_hexenPalette;
                    else
                        palette = m_hereticPalette;
                }
                else
                {
                    return SyncResult.EMPTY;
                }
            }

            palette = palette ?? GetPaletteOrDefault(file, reader);

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
                return m_doomPalette;
            }

            Palette palette = Palette.From(paletteEntry.ReadEntry());
            if (palette != null)
                return palette;

            return m_doomPalette;
        }
    }
}