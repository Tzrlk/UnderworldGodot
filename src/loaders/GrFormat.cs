using System.IO;
using Godot;
using Godot.Collections;

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

            GrResource result = new(new GrResourceItem[imageCount]);
            for (int i = 0; i < imageCount; i++)
            {
                result.Items[i] = new()
                {
                    // loader.LoadImageAt(i),
                    Material = loader.GetMaterial(i)
                };
            }

            return Variant.From(result);

        }

    }

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
        public override bool _CanImportThreaded()
            => true;

        public override string[] _GetRecognizedExtensions()
            => ["gr"];

        public override string _GetSaveExtension()
            => ".packed_scene???";

        public override string _GetResourceType() => ResourceType;

        public override string _GetImporterName()
            => "UnderworldGR";

        public override int _GetFormatVersion()
            => 1;

        public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles)
        {
            // TODO:
            return 0;
        }

    }
    
    [Tool]
    public partial class GrResource(GrResourceItem[] items) : Resource
    {
        [Export]
        public GrResourceItem[] Items { get; set; } = items;

    }

    [Tool]
    public partial class GrResourceItem : ImageTexture
    {

        [Export]
        public ShaderMaterial Material { get; set; }

    }

}
