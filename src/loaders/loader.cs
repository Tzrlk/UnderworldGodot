using System;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Underworld;

/// <summary>
/// Base class for loading data
/// </summary>
public class Loader : UWClass
{

    [Obsolete("Impure")]
    public string filePath;//To the file relative to the root of the game folder

    [Obsolete("Impure")]
    public bool DataLoaded;

    /// <summary>
    /// Reads the file into the file buffer
    /// </summary>
    /// <returns><c>true</c>, if stream file was  read, <c>false</c> otherwise.</returns>
    /// <param name="Path">Path.</param>
    /// <param name="buffer">Buffer.</param>
    [Obsolete("Sullied")]
    public static bool ReadStreamFile(String Path, out byte[] buffer)
    {
        if (File.Exists(Path))
        {
            buffer = File.ReadAllBytes(Path);
            return buffer != null;
        }

        Debug.Print("DataLoader.ReadStreamFile : File not found : " + Path);
        buffer = null;
        return false;
    }

    [Pure]
    private static uint ConvertInt16(byte byte1, byte byte2)
        => (uint)(byte2 << 8 | byte1);

    [Pure]
    private static uint ConvertInt24(byte byte1, byte byte2, byte byte3)
        => (uint)(byte3 << 16 | byte2 << 8 | byte1);

    [Pure]
    private static uint ConvertInt32(byte byte1, byte byte2, byte byte3, byte byte4)
        => (uint)(byte4 << 24 | byte3 << 16 | byte2 << 8 | byte1);

    /// <summary>
    /// Gets the value at the specified address in the file buffer and performs any necessary -endian conversions
    /// Gets contents of bytes the the specific integer address. int(8), int(16), int(32) per uw-formats.txt
    /// </summary>
    /// <returns>The value at address.</returns>
    /// <param name="buffer">Buffer.</param>
    /// <param name="address">Address.</param>
    /// <param name="size">Size of the data in bits</param>
    [Pure]
    public static uint GetAt(byte[] buffer, long address, int size)
        => size switch
        {
            8 => buffer[address],
            16 => ConvertInt16(buffer[address], buffer[address + 1]),
            24 => ConvertInt24(buffer[address], buffer[address + 1], buffer[address + 2]),
            32 => ConvertInt32(buffer[address], buffer[address + 1], buffer[address + 2], buffer[address + 3]),
            _ => throw new InvalidOperationException($"Unsupported size: {size}."),
        };

    public static void SetAt(byte[] buffer, long address, int size, int val)
    {
        switch (size)
        {
            case 8:
                buffer[address] = (byte)(val & 0xff);
                break;
            case 16:
                buffer[address] = (byte)(val & 0xff);
                buffer[address + 1] = (byte)(val >> 8 & 0xff);
                break;
            case 24:
                // Why doesn't this have an implementation?
                Debug.WriteLine($"Tried to write {size} bits of {val} to {address}");
                break;
            case 32:
                buffer[address] = (byte)(val & 0xff);
                buffer[address + 1] = (byte)(val >> 8 & 0xff);
                buffer[address + 2] = (byte)(val >> 16 & 0xff);
                buffer[address + 3] = (byte)(val >> 24 & 0xff);
                break;
            case _:
                throw new InvalidOperationException($"Unsupported size: {size}.");
        }
    }

}