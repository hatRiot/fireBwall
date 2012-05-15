using System;
using fireBwall.Utils;

namespace fireBwall.Configuration
{
    public class ColorScheme
    {
        [Serializable()]
        public class Color
        {
            public Color(byte A, byte R, byte G, byte B)
            {
                Alpha = A;
                Red = R;
                Green = G;
                Blue = B;
            }

            public Color(System.Drawing.Color c)
            {
                Alpha = c.A;
                Red = c.R;
                Green = c.G;
                Blue = c.B;
            }

            public byte Alpha = 0x00;
            public byte Red = 0x00;
            public byte Green = 0x00;
            public byte Blue = 0x00;
        }

        public SerializableDictionary<string, Color> colors = new SerializableDictionary<string, Color>();
        public string Base64Image = null;
    }
}
