using Godot;
using System.Collections.Generic;

namespace Underworld
{
    public partial class uimanager : Node2D
    {
        public GRLoader grCursors;
		public GRLoader grButtons;
		public GRLoader grObjects;
		public GRLoader grLfti;
		public GRLoader grOptBtns;
		public GRLoader grConverse;
        public GRLoader grBody;
		public GRLoader grArmour_F;
		public GRLoader grArmour_M;
		public GRLoader grFlasks;
        public GRLoader grOptbtn; //main menu buttons
        public GRLoader grSpells; //spell icons
        public GRLoader grCompass;
        public GRLoader grGempt;
        public GRLoader grPower;
        public BytLoader bitmaps;  
        public WeaponsLoader grWeapon;   
        public GRLoader grInv;  

        public Dictionary<string, CutsLoader> csCuts;

		public void InitArt()
		{
            grCursors = new GRLoader(GRLoader.CURSORS_GR, GRLoader.GRShaderMode.None);
            grObjects = new GRLoader(GRLoader.OBJECTS_GR, GRLoader.GRShaderMode.UIShader)
            {
                UseRedChannel = true
            };
            instance.mousecursor.SetCursorToCursor(0);
            grButtons = new GRLoader(GRLoader.BUTTONS_GR, GRLoader.GRShaderMode.UIShader);
            grOptBtns = new GRLoader(GRLoader.OPTBTNS_GR, GRLoader.GRShaderMode.UIShader);
            grLfti = new GRLoader(GRLoader.LFTI_GR, GRLoader.GRShaderMode.UIShader);
            grArmour_F = new GRLoader(GRLoader.ARMOR_F_GR, GRLoader.GRShaderMode.UIShader);
            grArmour_M = new GRLoader(GRLoader.ARMOR_M_GR, GRLoader.GRShaderMode.UIShader);
			grConverse = new GRLoader(GRLoader.CONVERSE_GR, GRLoader.GRShaderMode.UIShader);
            grOptbtn = new GRLoader(GRLoader.OPBTN_GR, GRLoader.GRShaderMode.UIShader)
            {
                PaletteNo = 6
            };

            grSpells = new GRLoader(GRLoader.SPELLS_GR, GRLoader.GRShaderMode.UIShader)
            {
                UseRedChannel = true
            };
            bitmaps = new BytLoader();
            grCompass= new GRLoader(GRLoader.COMPASS_GR, GRLoader.GRShaderMode.UIShader);
            grPower = new GRLoader(GRLoader.POWER_GR, GRLoader.GRShaderMode.UIShader);
            grInv = new GRLoader(GRLoader.INV_GR, GRLoader.GRShaderMode.UIShader);
            csCuts = [];
            if (UWClass._RES==UWClass.GAME_UW2)
            {
                grGempt = new GRLoader(GRLoader.GEMPT_GR, GRLoader.GRShaderMode.UIShader)
                {
                    PaletteNo = 3
                };
            }
            grWeapon = new WeaponsLoader(0);
  
		}
    }//end class
}//end namespace