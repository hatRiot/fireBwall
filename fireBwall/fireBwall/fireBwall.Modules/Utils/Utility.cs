using System;
using System.Runtime.InteropServices;

namespace fireBwall.Modules.Utils
{
    public class Utility
    {
        [DllImport("msvcrt.dll")]
        public static extern int memcmp(byte[] b1, byte[] b2, int count);
    }
}
