// System
using System.Runtime.InteropServices;

namespace GUPS.AntiCheat.Protected
{
    /// <summary>
    /// Helper class to parse float to int and the other way around.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntFloat
    {
        [FieldOffset(0)]
        public float floatValue;

        [FieldOffset(0)]
        public uint intValue;
    }
}
