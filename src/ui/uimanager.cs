using System.Threading.Tasks;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using Underworld.config;
using Underworld.utility;

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

            // TODO: Can this be put somewhere even earlier in the application?
            Injection.Configure(services =>
            {
                services.AddSingleton<IGameConfig, GameConfig>();
            }).RunSynchronously();

            // Get settings instance from services.
            var config = Injection.GetService<IGameConfig>();

            // Populate game selection path text and trigger saving any changes
            // to the config.
            pathuw1.Text = config.PathUW1;
            pathuw1.TextChanged += delegate
            {
                if (!pathuw1.Text.Equals(config.PathUW1))
                {
                    config.PathUW1 = pathuw1.Text;
                    config.Save();
                }
            };
            pathuw2.Text = config.PathUW2;
            pathuw2.TextChanged += delegate
            {
                if (!pathuw1.Text.Equals(config.PathUW2))
                {
                    config.PathUW2 = pathuw2.Text;
                    config.Save();
                }
            };

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
