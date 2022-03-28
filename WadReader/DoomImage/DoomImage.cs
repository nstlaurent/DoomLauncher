using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WadReader
{
    public enum ImageType
    {
        Palette,
        Argb
    }

    public class DoomImage
    {
        public const ushort TransparentIndex = 0xFF00;

        public readonly Bitmap Bitmap;
        public readonly ImageType ImageType;
        public readonly int OffsetX;
        public readonly int OffsetY;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public DoomImage(Bitmap bitmap, ImageType imageType, int offsetX, int offsetY)
        {
            Bitmap = EnsureExpectedFormat(bitmap);
            ImageType = imageType;
            Width = Bitmap.Width;
            Height = Bitmap.Height;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public DoomImage(int width, int height, ImageType imageType, int offsetX, int offsetY, Color? fillColor = null)
        {
            Width = Math.Max(width, 1);
            Height = Math.Max(height, 1);
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            ImageType = imageType;
            OffsetX = offsetX;
            OffsetY = offsetY;

            Fill(fillColor ?? Color.Transparent);
        }

        private static Bitmap EnsureExpectedFormat(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                return bitmap;

            Bitmap copy = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(copy))
                g.DrawImage(bitmap, new Rectangle(0, 0, copy.Width, copy.Height));

            return copy;
        }

        public static DoomImage FromArgbBytes(int w, int h, byte[] argb, int offsetX = 0, int offsetY = 0)
        {
            int numBytes = w * h * 4;

            if (argb.Length != numBytes || w <= 0 || h <= 0)
                return null;

            Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData metadata = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Marshal.Copy(argb, 0, metadata.Scan0, numBytes);
            bitmap.UnlockBits(metadata);

            return new DoomImage(bitmap, ImageType.Argb, offsetX, offsetY);
        }

        public static DoomImage FromPaletteIndices(int width, int height, ushort[] indices, int offsetX, int offsetY)
        {
            if (width <= 0 || height <= 0 || indices.Length != width * height)
                return null;

            int numBytes = width * height * 4;
            byte[] paletteData = new byte[numBytes];

            int argbIndex = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                ushort index = indices[i];
                paletteData[argbIndex] = (byte)~(index >> 8);
                paletteData[argbIndex + 3] = (byte)index;

                argbIndex += 4;
            }

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData metadata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            Marshal.Copy(paletteData, 0, metadata.Scan0, numBytes);
            bitmap.UnlockBits(metadata);

            return new DoomImage(bitmap, ImageType.Palette, offsetX, offsetY);
        }

        public void Fill(Color color)
        {
            using (SolidBrush b = new SolidBrush(color))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(Bitmap))
                    g.FillRectangle(b, 0, 0, Bitmap.Width, Bitmap.Height);
        }

        public DoomImage PaletteToArgb(Palette palette)
        {
            if (ImageType == ImageType.Argb)
                return this;

            int numBytes = Width * Height * 4;
            byte[] paletteBytes = new byte[numBytes];
            byte[] argbBytes = new byte[numBytes];

            Rectangle rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            BitmapData metadata = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            Marshal.Copy(metadata.Scan0, paletteBytes, 0, numBytes);
            Bitmap.UnlockBits(metadata);

            Color[] colors = palette.DefaultLayer;
            int offset = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // The first of the four bytes is the alpha, and if we have
                    // alpha, then we have to write the RGB. Otherwise, it is
                    // already set to transparent so our job would be done.
                    if (paletteBytes[offset] != 0)
                    {
                        int index = paletteBytes[offset + 3];
                        Color color = colors[index];

                        // Apparently since it reads it as a 32-bit word, then
                        // we need to write it in BGRA format.
                        argbBytes[offset] = color.B;
                        argbBytes[offset + 1] = color.G;
                        argbBytes[offset + 2] = color.R;
                        argbBytes[offset + 3] = 255;
                    }

                    offset += 4;
                }
            }

            DoomImage image = FromArgbBytes(Width, Height, argbBytes, OffsetX, OffsetY);
            if (image != null)
                return image;
            
            return new DoomImage(1, 1, ImageType.Argb, 0, 0);
        }
    }
}
