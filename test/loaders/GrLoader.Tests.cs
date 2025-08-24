namespace Underworld.Test.Loaders;

using System;
using System.IO;
using GdUnit4;

[TestSuite]
public sealed class GRLoaderTest
{
    [TestCase]
    public void LoadDoorAndExportToPng()
    {

        // TODO: Need Abyss data location
        var loader = new GRLoader(GRLoader.DOORS_GR, GRLoader.GRShaderMode.TextureShader)
        {
            UseRedChannel = true,
        };

        var loaded = loader.LoadImageFile();
        Assertions.AssertBool(loaded).IsTrue();

        // TODO: Generate temp directory
        var exportPath = Utils.CreateTempDir("exports");
        loader.ExportImages(exportPath);

        Console.WriteLine($"Listing files in {exportPath}");
        foreach (var item in Directory.EnumerateFiles(exportPath))
            Console.WriteLine($">> {item}");

        Console.WriteLine("All done.");
    }

}
