using Godot;
using System.Diagnostics;


namespace Underworld
{
	public partial class uimanager : Node2D
	{
		public static uimanager instance;

		public static bool InGame = false;
		[ExportGroup("Placeholders")]
		[Export] public TextureRect placeholderuw1;
		[Export] public TextureRect placeholderuw2;


		public override void _Ready()
		{			
			//Debug.Print("Uimanager about to set instance to this");
			instance = this;
			
			// if (uwsettings.instance==null)
			// {				
			// 	uwsettings.LoadSettings();
			// }			
			uimanager.instance.pathuw1.Text = uwsettings.instance.pathuw1;
			Debug.Print(uwsettings.instance.pathuw1);
			uimanager.instance.pathuw2.Text = uwsettings.instance.pathuw2;
			Debug.Print(uwsettings.instance.pathuw2);
			Debug.Print("Settings loaded");
		}

		public void InitUI()
		{
			InitArt();
			InitMainMenu();
			InitFlasks();
			InitCoversation();
			InitPanels();
			InitPaperdoll();
			InitGameOptions();
			InitInteraction();
			InitViews();
			InitMessageScrolls();
			InitCuts();
			InitSpellIcons();
			InitCompass();
			InitAutomap();
			InitPower();
			InitStats();
			InitWeaponAnimation();

			AutomapBG.Texture = bitmaps.LoadImageAt(BytLoader.BLNKMAP_BYT);
			EnableDisable(AutomapPanel,false);

			mousecursor.InitCursor();

			EnableDisable(placeholderuw1, false);
			EnableDisable(placeholderuw2, false);

			EnableDisable(uw1UI, UWClass._RES == UWClass.GAME_UW1);
			EnableDisable(uw2UI, UWClass._RES != UWClass.GAME_UW1);  
			EnableDisable(PanelMainMenu,true);          
		}
		

		public override void _Process(double delta)
		{
			_ProcessPanels(delta);
			_ProcessWeaponAnims(delta);
		}

		public static void EnableDisable(Control ctrl, bool state)
		{
			if (ctrl != null)
			{
				ctrl.Visible = state;
			}
		}

		public static void EnableDisable(CanvasLayer ctrl, bool state)
		{
			if (ctrl != null)
			{
				ctrl.Visible = state;
			}
		}
	} //end class
}   //end namespace
