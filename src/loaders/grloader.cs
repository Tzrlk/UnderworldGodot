using Godot;
using System.IO;
using System.Diagnostics;

namespace Underworld
{

/// <summary>
/// Loads data from various GR Files.
/// One instance per file type
/// </summary>
    public class GRLoader : ArtLoader
    {
        public bool UseRedChannel = false;
        const int repeat_record_start = 0;
        const int repeat_record = 1;
        const int run_record = 2;

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

        private readonly string[] pathGR ={
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
                    "GHED.GR"
            };

        private readonly string AuxPalPath = "ALLPALS.DAT";
        readonly bool useOverrideAuxPalIndex = false;
        readonly int OverrideAuxPalIndex = 0;

        public int FileToLoad;
        private bool ImageFileDataLoaded;
        int NoOfImages;

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
        public enum GRShaderMode
        {
            None = 0, 
            SpriteShader = 1,  //Spritesthat will not be billboarded.
            BillboardSpriteShader = 2, //Sprites that will be billboarded
            TextureShader= 3,  //World textures
            UIShader = 4  //For ui elements that need palette cycling

        };

        public GRLoader(int File, GRShaderMode shadermode)
        {      
            switch (shadermode)
            {
                case GRShaderMode.None:
                    textureshader = null;
                    break;
                case GRShaderMode.TextureShader:
                case GRShaderMode.SpriteShader:
                    textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uwshader.gdshader");
                    break;
                case GRShaderMode.BillboardSpriteShader:
                    textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uwsprite.gdshader");
                    break;  
                case GRShaderMode.UIShader:
                    textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uisprite.gdshader");
                    break;
            }     
            // AuxPalPath = AuxPalPath.Replace("--", sep.ToString());
            useOverrideAuxPalIndex = false;
            OverrideAuxPalIndex = 0;
            FileToLoad = File;
            PaletteNo = 0;
            LoadImageFile();
        }       

        public ShaderMaterial GetMaterial(int textureno)
        {            
            if (materials[textureno] == null)
            {
                //materials[textureno] = new surfacematerial(textureno);
                //create this material and add it to the list
                var newmaterial = new ShaderMaterial();
                newmaterial.Shader = textureshader;
                newmaterial.SetShaderParameter("texture_albedo", (Texture)LoadImageAt(textureno,true));
                newmaterial.SetShaderParameter("albedo", new Color(1, 1, 1, 1));
                newmaterial.SetShaderParameter("uv1_scale", new Vector3(1, 1, 1));
                newmaterial.SetShaderParameter("uv2_scale", new Vector3(1, 1, 1));
                newmaterial.SetShaderParameter("UseAlpha", true);
                materials[textureno] = newmaterial;
            }
            return materials[textureno];    
        }

        public override bool LoadImageFile()
        {
            var toLoad = Path.Combine(BasePath, "DATA", pathGR[FileToLoad]);
            return LoadImageFile(toLoad);
        }

        public bool LoadImageFile(string toLoad)
        {
            if (!ReadStreamFile(toLoad, out ImageFileData))
            {
                Debug.Print("Unable to LoadImageFile() " + toLoad);
                return false;
            }
            else
            {
                NoOfImages = (int)getAt(ImageFileData, 1, 16);
                ImageCache = new ImageTexture[NoOfImages];
                materials = new ShaderMaterial[NoOfImages];
                ImageFileDataLoaded = true;
                return true;
            }
        }

        public override ImageTexture LoadImageAt(int index)
        {
            return LoadImageAt(index, true);
        }

        public override ImageTexture LoadImageAt(int index, bool UseAlphaChannel)
        {
            if (ImageFileDataLoaded == false)
            {
                if (!LoadImageFile())
                {
                    return base.LoadImageAt(index);
                }
            }
            else
            {
                if (ImageCache[index] != null)
                {
                    return ImageCache[index];
                }
            }


            long imageOffset = getAt(ImageFileData, (index * 4) + 3, 32);
            if (imageOffset >= ImageFileData.GetUpperBound(0))
            {//Image out of range
                return base.LoadImageAt(index);
            }
            int BitMapWidth = (int)getAt(ImageFileData, imageOffset + 1, 8);
            int BitMapHeight = (int)getAt(ImageFileData, imageOffset + 2, 8);
            int datalen;
            Palette auxpal;
            int auxPalIndex;
            byte[] imgNibbles;
            byte[] outputImg;


            switch (getAt(ImageFileData, imageOffset, 8))//File type
            {
                case 0x4://8 bit uncompressed
                    {
                        imageOffset += 5;
                        ImageCache[index] = Image(
                            databuffer: ImageFileData, 
                            dataOffSet: imageOffset, 
                            width: BitMapWidth, height: BitMapHeight, 
                            palette: PaletteLoader.Palettes[PaletteNo], 
                            useAlphaChannel: UseAlphaChannel, 
                            useSingleRedChannel: UseRedChannel,
                            crop: UseCropping);
                        return ImageCache[index];
                    }
                case 0x8://4 bit run-length
                    {
                        if (!useOverrideAuxPalIndex)
                        {
                            auxPalIndex = (int)getAt(ImageFileData, imageOffset + 3, 8);
                        }
                        else
                        {
                            auxPalIndex = OverrideAuxPalIndex;
                        }
                        datalen = (int)getAt(ImageFileData, imageOffset + 4, 16);
                        imgNibbles = new byte[Mathf.Max(BitMapWidth * BitMapHeight * 2, (datalen + 5) * 2)];
                        imageOffset += 6;  //Start of raw data.
                        copyNibbles(ImageFileData, ref imgNibbles, datalen, imageOffset);
                        //auxpal =PaletteLoader.LoadAuxilaryPal(Loader.BasePath+ AuxPalPath,PaletteLoader.Palettes[PaletteNo],auxPalIndex);
                        int[] aux = PaletteLoader.LoadAuxilaryPalIndices(Path.Combine(BasePath, "DATA", AuxPalPath), auxPalIndex);
                        outputImg = DecodeRLEBitmap(imgNibbles, datalen, BitMapWidth, BitMapHeight, 4, aux);
                        ImageCache[index] = Image(
                            databuffer: outputImg, 
                            dataOffSet: 0, 
                            width: BitMapWidth, height: BitMapHeight, 
                            palette: PaletteLoader.Palettes[PaletteNo], 
                            useAlphaChannel: UseAlphaChannel, 
                            useSingleRedChannel: UseRedChannel, 
                            crop: UseCropping);
                        return ImageCache[index];
                    }
                case 0xA://4 bit uncompressed//Same as above???
                    {
                        if (!useOverrideAuxPalIndex)
                        {
                            auxPalIndex = (int)getAt(ImageFileData, imageOffset + 3, 8);
                        }
                        else
                        {
                            auxPalIndex = OverrideAuxPalIndex;
                        }
                        datalen = (int)getAt(ImageFileData, imageOffset + 4, 16);
                        imgNibbles = new byte[Mathf.Max(BitMapWidth * BitMapHeight * 2, (5 + datalen) * 2)];
                        imageOffset += 6;  //Start of raw data.
                        copyNibbles(ImageFileData, ref imgNibbles, datalen, imageOffset);
                        auxpal = PaletteLoader.LoadAuxilaryPal(Path.Combine(BasePath, "DATA", AuxPalPath), PaletteLoader.Palettes[PaletteNo], auxPalIndex);
                        ImageCache[index] = Image(
                            databuffer: imgNibbles, 
                            dataOffSet: 0, 
                            width: BitMapWidth, height: BitMapHeight, 
                            palette: auxpal, 
                            useAlphaChannel: UseAlphaChannel , 
                            useSingleRedChannel: UseRedChannel,
                            crop: UseCropping);
                        return ImageCache[index];
                    }
                //break;
                default:
                    //Check to see if the file is panels.gr
                    if (pathGR[FileToLoad].ToUpper().EndsWith("PANELS.GR"))
                    {
                        BitMapWidth = 83;  //getValAtAddress(textureFile, textureOffset + 1, 8);
                        BitMapHeight = 114; // getValAtAddress(textureFile, textureOffset + 2, 8);
                        if (_RES == GAME_UW2)
                        {
                            BitMapWidth = 79;
                            BitMapHeight = 112;
                        }
                        imageOffset = getAt(ImageFileData, (index * 4) + 3, 32);
                        ImageCache[index] = Image(
                            databuffer: ImageFileData, 
                            dataOffSet: imageOffset, 
                            width: BitMapWidth, height: BitMapHeight, 
                            palette: PaletteLoader.Palettes[PaletteNo], 
                            useAlphaChannel: UseAlphaChannel, 
                            useSingleRedChannel: UseRedChannel,
                            crop: UseCropping);
                        return ImageCache[index];
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Copies the nibbles.
        /// </summary>
        /// <param name="InputData">Input data.</param>
        /// <param name="OutputData">Output data.</param>
        /// <param name="NoOfNibbles">No of nibbles.</param>
        /// <param name="add_ptr">Add ptr.</param>
        /// This code from underworld adventures
        protected void copyNibbles(byte[] InputData, ref byte[] OutputData, int NoOfNibbles, long add_ptr)
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
        byte[] DecodeRLEBitmap(byte[] imageData, int datalen, int imageWidth, int imageHeight, int BitSize, int[] auxpal)
        //, palette *auxpal, int index, int BitSize, char OutFileName[255])
        {
            byte[] outputImg = new byte[imageWidth * imageHeight];
            int state = 0;
            int curr_pxl = 0;
            int count = 0;
            int repeatcount = 0;
            byte nibble;

            int add_ptr = 0;

            while ((curr_pxl < imageWidth * imageHeight) || (add_ptr <= datalen))
            {
                switch (state)
                {
                    case repeat_record_start:
                        {
                            count = getcount(imageData, ref add_ptr, BitSize);
                            if (count == 1)
                            {
                                state = run_record;
                            }
                            else if (count == 2)
                            {
                                repeatcount = getcount(imageData, ref add_ptr, BitSize) - 1;
                                state = repeat_record_start;
                            }
                            else
                            {
                                state = repeat_record;
                            }
                            break;
                        }
                    case repeat_record:
                        {
                            nibble = GetNibble(imageData, ref add_ptr);
                            //for count times copy the palette data to the image at the output pointer
                            if (imageWidth * imageHeight - curr_pxl < count)
                            {
                                count = imageWidth * imageHeight - curr_pxl;
                            }
                            for (int i = 0; i < count; i++)
                            {
                                outputImg[curr_pxl++] = (byte)auxpal[nibble];
                            }
                            if (repeatcount == 0)
                            {
                                state = run_record;
                            }
                            else
                            {
                                state = repeat_record_start;
                                repeatcount--;
                            }
                            break;
                        }


                    case 2: //runrecord
                        {
                            count = getcount(imageData, ref add_ptr, BitSize);
                            if (imageWidth * imageHeight - curr_pxl < count)
                            {
                                count = imageWidth * imageHeight - curr_pxl;
                            }
                            for (int i = 0; i < count; i++)
                            {
                                //get nibble for the palette;
                                nibble = GetNibble(imageData, ref add_ptr);
                                outputImg[curr_pxl++] = (byte)auxpal[nibble];
                            }
                            state = repeat_record_start;
                            break;
                        }
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
        int getcount(byte[] nibbles, ref int addr_ptr, int size)
        {
            int n1;
            int n2;
            int n3;
            n1 = GetNibble(nibbles, ref addr_ptr);
            int count = n1;
            if (count == 0)
            {
                n1 = GetNibble(nibbles, ref addr_ptr);
                n2 = GetNibble(nibbles, ref addr_ptr);
                count = (n1 << size) | n2;
            }
            if (count == 0)
            {
                n1 = GetNibble(nibbles, ref addr_ptr);
                n2 = GetNibble(nibbles, ref addr_ptr);
                n3 = GetNibble(nibbles, ref addr_ptr);
                count = (((n1 << size) | n2) << size) | n3;
            }
            return count;
        }

        /// <summary>
        /// Gets the nibble.
        /// </summary>
        /// <returns>The nibble.</returns>
        /// <param name="nibbles">Nibbles.</param>
        /// <param name="addr_ptr">Address ptr.</param>
        /// This code from underworld adventures
        byte GetNibble(byte[] nibbles, ref int addr_ptr)
        {
            byte n1 = nibbles[addr_ptr];
            addr_ptr++;
            return n1;
        }   

        public void ExportImages(string exportpath)
        {
            for (int i =0; i<NoOfImages;i++)
            {
                var img = LoadImageAt(i);
                img.GetImage().SavePng(Path.Combine(exportpath,$"{i.ToString("000")}.png"));
            }
        }
    }//end class
}//end namespaces