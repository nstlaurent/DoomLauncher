using DoomLauncher.Interfaces;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DoomLauncher.Handlers.Sync {


    /// <summary>
    /// https://zdoom.org/wiki/Startup_lumps
    /// Planar image reading code adapted from https://github.com/ZDoom/gzdoom/blob/5e35ebc8fe698f86c8b0c4c98774bc397f30e7d4/src/common/textures/formats/startuptexture.cpp
    /// </summary>
    public class StartupImageSyncAction : ISyncAction
    {
        private static readonly int WIDTH = 640;
        private static readonly int HEIGHT = 480;
        private static readonly int PALETTE_LENGTH = 48; // 16 colors, 3 bytes each (RGB)
        private static readonly int PIXELS_LENGTH = WIDTH * HEIGHT / 2; // 4 bits per pixel
        private static readonly int STARTUP_LENGTH = PALETTE_LENGTH + PIXELS_LENGTH;

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            var entry = reader.Entries.FirstOrDefault(e => 
                e.Name.ToLower().StartsWith("startup") // STARTUP, startup.dat, startup.png, startup.lmp etc
                && !e.Name.ToLower().EndsWith(".wad") // Not an image!
                && !e.Name.ToLower().StartsWith("startup0")); // Ignore Strife startup image

            if (entry != null)
            {
                byte[] entryBytes = entry.ReadEntry();

                try 
                { 
                    Image pngImage = new Bitmap(new MemoryStream(entryBytes), true);
                    return SyncResult.TitlePic(gameFile, pngImage);
                }
                catch (System.Exception)
                {
                    // OK it's not a PNG... let's try planar image
                }

                if (entry.Length != STARTUP_LENGTH)
                {
                    return SyncResult.FailedTitlePicFile(gameFile);
                }

                Color[] palette = ReadPalette(entryBytes);
                Image image = ReadPlanarPalettedImage(palette, entryBytes);
                return SyncResult.TitlePic(gameFile, image);
            }

            return SyncResult.EMPTY;
        }


        // Each pixel is an index into the 16-length palette array.
        // However, this is a planar image, meaning that each index is split apart into bits, 
        // rather than being stored as a single byte.
        // All the first bits of all the indices come first, then all the second bits, etc.
        private Image ReadPlanarPalettedImage(Color[] palette, byte[] entryBytes)
        {
            Bitmap image = new Bitmap(640, 480);
            int planeSize = WIDTH * HEIGHT / 8;

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH / 8; x++)
                {
                    int offset = PALETTE_LENGTH + y * (WIDTH / 8) + x;
                    byte p0 = entryBytes[offset];
                    byte p1 = entryBytes[offset + planeSize];
                    byte p2 = entryBytes[offset + 2 * planeSize];
                    byte p3 = entryBytes[offset + 3 * planeSize];

                    for (int bit = 0; bit < 8; bit++)
                    {
                        int mask = 1 << (7 - bit);
                        int colorIndex =
                            ((p0 & mask) != 0 ? 1 : 0) |
                            ((p1 & mask) != 0 ? 2 : 0) |
                            ((p2 & mask) != 0 ? 4 : 0) |
                            ((p3 & mask) != 0 ? 8 : 0);

                        image.SetPixel(x * 8 + bit, y, palette[colorIndex]);
                    }
                }
            }
            return image;
        }

        private Color[] ReadPalette(byte[] entryBytes)
        {
            Color[] palette = new Color[16];
            for (int i = 0; i < 16; i++)
            {
                int r = entryBytes[i * 3 + 0] * 255 / 63;
                int g = entryBytes[i * 3 + 1] * 255 / 63;
                int b = entryBytes[i * 3 + 2] * 255 / 63;
                palette[i] = Color.FromArgb(r, g, b);
            }
            return palette;
        }
    }
}
