using System.Diagnostics;

namespace Underworld
{
    public partial class motion : Loader
    {
        static int iteration = 0;
        public static bool MotionSingleStepEnabled = false;
        static int MaybeSpecialMotionObjectDatFlag_dseg_67d6_26D2;

        static motion()
        {
            for (short i = 0; i <= collisionTable.GetUpperBound(0); i++)
            {//initialise collision records
                collisionTable[i] = new CollisionRecord(i);
            }
        }
        public static bool MotionProcessing(uwObject projectile)
        {
            if (!MotionSingleStepEnabled)
            {
                return false;
            }
            MotionSingleStepEnabled = false;//temp to only allow one step
            //Check if object is still "alive"
            if (projectile.npc_hp == 0)
            {
                if (commonObjDat.qualityclass(projectile.item_id) < 3)
                {
                    if (ObjectRemover.DeleteObjectFromTile(projectile.tileX, projectile.tileY, projectile.index))
                    {
                        return false;
                    }
                    else
                    {
                        projectile.npc_hp = 1;
                    }
                }
            }

            if (commonObjDat.maybeMagicObjectFlag(projectile.item_id))
            {
                MaybeSpecialMotionObjectDatFlag_dseg_67d6_26D2 = 0x1000;
            }
            else
            {
                MaybeSpecialMotionObjectDatFlag_dseg_67d6_26D2 = 0;
            }

            UWMotionParamArray MotionParams = new();

            if (iteration == 0)
            {
                Debug.Print($"Initial {projectile.a_name} Tile ({projectile.tileX},{projectile.tileY}), Position({projectile.xpos},{projectile.ypos},{projectile.zpos}) NPC_HP:{projectile.npc_hp} ProjectileHeading:{projectile.ProjectileHeading} UNKA_0123:{projectile.UnkBit_0XA_Bit0123} UNK_456:{projectile.UnkBit_0XA_Bit456} Coords ({projectile.CoordinateX},{projectile.CoordinateY},{projectile.CoordinateZ}) UNK13_0_6:{projectile.UnkBit_0X13_Bit0to6} UNK13_7:{projectile.UnkBit_0X13_Bit7} ProjectileSpeed:{projectile.Projectile_Speed}, ProjectilePitch:{projectile.Projectile_Pitch}");
            }

            InitMotionParams(projectile, MotionParams);

            CalculateMotion_TopLevel(projectile, MotionParams, MaybeSpecialMotionObjectDatFlag_dseg_67d6_26D2);
            DumpMotionMemory(MotionParams, "AfterCalculatingMotion");

            //store current x/y homes in globals                        
            var result = ApplyProjectileMotion(projectile, MotionParams);
            if (result)
            {
                projectile.UnkBit_0XA_Bit0123 = (short)((projectile.UnkBit_0XA_Bit0123 + projectile.Projectile_Speed) & 0xF);
                if (_RES == GAME_UW2)
                {
                    switch (projectile.item_id)
                    {
                        case 0x1B://homing dart
                            break;
                        case 0x1E://satellite
                            break;
                    }
                }
            }
            iteration++;
            Debug.Print($"After {iteration} {projectile.a_name} Tile ({projectile.tileX},{projectile.tileY}), Position({projectile.xpos},{projectile.ypos},{projectile.zpos}) NPC_HP:{projectile.npc_hp} ProjectileHeading:{projectile.ProjectileHeading} UNKA_0123:{projectile.UnkBit_0XA_Bit0123} UNK_456:{projectile.UnkBit_0XA_Bit456} Coords ({projectile.CoordinateX},{projectile.CoordinateY},{projectile.CoordinateZ}) UNK13_0_6:{projectile.UnkBit_0X13_Bit0to6} UNK13_7:{projectile.UnkBit_0X13_Bit7} ProjectileSpeed:{projectile.Projectile_Speed}, ProjectilePitch:{projectile.Projectile_Pitch}");
            objectInstance.Reposition(projectile);//finally move!            
            return result;
        }

        /// <summary>
        /// Applies the calculated motion values to the object in motion
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="MotionParams"></param>
        /// <returns></returns>
        public static bool ApplyProjectileMotion(uwObject projectile, UWMotionParamArray MotionParams)
        {//seg030_2BB7_689

            var di_tileYHasChanged = false;
            var var2_tilehaschanged = false;

            if (projectile.tileX != MotionParams.x_0 >> 8)
            {
                var2_tilehaschanged = true;
            }
            if (projectile.tileY != MotionParams.y_2 >> 8)
            {
                di_tileYHasChanged = true;
            }

            if (var2_tilehaschanged || di_tileYHasChanged)
            {
                //seg030_2BB7_6C7:
                //object has changed tiles
                var tileVar6 = UWTileMap.current_tilemap.Tiles[projectile.tileX, projectile.tileY];//the current tile.
                ObjectRemover.RemoveObjectFromLinkedList(tileVar6.indexObjectList, projectile.index, UWTileMap.current_tilemap.LevelObjects, tileVar6.Ptr + 2);

                if (_RES == GAME_UW2)
                {
                    //seg030_2BB7_718:         
                    //Debug.Print("TODO Run Exit trigger for projectile");
                }

                projectile.tileX = MotionParams.x_0 >> 8;
                projectile.tileY = MotionParams.y_2 >> 8;
                tileVar6 = UWTileMap.current_tilemap.Tiles[projectile.tileX, projectile.tileY];//the new tile
                projectile.next = tileVar6.indexObjectList;//TODO. Make sure .next does not already have a value!
                tileVar6.indexObjectList = projectile.index;

                //Set zpos now as pressure triggers need this info.
                projectile.zpos = (short)(MotionParams.z_4 >> 3);

                if (_RES == GAME_UW2)
                {
                    //seg030_2BB7_796:      
                    //Debug.Print("TODO Run enter trigger for projectile");
                }
            }
            else
            {
                //seg030_2BB7_7A3:
                if (_RES == GAME_UW2)
                {
                    //seg030_2BB7_7B7:
                    if (projectile.zpos != MotionParams.z_4)
                    {
                        //Debug.Print("TODO Run pressure trigger for projectile");                        
                    }
                }
            }

            projectile.zpos = (short)(MotionParams.z_4 >> 3);
            projectile.xpos = (short)(MotionParams.x_0 >> 5);
            projectile.ypos = (short)(MotionParams.y_2 >> 5);

            if (projectile.IsStatic)
            {
                projectile.quality = MotionParams.hp_1b;
            }
            else
            {
                projectile.npc_hp = MotionParams.hp_1b;
            }

            //seg030_2BB7_83E:
            if (MotionParams.unk_26 > 0x180)
            {
                var var6 = MotionParams.unk_26 >> 8;
                if (projectile.majorclass == 1)
                {
                    var6 = var6 >> 2;
                }
                else
                {
                    if (projectile.npc_hp < 0x20)
                    {
                        var6 = var6 << 2;
                    }
                }
                var var4_mass = commonObjDat.mass(projectile.item_id);
                if (projectile.majorclass != 1)
                {
                    //not an npc
                    Debug.Print($"seg030_2BB7_8B9 playsound effect {(var4_mass - 600) / 0x32}");
                }
                //fall damage?
                damage.DamageObject(
                    objToDamage: projectile,
                    basedamage: var6,
                    damagetype: 0,
                    objList: UWTileMap.current_tilemap.LevelObjects,
                    WorldObject: true, hitCoordinate: Godot.Vector3.Zero,
                    damagesource: 0,
                    ignoreVector: true);
            }

            if ((MotionParams.tilestate25 & 4) != 0)
            {//on lava, apply fire damage.
                //seg030_2BB7_90C:
                damage.DamageObject(
                    objToDamage: projectile,
                    basedamage: 1,
                    damagetype: 8,
                    objList: UWTileMap.current_tilemap.LevelObjects,
                    WorldObject: true, hitCoordinate: Godot.Vector3.Zero,
                    damagesource: 0,
                    ignoreVector: true);
            }

            if (projectile.majorclass != 1)
            {
                if (projectile.IsStatic)
                {//seg030_2BB7_937
                    if ((MotionParams.unk_14 | MotionParams.unk_a_pitch) != 0)
                    {
                        Debug.Print("Object should become mobile");
                        projectile = MoveObjectToMobileObjectList_seg030_2BB7_B0C(projectile);
                    }
                }
                else
                {
                    //seg030_2BB7_956:
                    if ((MotionParams.unk_14 | MotionParams.unk_a_pitch) == 0)
                    {
                        //object has likely stopped
                        projectile.UnkBit_0XA_Bit456 = (short)(dseg_67d6_3E8[MotionParams.tilestate25]);
                        projectile = ObjectHitsFloorTile_seg030_2BB7_DDF(projectile);
                        if (projectile == null)
                        {
                            return false;
                        }

                        //seg030_2BB7_99F:
                        projectile = ProbablyThrownObjectCollision_seg030_2BB7_10BC(projectile, projectile.tileX, projectile.tileY, 0);
                        if (projectile == null)
                        {
                            return false;
                        }
                        //seg030_2BB7_9C4:
                        if (!projectile.IsStatic)
                        {
                            seg030_2BB7_107A(MotionParams);
                        }
                    }
                }
            }

            //seg030_2BB7_9D5:
            if (projectile.IsStatic)
            {
                //seg030_2BB7_ADB: 
                if (projectile.majorclass == 5)
                {
                    projectile.heading = (ushort)((MotionParams.heading_1E >> 0xD) & 0x7);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //seg030_2BB7_9E1:
                projectile.ProjectileHeading = (ushort)(MotionParams.heading_1E >> 8);
                projectile.npc_xhome = (short)projectile.tileX;
                projectile.npc_yhome = (short)projectile.tileY;
                if (MotionParams.unk_10 == 0)
                {
                    projectile.UnkBit_0X13_Bit7 = 0;
                }
                else
                {
                    projectile.UnkBit_0X13_Bit7 = 1;
                }
                var cx = 0x10 + (MotionParams.unk_a_pitch / 0x40);
                if (cx < 0)
                {
                    cx = 0;
                }
                else
                {
                    if (cx > 0x1F)
                    {
                        cx = 0x1F;
                    }
                }

                projectile.Projectile_Pitch = (short)cx;
                projectile.UnkBit_0X13_Bit0to6 = (short)(MotionParams.unk_14 / 0x2F);
                projectile.UnkBit_0XA_Bit456 = dseg_67d6_3E8[MotionParams.tilestate25];
                if (projectile.majorclass != 1)
                {
                    projectile.CoordinateX = MotionParams.x_0;
                    projectile.CoordinateY = MotionParams.y_2;
                    projectile.CoordinateZ = MotionParams.z_4;
                }
            }
            return true;
        }



        /// <summary>
        /// Utililty function for mimicing the follwoing assembly
        /// NEG AX 
        /// SBB AX,AX
        /// inc ax
        /// </summary>
        /// <param name="value"></param>
        /// <returns>1 if value is 0, otherwise 0</returns>
        public static int SBB(int value)
        {
            if (value == 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        static uwObject MoveObjectToMobileObjectList_seg030_2BB7_B0C(uwObject toMove)
        {
            Debug.Print("move from static to mobile");
            return toMove;
        }

        static uwObject ObjectHitsFloorTile_seg030_2BB7_DDF(uwObject projectile)
        {
            Debug.Print("object hits floor tile");
            return projectile;
        }

        static void seg030_2BB7_107A(UWMotionParamArray MotionParams)
        {
            if (UWMotionParamArray.dseg_67d6_25C1 == 0)
            {
                MotionParams.heading_1E = (ushort)((Rng.r.Next(0x7FFF) & 0x3FFF) + MotionParams.heading_1E + 0xE000);
                MotionParams.unk_14 = 0xBC;

            }
            else
            {
                MotionParams.unk_14 = (short)(0x2F * ((Rng.r.Next(0x7FFF) + 1) & 0x3));
                MotionParams.unk_10 = -4;
            }
        }



        public static void seg006_1413_9F5(uwObject projectile)
        {//Init motion for NPCs

        }

    }//end class
}//end namespace