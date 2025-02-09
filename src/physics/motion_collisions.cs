namespace Underworld
{
    /// <summary>
    /// code and data relating to collisions
    /// </summary>
    public partial class motion : Loader
    {
        public static CollisionRecord[] collisionTable = new CollisionRecord[32]; 
        

        static void ProbablyCollisions_seg031_2CFA_10E(int arg0)
        {
            //todo
        }



        /// <summary>
        /// Looks like this adds the object as a possible candidate for collision checking.
        /// </summary>
        /// <param name="MotionParams"></param>
        /// <param name="CollidingObject"></param>
        /// <param name="ListHead"></param>
        /// <param name="xArg6"></param>
        /// <param name="yArg8"></param>
        /// <param name="isNPCArgA"></param>
        static void CreateCollisonRecord_Seg028_2941_A78(UWMotionParamArray MotionParams, uwObject CollidingObject, long ListHead, int xArg6, int yArg8, bool isNPCArgA)
        {
            if (MotionCalcArray.Unk14_collisoncount <= 8)
            {
                int XCoordVar1;
                int YCoordVar2;
                int XVar3;
                int YVar4;
                int cx_radius;
                if (commonObjDat.radius(CollidingObject.item_id) == 4)
                {
                    XCoordVar1 = xArg6 << 3;
                    XVar3 = 7 + xArg6 << 3;

                    YCoordVar2 = yArg8 << 3;
                    YVar4 = 7 + yArg8 << 3;
                }
                else
                {
                    XCoordVar1 = CollidingObject.xpos + (xArg6 << 3);
                    YCoordVar2 = CollidingObject.ypos + (yArg8 << 3);
                    cx_radius = commonObjDat.radius(CollidingObject.item_id);
                    if ((CollidingObject.majorclass == 1) && (cx_radius > 0) && (isNPCArgA))
                    {
                        cx_radius--;
                    }
                    XVar3 = XCoordVar1 + cx_radius;
                    YVar4 = YCoordVar2 + cx_radius;
                    XCoordVar1 -= cx_radius;
                    YCoordVar2 -= cx_radius;
                    if (
                        (XVar3 >= UWMotionParamArray.XposMinusRad)
                        &&
                        (XCoordVar1 <= UWMotionParamArray.XposPlusRad)
                        &&
                        (YVar4 >= UWMotionParamArray.YposMinusRad)
                        &&
                        (YCoordVar2 <= UWMotionParamArray.YposMinusRad)
                    )
                    {
                        //seg028_2941_B73
                        MotionCalcArray.Unk14_collisoncount++;
                        var si_ptr = MotionCalcArray.Unk14_collisoncount;// * 6;
                        collisionTable[si_ptr].zpos = (byte)CollidingObject.zpos;
                        //DataLoader.setAt(motion.Collisions_dseg_2520, si_ptr + 1, 8, CollidingObject.zpos);
                        collisionTable[si_ptr].height = (byte)commonObjDat.height(CollidingObject.item_id);
                        //DataLoader.setAt(motion.Collisions_dseg_2520, si_ptr, 8, commonObjDat.height(CollidingObject.item_id));
                        if (commonObjDat.height(CollidingObject.item_id) == 0)
                        {
                            //motion.Collisions_dseg_2520[0]++;
                            collisionTable[si_ptr].height++;
                        }

                        //seg028_2941_BB3:
                        collisionTable[si_ptr].link = (short)ListHead;
                        // var temp = DataLoader.getAt(motion.Collisions_dseg_2520, si_ptr + 2, 16);
                        // temp = temp & 0x3F;
                        // temp = (uint)(temp | (ListHead << 6));

                        collisionTable[si_ptr].quality = 9;
                        // temp = temp & 0xC0;
                        // temp = temp | 9;
                        //DataLoader.setAt(motion.Collisions_dseg_2520, si_ptr + 2, 16, (int)temp);

                        //seg028_2941_BCF
                        if (UWMotionParamArray.xpos_dseg_67d6_2585 >= XCoordVar1)
                        {
                            if (UWMotionParamArray.xpos_dseg_67d6_2585 <= XVar3)
                            {
                                if (UWMotionParamArray.ypos_dseg_67d6_251C >= YCoordVar2)
                                {
                                    if (UWMotionParamArray.ypos_dseg_67d6_251C <= YVar4)
                                    {
                                        collisionTable[si_ptr].quality |= 0x10; 
                                        // var tmp = DataLoader.getAt(motion.Collisions_dseg_2520, si_ptr + 2, 16);
                                        // var tochange = tmp & 0x3F;
                                        // tochange = tochange | 0x10;
                                        // tmp = tmp & 0xC0;
                                        // tmp = tmp | tochange;
                                        // DataLoader.setAt(motion.Collisions_dseg_2520, si_ptr + 2, 16, (int)tmp);
                                    }
                                }
                            }
                        }

                        //seg028_2941_BF9:
                        collisionTable[si_ptr].unkxyvalue = (short)(xArg6 + (yArg8 << 6));
                        //DataLoader.setAt(motion.Collisions_dseg_2520, si_ptr + 4, 16, xArg6 + (yArg8 << 6));
                    }
                }
            }
        }

        /// <summary>
        /// Looks like this checks the tile for possible objects to collide with.
        /// </summary>
        /// <param name="MotionParams"></param>
        /// <param name="arg0"></param>
        /// <param name="arg2"></param>
        static void ScanForCollisions_seg028_2941_C0E(UWMotionParamArray MotionParams, int arg0, int arg2)
        {
            var isNPC_var14 = false;
            var tile_var4 = UWTileMap.current_tilemap.Tiles[MotionCalcArray.x0 >> 3, MotionCalcArray.y2 >> 3];
            if (MotionCalcArray.MotionArrayObjectIndexA != 0)
            {
                var obj = UWTileMap.current_tilemap.LevelObjects[MotionCalcArray.MotionArrayObjectIndexA];
                if (obj.majorclass == 1)
                {
                    isNPC_var14 = true;
                }
            }

            MotionCalcArray.Unk14_collisoncount = 0;
            UWMotionParamArray.xpos_dseg_67d6_2585 = MotionCalcArray.x0 & 0x7;
            UWMotionParamArray.ypos_dseg_67d6_251C = MotionCalcArray.y2 & 0x7;

            int Radius_var13 = 0;
            if ((arg0 != 0) && (MotionCalcArray.Height9 == 0))
            {
                Radius_var13 = 0;
                if ((isNPC_var14) && (Radius_var13 > 0))
                {
                    Radius_var13--;
                }
            }
            else
            {
                Radius_var13 = MotionCalcArray.Radius8;
            }

            //seg028_2941_CAF
            UWMotionParamArray.XposPlusRad = UWMotionParamArray.xpos_dseg_67d6_2585 + Radius_var13;
            UWMotionParamArray.YposPlusRad = UWMotionParamArray.ypos_dseg_67d6_251C + Radius_var13;
            UWMotionParamArray.XposMinusRad = UWMotionParamArray.xpos_dseg_67d6_2585 - Radius_var13;
            UWMotionParamArray.YposMinusRad = UWMotionParamArray.ypos_dseg_67d6_251C - Radius_var13;

            //seg028_2941_CD3:
            var varD = (UWMotionParamArray.XposMinusRad - 11) / 8;
            var varE = (UWMotionParamArray.YposMinusRad - 11) / 8;

            //seg028_2941_CF0:
            var varF = (UWMotionParamArray.XposPlusRad + 4) / 8;
            var var10 = (UWMotionParamArray.YposPlusRad + 4) / 8;

            var var11_maybeX = varD;

            //seg028_2941_E64:
            while (var11_maybeX > varF)
            {
                //seg028_2941_D13:
                var var12_maybeY = varE;
                while (var12_maybeY <= var10)
                {
                    //seg028_2941_D1C:
                    var si = 0;

                    var di = var11_maybeX + (var12_maybeY << 6);
                    var tilePtr = (int)(tile_var4.Ptr + di);
                    var tileAtPTR = UWTileMap.GetTileByPTR(tilePtr);
                    int next = tileAtPTR.indexObjectList;
                    //seg028_2941_E36:
                    //loop the object list on the tile.
                    var indexByte = tileAtPTR.indexObjectList;
                    while (next != 0 && si <= 0x40)
                    {
                        var obj = UWTileMap.current_tilemap.LevelObjects[next];
                        if (next != MotionCalcArray.MotionArrayObjectIndexA)
                        {//maybe check for colliding.
                            if (
                                (!isNPC_var14)
                                ||
                                (isNPC_var14 && (commonObjDat.UnknownFlag(obj.item_id) == false))
                            )
                            {
                                //seg028_2941_DA1
                                if (
                                    (commonObjDat.height(obj.item_id) != 0)
                                    ||
                                    (commonObjDat.height(obj.item_id) == 0) && (!obj.IsStatic)
                                )
                                {
                                    //seg028_2941_DB7:
                                    if (
                                        (obj.IsStatic)
                                        ||
                                        ((!obj.IsStatic) && (obj.majorclass != 1))
                                        ||
                                        ((!obj.IsStatic) && (obj.majorclass == 1) && (obj.UnkBit_0X15_Bit7 == 0))
                                        )
                                    {
                                        if (
                                            (arg2 == 0)
                                            ||
                                            ((arg2 != 0) && commonObjDat.ActivatedByCollision(obj.item_id))
                                            )
                                        {
                                            CreateCollisonRecord_Seg028_2941_A78(MotionParams, obj, indexByte, var11_maybeX, var12_maybeY, isNPC_var14);
                                        }
                                    }
                                }
                            }
                        }
                        indexByte = obj.index;
                        next = obj.next;
                        si++;
                    }
                    if (si > 0x40)
                    {
                        return;
                    }
                    else
                    {
                        var12_maybeY++;
                    }
                }
                //seg028_2941_E61:
                var11_maybeX++;
            }
        }

    }//end class
}//end namespace