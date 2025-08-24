using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Underworld.Loaders.Graphics;

public readonly record struct GraphicsFileImage(
        uint Offset,
        GraphicsFileImageType Type,
        byte Width,
        byte Height,
        byte AuxPal,
        uint Length,
        byte[] Data
)
{

    public static GraphicsFileImage FromFile(GraphicsFile fileInfo, FileAccess file, uint imageOffset)
    {
        // Ensure we're at the right offset.
        file.Seek(imageOffset);

        // Determine image type.
        // C# handles enums from raw values in an annoying way, especially 
        // since the underlying type should be able to be directly compared,
        // but is prevented for some reason.
        var type = file.Get8(); // o+1 (WARNING, might actually be o+0)
        if (!Enum.IsDefined(typeof(GraphicsFileImageType), type))
            throw new Exception($"Unexpected image type: {type} at {imageOffset} in {file.GetPath()}");

        // Determine image dimensions.
        var width = file.Get8(); // o+2
        var height = file.Get8(); // o+3
        var auxPal = file.Get8(); // o+4
        uint length;
        byte[] data;

        // Really feels like the RAW_4BIT should be simply read as well.
        // Wouldn't only the RLE images need a length field?
        if (type == (byte)GraphicsFileImageType.RAW_8BIT)
        {
            length = (ushort)(width * height * 2);
            data = file.GetBuffer(length); // o+5
        }
        else
        {

            // Get stored length, then check against bitmap dimensions.
            length = file.Get16(); // o+5
            var nibbles = Math.Max(
                (long)width * height * 2,
                (length + 5) * 2
            );

            // Should we even be loading the whole image into a buffer here?
            // Or storing an offset and allowing for it to be streamed instead?
            // Would definitely serialise the processing, unfortunately, though
            // it would certainly be cool to be able to load the data back from
            // the file on an ad-hoc basis because we know all the offsets.
            data = file.GetBuffer(nibbles); // o+6
        }

        return new(
            Offset: imageOffset,
            Type: (GraphicsFileImageType)type,
            Width: width,
            Height: height,
            AuxPal: auxPal,
            Length: length,
            Data: data
        );
    }

    /// <summary>
    /// This function is just for getting data from a panel file (PANELS.GR),
    /// and barely needs to exist, since most of the logic is handled when the
    /// file is initially inspected.
    /// </summary>
    /// <param name="file">The file access stream to read from.</param>
    /// <param name="panel">A template for the panels to load with dimensions.</param>
    /// <param name="imageOffset">Where in the file to start reading from.</param>
    /// <returns>A copy of the supplied panel template, with the correct data loaded.</returns>
    public static GraphicsFileImage FromPanelsFile(FileAccess file, GraphicsFileImage panel, uint imageOffset)
    {

        // Ensure we're at the right place in the file.
        file.Seek(imageOffset);

        // Load the expected data size from the stream.
        var data = file.GetBuffer(panel.Length);

        // Copy the panel template with the new file data.
        return panel with
        {
            Offset = imageOffset,
            Data = data
        };

    }

    /// <summary>
    /// Loads all panel images from the provided file accessor as an async stream.
    /// </summary>
    /// <param name="file">The file accessor to read from.</param>
    /// <param name="panel">The expected panel metadata with dimensions.</param>
    /// <param name="imageOffsets">The sequence of image offsets to load.</param>
    /// <returns>A stream of panel data.</returns>
    public static IAsyncEnumerable<GraphicsFileImage> FromPanelsFile(FileAccess file, GraphicsFileImage panel, uint[] imageOffsets)
        => FromPanelsFile(file, panel, imageOffsets, CancellationToken.None);

    /// <summary>
    /// Lo
    /// </summary>
    /// <param name="file"></param>
    /// <param name="panel"></param>
    /// <param name="imageOffsets"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<GraphicsFileImage> FromPanelsFile(FileAccess file, GraphicsFileImage panel, uint[] imageOffsets,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var imageOffset in imageOffsets)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
            yield return FromPanelsFile(file, panel, imageOffset);
            await Task.Yield();
        }
    }

}