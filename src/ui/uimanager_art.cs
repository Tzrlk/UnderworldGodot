using Godot;
using System.Collections.Generic;

namespace Underworld
{
    public partial class uimanager : Node2D
    {
        public static GRLoader grCursors;
		public static GRLoader grButtons;
		public static GRLoader grObjects;
		public static GRLoader grLfti;
		public static GRLoader grOptBtns;
		public static GRLoader grConverse;
        public static GRLoader grBody;
		public static GRLoader grArmour_F;
		public static GRLoader grArmour_M;
		public static GRLoader grFlasks;
        public static GRLoader grOptbtn; //main menu buttons
        public static GRLoader grSpells; //spell icons
        public static GRLoader grCompass;
        public static GRLoader grGempt;
        public static GRLoader grPower;
        public static BytLoader bitmaps;  
        public static WeaponsLoader grWeapon;   
        public static GRLoader grInv;  

        public static Dictionary<string, CutsLoader> csCuts;

		public static void InitArt()
		{
            grCursors = new GRLoader(GRLoader.CURSORS_GR, GRLoader.GRShaderMode.None);
            grObjects = new GRLoader(GRLoader.OBJECTS_GR, GRLoader.GRShaderMode.UIShader);
            grObjects.UseRedChannel = true;
            instance.mousecursor.SetCursorToCursor(0);
            grButtons = new GRLoader(GRLoader.BUTTONS_GR, GRLoader.GRShaderMode.UIShader);
            grOptBtns = new GRLoader(GRLoader.OPTBTNS_GR, GRLoader.GRShaderMode.UIShader);
            grLfti = new GRLoader(GRLoader.LFTI_GR, GRLoader.GRShaderMode.UIShader);
            grArmour_F = new GRLoader(GRLoader.ARMOR_F_GR, GRLoader.GRShaderMode.UIShader);
            grArmour_M = new GRLoader(GRLoader.ARMOR_M_GR, GRLoader.GRShaderMode.UIShader);
			grConverse = new GRLoader(GRLoader.CONVERSE_GR, GRLoader.GRShaderMode.UIShader);
            grOptbtn = new  GRLoader(GRLoader.OPBTN_GR, GRLoader.GRShaderMode.UIShader);            
            grOptbtn.PaletteNo = 6;
            grSpells = new  GRLoader(GRLoader.SPELLS_GR, GRLoader.GRShaderMode.UIShader);
            grSpells.UseRedChannel = true;
			bitmaps = new BytLoader();
            grCompass= new GRLoader(GRLoader.COMPASS_GR, GRLoader.GRShaderMode.UIShader);
            grPower = new GRLoader(GRLoader.POWER_GR, GRLoader.GRShaderMode.UIShader);
            grInv = new GRLoader(GRLoader.INV_GR, GRLoader.GRShaderMode.UIShader);
            csCuts = new Dictionary<string, CutsLoader>();
            if (UWClass._RES==UWClass.GAME_UW2)
            {
                grGempt = new GRLoader(GRLoader.GEMPT_GR, GRLoader.GRShaderMode.UIShader);
                grGempt.PaletteNo = 3;
            }
            grWeapon = new WeaponsLoader(0);
  
		}
    }//end class
}//end namespace