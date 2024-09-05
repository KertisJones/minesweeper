// System
using System.Runtime.InteropServices;

namespace GUPS.AntiCheat.Protected
{
    /// <summary>
    /// Helper class to parse double to long and the other way around.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct ULongDouble
    {
        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong longValue;
    }
}
