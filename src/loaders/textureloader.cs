using System.IO;
using Godot;

namespace Underworld
{
    /// <summary>
    /// Loads textures.
    /// </summary>
    public class TextureLoader : ArtLoader
    {

        /// <summary>
        /// Force all textures to be loaded in greyscale so that they actual colors are applied later in shaders.
        /// </summary>
        static bool UseRedChannel = true;

        private readonly string pathTexW_UW0 = "DW64.TR";
        private readonly string pathTexF_UW0 = "DF32.TR";
        private string pathTexW_UW1 = "W64.TR";
        private string pathTexF_UW1 = "F32.TR";
        private readonly string pathTex_UW2 = "T64.TR";

        byte[] texturebufferW;
        byte[] texturebufferF;
        byte[] texturebufferT;

        public bool texturesWLoaded;
        public bool texturesFLoaded;
        private int TextureSplit = 210;//at what point does a texture index refer to the floor instead of a wall in uw1/demo
        private int FloorDim = 32;
        public Shader textureshader;
        
        public ShaderMaterial[] materials = new ShaderMaterial[512];
        public TextureLoader()
        {  
            textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uwshader.gdshader");            
        }        


        public override ImageTexture LoadImageAt(int index)
        {
            return LoadImageAt(index, PaletteLoader.GreyScaleIndexPalette); //PaletteLoader.Palettes[0]);
        }

        /// <summary>
        /// Loads the image at index.
        /// </summary>
        /// <returns>The texture</returns>
        /// <param name="index">Index.</param>
        /// <param name="palToUse">Pal to use.</param>
        /// If the index is greater than 209 I return a floor texture.
        private ImageTexture LoadImageAt(int index, Palette palToUse)
        {
            if (GameConfig.GameSelected == Game.Uw0)
            {//Point the UW1 texture files to the demo files
                TextureSplit = 48;
                pathTexW_UW1 = pathTexW_UW0;
                pathTexF_UW1 = pathTexF_UW0;
            }
            if (GameConfig.GameSelected == Game.Uw2)
            {
                FloorDim = 64;
            }

            switch (GameConfig.GameSelected)
            {
                case Game.Uw2:
                    {
                        if (texturesFLoaded == false)
                        {
                            if (!ReadStreamFile(Path.Combine(GameConfig.GamePath, "DATA", pathTex_UW2), out texturebufferT))
                            {
                                return base.LoadImageAt(index);
                            }
                            else
                            {
                                texturesFLoaded = true;
                            }
                        }
                        long textureOffset = getAt(texturebufferT, ((index) * 4) + 4, 32);
                        return Image(
                            databuffer: texturebufferT, 
                            dataOffSet: textureOffset, 
                            width: FloorDim, height: FloorDim, 
                            palette: palToUse, 
                            useAlphaChannel: false, 
                            useSingleRedChannel: UseRedChannel,
                            crop: UseCropping);
                    }


                case Game.Uw0:
                case Game.Uw1:
                default:
                    {
                        if (index < TextureSplit)
                        {//Wall textures
                            if (texturesWLoaded == false)
                            {
                                if (!ReadStreamFile(Path.Combine(GameConfig.GamePath, "DATA", pathTexW_UW1), out texturebufferW))
                                {
                                    return base.LoadImageAt(index);
                                }
                                else
                                {
                                    texturesWLoaded = true;
                                }
                            }
                            long textureOffset = getAt(texturebufferW, (index * 4) + 4, 32);
                            return Image(
                                databuffer: texturebufferW, 
                                dataOffSet: textureOffset, 
                                width: 64, height: 64, 
                                palette: palToUse, 
                                useAlphaChannel: false, 
                                useSingleRedChannel: UseRedChannel,
                                crop: UseCropping);
                        }
                        else
                        {//Floor textures (to match my list of textures)
                            if (texturesFLoaded == false)
                            {
                                if (!ReadStreamFile(Path.Combine(GameConfig.GamePath, "DATA", pathTexF_UW1), out texturebufferF))
                                {
                                    return base.LoadImageAt(index);
                                }
                                else
                                {
                                    texturesFLoaded = true;
                                }
                            }
                            long textureOffset = getAt(texturebufferF, ((index - TextureSplit) * 4) + 4, 32);
                            return Image(
                                databuffer: texturebufferF, 
                                dataOffSet: textureOffset, 
                                width: FloorDim, height: FloorDim, 
                                palette: palToUse, 
                                useAlphaChannel: false, 
                                useSingleRedChannel: UseRedChannel,
                                crop: UseCropping);
                        }
                    }//end switch	
            }
        }


        public ShaderMaterial GetMaterial(int textureno, short[] texturemap)
        {
            if (materials[textureno] == null)
            {
                //create this material and add it to the list
                var newmaterial = new ShaderMaterial();
                newmaterial.Shader = textureshader;                
                newmaterial.SetShaderParameter("texture_albedo", (Texture)LoadImageAt(texturemap[textureno]));
                newmaterial.SetShaderParameter("albedo", new Color(1, 1, 1, 1));
                newmaterial.SetShaderParameter("uv1_scale", new Vector3(1, 1, 1));
                newmaterial.SetShaderParameter("uv2_scale", new Vector3(1, 1, 1));
                newmaterial.SetShaderParameter("UseAlpha", false);
                materials[textureno] = newmaterial;
            }
            return materials[textureno];   
        }
    }//end class
}//end namespace