namespace Underworld
{
    /// <summary>
    /// A class based implementation of an array of motion params that UW uses for projectile calcs
    /// </summary>
    public class UWMotionParamArray : UWClass
    {
        public byte[] data = new byte[0x28];

        public static byte[] data_3FC = new byte[40];//globals at 0x3FC
        public static byte[] dseg_26b8 = new byte[16];//confirm size

        public static byte[] DSEG_27B2_SpecialMotionHandling = new byte[8];
        public static byte[] DSEG_26BA_LandNPCMotionHandler = new byte[] { 0x0, 0x0, 0x30, 0x1F, 0x10, 0x10, 0x20, 0x0 };
        public static byte[] DSEG_26DE_SwimmingNPCMotionHandler = new byte[] { 0x10, 0, 0x28, 0x17, 0x10, 0x10, 0x20, 0 };
        public static byte[] DSEG_26C6_FlyingNPCMotionHandler = new byte[] { 0x0, 0x10, 0x0, 0x7, 0x80, 0x0, 0x0, 0x0 };
        public static byte[] PlayerMotionHandler_dseg_67d6_26AA = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        public static short LikelyNPCTileStates_222C;

        public static byte[] PtrTo26D2_DSEG_26B8_MotionHandler = new byte[8];

        public static short PtrTo267D2_dseg_67d6_26B8_table0
        {
            get
            {
                return (short)DataLoader.getAt(UWMotionParamArray.PtrTo26D2_DSEG_26B8_MotionHandler, 0, 16);
            }
            set
            {
                DataLoader.setAt(UWMotionParamArray.PtrTo26D2_DSEG_26B8_MotionHandler, 0, 16, value);
            }
        }

        public static short dseg_67d6_26BA_MotionHandler2
        {
            get
            {
                return (short)DataLoader.getAt(UWMotionParamArray.PtrTo26D2_DSEG_26B8_MotionHandler, 2, 16);
            }
        }


        /// <summary>
        /// Magic projectile + 4
        /// </summary>
        public static short dseg_67d6_26BC_table4
        {
            get
            {
                return (short)DataLoader.getAt(UWMotionParamArray.PtrTo26D2_DSEG_26B8_MotionHandler, 4, 16);
            }
        }


        //globals        



        public static short RelatedToMotionX_dseg_67d6_3FE
        {//2
            get
            {
                return (short)DataLoader.getAt(data_3FC, 2, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 2, 16, value);
            }
        }
        public static short RelatedToMotionY_dseg_67d6_400
        {//4
            get
            {
                return (short)DataLoader.getAt(data_3FC, 4, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 4, 16, value);
            }
        }
        public static short RelatedToMotionZ_dseg_67d6_402
        {//6
            get
            {
                return (short)DataLoader.getAt(data_3FC, 6, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 6, 16, value);
            }
        }

        //public static short[] dseg_67d6_404 = new short[2];//8
        //public const int dseg_67d6_404 = 8; //for indexing to this value array     
        public static short Gravity_related_dseg_67d6_408
        {//0xC
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0xC, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0xC, 16, value);
            }
        }

        public static short MotionGlobal_dseg_67d6_40A_indexer
        {//0xE
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0xE, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0xE, 16, value);
            }
        }
        public static short dseg_67d6_40C_indexer
        {//0x10
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x10, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x10, 16, value);
            }
        }
        public static short MAYBEcollisionOrGravity_dseg_67d6_40E
        {//0x12
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x12, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x12, 16, value);
            }
        }

        public static short dseg_67d6_410
        {//0x14
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x14, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x14, 16, value);
            }
        }

        public static short dseg_67d6_412
        {//0x16
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x16, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x16, 16, value);
            }
        }
        public static short GravityCollisionRelated_dseg_67d6_414
        {//0x18
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x18, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x18, 16, value);
            }
        }

        public static sbyte ACollisionIndex_dseg_67d6_416
        {//0x1A
            get
            {
                return (sbyte)DataLoader.getAt(data_3FC, 0x1A, 8);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x1A, 8, value);
            }
        }

        public static int CollisionItemID_dseg_67d6_417
        {
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x1B, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x1B, 16, value);
            }
        }

        public static ushort CollisionZposHeightRelated_dseg_67d6_419
        {//0x1D
            get
            {
                return (ushort)DataLoader.getAt(data_3FC, 0x1D, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x1D, 16, value);
            }
        }


        public static int dseg_67d6_41D//height related in collision
        {//0x21
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x21, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x21, 16, value);
            }
        }


        public static short Gravity_Related_dseg_67d6_41F
        {//0x23
            get
            {
                return (short)DataLoader.getAt(data_3FC, 0x23, 16);
            }
            set
            {
                DataLoader.setAt(data_3FC, 0x23, 16, value);
            }
        }

        public static short GetMotionXY_3FE(int index)
        {
            return (short)DataLoader.getAt(data_3FC, 2 + index * 2, 16);
        }
        public static void SetMotionXY3FE(int index, short value)
        {
            DataLoader.setAt(data_3FC, 2 + index * 2, 16, value);
        }


        public static short GetMotionXY_404(int index)
        {
            return (short)DataLoader.getAt(data_3FC, 8 + index * 2, 16);
        }
        public static void SetMotionXY404(int index, short value)
        {
            DataLoader.setAt(data_3FC, 8 + index * 2, 16, value);
        }


        public static short GetMotionXY_421(int index)
        {
            return (short)DataLoader.getAt(data_3FC, 25 + index * 2, 16);
        }
        // public static void SetMotionXY421(int index, short value)
        // {
        //     DataLoader.setAt(data_3FC, 25 + index * 2, 16, value);
        // }




        public static int XposPlusRad;
        public static int YposPlusRad;
        public static int XposMinusRad;
        public static int YposMinusRad;
        public static OtherMotionArray SubArray = new OtherMotionArray();
        public MotionCalcArray CalcArray = new MotionCalcArray();
        public static short[] TileAttributesArray;
        public static int ypos_dseg_67d6_251C;
        public static TileInfo TileRelatedToMotion_dseg_67d6_257E;

        public static int Z_dseg_67d6_2582;
        public static int dseg_67d6_2584;
        public static int xpos_dseg_67d6_2585;
        public static int dseg_67d6_2586;
        public static int dseg_67d6_25BC;
        public static int UnknownX_dseg_67d6_25BD;
        public static int UnknownY_dseg_67d6_25BE;
        public static byte MotionParam0x25_dseg_67d6_26A9;
        //public static int CalculateMotionGlobal_dseg_67d6_25DB; //or offset 17 in calc array                
        public static byte dseg_67d6_26A4;
        public static byte dseg_67d6_26A5;
        public static int dseg_67d6_26A8;

        public static int CalculateMotionGlobal_dseg_67d6_26B6;

        public static int dseg_67d6_25BF_X;
        public static int dseg_67d6_25C0_Y;
        public static int dseg_67d6_25C1;
        public static int dseg_67d6_25C2;
        public static int dseg_67d6_2614;
        public static bool dseg_67d6_260C;
        public static bool dseg_67d6_260A;

        public static int DoorX_222E;
        public static int DoorY_222F;

        //The class properties
        public short x_0
        {
            get
            {
                return (short)DataLoader.getAt(data, 0, 16);
            }
            set
            {
                DataLoader.setAt(data, 0, 16, value);
            }
        }
        public short y_2
        {
            get
            {
                return (short)DataLoader.getAt(data, 2, 16);
            }
            set
            {
                DataLoader.setAt(data, 2, 16, value);
            }
        }
        public short z_4
        {
            get
            {
                return (short)DataLoader.getAt(data, 4, 16);
            }
            set
            {
                DataLoader.setAt(data, 4, 16, value);
            }
        }

        public short unk_6_x
        {
            get
            {
                return (short)DataLoader.getAt(data, 6, 16);
            }
            set
            {
                DataLoader.setAt(data, 6, 16, value);
            }
        }
        public short unk_8_y
        {
            get
            {
                return (short)DataLoader.getAt(data, 8, 16);
            }
            set
            {
                DataLoader.setAt(data, 8, 16, value);
            }
        }

        /// <summary>
        /// Possibly related to mass or force?
        /// </summary>
        public short unk_a_pitch
        {
            get
            {
                return (short)DataLoader.getAt(data, 0xA, 16);
            }
            set
            {
                DataLoader.setAt(data, 0xA, 16, value);
            }
        }
        public short unk_c_X
        {
            get
            {
                return (short)DataLoader.getAt(data, 0xC, 16);
            }
            set
            {
                DataLoader.setAt(data, 0xC, 16, value);
            }
        }
        // public short unk_d
        // {
        //     get
        //     {
        //         return (short)DataLoader.getAt(data, 0xD, 16);
        //     }
        //     set
        //     {
        //         DataLoader.setAt(data, 0xD, 16, value);
        //     }
        // }

        public short unk_e
        {
            get
            {
                return (short)DataLoader.getAt(data, 0xE, 16);
            }
            set
            {
                DataLoader.setAt(data, 0xE, 16, value);
            }
        }

        /// <summary>
        /// Possibly a Z speedvalue
        /// </summary>
        public short unk_10
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x10, 16);
            }
            set
            {
                DataLoader.setAt(data, 0x10, 16, value);
            }
        }
        public byte speed_12
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x12, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x12, 8, value);
            }
        }
        // public sbyte unk_13_falldamage
        // {
        //     get
        //     {
        //         return (sbyte)DataLoader.getAt(data, 0x13, 8);
        //     }
        //     set
        //     {
        //         DataLoader.setAt(data, 0x13, 8, value);
        //     }
        // }
        public short unk_14
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x14, 16);
            }
            set
            {
                DataLoader.setAt(data, 0x14, 16, value);
            }
        }
        public byte unk_16_relatedtoPitch
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x16, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x16, 8, value);
            }
        }

        /// <summary>
        /// Not sure where this is set?
        /// </summary>
        public short unk_17
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x17, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x17, 8, value);
            }
        }

        public short mass_18
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x18, 16);
            }
            set
            {
                DataLoader.setAt(data, 0x18, 16, value);
            }
        }
        public byte unk_1a
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x1A, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x1A, 8, value);
            }
        }
        public byte hp_1b
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x1B, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x1B, 8, value);
            }
        }
        public byte scaleresistances_1C
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x1C, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x1C, 8, value);
            }
        }

        public sbyte unk_1d
        {
            get
            {
                return (sbyte)DataLoader.getAt(data, 0x1D, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x1D, 8, value);
            }
        }

        public short heading_1E
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x1E, 16);
            }
            set
            {
                DataLoader.setAt(data, 0x1E, 16, value);
            }
        }

        public short index_20
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x20, 16);
            }
            set
            {
                DataLoader.setAt(data, 0x20, 16, value);
            }
        }
        public byte radius_22
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x22, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x22, 8, value);
            }
        }
        public byte height_23
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x23, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x23, 8, value);
            }
        }
        public byte unk_24
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x24, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x24, 8, value);
            }
        }


        /// <summary>
        /// indicates if on a tile and what type of surface the tile has(eg lava = 4)
        /// </summary>
        public byte tilestate25
        {
            get
            {
                return (byte)DataLoader.getAt(data, 0x25, 8);
            }
            set
            {
                DataLoader.setAt(data, 0x25, 8, value);
            }
        }


        /// <summary>
        /// Possibly related to fall damage.
        /// </summary>
        public short unk_26_falldamage
        {
            get
            {
                return (short)DataLoader.getAt(data, 0x26, 16);
            }
            set
            {
                DataLoader.setAt(data, 0x26, 16, value);
            }
        }

        public short GetParam6_XorYValue(int offset)
        {
            return (short)DataLoader.getAt(data, 0x6 + offset * 2, 16);
        }

        public void SetParam6_XorYValue(int offset, short value)
        {
            DataLoader.setAt(data, 0x6 + offset * 2, 16, value);
        }
    }

    /// <summary>
    /// Class representing data starting at DSEG:25C4
    /// </summary>
    public class MotionCalcArray : UWClass
    {
        /// <summary>
        /// Raw data
        /// </summary>
        public static byte[] base_dseg_25c4 = new byte[0x20];

        public static byte[] PtrToMotionCalc;

        public static ushort x0_base
        {
            get
            {
                return (ushort)DataLoader.getAt(base_dseg_25c4, 0, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0, 16, value);
            }
        }

        public static ushort x0
        {
            get
            {
                return (ushort)DataLoader.getAt(PtrToMotionCalc, 0, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0, 16, value);
            }
        }



        public static ushort y2_base
        {
            get
            {
                return (ushort)DataLoader.getAt(base_dseg_25c4, 2, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 2, 16, value);
            }
        }

        public static ushort y2
        {
            get
            {
                return (ushort)DataLoader.getAt(PtrToMotionCalc, 2, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 2, 16, value);
            }
        }

        public static ushort z4_base
        {
            get
            {
                return (ushort)DataLoader.getAt(base_dseg_25c4, 4, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 4, 16, value);
            }
        }

        public static ushort z4
        {
            get
            {
                return (ushort)DataLoader.getAt(PtrToMotionCalc, 4, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 4, 16, value);
            }
        }


        public static short Heading6_base
        {//TO confirm should this be signed or not?
            get
            {
                return (short)DataLoader.getAt(base_dseg_25c4, 6, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 6, 16, value);
            }
        }

        /// <summary>
        /// At offset + 6
        /// </summary>
        public static short Heading6
        {//TO confirm should this be signed or not?
            get
            {
                return (short)DataLoader.getAt(PtrToMotionCalc, 6, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 6, 16, value);
            }
        }

        public static byte Radius8_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 8, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 8, 8, value);
            }
        }

        public static byte Radius8
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 8, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 8, 8, value);
            }
        }
        public static byte Height9
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 9, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 9, 8, value);
            }
        }

        public static short MotionArrayObjectIndexA_base
        {
            get
            {
                return (short)DataLoader.getAt(base_dseg_25c4, 0xA, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0xA, 16, value);
            }
        }

        public static short MotionArrayObjectIndexA
        {
            get
            {
                return (short)DataLoader.getAt(PtrToMotionCalc, 0xA, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0xA, 16, value);
            }
        }

        public static short UnkC_terrain_base
        {
            get
            {
                return (short)DataLoader.getAt(base_dseg_25c4, 0xC, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0xC, 16, value);
            }
        }

        public static short UnkC_terrain
        {
            get
            {
                return (short)DataLoader.getAt(PtrToMotionCalc, 0xC, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0xC, 16, value);
            }
        }

        public static short UnkE_base
        {
            get
            {
                return (short)DataLoader.getAt(base_dseg_25c4, 0xE, 16);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0xE, 16, value);
            }
        }

        public static short UnkE
        {
            get
            {
                return (short)DataLoader.getAt(PtrToMotionCalc, 0xE, 16);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0xE, 16, value);
            }
        }


        public static byte Unk10_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 0x10, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x10, 8, value);
            }
        }

        public static byte Unk10_relatedtotileheight
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 0x10, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x10, 8, value);
            }
        }

        public static byte Unk11_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 0x11, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x11, 8, value);
            }
        }

        public static byte Unk11
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 0x11, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x11, 8, value);
            }
        }


        public static byte Unk12_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 0x12, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x12, 8, value);
            }
        }

        public static sbyte Unk12
        {
            get
            {
                return (sbyte)DataLoader.getAt(PtrToMotionCalc, 0x12, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x12, 8, value);
            }
        }

        public static sbyte Unk13
        {
            get
            {
                return (sbyte)DataLoader.getAt(PtrToMotionCalc, 0x13, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x13, 8, value);
            }
        }

        public static byte Unk14_collisoncount_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 0x14, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x14, 8, value);
            }
        }

        public static byte Unk14_collisoncount
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 0x14, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x14, 8, value);
            }
        }


        public static byte Unk15_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 0x15, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x15, 8, value);
            }
        }

        public static byte Unk15
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 0x15, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x15, 8, value);
            }
        }

        public static byte Unk16_collisionindex_base
        {
            get
            {
                return (byte)DataLoader.getAt(base_dseg_25c4, 0x16, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x16, 8, value);
            }
        }

        public static byte Unk16_collisionindex
        {
            get
            {
                return (byte)DataLoader.getAt(PtrToMotionCalc, 0x16, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x16, 8, value);
            }
        }

        public static sbyte Unk17_base
        {
            get
            {
                return (sbyte)DataLoader.getAt(base_dseg_25c4, 0x17, 8);
            }
            set
            {
                DataLoader.setAt(base_dseg_25c4, 0x17, 8, value);
            }
        }

        public static sbyte Unk17
        {
            get
            {
                return (sbyte)DataLoader.getAt(PtrToMotionCalc, 0x17, 8);
            }
            set
            {
                DataLoader.setAt(PtrToMotionCalc, 0x17, 8, value);
            }
        }
    }


    /// <summary>
    /// This may need to be removed as it's just globals.
    /// </summary>
    public class OtherMotionArray : Loader
    {
        public static OtherMotionArray instance;
        public byte[] dseg_2562 = new byte[0x20];


        public OtherMotionArray()
        {
            instance = this;
        }

        // public int Unk0//dseg_2562
        // {
        //     get
        //     {
        //         return (int)DataLoader.getAt(dseg_2562, 0, 16);
        //     }
        //     set
        //     {
        //         DataLoader.setAt(dseg_2562, 0, 16, value);
        //     }
        // }

        public int Unk2_offset//dseg_2564
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 2, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 2, 8, value);
            }
        }
        public int Unk3_X//dseg_2565
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 3, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 3, 8, value);
            }
        }

        public int Unk4_Y//dseg_2566
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 4, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 4, 8, value);
            }
        }
        public int Unk5//dseg_2567
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 5, 16);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 5, 16, value);
            }
        }
        public int Unk7_offset//dseg_2569
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 7, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 7, 8, value);
            }
        }
        public int Unk8_X//dseg_256a
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 8, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 8, 8, value);
            }
        }
        public int Unk9_Y//dseg_256B
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 9, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 9, 8, value);
            }
        }
        public int UnkA//dseg 256C              
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0xA, 16);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0xA, 16, value);
            }
        }
        public int Unkc_offset
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0xC, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0xC, 8, value);
            }
        }
        public int UnkD_x
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0xD, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0xD, 8, value);
            }
        }
        public int UnkE
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0xE, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0xE, 8, value);
            }
        }
        public int UnkF//2571
        {
            get
            {//to confirm is this a byte or a word?
                return (int)DataLoader.getAt(dseg_2562, 0xF, 16);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0xF, 16, value);
            }
        }

        // public int Unk10
        // {
        //     get
        //     {
        //         return (int)DataLoader.getAt(dseg_2562, 0x10, 8);
        //     }
        //     set
        //     {
        //         DataLoader.setAt(dseg_2562, 0x10, 8, value);
        //     }
        // }

        public int Unk11_offset
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x11, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x11, 8, value);
            }
        }
        public int Unk12_x
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x12, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x12, 8, value);
            }
        }
        public int Unk13_y
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x13, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x13, 8, value);
            }
        }
        public int Unk14//2576
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x14, 16);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x14, 16, value);
            }
        }
        public int Unk16
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x16, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x16, 8, value);
            }
        }
        public int Unk17
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x17, 8);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x17, 8, value);
            }
        }
        public int Unk18
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x18, 16);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x18, 16, value);
            }
        }
        public int Unk1A
        {
            get
            {
                return (int)DataLoader.getAt(dseg_2562, 0x1A, 16);
            }
            set
            {
                DataLoader.setAt(dseg_2562, 0x1A, 16, value);
            }
        }
    }//end class
}//end namespace