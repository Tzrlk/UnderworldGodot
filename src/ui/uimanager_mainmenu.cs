using System.Collections;
using System.Diagnostics;
using System.IO;
using Godot;
using Peaky.Coroutines;

namespace Underworld
{
    public partial class uimanager : Node2D
    {
        [ExportGroup("MainMenu")]
        [Export]
        public Panel PanelMainMenu;
        [Export]
        public TextureRect MainMenuBG;

        [Export]
        public TextureRect[] MainMenuButtons = new TextureRect[4];

        [Export] public Label LoadingLabel;

        [Export] public Label[] SaveGamesNames = new Label[4];

        public static bool AtMainMenu;
        private void InitMainMenu()
        {
            
            if (UWClass._RES == UWClass.GAME_UW2)
            {
                MainMenuBG.Texture = bitmaps.LoadImageAt(5);
                //move main menu buttons
                MainMenuButtons[0].Size = new Vector2(436, 68);
                MainMenuButtons[0].Position = new Vector2(420, 308);
                MainMenuButtons[1].Position =  new Vector2(324, 392);
                
                MainMenuButtons[2].Size = new Vector2(668, 96);
                MainMenuButtons[2].Position = new Vector2(300, 484);

                MainMenuButtons[3].Size = new Vector2(580, 100);
                MainMenuButtons[3].Position = new Vector2(348, 584);
            }
            else
            {
                bitmaps.UseRedChannel = true;
                MainMenuBG.Texture = bitmaps.LoadImageAt(BytLoader.OPSCR_BYT);
                MainMenuBG.Material = bitmaps.GetMaterial(BytLoader.OPSCR_BYT);
                var img = bitmaps.LoadImageAt(BytLoader.OPSCR_BYT);
                Palette.CurrentPalette = 6;  
                bitmaps.UseRedChannel = false;             
            }

            //MainMenuBG.Material = bitmaps.GetMaterial(BytLoader.OPSCR_BYT);
            LoadingLabel.Text = "";
            TurnButtonsOff();
            ToggleMainMenuButtons(true);
            HideSaves();
            
            AtMainMenu = true;   
        }

        /// <summary>
        /// loads the off graphics for the main menu buttons
        /// </summary>
        private void TurnButtonsOff()
        {
            for (int i = 0; i < 4; i++)
            {
                MainMenuButtons[i].Texture = grOptbtn.LoadImageAt(i * 2);
            }
        }


        /// <summary>
        /// Shows or hides the 4 main menu options
        /// </summary>
        /// <param name="show"></param>
        private void ToggleMainMenuButtons(bool show)
        {
            for (int i = 0; i < 4; i++)
            {
                EnableDisable(MainMenuButtons[i], show);
            }
        }


        /// <summary>
        /// Hides the list of save games
        /// </summary>
        private void HideSaves()
        {
            for (int i = 0; i < 4; i++)
            {
                EnableDisable(SaveGamesNames[i], false);
            }
        }

        private void _on_introduction_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[0].Texture = grOptbtn.LoadImageAt(1);
        }


        private void _on_introduction_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[0].Texture = grOptbtn.LoadImageAt(0);
        }


        private void _on_create_character_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[1].Texture = grOptbtn.LoadImageAt(3);
        }


        private void _on_create_character_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[1].Texture = grOptbtn.LoadImageAt(2);
        }


        private void _on_acknowledgements_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[2].Texture = grOptbtn.LoadImageAt(5);
        }


        private void _on_acknowledgements_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[2].Texture = grOptbtn.LoadImageAt(4);
        }


        private void _on_journey_onwards_mouse_entered()
        {
            TurnButtonsOff();
            MainMenuButtons[3].Texture = grOptbtn.LoadImageAt(7);
        }


        private void _on_journey_onwards_mouse_exited()
        {
            TurnButtonsOff();
            MainMenuButtons[3].Texture = grOptbtn.LoadImageAt(6);
        }


        /// <summary>
        /// Load the save game specified in the config file
        /// </summary>
        /// <param name="event"></param>
        private void _on_journey_onwards_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                ToggleMainMenuButtons(false);
                ToggleSaves();
            }
        }

        private void ToggleSaves(bool show = true)
        {
            if (show)
            {
                for (int i = 1; i <= 4; i++)
                {
                    var path = Path.Combine(UWClass.BasePath, $"SAVE{i}", "DESC");
                    if (File.Exists(path))
                    {
                        var savename = File.ReadAllText(path);
                        EnableDisable(SaveGamesNames[i - 1], true);
                        SaveGamesNames[i - 1].Text = savename;
                    }
                    else
                    {
                        EnableDisable(SaveGamesNames[i - 1], false);
                    }
                }
            }
            else
            {
                for (int i = 1; i <= 4; i++)
                {
                     EnableDisable(SaveGamesNames[i - 1], false);
                }
            }
        }

        private IEnumerator ClearMainMenu()
        {
            ToggleMainMenuButtons(false);
            if (UWClass._RES == UWClass.GAME_UW2)
            {
                LoadingLabel.Text = GameStrings.GetString(1, 273);
            }
            else
            {
                LoadingLabel.Text = GameStrings.GetString(1, 257);
            }
            yield return 0;
        }

        public void JourneyOnwards(string folder)
        {
            playerdat.currentfolder = folder;
            playerdat.LoadPlayerDat(datafolder: folder);
            // //Common launch actions            
            UWTileMap.LoadTileMap(
                    newLevelNo: playerdat.dungeon_level - 1,
                    datafolder: folder,
                    newGameSession: true);
            
            //add player object to the map
            for (int i = 0;i <=0x1A; i++)
            {
                playerdat.playerObject.DataBuffer[playerdat.playerObject.PTR + i] = playerdat.pdat[playerdat.PlayerObjectStoragePTR + i];
            }
            playerdat.playerObject.item_id = 127;//make sure the object is an adventurer.
            playerdat.PlacePlayerInTile(playerdat.tileX, playerdat.tileY);            

            instance.InitViews();            
            SetPanelMode(0);
        }

        private void _on_create_character_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                InitChargenUI();
                // playerdat.currentfolder = "DATA";
                // _ = Coroutine.Run(
                //    ClearMainMenu()
                //    , main.instance);
                // JourneyOnwards("DATA");
            }
        }



        private void _on_save_1_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);
                JourneyOnwards("SAVE1");
            }
        }

        private void _on_save_2_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                JourneyOnwards("SAVE2");
            }
        }


        private void _on_save_3_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                JourneyOnwards("SAVE3");
            }
        }


        private void _on_save_4_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _ = Coroutine.Run(
                   ClearMainMenu()
                   , main.instance);

                JourneyOnwards("SAVE4");
            }
        }

        private void _on_introduction_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                cutsplayer.PlayCutscene(0, ReturnToMainMenu);
            }
        }


       public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey keyinput)
            {
                if (keyinput.Pressed & AtMainMenu)
                {
                    if(keyinput.Keycode == Key.Escape)
                    {//return to main menu
                        ToggleMainMenuButtons(true);
                        ToggleSaves(false);
                        EnableDisable(PanelChargen,false);
                        EnableDisable(PanelMainMenu,true);
                    }                    
                }
            }
        }


        /// <summary>
        /// Returns to main menu from the game (eg on player death)
        /// </summary>
        public static void ReturnToMainMenu()
        {
            Debug.Print("Return to main menu");
            //Still some weirdness with enabling the main menu again. eg palette switch in UW1
            EnableDisable(instance.PanelMainMenu, true);    
            instance.ToggleMainMenuButtons(true);      
            AtMainMenu = true;
            InGame = false;
            Node3D the_tiles = main.instance.GetNode<Node3D>("/root/Underworld/tilemap");
            if (the_tiles != null)
            {
                UWTileMap.DestroyTileMapAndContents(the_tiles);
            }
        }

    }//end class
}//end namespace