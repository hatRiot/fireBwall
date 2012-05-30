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
    }
}
