using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Underworld.Utility;

namespace Underworld.Loaders;

public static class GrFormat
{
    static readonly string[] RecognisedExtensions = ["gr"];
    static readonly string ResourceType = nameof(Resource);

    public partial class ResourceLoader : ResourceFormatLoader
    {

        public override string[] _GetRecognizedExtensions()
            => RecognisedExtensions;

        public override string _GetResourceType(string path)
            => path.EndsWith(".gr", true, null)
                ? ResourceType
                : "";

        public override bool _HandlesType(StringName type)
            => type == ResourceType;

        public override bool _Exists(string path)
            => File.Exists(path);

        public override Variant _Load(string path, string originalPath,
                bool useSubThreads, int cacheMode)
        {
            // Always prefer the original (non-imported) path.
            path = originalPath ?? path;

            // TODO make this async.
            var imageCount = GRLoader.LoadImageFile(path, out byte[] imageFileData);
            if (imageCount < 1) {
                GD.PushError("Unable to load GR file from ", path);
                return Variant.From(Error.CantAcquireResource);
            }

            // This is super dumb.


            return Variant.From(result);

        }

    }

    // I don't think this is the right implementation, since there's no
    // facility for producing packed images.
    public partial class ImageLoader : ImageFormatLoaderExtension
    {

        public override string[] _GetRecognizedExtensions()
            => ["gr"];

        public override Error _LoadImage(
                Image image,
                Godot.FileAccess fileAccess,
                LoaderFlags flags,
                float scale)
        {

            image.CopyFrom(null);
            // TODO try and allow for loading images from the files.
            return 0;

        }

    }

    public partial class GrEditorExport : EditorExportPlugin
    {
        // TODO: Prevent UW files being exported.
    }

    public partial class GrEditorImport : EditorImportPlugin
    {

        public const byte PANELS_WIDTH_UW1 = 83;
        public const byte PANELS_WIDTH_UW2 = 79;
        public const byte PANELS_HEIGHT_UW1 = 114;
        public const byte PANELS_HEIGHT_UW2 = 112;

        public override bool _CanImportThreaded()
            => true;

        public override string[] _GetRecognizedExtensions()
            => ["gr"];

        public override string _GetSaveExtension()
            => ".png";

        public override string _GetResourceType()
            => ResourceType;

        public override string _GetImporterName()
            => "UnderworldGR";

        public override int _GetFormatVersion()
            => 1;

        public override Error _Import(
            string sourceFile,
            string savePath,
            Dictionary options,
            Array<string> platformVariants,
            Array<string> genFiles)
        {
            // Start the import process.
            var task = _ImportAsync(sourceFile, savePath, options, platformVariants, genFiles);

            // Wait for it to complete, using a timeout if specified.
            if (options.ContainsKey("timeout"))
                task.Wait(options["timeout"].AsInt32());
            else
                task.Wait();

            // Finish with the final result.
            return task.Result;
        }

        // NOTE: Extract images from file in sequence within one thread. _Process_ those images in multiple threads.
        public static async Task<Error> _ImportAsync(
            string sourceFile,
            string savePath,
            Dictionary options,
            Array<string> platformVariants,
            Array<string> genFiles
        )
        {

            // Open source file for reading; bail immediately if anything goes wrong.
            using var file = Godot.FileAccess.Open(sourceFile, Godot.FileAccess.ModeFlags.Read);
            if (file.GetError() != Error.Ok)
                return file.GetError();

            // Start by loading the primary file info.
            var fileData = GraphicsFile.FromFile(file);

            // Load the palette to use.
            var paletteNo = options["palette"].AsInt16();
            var palette = PaletteLoader.Palettes[paletteNo];

            // PANELS.GR is a special case.
            GRLoader.ImageInfo? panels = null;
            if (sourceFile.ToUpper().EndsWith("PANELS.GR"))
            {
                foreach (var bleh in ReadImageDataPanels(file, fileData))
                {
                    // TODO
                }
            }

            // TODO: load file from path, iterate over images, load each one.
            await foreach (var image in LoadImageData(file, panels))
            {
                // TODO: Turn data into Images.
                // TODO: Save images into files.
            }

            return Error.Ok;

        }

        public static async IAsyncEnumerable<GRLoader.ImageInfo> ReadImageDataPanels(
            Godot.FileAccess file, GraphicsFile fileData)
        {

            // First figure out which game we're importing from.
            byte game = await Async.WhichGame(file.GetPath());

            // Then build the standard image info from that. The rest is
            // handled by the individual image import processes.
            var (width, height) = game switch
            {
                UWClass.GAME_UWDEMO or UWClass.GAME_UW1
                    => (PANELS_WIDTH_UW1, PANELS_HEIGHT_UW1),
                UWClass.GAME_UW2
                    => (PANELS_WIDTH_UW2, PANELS_HEIGHT_UW2),
                _
                    => throw new Exception($"Unrecognised game type: {game}"),
            };

            var length = width * height;
            foreach (var imageOffset in fileData.ImageOffsets)
            {
                file.Seek(imageOffset);
                yield return new(
                    Type: 0,
                    Width: width,
                    Height: height,
                    AuxPal: 0,
                    Length: length,
                    Data: file.GetBuffer(length)
                );
            }
                
        }

        
        public static async IAsyncEnumerable<GRLoader.ImageInfo> ReadImageData(
            Godot.FileAccess file, GraphicsFile fileData)
        {
            foreach (var imageOffset in fileData.ImageOffsets) {

                file.Seek(imageOffset);

                // Determine type and dimensions
                var type = file.Get8(); // o+1 (WARNING, might actually be o+0)

                
                var width = file.Get8(); // o+2
                var height = file.Get8(); // o+3
                var auxPal = file.Get8(); // o+4
                ushort length;
                byte[] data;

                if (type == GraphicsFileImageType.RAW_8BIT)
                {
                    length = (ushort)(width * height);
                    data = file.GetBuffer(length); // o+5
                }
                else
                {

                    // Get stored length, then check against bitmap dimensions.
                    length = file.Get16(); // o+5
                    var nibbles = Mathf.Max(
                        width * height * 2,
                        (length + 5) * 2
                    );

                    // Load the raw image data.
                    data = file.GetBuffer(nibbles); // o+6
                }

                // Package everything into a struct, and yield to force async behaviour.
                yield return new(type, width, height, auxPal, length, data);
                await Task.Yield();



                // TODO: Handle the rest as post-processing (since it also needs palette reads).

                // // Figure-out what aux palette we're supposed to be using
                // var auxPalIndex = options["auxPal"].AsByte(); // TODO: ?? auxPal;

                // // If it isn't compressed, finish early.
                // if (type == GRLoader.IMAGE_4BIT_UNCOMPRESSED)
                // {
                //     // TODO: Does the palette matter at this stage?
                //     var auxPalette = PaletteLoader.LoadAuxilaryPal("DATA/AUXPALFILE", palette, auxPalIndex);
                //     continue;
                // }

                // // Otherwise, decompress the bitmap
                // // TODO: Still need to load the aux palette??
                // var auxPalData = PaletteLoader.LoadAuxilaryPalIndices("DATA/AUXPALFILE", auxPalIndex);
                // var decoded = GRLoader.DecodeRLEBitmap(new GRLoader.ImageInfo(imageData, length, width, height), 4, auxPalData);

            }

        }

    }
    
    [Tool]
    public partial class GrResource : Resource
    {

        [Export]
        public GrResourceItem[] Items { get; set; } = [];

    }

    [Tool]
    public partial class GrResourceItem : ImageTexture
    {

        [Export]
        public ShaderMaterial Material { get; set; }

        public GrResourceItem(Image image)
        {
            SetImage(image);
        }

    }

}
