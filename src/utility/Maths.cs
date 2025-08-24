using System.Diagnostics.Contracts;

namespace Underworld.Utility;

public static class Maths
{
    
    [Pure]
    private static ushort ConvertInt16(byte byte1, byte byte2)
        => (ushort)(byte2 << 8 | byte1);

    [Pure]
    private static uint ConvertInt24(byte byte1, byte byte2, byte byte3)
        => (uint)(byte3 << 16 | byte2 << 8 | byte1);

    [Pure]
    private static uint ConvertInt32(byte byte1, byte byte2, byte byte3, byte byte4)
        => (uint)(byte4 << 24 | byte3 << 16 | byte2 << 8 | byte1);

    [Pure]
    private static ulong ConvertInt64(byte byte1, byte byte2, byte byte3, byte byte4, byte byte5)
        => (ulong)(byte5 << 32 | byte4 << 24 | byte3 << 16 | byte2 << 8 | byte1);

}