using Godot;
using System.IO;
using System.Linq;

namespace Underworld;

/// <summary>
/// Loads data from various GR Files.
/// One instance per file type
/// </summary>
public class GRLoader : ArtLoader
{
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

    readonly string AuxPalPath = "ALLPALS.DAT";
    readonly bool useOverrideAuxPalIndex = false;
    readonly int OverrideAuxPalIndex = 0;

    public int FileToLoad;
    private bool ImageFileDataLoaded;
    public int NoOfImages;

    public Shader textureshader;
    public ImageTexture[] ImageCache = new ImageTexture[1];

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

    public GRLoader(int File, GRShaderMode shaderMode)
    {
        textureshader = GetShader(shaderMode);
        // AuxPalPath = AuxPalPath.Replace("--", sep.ToString());
        useOverrideAuxPalIndex = false;
        OverrideAuxPalIndex = 0;
        FileToLoad = File;
        PaletteNo = 0;
        LoadImageFile();
    }

    public ShaderMaterial GetMaterial(int textureno)
        => materials[textureno] ??= GetMaterial(LoadImageAt(textureno, true), textureshader);

    public override bool LoadImageFile()
    {
        var fileToLoad = Path.Combine(BasePath, "DATA", pathGR[FileToLoad]);
        NoOfImages = LoadImageFile(fileToLoad, out ImageFileData);
        if (NoOfImages < 1) {
            GD.PushError("Unable to load GR file from ", fileToLoad);
            return false;
        }

        ImageCache = new ImageTexture[NoOfImages];
        materials = new ShaderMaterial[NoOfImages];
        ImageFileDataLoaded = true;
        return true;

    }

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
            UseAlphaChannel,
            UseRedChannel,
            UseCropping);
    }

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
    public static int LoadImageFile(string path, out byte[] data)
        => ReadStreamFile(path, out data)
            ? (int)getAt(data, 1, 16)
            : -1;

    /// <summary>
    /// Copies the nibbles.
    /// </summary>
    /// <param name="InputData">Input data.</param>
    /// <param name="OutputData">Output data.</param>
    /// <param name="NoOfNibbles">No of nibbles.</param>
    /// <param name="add_ptr">Add ptr.</param>
    /// This code from underworld adventures
    public static void CopyNibbles(byte[] InputData, ref byte[] OutputData, int NoOfNibbles, long add_ptr)
    {
        //Split the data up into it's nibbles.
        int i = 0;
        NoOfNibbles *= 2;
        while (NoOfNibbles > 1)
        {
            if (add_ptr <= InputData.GetUpperBound(0))
            {
                OutputData[i] = (byte)((getAt(InputData, add_ptr, 8) >> 4) & 0x0F);        //High nibble
                OutputData[i + 1] = (byte)((getAt(InputData, add_ptr, 8)) & 0xf);  //Low nibble							
            }
            i += 2;
            add_ptr++;
            NoOfNibbles -= 2;
        }
        if (NoOfNibbles == 1)
        {   //Odd nibble out.
            OutputData[i] = (byte)((getAt(InputData, add_ptr, 8) >> 4) & 0x0F);
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
    /// <param name="BitSize">Bit size.</param>
    /// This code from underworld adventures
    public static byte[] DecodeRLEBitmap(byte[] imageData, int datalen, int imageWidth, int imageHeight, int BitSize, int[] auxpal)
    //, palette *auxpal, int index, int BitSize, char OutFileName[255])
    {
        byte[] outputImg = new byte[imageWidth * imageHeight];
        RecordState state = 0;
        int curr_pxl = 0;
        int count = 0;
        int repeatcount = 0;
        byte nibble;

        int add_ptr = 0;

        while ((curr_pxl < imageWidth * imageHeight) || (add_ptr <= datalen))
        {
            switch (state)
            {
                case RecordState.RepeatStart:
                    count = GetCount(imageData, ref add_ptr, BitSize);
                    switch (count)
                    {
                        case 1:
                            state = RecordState.Run;
                            break;
                        case 2:
                            repeatcount = GetCount(imageData, ref add_ptr, BitSize) - 1;
                            state = RecordState.RepeatStart;
                            break;
                        default:
                            state = RecordState.Repeat;
                            break;
                    }
                    break;

                case RecordState.Repeat:
                    if (imageWidth * imageHeight - curr_pxl < count)
                        count = imageWidth * imageHeight - curr_pxl;
                    //for count times copy the palette data to the image at the output pointer
                    nibble = GetNibble(imageData, ref add_ptr);
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
                    count = GetCount(imageData, ref add_ptr, BitSize);
                    if (imageWidth * imageHeight - curr_pxl < count)
                        count = imageWidth * imageHeight - curr_pxl;
                    for (int i = 0; i < count; i++)
                    {
                        //get nibble for the palette;
                        nibble = GetNibble(imageData, ref add_ptr);
                        outputImg[curr_pxl++] = (byte)auxpal[nibble];
                    }
                    state = RecordState.RepeatStart;
                    break;
            }
        }
        return outputImg;
    }

    /// <summary>
    /// Getcount the specified nibbles, addr_ptr and size.
    /// </summary>
    /// <param name="nibbles">Nibbles.</param>
    /// <param name="addr_ptr">Address ptr.</param>
    /// <param name="size">Size.</param>
    /// This code from underworld adventures
    public static int GetCount(byte[] nibbles, ref int addr_ptr, int size)
    {

        int n1 = GetNibble(nibbles, ref addr_ptr);
        int count = n1;
        if (count != 0)
            return count;

        n1 = GetNibble(nibbles, ref addr_ptr);
        int n2 = GetNibble(nibbles, ref addr_ptr);
        count = (n1 << size) | n2;
        if (count != 0)
            return count;

        n1 = GetNibble(nibbles, ref addr_ptr);
        n2 = GetNibble(nibbles, ref addr_ptr);
        int n3 = GetNibble(nibbles, ref addr_ptr);
        return (((n1 << size) | n2) << size) | n3;

    }

    /// <summary>
    /// Gets the nibble, incrementing to the next.
    /// </summary>
    /// <returns>The nibble.</returns>
    /// <param name="nibbles">Nibbles.</param>
    /// <param name="addr_ptr">Address ptr.</param>
    /// This code from underworld adventures
    public static byte GetNibble(byte[] nibbles, ref int addr_ptr)
        => nibbles[addr_ptr++];
        
    public static ImageTexture LoadImageAt(
        int FileToLoad,
        byte[] ImageFileData,
        int index,
        int PaletteNo,
        string AuxPalPath,
        int OverrideAuxPalIndex,
        bool useOverrideAuxPalIndex,
        bool UseAlphaChannel = true,
        bool UseRedChannel = false,
        bool UseCropping = false)
    {

        // Make sure the image being loaded is within range.
        long imageOffset = getAt(ImageFileData, (index * 4) + 3, 32);
        if (imageOffset >= ImageFileData.GetUpperBound(0)) //Image out of range
            return null;

        // Get our starting Palette
        var palette = PaletteLoader.Palettes[PaletteNo];

        //Check to see if the file is panels.gr
        var fileName = pathGR[FileToLoad];
        if (fileName.ToUpper().EndsWith("PANELS.GR"))
            return Image(
                databuffer: ImageFileData,
                dataOffSet: imageOffset,
                width: _RES == GAME_UW2 ? 79 : 83,
                height: _RES == GAME_UW2 ? 112 : 114,
                palette: palette,
                useAlphaChannel: UseAlphaChannel,
                useSingleRedChannel: UseRedChannel,
                crop: UseCropping);

        // Determine our size and type.
        int BitMapWidth = (int)getAt(ImageFileData, imageOffset + 1, 8);
        int BitMapHeight = (int)getAt(ImageFileData, imageOffset + 2, 8);
        uint fileType = getAt(ImageFileData, imageOffset, 8);

        // 8 bit uncompressed
        if (fileType == 0x4)
            return Image(
                databuffer: ImageFileData,
                dataOffSet: imageOffset + 5,
                width: BitMapWidth,
                height: BitMapHeight,
                palette: palette,
                useAlphaChannel: UseAlphaChannel,
                useSingleRedChannel: UseRedChannel,
                crop: UseCropping);

        // If it's not one of the other file types we recognise, stop and report.
        if (fileType != 0x8 && fileType != 0xA)
        {
            GD.PushError($"Can't understand fileType:{fileType} in {fileName}");
            return null;
        }

        // Retrieve the raw data.
        int datalen = (int)getAt(ImageFileData, imageOffset + 4, 16);
        byte[] imgNibbles = new byte[Mathf.Max(BitMapWidth * BitMapHeight * 2, (datalen + 5) * 2)];
        CopyNibbles(ImageFileData, ref imgNibbles, datalen, imageOffset + 6);

        // Determine which palette we should start with.
        int auxPalIndex = useOverrideAuxPalIndex
            ? OverrideAuxPalIndex
            : (int)getAt(ImageFileData, imageOffset + 3, 8);
        string auxPalPath = Path.Combine(BasePath, "DATA", AuxPalPath);

        if (fileType == 0x8) // 4 bit run-length
        {
            int[] aux = PaletteLoader.LoadAuxilaryPalIndices(auxPalPath, auxPalIndex);
            imgNibbles = DecodeRLEBitmap(imgNibbles, datalen, BitMapWidth, BitMapHeight, 4, aux);
        }
        else // 0xA 4 bit uncompressed
        {
            palette = PaletteLoader.LoadAuxilaryPal(auxPalPath, palette, auxPalIndex);
        }

        return Image(
            databuffer: imgNibbles,
            dataOffSet: 0,
            width: BitMapWidth,
            height: BitMapHeight,
            palette: palette,
            useAlphaChannel: UseAlphaChannel,
            useSingleRedChannel: UseRedChannel,
            crop: UseCropping);

    }

}