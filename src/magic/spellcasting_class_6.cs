using System.Diagnostics;

namespace Underworld
{
    public partial class SpellCasting : UWClass
    {
        static int FlameWindGlobal = 0;
        public static void CastClass6_SpellsAroundPlayer(int minorclass)
        {        
            CallBacks.AreaEffectCallBack methodtorun = null;    
            //These values come from a table in the exe
            int tileRadius = 0;
            int distanceFromCaster = 0;
            int rngProbablity=0;
            if (_RES == GAME_UW2)
            {
                switch (minorclass & 0x3F)
                {
                    case 0: //reveal
                        Debug.Print("reveal");
                        break;
                    case 1: //sheetlightning
                        Debug.Print("Sheetlighning");
                        break;
                    case 2://maybe mass confuse
                        Debug.Print("mass confuse?");
                        break;
                    case 4: //flame wind
                        Debug.Print("Flamewind");
                        FlameWindGlobal = 0;
                        methodtorun = FlameWind;
                        rngProbablity = 0xA; distanceFromCaster = 4; tileRadius = 2;
                        break;
                    case 5://repel undead
                        Debug.Print ("Repel undead");
                        break;
                    case 6://shockwave
                        Debug.Print("Shockwave");
                        break;
                    case 7://frost
                        Debug.Print ("Frost");
                        break;
                }
            }
            else
            {
                Debug.Print($"{minorclass & 0x3F}");
                switch (minorclass & 0x3F)
                {
                    case 1: //reveal
                        Debug.Print("reveal");
                        break;
                    case 2: //sheetlightning
                        Debug.Print("Sheetlighning");
                        break;
                    case 3://maybe mass confuse
                       Debug.Print("mass confuse?");
                       break;
                    case 4: //flame wind
                        Debug.Print("Flamewind");
                        break;
                }
            }
            if (methodtorun!=null)
            {
                CallBacks.RunCodeOnTargetsAroundObject(methodtorun,0, rngProbablity, minorclass&0xC0, distanceFromCaster, tileRadius);
            }            
        }

        public static bool FlameWind(int x, int y, uwObject critter, TileInfo tile, int srcIndex)
        {
            var di = x<<6 + y;
            var var6 = FlameWindGlobal >>4;
            var si = FlameWindGlobal & 0xF;

            if ((var6 != di) && (si<5))
            {//likely a check that the spell is cast once per tile?
                if (critter.majorclass != 1)
                {
                    if (Rng.r.Next(3) !=0)
                    {
                        return false;
                    }
                }

                animo.SpawnAnimoInTile(2,critter.xpos,critter.ypos, critter.zpos, x, y);
                damage.DamageObjectsInTile(x, y, 1, 1);
                if (critter.majorclass == 1)
                {
                    si++;
                }
                FlameWindGlobal = (di<<4) | (si & 0xF);
            }
            
            return true;
        }
    }//end class
}//end namespace