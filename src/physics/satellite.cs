using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Godot;

namespace Underworld
{
    public partial class motion : Loader
    {
        /// <summary>
        /// Handles the rotation of the satellite spell projectile around it's caster
        /// </summary>
        /// <param name="projectile"></param>
        static void Satellite(uwObject projectile)
        {
            var caster = UWTileMap.current_tilemap.LevelObjects[projectile.ProjectileSourceID];
            var casterXCoord = (caster.npc_xhome << 3) + caster.xpos;
            var casterYCoord = (caster.npc_yhome << 3) + caster.ypos;

            var ProjXCoord = (projectile.npc_xhome << 3) + projectile.xpos;
            var ProjYCoord = (projectile.npc_yhome << 3) + projectile.ypos;
            var di = SatelliteVectoring(ProjXCoord, ProjYCoord, casterXCoord, casterYCoord);
            var ax = (short)(di + 0x4000);
            var CalcedHeading_1C = ax;
            var si_projectileheading = (short)Math.Abs(projectile.ProjectileHeading - (ax >> 8));
            if (si_projectileheading > 0x80)
            {
                si_projectileheading -= 0xFF;
            }
            if (si_projectileheading > 0x40)
            {
                CalcedHeading_1C = (short)(CalcedHeading_1C - 32768);
            }

            var varA_distanceapart = (short)((short)Math.Abs(ProjXCoord - casterXCoord) + (short)Math.Abs(ProjYCoord - casterYCoord));
            var var1E = 2;
            if (varA_distanceapart >= 4)
            {
                var1E = 8;
            }

            var varC_orbit = (short)((((short)Math.Abs(varA_distanceapart - 4)) << 3) / var1E);
            if (varC_orbit > 0x40)
            {
                varC_orbit = 0x40;
            }

            var var1A_diff = GetRelativeHeadingByMagnitude(projectile.ProjectileHeading, CalcedHeading_1C>>8, 0x20);

            var1A_diff = GetRelativeHeadingByMagnitude(var1A_diff, di >> 8, varC_orbit);

            projectile.ProjectileHeading = (ushort)var1A_diff;


            var pitch_varE = projectile.Projectile_Pitch;
            var var10_zDiff = 0xF + (caster.zpos - projectile.zpos);
            var var12 = 0x10 + (var10_zDiff / 8);

            var newPitch_var14 = (pitch_varE + var12) / 2;

            if ((Rng.r.Next(0x7FFF) & 0x3) == 0)
            {
                if (newPitch_var14 > 0xF)
                {
                    if (newPitch_var14 > 0x11)
                    {
                        newPitch_var14--;
                    }
                    else
                    {
                        newPitch_var14 = newPitch_var14 + Rng.r.Next(4) - 1;
                    }
                }
                else
                {
                    newPitch_var14++;
                }
            }

            projectile.Projectile_Pitch = (short)newPitch_var14;

            if (projectile.UnkBit_0X13_Bit0to6 < 0xF)
            {
                projectile.UnkBit_0X13_Bit0to6 = 0xF;
            }

            if (projectile.UnkBit_0XA_Bit7 == 0)
            {
                if (Rng.r.Next(32) == 1)
                {
                    if (projectile.npc_hp > 0)
                    {
                        projectile.npc_hp--;
                    }
                }
            }
            else
            {
                projectile.npc_hp = 0;
            }
        }


        static short SatelliteVectoring(int projectileX, int projectileY, int sourceX, int sourceY)
        {
            var xComponent = sourceX - projectileX;
            var yComponent = sourceY - projectileY;
            var si = (int)Math.Sqrt(Math.Pow(xComponent, 2) + Math.Pow(yComponent, 2));
            if (si == 0)
            {
                return 0;
            }
            else
            {
                var negXComponent = (int)((xComponent & 0xFF) * 0x7FFF);
                var negYComponent = (int)((yComponent & 0xFF) * 0x7FFF);

                negXComponent = (short)(negXComponent / si);
                negYComponent = (short)(negYComponent / si);
                return seg021_22FD_EFB_MaybeGetTangent((short)negXComponent, (short)negYComponent);
            }

        }


        static short seg021_22FD_EFB_MaybeGetTangent(short x, short y)
        {
            if ((x <= 23170) && (x >= -23042))
            {
                //seg021_22FD_BDE
                var cx = seg021_22FD_B78_maybetangent(x);
                if (y < 0)
                {
                    cx = (short)(cx - 0x8000);
                    cx = (short)-cx;
                }
                return cx;
            }
            else
            {
                //seg021_22FD_BEE:
                var dx = (short)((int)(x) >> 16);

                var cx = seg021_22FD_BA2_MaybeTangent(y);
                cx = (short)(cx ^ dx);
                cx = (short)(cx - dx);
                return cx;
            }
        }

        static short seg021_22FD_BA2_MaybeTangent(short bx)
        {
            var ax = (short)Math.Abs(bx);
            short dx_initial = 0;
            if (bx < 0)
            {
                dx_initial = -1;
            }
            var cx = (short)(ax & 0xFF);
            bx = (short)(ax >> 8);

            var bp = TangentTable[bx];
            ax = TangentTable[bx + 1];
            ax = (short)(ax - bp);
            ax = (short)(ax * cx);
            var dl = (short)((int)ax) >> 16;
            ax = (short)(ax >> 8);
            if (dl < 0)
            {
                ax = (short)-Math.Abs(ax);
            }
            else
            {
                ax = Math.Abs(ax);
            }

            cx = (short)(ax + bp);

            cx = (short)(cx ^ dx_initial);
            cx = (short)(cx - dx_initial);
            dx_initial = (short)(dx_initial & 0x8000);
            cx = (short)(cx + dx_initial);

            return cx;
        }

        static short seg021_22FD_B78_maybetangent(int ax)
        {
            Debug.Print("TODO seg021_22FD_B78()");
            return 0;
        }

        static short GetRelativeHeadingByMagnitude(int srcHeading, int dstHeading, int Magnitude)
        {
            Debug.Print("TODO GetRelativeHeadingByMagnitude()");
            return 0;
        }
    }//end class
}//end namespace