using System;
using System.Collections.Generic;
using System.Drawing;

namespace WadReader
{
    public class Palette
    {
        public static readonly int NumColors = 256;
        public static readonly int ColorComponents = 3;
        public static readonly int BytesPerLayer = NumColors * ColorComponents;
        private static Palette DefaultPalette;

        private readonly List<Color[]> layers;

        public int Count => layers.Count;
        public Color[] DefaultLayer => layers[0];

        private Palette(List<Color[]> paletteLayers)
        {
            layers = paletteLayers;
        }

        public static Palette From(byte[] data)
        {
            if (data.Length == 0 || data.Length % BytesPerLayer != 0)
                return null;

            List<Color[]> paletteLayers = new List<Color[]>();
            for (int layer = 0; layer < data.Length / BytesPerLayer; layer++)
            {
                int offset = layer * BytesPerLayer;
                Span<byte> layerSpan = new Span<byte>(data, offset, BytesPerLayer);
                paletteLayers.Add(PaletteLayerFrom(layerSpan));
            }

            return new Palette(paletteLayers);
        }

        private static Color[] PaletteLayerFrom(Span<byte> data)
        {
            Color[] paletteColors = new Color[NumColors];

            int offset = 0;
            for (int i = 0; i < BytesPerLayer; i += ColorComponents)
                paletteColors[offset++] = Color.FromArgb(255, data[i], data[i + 1], data[i + 2]);

            return paletteColors;
        }

        public Color[] Layer(int index) => layers[index];

        public static Palette GetDefaultPalette()
        {
            if (DefaultPalette != null)
                return DefaultPalette;

            byte[] data = new byte[NumColors * ColorComponents];

            for (int i = 0; i < NumColors; i++)
            {
                data[i] = (byte)i;
                data[i + 1] = (byte)i;
                data[i + 2] = (byte)i;
            }

            Palette palette = From(data);
            if (palette == null)
                throw new NullReferenceException("Failed to create the default palette, shouldn't be possible");

            DefaultPalette = palette;
            return palette;
        }
    }
}