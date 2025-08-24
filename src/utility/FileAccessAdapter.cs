using System;
using System.Buffers.Binary;
using System.ComponentModel;

namespace Underworld.Utility;

public interface IFileAccess
{

    public void Seek(ulong position);
    public byte Get8();
    public ushort Get16();
    public uint Get32();
    public ulong Get64();  

    public class GodotFileAccess(Godot.FileAccess file) : IFileAccess
    {
        public void Seek(ulong position) => file.Seek(position);
        public byte Get8() => file.Get8();
        public ushort Get16() => file.Get16();
        public uint Get32() => file.Get32();
        public ulong Get64() => file.Get64();
    }

    public class SystemIoFileAccess(System.IO.FileStream file) : IFileAccess
    {

        // Used to store read data for individual values.
        private readonly byte[] ShortBuffer = new byte[6];

        // Caution, this is almost certainly a bad numeric conversion and will hurt painfully.
        public void Seek(ulong position) => file.Seek((long)position, System.IO.SeekOrigin.Begin);

        public byte Get8()
        {
            file.ReadExactly(ShortBuffer, 0, 1);
            return ShortBuffer[0];
        }
        public ushort Get16()
        {
            file.ReadExactly(ShortBuffer, 0, 2);
            return (ushort)(ShortBuffer[1] << 8 | ShortBuffer[0]);
        }
        public uint Get32()
        {
            file.ReadExactly(ShortBuffer, 0, 4);
            return BinaryPrimitives.ReadUInt
            return (uint)(ShortBuffer[3] << 24 | ShortBuffer[2] << 16 | ShortBuffer[1] << 8 | ShortBuffer[0]);
        }
        public ulong Get64()
        {
            file.ReadExactly(ShortBuffer, 0, 8);
            return BinaryPrimitives.ReadUInt64BigEndian(ShortBuffer.AsSpan());
        }
    }

}
