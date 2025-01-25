using Godot;
using System.IO;

namespace Underworld
{
    public class CutsLoader : ArtLoader
    {
        private bool _alpha;//does thie file use an alpha channel

        public ImageTexture[] ImageCache = new ImageTexture[1];
        byte[] dstImage; //repeating buffer

        public ShaderMaterial[] materials = new ShaderMaterial[1];
        public Shader textureshader;


        struct lpHeader
        {
            public int NoOfPages;
            public int NoOfRecords;
            public int width;
            public int height;
            public int nFrames;
        };

        struct lp_descriptor
        {
            public int baseRecord;  /* Number of first record in this large page. */
            public int nRecords;  /* Number of records in lp.
                        bit 15 of "nRecords" == "has continuation from previous lp".
                        bit 14 of "nRecords" == "final record continues on next lp". */
            public int nBytes;    /* Total number of bytes of contents, excluding header. */
        };


        public CutsLoader(string File)
        {
            filePath = Path.Combine(BasePath, "CUTS", File.ToUpper());
            _alpha = UseAlpha(File);
            if (LoadImageFile())
            {
                var filename = Path.GetFileName(File);
                ReadCutsFile(
                    cutsFile: ref ImageFileData, 
                    Alpha: _alpha, 
                    ErrorHandling: UseErrorHandling(filename), 
                    file: File);
            }
            textureshader = (Shader)ResourceLoader.Load("res://resources/shaders/uisprite.gdshader");          
        }

        static bool UseAlpha(string File)
        {
            switch (File.ToLower())
            {
                //case "cs400.n01"://  Look graphics for volcano
                case "cs401.n01"://   grave stones
                case "cs402.n01"://   death skulls w / silver sapling
                case "cs403.n01"://   death skulls animation
                case "cs403.n02"://   death skull end anim
                case "cs404.n01"://   anvil graphics
                case "cs410.n01"://   map piece showing some traps
                    return true;
                default:
                    return false;
            }
        }

        bool UseErrorHandling(string File)
        {//Special case for bugged file
            switch (File.ToLower())
            {
                case "cs000.n23":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Reads the cuts file.
        /// </summary>
        /// <param name="cutsFile">Cuts file.</param>
        public void ReadCutsFile(ref byte[] cutsFile, bool Alpha, bool ErrorHandling, string file)
        {
            long addptr = 0;
            int imagecount = 0;
            Palette pal = new Palette();
            //Read in lp header. Size is 128
            /*
        The file starts with a "large page file header":

    0000   Int32   file ID, always contains "LPF "
    ...
    0006   Int16   number of large pages in the file
    0008   Int32   number of records in the file
    ...
    0010   Int32   content type, always contains "ANIM"
    0014   Int16   width in pixels
    0016   Int16   height in pixels

    The whole header is 128 bytes long. After the header color cycling info
    follows (which also is 128 bytes long), which is not used in uw1. Then
    comes the color palette:*/
            lpHeader lpH;
            lpH.NoOfPages = (int)getAt(cutsFile, 0x6, 16);
            lpH.NoOfRecords = (int)getAt(cutsFile, 0x8, 32);
            lpH.width = (int)getAt(cutsFile, 0x14, 16);
            lpH.height = (int)getAt(cutsFile, 0x16, 16);
            lpH.nFrames = (int)getAt(cutsFile, 0x40, 16);
            addptr += 128;//past header.
            addptr += 128;//colour cycling info.

            //Init the buffer
            dstImage = new byte[lpH.height * lpH.width]; //+ 4000];

            //Read in the palette
            for (int i = 0; i < 256; i++)
            {
                pal.blue[i] = (byte)getAt(cutsFile, addptr++, 8);
                pal.green[i] = (byte)getAt(cutsFile, addptr++, 8);
                pal.red[i] = (byte)getAt(cutsFile, addptr++, 8);
                addptr++;//skip reserved.
                        //pal.reserved = fgetc(fd);
            }

            //Read in 256 lp descriptors
            lp_descriptor[] lpd = new lp_descriptor[256];
            for (int i = 0; i < lpd.GetUpperBound(0); i++)
            {
                lpd[i].baseRecord = (int)getAt(cutsFile, addptr, 16);
                lpd[i].nRecords = (int)getAt(cutsFile, addptr + 2, 16);
                lpd[i].nBytes = (int)getAt(cutsFile, addptr + 4, 16);
                addptr += 6;
            }
            byte[] pages = new byte[cutsFile.GetUpperBound(0) - 2816 + 1];
            for (int i = 0; i <= pages.GetUpperBound(0); i++)
            {
                pages[i] = cutsFile[i + 2816];
            }
            ImageCache = new ImageTexture[lpH.nFrames];
            materials = new ShaderMaterial[lpH.nFrames];
            for (int framenumber = 0; framenumber < lpH.nFrames; framenumber++)
            {
                if ((ErrorHandling == true) && (framenumber == 10))
                {//Special case crashes on a particular cutscene. (doors closing on avatar)
                    return;
                }

                int i = 0;
                for (; i < lpH.NoOfPages; i++)
                    if ((lpd[i].baseRecord <= framenumber) && (lpd[i].baseRecord + lpd[i].nRecords > framenumber))
                        break;
                addptr = (0x10000 * i);
                long curlp = addptr;
                //long page= addptr;
                lp_descriptor curl;
                curl.baseRecord = (int)getAt(pages, curlp + 0, 16);
                curl.nRecords = (int)getAt(pages, curlp + 2, 16);
                curl.nBytes = (int)getAt(pages, curlp + 4, 16);
                long thepage = curlp + 6 + 2;//reinterpret_cast<Uint8*>(curlp)+sizeof(lp_descriptor)+2 ;
                                            //long thepage = curlp;
                int destframe = framenumber - curl.baseRecord;

                int offset = 0;
                long pagepointer = thepage;
                for (int k = 0; k < destframe; k++)
                {
                    offset += (int)getAt(pages, pagepointer + (k * 2), 16);
                }
                //offset+= (int)cutsFile[k+pagepointer];//offset += pagepointer[i];
                //offset += (int)DataLoader.getValAtAddress(cutsFile,thepage,16);

                long ppointer = thepage + (curl.nRecords * 2) + offset;

                //Uint16 *ppointer16 = (Uint16*)(ppointer);
                if (cutsFile[ppointer + 1] == 0)
                {
                    ppointer += (4 + (cutsFile[ppointer + 1] + (cutsFile[ppointer + 1] & 1)));
                }
                else
                {
                    ppointer += 4;
                }
                //	byte[] imgOut ;//= //new byte[lpH.height*lpH.width+ 4000];
                if (!SkipImage(file, imagecount))
                {
                    myPlayRunSkipDump(ppointer, pages);//Stores in the global memory
                }                
                                               
                ImageCache[imagecount++] = Image(
                    databuffer: dstImage, 
                    dataOffSet: 0, 
                    width: lpH.width, height: lpH.height, 
                    palette: pal, 
                    useAlphaChannel: Alpha, 
                    useSingleRedChannel: false,
                    crop: UseCropping);

            }
        }


        /// <summary>
        /// Decodes a cutscene file. Heavily based on Underworld Adventures Hacking Tools
        /// </summary>
        /// <param name="inptr">Inptr.</param>
        /// <param name="srcData">Source data.</param>
        void myPlayRunSkipDump(long inptr, byte[] srcData)
        {//From an implemtation by Underworld Adventures (hacking tools)
            long outPtr = 0;

            //dstImage = new byte[size];
            while (true)
            {
                int sgn = (srcData[inptr] & 0x80) >> 7;//try and get the sign.
                if (sgn == 1)
                {
                    sgn = -1;
                }
                else
                {
                    sgn = 1;
                }
                int cnt = srcData[inptr++]; //(Sint8)*srcP++;
                                            //cnt=cnt*sgn;
                if (cnt * sgn > 0)
                {
                    // dump
                    while (cnt > 0)
                    {
                        //*dstP++ = *srcP++;
                        dstImage[outPtr++] = srcData[inptr++];
                        cnt--;
                    }
                }
                else if (cnt == 0)
                {
                    // run
                    //Uint8 wordCnt = *srcP++;
                    int wordCnt = srcData[inptr++];
                    //Uint8 pixel = *srcP++;
                    byte pixel = srcData[inptr++];
                    while (wordCnt > 0)
                    {
                        //*dstP++ = pixel;
                        dstImage[outPtr++] = pixel;
                        wordCnt--;
                    }
                }
                else
                {
                    cnt &= 0x7f; // cnt -= 0x80;
                    if (cnt != 0)
                    {
                        // shortSkip
                        //dstP += cnt;
                        outPtr += cnt;
                    }
                    else
                    {
                        // longOp
                        //Uint16 wordCnt = *((Uint16*)srcP);
                        int wordCnt = (int)getAt(srcData, inptr, 16);//srcData[inptr];
                                                                            //srcP += 2;
                        inptr += 2;
                        int wordcntSign = (wordCnt & 0x8000) >> 15;//try and get the sign.
                        if (wordcntSign == 1)
                        {
                            wordcntSign = -1;
                        }
                        else
                        {
                            wordcntSign = 1;
                        }
                        if (wordCnt * wordcntSign <= 0)
                        {
                            // notLongSkip
                            if (wordCnt == 0)
                            {
                                break; // end loop
                            }

                            wordCnt &= 0x7fff; // wordCnt -= 0x8000; // Remove sign bit.
                            if (wordCnt >= 0x4000)
                            {
                                // longRun
                                wordCnt -= 0x4000; // Clear "longRun" bit
                                                //Uint8 pixel = *srcP++;
                                byte pixel = srcData[inptr++];
                                while (wordCnt > 0)
                                {
                                    //*dstP++ = pixel;
                                    dstImage[outPtr++] = pixel;
                                    wordCnt--;
                                }
                                //                  dstP += wordCnt;
                            }
                            else
                            {
                                // longDump
                                while (wordCnt > 0)
                                {
                                    //*dstP++ = *srcP++;
                                    dstImage[outPtr++] = srcData[inptr++];
                                    wordCnt--;
                                }

                                //                  dstP += wordCnt;
                                //                  srcP += wordCnt;
                            }
                        }
                        else
                        {
                            // longSkip
                            //dstP += wordCnt;
                            outPtr += wordCnt;
                        }
                    }
                }
            }
        }


        public override ImageTexture LoadImageAt(int index)
        {
            return ImageCache[index];
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
                newmaterial.SetShaderParameter("UseAlpha", _alpha);
                materials[textureno] = newmaterial;
            }
            return materials[textureno];    
        }

        /// <summary>
        /// IN UW2 for some reason some frames are bugged and overwrite dstimage in the wrong location screwing up the rest of the cuts file
        /// this hack skips loading the image data for these images by not overwriting dstimage with their data.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        bool SkipImage(string file, int frame)
        {
            if (_RES == GAME_UW2)
            {
                switch(file.ToUpper())
                {
                    case "CS012.N01"://in acknowledgements only 1 in 4 frames is valid
                        return !(frame % 4 == 0);
                }
            }
                return false;
        }

    }//end class
} //end namespace