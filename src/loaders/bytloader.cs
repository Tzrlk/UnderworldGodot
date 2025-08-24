using System.IO;
using Godot;

namespace Underworld
{

    public class BytLoader : ArtLoader
    {
        public const int BLNKMAP_BYT = 0;
        public const int CHARGEN_BYT = 1;
        public const int CONV_BYT = 2;
        public const int MAIN_BYT = 3;
        public const int OPSCR_BYT = 4;
        public const int PRES1_BYT = 5;
        public const int PRES2_BYT = 6;
        public const int WIN1_BYT = 7;
        public const int WIN2_BYT = 8;
        public const int PRESD_BYT = 9;

        //UW2 bitmap indices
        public const int UW2MAIN_BYT = 5;
        public const int UW2ThreeDWin_BYT = 4;

        private readonly int currentIndex = -1;

        private readonly string[] FilePaths ={
                "BLNKMAP.BYT",
                "CHARGEN.BYT",
                "CONV.BYT",
                "MAIN.BYT",
                "OPSCR.BYT",
                "PRES1.BYT",
                "PRES2.BYT",
                "WIN1.BYT",
                "WIN2.BYT",
                "PRESD.BYT"
            };

        private readonly int[] PaletteIndices =
        {
                3,
                9,
                0,
                0,
                6,
                15,
                15,
                21,
                21,
                0
            };


        private readonly int[] PaletteIndicesUW2 =
        {
                3,
                0,
                0,
                0,
                0,
                0,
                15,
                15,
                0,
                0,
                0
            };

        public Shader textureshader;

        public bool UseRedChannel;
        

         public ShaderMaterial[] materials = new ShaderMaterial[10];


        public ShaderMaterial GetMaterial(int textureno)
        {            
            if (textureshader==null)
            {
                textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uisprite.gdshader");
            }
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



        /// <summary>
        /// Loads the texture form a byt file
        /// </summary>
        /// <returns>The <see cref="UnityEngine.Texture2D"/>.</returns>
        /// <param name="index">Index.</param>
        /// In this case the index is a loading of the seperate file. 
        public override ImageTexture LoadImageAt(int index)
        {
            return LoadImageAt(index, false);
        }

        public override ImageTexture LoadImageAt(int index, bool UseAlphaChannel)
        {
            switch (_RES)
            {
                case GAME_UW2:
                    {
                        return extractUW2Bitmap(Path.Combine(BasePath, "DATA", "BYT.ARK"), index, UseAlphaChannel);      //    "DATA" + sep + "BYT.ARK", index, Alpha);
                    }
                default:
                    {
                        var toLoad = Path.Combine(BasePath, "DATA", FilePaths[index]);
                        if (currentIndex != index)
                        {//Only load from disk if the image to bring back has changed.
                            DataLoaded = false;
                            filePath = toLoad;   //FilePaths[index];
                            LoadImageFile();
                        }
                        return Image(
                            databuffer: ImageFileData, 
                            dataOffSet: 0, 
                            width: 320, 
                            height: 200, 
                            palette: PaletteLoader.Palettes[PaletteIndices[index]], 
                            useAlphaChannel: UseAlphaChannel, 
                            useSingleRedChannel: UseRedChannel,
                            crop: UseCropping);
                    }
            }
        }


    public ImageTexture extractUW2Bitmap(string toLoad, int index, bool UseAlphaChannel)
    {
        long NoOfTextures;

        if (!ReadStreamFile(toLoad, out byte[] textureFile))
        { return null; }

        NoOfTextures = GetAt(textureFile, 0, 8);
        int textureOffset = (int)GetAt(textureFile, (index * 4) + 6, 32);
        if (textureOffset != 0)
        {
            int compressionFlag = (int)GetAt(textureFile, ((index * 4) + 6) + (NoOfTextures * 4), 32);
            int isCompressed = (compressionFlag >> 1) & 0x01;
            if (isCompressed == 1)
            {
                int datalen = 0;
                return Image(
                    databuffer: DataLoader.unpackUW2(tmpBuffer: textureFile, address_pointer: textureOffset, datalen: ref datalen), 
                    dataOffSet: 0, 
                    width: 320, 
                    height: 200, 
                    palette: PaletteLoader.Palettes[PaletteIndicesUW2[index]], 
                    useAlphaChannel: UseAlphaChannel, 
                    useSingleRedChannel: UseRedChannel,
                    crop: UseCropping);
            }
            else
            {
                return Image(
                    databuffer: textureFile, 
                    dataOffSet: textureOffset, 
                    width: 320, 
                    height: 200, 
                    palette: PaletteLoader.Palettes[PaletteIndicesUW2[index]], 
                    useAlphaChannel: UseAlphaChannel, 
                    useSingleRedChannel: UseRedChannel,
                    crop: UseCropping);
            }
        }
        return null;
    }


    } //end class

}//end namespace