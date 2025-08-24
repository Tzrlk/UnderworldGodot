namespace Underworld.Loaders.Graphics;

/// <summary>
/// Captures the image type information stored for each item in a 'GR' file.
/// </summary>
/// <remarks>
/// I was hoping that an obvious bitmask was apparent. While the pixel size
/// looks like it can be determined from the first two(?) bits, the encoding
/// method appears arbitrary.
/// I could also just be missing some annoying mathematical trick they used.
/// </remarks>
public enum GraphicsFileImageType : byte
{

    /// <summary>
    /// 8-bit uncompressed.
    /// </summary>
    RAW_8BIT = 0x4, // 0b_0100

    /// <summary>
    /// 4-bit Run-length encoding.
    /// </summary>
    RLE_4BIT = 0x8, // 0b_1000

    /// <summary>
    /// 4-bit uncompressed.
    /// </summary>
    RAW_4BIT = 0xA  // 0b_1010

}