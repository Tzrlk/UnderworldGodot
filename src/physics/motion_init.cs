namespace Underworld
{
    public partial class motion : Loader
    {

        /// <summary>
        /// Initialises the params needed for calculating object motion.
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="MotionParams"></param>
        public static void InitMotionParams(uwObject projectile, UWMotionParamArray MotionParams)
        {
            bool isNPC = true;
            var itemid = projectile.item_id;

            //Common items
            MotionParams.index_20 = projectile.index;
            MotionParams.mass_18 = (short)commonObjDat.mass(itemid);
            MotionParams.unk_1a = (byte)commonObjDat.unk6_4(itemid);
            MotionParams.unk_16_relatedtoPitch = (byte)commonObjDat.unk6_5678(itemid);
            MotionParams.scaleresistances_1C = (byte)commonObjDat.scaleresistances(itemid);
            MotionParams.unk_1d = 0;
            MotionParams.heading_1E = (short)(projectile.heading << 0xD);
            MotionParams.unk_24 = 0;
            MotionParams.radius_22 = (byte)commonObjDat.radius(itemid);
            MotionParams.height_23 = (byte)commonObjDat.height(itemid);
            MotionParams.x_0 = projectile.xpos;
            MotionParams.y_2 = projectile.ypos;
            MotionParams.z_4 = projectile.zpos;

            if (projectile.IsStatic)
            {//Not sure when a static projectile will hit this but including for completedness. (possibly collisions?)
                MotionParams.unk_a_pitch = 0;
                MotionParams.unk_10 = 0;
                MotionParams.unk_14 = 0;
                MotionParams.hp_1b = (byte)projectile.quality;
                MotionParams.x_0 += (short)(projectile.tileX << 3);
                MotionParams.y_2 += (short)(projectile.tileY << 3);
            }
            else
            {
                MotionParams.x_0 += (short)(projectile.npc_xhome << 3);
                MotionParams.y_2 += (short)(projectile.npc_yhome << 3);
                MotionParams.heading_1E = (short)(projectile.ProjectileHeading << 8);
                MotionParams.tilestate25 = (byte)(1 << projectile.TileState_0XA_Bit456);
                MotionParams.unk_a_pitch = (short)((projectile.Projectile_Pitch - 16) << 6);
                MotionParams.unk_10 = (short)(projectile.UnkBit_0X13_Bit7 * -4);
                MotionParams.hp_1b = projectile.npc_hp;

                if (projectile.majorclass != 1)
                {//seg030_2BB7_583:
                    isNPC = false;
                    MotionParams.x_0 = (short)projectile.CoordinateX;
                    MotionParams.y_2 = (short)projectile.CoordinateY;
                    MotionParams.z_4 = (short)projectile.CoordinateZ;
                }
                MotionParams.unk_14 = projectile.UnkBit_0X13_Bit0to6;
            }


            if (
                (projectile.majorclass != 1)
                &&
                (commonObjDat.maybeMagicObjectFlag(itemid) == false)
                &&
                ((MotionParams.unk_a_pitch | MotionParams.unk_10) == 0)
            )
            {
                //seg030_2BB7_5CE:
                if (2 + (MotionParams.unk_1a << 1) >= MotionParams.unk_14)
                {//seg030_2BB7_5DA: 
                    MotionParams.unk_14 = 0;
                }
                else
                {
                    MotionParams.unk_14 = (short)(projectile.UnkBit_0X13_Bit0to6 * (0x29 + (MotionParams.unk_1a << 2)));
                    if (projectile.majorclass == 1)
                    {
                        MotionParams.unk_24 = 8;
                    }
                }
            }
            else
            {//seg030_2BB7_5F8
                MotionParams.unk_14 = (short)(MotionParams.unk_14 * 0x2F);
                if (projectile.majorclass == 1)
                {
                    MotionParams.unk_24 = 8;
                }
            }


            if (isNPC)
            {
                MotionParams.x_0 = (short)((MotionParams.x_0 << 5) + Rng.r.Next(32));//This randomness is used to give npcs a nudge
                MotionParams.y_2 = (short)((MotionParams.y_2 << 5) + Rng.r.Next(32));//and prevents them from getting
                MotionParams.z_4 = (short)((MotionParams.z_4 << 3) + Rng.r.Next(8));//stuck when following a path
            }
            MotionParams.unk_26_falldamage = 0;
        }


    }//end class
}//end namespace