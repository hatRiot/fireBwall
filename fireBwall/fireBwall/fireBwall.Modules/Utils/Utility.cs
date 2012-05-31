using System;
using System.Runtime.InteropServices;

namespace fireBwall.Utils
{
    public class Utility
    {
        [DllImport("msvcrt.dll")]        
        public static extern int memcmp(byte[] b1, byte[] b2, int count);

        public static bool ByteArrayEq(byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (int x = 0; x < b1.Length; x++)
                {
                    if (b1[x] != b2[x])
                        return false;
                }
                return true;
            }
            return false;
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
