using Godot;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Underworld.Loaders;

namespace Underworld;

/// <summary>
/// Loads data from various GR Files.
/// One instance per file type
/// </summary>
public class GRLoader : ArtLoader
{

    [Obsolete("Stateful")]
    public bool UseRedChannel = false;

    public const int ThreeDWIN_GR = 0;
    public const int ANIMO_GR = 1;
    public const int ARMOR_F_GR = 2;
    public const int ARMOR_M_GR = 3;
    public const int BODIES_GR = 4;
    public const int BUTTONS_GR = 5;
    public const int CHAINS_GR = 6;
    public const int CHARHEAD_GR = 7;
    public const int CHRBTNS_GR = 8;
    public const int COMPASS_GR = 9;
    public const int CONVERSE_GR = 10;
    public const int CURSORS_GR = 11;
    public const int DOORS_GR = 12;
    public const int DRAGONS_GR = 13;
    public const int EYES_GR = 14;
    public const int FLASKS_GR = 15;
    public const int GENHEAD_GR = 16;
    public const int HEADS_GR = 17;
    public const int INV_GR = 18;
    public const int LFTI_GR = 19;
    public const int OBJECTS_GR = 20;
    public const int OPBTN_GR = 21;
    public const int OPTB_GR = 22;
    public const int OPTBTNS_GR = 23;
    public const int PANELS_GR = 24;
    public const int POWER_GR = 25;
    public const int QUEST_GR = 26;
    public const int SCRLEDGE_GR = 27;
    public const int SPELLS_GR = 28;
    public const int TMFLAT_GR = 29;
    public const int TMOBJ_GR = 30;
    public const int WEAPONS_GR = 31;
    public const int GEMPT_GR = 32;
    public const int GHED_GR = 33;

    public static readonly string[] pathGR = [
        "3DWIN.GR",
        "ANIMO.GR",
        "ARMOR_F.GR",
        "ARMOR_M.GR",
        "BODIES.GR",
        "BUTTONS.GR",
        "CHAINS.GR",
        "CHARHEAD.GR",
        "CHRBTNS.GR",
        "COMPASS.GR",
        "CONVERSE.GR",
        "CURSORS.GR",
        "DOORS.GR",
        "DRAGONS.GR",
        "EYES.GR",
        "FLASKS.GR",
        "GENHEAD.GR",
        "HEADS.GR",
        "INV.GR",
        "LFTI.GR",
        "OBJECTS.GR",
        "OPBTN.GR",
        "OPTB.GR",
        "OPTBTNS.GR",
        "PANELS.GR",
        "POWER.GR",
        "QUEST.GR",
        "SCRLEDGE.GR",
        "SPELLS.GR",
        "TMFLAT.GR",
        "TMOBJ.GR",
        "WEAPONS.GR",
        "GEMPT.GR",
        "GHED.GR",
    ];

    [Obsolete("Stateful")]
    readonly string AuxPalPath = "ALLPALS.DAT";
    
    [Obsolete("Stateful")]
    readonly bool useOverrideAuxPalIndex = false;
    
    [Obsolete("Stateful")]
    readonly int OverrideAuxPalIndex = 0;

    [Obsolete("Stateful")]
    public int FileToLoad;
    
    [Obsolete("Stateful")]
    private bool ImageFileDataLoaded;
    
    [Obsolete("Stateful")]
    public int NoOfImages;

    [Obsolete("Stateful")]
    public Shader textureshader;
    
    [Obsolete("Stateful")]
    public ImageTexture[] ImageCache = new ImageTexture[1];

    [Obsolete("Stateful")]
    public ShaderMaterial[] materials = new ShaderMaterial[1];

    // public GRLoader(int File, int PalToUse)
    // {
    //     // AuxPalPath = AuxPalPath.Replace("--", sep.ToString());
    //     useOverrideAuxPalIndex = false;
    //     OverrideAuxPalIndex = 0;
    //     FileToLoad = File;
    //     PaletteNo = (short)PalToUse;
    //     LoadImageFile();
    // }

    [Obsolete("Stateful")]
    public GRLoader(int File, GRShaderMode shaderMode)
    {
        textureshader = GetShader(shaderMode);
        // AuxPalPath = AuxPalPath.Replace("--", sep.ToString());
        useOverrideAuxPalIndex = false;
        OverrideAuxPalIndex = 0;
        FileToLoad = File;
        artData.PaletteNo = 0;
        LoadImageFile();
    }

    [Obsolete("Stateful")]
    public ShaderMaterial GetMaterial(int textureno)
        => materials[textureno] ??= GetMaterial(LoadImageAt(textureno, true), textureshader);

    [Obsolete("Stateful")]
    public override bool LoadImageFile()
    {
        var fileToLoad = Path.Combine(BasePath, "DATA", pathGR[FileToLoad]);
        NoOfImages = LoadImageFile(fileToLoad, out artData.ImageFileData);
        if (NoOfImages < 1)
        {
            GD.PushError("Unable to load GR file from ", fileToLoad);
            return false;
        }

        ImageCache = new ImageTexture[NoOfImages];
        materials = new ShaderMaterial[NoOfImages];
        ImageFileDataLoaded = true;
        return true;

    }

    [Obsolete("Stateful")]
    public override ImageTexture LoadImageAt(int index, bool UseAlphaChannel = true)
    {
        if (!ImageFileDataLoaded && !LoadImageFile())
            return null;
        return ImageCache[index] ??= LoadImageAt(
            FileToLoad,
            ImageFileData,
            index,
            PaletteNo,
            AuxPalPath,
            OverrideAuxPalIndex,
            useOverrideAuxPalIndex,
            new ImageConfig(
                AlphaChannel: UseAlphaChannel,
                RedChannel: UseRedChannel,
                Crop: UseCropping
            ));
    }

    [Obsolete("Stateful")]
    public void ExportImages(string exportpath)
        => Enumerable.Range(0, NoOfImages).AsParallel().ForAll(
            (i) => LoadImageAt(i).GetImage()
                .SavePng(Path.Combine(exportpath, $"{i:000}.png")));

    ///////////////////////////////////////////////////////////////////////////
    
    public enum RecordState
    {
        RepeatStart = 0,
        Repeat = 1,
        Run = 2,
    }

    public enum GRShaderMode
    {
        None = 0,
        SpriteShader = 1,  //Spritesthat will not be billboarded.
        BillboardSpriteShader = 2, //Sprites that will be billboarded
        TextureShader = 3,  //World textures
        UIShader = 4,  //For ui elements that need palette cycling
    };

    [Pure]
    private static Shader GetShader(GRShaderMode shaderMode) => shaderMode switch
    {
        GRShaderMode.TextureShader or GRShaderMode.SpriteShader
            => ResourceLoader.Load<Shader>("res://resources/shaders/uwshader.gdshader"),
        GRShaderMode.BillboardSpriteShader
            => ResourceLoader.Load<Shader>("res://resources/shaders/uwsprite.gdshader"),
        GRShaderMode.UIShader
            => ResourceLoader.Load<Shader>("res://resources/shaders/uisprite.gdshader"),
        _
            => null,
    };

    /// <summary>
    /// Configures and creates a shader material for the given texture and shader.
    /// </summary>
    /// <param name="texture">Texture with alpha-channel.</param>
    /// <param name="textureShader">The particular shader to use.</param>
    /// <returns></returns>
    [Pure]
    public static ShaderMaterial GetMaterial(ImageTexture texture, Shader textureShader)
    {
        var newmaterial = new ShaderMaterial { Shader = textureShader };
        newmaterial.SetShaderParameter("texture_albedo", texture);
        newmaterial.SetShaderParameter("albedo", new Color(1, 1, 1, 1));
        newmaterial.SetShaderParameter("uv1_scale", new Vector3(1, 1, 1));
        newmaterial.SetShaderParameter("uv2_scale", new Vector3(1, 1, 1));
        newmaterial.SetShaderParameter("UseAlpha", true);
        return newmaterial;
    }

    /// <summary>
    /// Loads the requested .GR file, returning the number of images it contains.
    /// </summary>
    /// <param name="path">Path to the .GR file to load.</param>
    /// <param name="data">The byte data obtained from the file.</param>
    /// <returns>The number of images in the file.</returns>
    [Obsolete("Sullied")]
    public static int LoadImageFile(string path, out byte[] data)
        => ReadStreamFile(path, out data)
            ? (int)GetAt(data, 1, 16)
            : -1;
            
    public const uint NIBBLE_HIGH = 0x0F;
    public const uint NIBBLE_LOW = 0xf;

    /// <summary>
    /// Copies the nibbles.
    /// </summary>
    /// <param name="inputData">Input data.</param>
    /// <param name="outputData">Output data.</param>
    /// <param name="noOfNibbles">No of nibbles.</param>
    /// <param name="addrPtr">Add ptr.</param>
    /// This code from underworld adventures
    public static void CopyNibbles(byte[] inputData, ref byte[] outputData, int noOfNibbles, long addrPtr)
    {
        //Split the data up into it's nibbles.
        int i = 0;
        noOfNibbles *= 2;
        while (noOfNibbles > 1)
        {
            if (addrPtr <= inputData.GetUpperBound(0))
            {
                outputData[i] = (byte)((GetAt(inputData, addrPtr, 8) >> 4) & NIBBLE_HIGH);
                outputData[i + 1] = (byte)((GetAt(inputData, addrPtr, 8)) & NIBBLE_LOW);							
            }
            i += 2;
            addrPtr++;
            noOfNibbles -= 2;
        }
        if (noOfNibbles == 1)
        {   //Odd nibble out.
            outputData[i] = (byte)((GetAt(inputData, addrPtr, 8) >> 4) & NIBBLE_HIGH);
        }
    }

    /// <summary>
    /// Decodes the RLE bitmap.
    /// </summary>
    /// <returns>The RLE bitmap.</returns>
    /// <param name="imageData">Image data.</param>
    /// <param name="datalen">Datalen.</param>
    /// <param name="imageWidth">Image width.</param>
    /// <param name="imageHeight">Image height.</param>
    /// <param name="bitSize">Bit size.</param>
    /// This code from underworld adventures
    // palette *auxpal, int index, int BitSize, char OutFileName[255])
    public static GraphicsFileImage DecodeRLEBitmap(GraphicsFileImage image, int bitSize, int[] auxpal)
    {
        byte[] outputImg = new byte[image.Width * image.Height];
        RecordState state = 0;
        int curr_pxl = 0;
        int count = 0;
        int newCount;
        int repeatcount = 0;
        byte nibble;

        int add_ptr = 0;

        while (curr_pxl < outputImg.Length || add_ptr <= image.Length)
        {
            switch (state)
            {
                case RecordState.RepeatStart:
                    count = GetCount(image.Data, ref add_ptr, bitSize);
                    switch (count)
                    {
                        case 1:
                            state = RecordState.Run;
                            break;
                        case 2:
                            repeatcount = GetCount(image.Data, ref add_ptr, bitSize) - 1;
                            state = RecordState.RepeatStart;
                            break;
                        default:
                            state = RecordState.Repeat;
                            break;
                    }
                    break;

                case RecordState.Repeat:
                    newCount = outputImg.Length - curr_pxl;
                    if (newCount < count)
                        count = newCount;
                    //for count times copy the palette data to the image at the output pointer
                    nibble = image.Data[add_ptr++];
                    for (int i = 0; i < count; i++)
                        outputImg[curr_pxl++] = (byte)auxpal[nibble];
                    if (repeatcount != 0)
                    {
                        state = RecordState.RepeatStart;
                        repeatcount--;
                        break;
                    }
                    state = RecordState.Run;
                    break;

                case RecordState.Run:
                    count = GetCount(image.Data, ref add_ptr, bitSize);
                    newCount = outputImg.Length - curr_pxl;
                    if (newCount < count)
                        count = newCount;
                    for (int i = 0; i < count; i++)
                    {
                        //get nibble for the palette;
                        nibble = image.Data[add_ptr++];
                        outputImg[curr_pxl++] = (byte)auxpal[nibble];
                    }
                    state = RecordState.RepeatStart;
                    break;
            }
        }
        return image with { Data = outputImg };
    }

    /// <summary>
    /// Getcount the specified nibbles, addr_ptr and size.
    /// </summary>
    /// <param name="nibbles">Nibbles.</param>
    /// <param name="addr_ptr">Address ptr.</param>
    /// <param name="size">Size.</param>
    /// This code from underworld adventures
    [Obsolete("Impure")]
    public static int GetCount(byte[] nibbles, ref int addr_ptr, int size)
    {
        (var count, addr_ptr) = GetCount(nibbles, addr_ptr, size);
        return count;
    }

    [Pure]
    public static (int, int) GetCount(byte[] nibbles, int addr_ptr, int size)
    {

        int n1 = nibbles[addr_ptr++]; // + 1
        int count = n1;
        if (count != 0)
            return (count, addr_ptr); // + 1

        n1 = nibbles[addr_ptr++]; // + 2
        int n2 = nibbles[addr_ptr++]; // +3
        count = (n1 << size) | n2;
        if (count != 0)
            return (count, addr_ptr); // +3

        n1 = nibbles[addr_ptr++]; // + 4
        n2 = nibbles[addr_ptr++]; // + 5
        int n3 = nibbles[addr_ptr++]; // + 6
        count = (((n1 << size) | n2) << size) | n3;
        return (count, addr_ptr); // + 6

    }

    [Pure]
    public static long ImageOffset(byte[] fileData, int index)
        => GetAt(fileData, (index * 4) + 3, 32);

    [Pure]
    public static uint GetImageType(byte[] fileData, long imageOffset)
        => GetAt(fileData, imageOffset, 8);

    [Pure]
    public static int BitMapWidth(byte[] fileData, long imageOffset)
        => (int)GetAt(fileData, imageOffset + 1, 8);

    [Pure]
    public static int BitMapHeight(byte[] fileData, long imageOffset)
        => (int)GetAt(fileData, imageOffset + 2, 8);

    [Pure]
    public static int GetAuxPalIndex(byte[] fileData, long imageOffset)
        => (int)GetAt(fileData, imageOffset + 3, 8);

    [Pure]
    public static int GetDataLength(byte[] fileData, long imageOffset)
        => (int)GetAt(fileData, imageOffset + 4, 16);

    public readonly record struct ImageConfig(
        bool AlphaChannel = true,
        bool RedChannel = false,
        bool Crop = false
    );
        
    public static ImageTexture LoadImageAt(
        int fileToLoad,
        byte[] imageFileData,
        int index,
        int paletteNo,
        string auxPalFile,
        int overrideAuxPalIndex,
        bool useOverrideAuxPalIndex,
        ImageConfig config)
    {

        // Make sure the image being loaded is within range.
        long imageOffset = ImageOffset(imageFileData, index);
        if (imageOffset >= imageFileData.GetUpperBound(0)) //Image out of range
            return null;

        // Get our starting Palette
        var palette = PaletteLoader.Palettes[paletteNo];

        //Check to see if the file is panels.gr
        var fileName = pathGR[fileToLoad];
        if (fileName.ToUpper().EndsWith("PANELS.GR"))
            return Image(
                databuffer: imageFileData,
                dataOffSet: imageOffset,
                width: _RES == GAME_UW2 ? 79 : 83,
                height: _RES == GAME_UW2 ? 112 : 114,
                palette: palette,
                useAlphaChannel: config.AlphaChannel,
                useSingleRedChannel: config.RedChannel,
                crop: config.Crop);

        // Determine our size and type.
        GraphicsFileImageType imageType = GetImageType(imageFileData, imageOffset);
        int bitMapWidth = BitMapWidth(imageFileData, imageOffset);
        int bitMapHeight = BitMapHeight(imageFileData, imageOffset);

        if (imageType == GraphicsFileImageType.RAW_4BIT)
            return Image(
                databuffer: imageFileData,
                dataOffSet: imageOffset + 5,
                width: bitMapWidth,
                height: bitMapHeight,
                palette: palette,
                useAlphaChannel: config.AlphaChannel,
                useSingleRedChannel: config.RedChannel,
                crop: config.Crop);

        // If it's not one of the other file types we recognise, stop and report.
        if (imageType != IMAGE_4BIT_RUNLENGTH && imageType != IMAGE_4BIT_UNCOMPRESSED)
        {
            GD.PushError($"Can't understand fileType:{imageType} in {fileName}");
            return null;
        }

        // Retrieve the raw data.
        int datalen = GetDataLength(imageFileData, imageOffset);
        byte[] imgNibbles = new byte[Mathf.Max(bitMapWidth * bitMapHeight * 2, (datalen + 5) * 2)];
        CopyNibbles(imageFileData, ref imgNibbles, datalen, imageOffset + 6);

        // Determine which palette we should start with.
        int auxPalIndex = useOverrideAuxPalIndex
            ? overrideAuxPalIndex
            : GetAuxPalIndex(imageFileData, imageOffset);
        string auxPalPath = Path.Combine(BasePath, "DATA", auxPalFile);

        if (imageType == IMAGE_4BIT_RUNLENGTH) // 4 bit run-length
        {
            int[] aux = PaletteLoader.LoadAuxilaryPalIndices(auxPalPath, auxPalIndex);
            imgNibbles = DecodeRLEBitmap(new GraphicsFileImage(imgNibbles, datalen, bitMapWidth, bitMapHeight), 4, aux).Data;
        }
        else // 0xA 4 bit uncompressed
        {
            palette = PaletteLoader.LoadAuxilaryPal(auxPalPath, palette, auxPalIndex);
        }

        return Image(
            databuffer: imgNibbles,
            dataOffSet: 0,
            width: bitMapWidth,
            height: bitMapHeight,
            palette: palette,
            useAlphaChannel: config.AlphaChannel,
            useSingleRedChannel: config.RedChannel,
            crop: config.Crop);

    }

}