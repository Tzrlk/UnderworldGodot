namespace Underworld.Loaders.Graphics;

public readonly record struct GraphicsFile(
    string Path,
    byte Type,
    ushort ImageCount,
    uint[] ImageOffsets
)
{

    /// <summary>
    /// Constructs file information by reading it from the supplied file
    /// stream. Will not reset the cursor after reading.
    /// </summary>
    /// <param name="file">The file accessor to read from.</param>
    /// <returns>All the parsed graphics file metadata.</returns>
    public static GraphicsFile FromFile(Godot.FileAccess file)
    {
        // Ensure we're at the start of the file.
        file.Seek(0);

        // Load file metadata
        var fileType = file.Get8(); // 0
        var imageCount = file.Get16(); // 1-2

        // Load individual image offsets.
        var imageOffsets = new uint[imageCount];
        for (var index = 0; index < imageCount; index++)
            imageOffsets[index] = file.Get32();

        return new(
            Path: file.GetPath(),
            Type: fileType,
            ImageCount: imageCount,
            ImageOffsets: imageOffsets
        );
    }

}