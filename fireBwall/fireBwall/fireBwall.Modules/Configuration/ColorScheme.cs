using System;
using System.Xml.Serialization;
using fireBwall.Utils;

namespace fireBwall.Configuration
{
    [Serializable()]
    public class ColorScheme
    {
        [Serializable()]
        public class Color
        {
            public Color()
            { }

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

            public System.Drawing.Color ToSystemColor()
            {
                return System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
            }

            [XmlAttribute("A")]
            public byte Alpha = 0x00;
            [XmlAttribute("R")]
            public byte Red = 0x00;
            [XmlAttribute("G")]
            public byte Green = 0x00;
            [XmlAttribute("B")]
            public byte Blue = 0x00;
        }

        public SerializableDictionary<string, Color> colors = new SerializableDictionary<string, Color>();
        public string Base64Image = null;
        
        [XmlAttribute("Name")]
        public string Name = null;
    }
}
