using Godot;

namespace Underworld
{

    public partial class uimanager : Node2D
    {
        /// <summary>
        /// Identifies what mode of object usage is currently being used.
        /// 0 = default interactions
        /// 1 = Player has selected an object and it has changed to prompt to be used on something. Eg rockhammer, doorkey, oil flask
        /// 2 = Player is casting a spell and the spell needs to be clicked on an object.
        /// </summary>
        public static int UsageMode
        {
            get
            {
                if (useon.CurrentItemBeingUsed != null)
                {
                    return 1;
                }
                if (SpellCasting.currentSpell != null)
                {
                    return 2;
                }
                return 0;
            }
        }

        public enum InteractionModes
        {
            ModeOptions = 0,
            ModeTalk = 1,
            ModePickup = 2,
            ModeLook = 3,
            ModeAttack = 4,
            ModeUse = 5
        };

        public static InteractionModes InteractionMode = InteractionModes.ModeUse;
        [ExportGroup("InteractionModes")]
        //Array to store the interaction mode mo
        [Export] public Godot.TextureRect[] InteractionButtonsUW1 = new Godot.TextureRect[6];
        [Export] public Godot.TextureRect[] InteractionButtonsUW2 = new Godot.TextureRect[6];

        private ImageTexture[] UW2InteractionBtnsOff;
        private ImageTexture[] UW2InteractionButtonsOn;

        private void InitInteraction()
        {

            if (UWClass._RES == UWClass.GAME_UW2)
            {//load interaction button art for uw2. the images need to be cropped out of a master image.

                UW2InteractionBtnsOff = new ImageTexture[6];
                UW2InteractionButtonsOn = new ImageTexture[6];

                var Off = grOptBtns.LoadImageAt(0).GetImage();
                var On = grOptBtns.LoadImageAt(1).GetImage();
                UW2InteractionBtnsOff[4] = ArtLoader.CropImage(Off, new Rect2I(0, 0, 25, 14)); //attack button off
                UW2InteractionButtonsOn[4] = ArtLoader.CropImage(On, new Rect2I(0, 0, 25, 14)); //attack button on

                UW2InteractionBtnsOff[5] = ArtLoader.CropImage(Off, new Rect2I(26, 0, 25, 14)); //use button off
                UW2InteractionButtonsOn[5] = ArtLoader.CropImage(On, new Rect2I(26, 0, 25, 14)); //use button on

                UW2InteractionBtnsOff[2] = ArtLoader.CropImage(Off, new Rect2I(52, 0, 25, 14)); //pickup button off
                UW2InteractionButtonsOn[2] = ArtLoader.CropImage(On, new Rect2I(52, 0, 25, 14)); //pickup button on

                UW2InteractionBtnsOff[1] = ArtLoader.CropImage(Off, new Rect2I(0, 15, 25, 14)); //talk button off
                UW2InteractionButtonsOn[1] = ArtLoader.CropImage(On, new Rect2I(0, 15, 25, 14)); //talk button on

                UW2InteractionBtnsOff[3] = ArtLoader.CropImage(Off, new Rect2I(26, 15, 25, 14)); //look button off
                UW2InteractionButtonsOn[3] = ArtLoader.CropImage(On, new Rect2I(26, 15, 25, 14)); //look button on

                UW2InteractionBtnsOff[0] = ArtLoader.CropImage(Off, new Rect2I(52, 15, 25, 14)); //options button off
                UW2InteractionButtonsOn[0] = ArtLoader.CropImage(On, new Rect2I(52, 15, 25, 14)); //option button on
            }


            switch (UWClass._RES)
            {
                case UWClass.GAME_UW2:
                    for (int i = 0; i <= InteractionButtonsUW2.GetUpperBound(0); i++)
                    {
                        if (i != (int)InteractionMode)
                        {
                            InteractionButtonsUW2[i].Texture = UW2InteractionBtnsOff[i];
                        }
                        else
                        {
                            InteractionButtonsUW2[i].Texture = UW2InteractionButtonsOn[i];
                        }
                    }
                    break;
                default:
                    for (int i = 0; i <= InteractionButtonsUW1.GetUpperBound(0); i++)
                    {
                        if (i != (int)InteractionMode)
                        {
                            InteractionButtonsUW1[i].Texture = grLfti.LoadImageAt(i * 2, false); //off button
                        }
                        else
                        {
                            InteractionButtonsUW1[i].Texture = grLfti.LoadImageAt(i * 2 + 1, false);//on button
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles interacting with objects in the world
        /// </summary>
        /// <param name="index"></param>
        public static void InteractWithObjectCollider(int index, bool LeftClick)
        {
            if (playerdat.ParalyseTimer>0)
            {
                return;//stop interaction while player is paralysed
            }
            switch (InteractionMode)
            {
                case InteractionModes.ModeTalk:
                    talk.Talk(
                        ObjectUsed: UWTileMap.current_tilemap.LevelObjects[index],
                        WorldObject: true);
                    break;
                case InteractionModes.ModeLook:
                    //Do a look interaction with the object
                    look.LookAt(
                        index: index,
                        objList: UWTileMap.current_tilemap.LevelObjects,
                        WorldObject: true);
                    break;
                case InteractionModes.ModeUse:
                    //do a use interaction with the object.
                    use.Use(
                        ObjectUsed: UWTileMap.current_tilemap.LevelObjects[index],
                        UsingObjectOrCharacter: playerdat.playerObject,
                        objList: UWTileMap.current_tilemap.LevelObjects,
                        WorldObject: true);
                    break;
                case InteractionModes.ModePickup:
                    pickup.PickUp(
                        index: index,
                        objList: UWTileMap.current_tilemap.LevelObjects,
                        WorldObject: true);
                    break;
            }
        }

        public static void InteractionModeToggle(InteractionModes index)
        {
            PreviousInteractionMode = InteractionMode;
            if (UWClass._RES == UWClass.GAME_UW2)
            {
                for (int i = 0; i <= instance.InteractionButtonsUW2.GetUpperBound(0); i++)
                {
                    if (i == (int)(index))
                    {
                        InteractionMode = index;
                        instance.InteractionButtonsUW2[i].Texture = instance.UW2InteractionButtonsOn[i];
                    }
                    else
                    {
                        instance.InteractionButtonsUW2[i].Texture = instance.UW2InteractionBtnsOff[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i <= instance.InteractionButtonsUW1.GetUpperBound(0); i++)
                {
                    if (i == (int)(index))
                    {
                        InteractionMode = index;
                        instance.InteractionButtonsUW1[i].Texture = grLfti.LoadImageAt(i * 2 + 1, false);//on button
                    }
                    else
                    {
                        instance.InteractionButtonsUW1[i].Texture = grLfti.LoadImageAt(i * 2, false);//off button
                    }
                }
            }

            ///special handling for some modes
            switch (index)
            {
                case InteractionModes.ModeOptions:
                    {
                        if (UWClass._RES == UWClass.GAME_UW2)
                        {

                        }
                        else
                        {
                            InteractionModeShowHide(false);//hide the interaction buttons.  
                        }
                        //turn off mouselook
                        Input.MouseMode = Input.MouseModeEnum.Hidden;
                        main.gamecam.Set("MOUSELOOK", false);

                        ReturnToTopOptionsMenu();
                        main.gamecam.Set("MOVE", false);
                        break;
                    }
                case InteractionModes.ModeAttack:
                    {
                        PreviousWeaponAnimation = -1; //force redraw.
                        playerdat.play_drawn = 1;//draw the weapon
                        var obj = playerdat.PrimaryHandObject;
                        switch (combat.isWeapon(obj))
                        {
                            case 1://melee weapon
                            case 2: //rangedweapon
                                combat.currentweapon = obj; break;
                            default:
                                combat.currentweapon = null; break;
                        }
                        break;
                    }
                default:
                    playerdat.play_drawn = 0; //ensure weapon is not drawn.
                    break;

            }
        }


        public static void InteractionModeShowHide(bool state)
        {
            if (UWClass._RES == UWClass.GAME_UW2)
            {

                for (int i = 0; i <= instance.InteractionButtonsUW2.GetUpperBound(0); i++)
                {
                    EnableDisable(instance.InteractionButtonsUW2[i], state);
                }
            }
            else
            {
                for (int i = 0; i <= instance.InteractionButtonsUW1.GetUpperBound(0); i++)
                {
                    EnableDisable(instance.InteractionButtonsUW1[i], state);
                }
            }
        }


        private void _on_interactionoptions_button(InputEvent @event, long extra_arg_0)
        {
            if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                if (!main.blockmouseinput)
                {
                    if (playerdat.ObjectInHand != -1)
                    {
                        return;
                    }
                    if (UsageMode != 0)
                    {
                        return;
                    }
                    InteractionModeToggle((InteractionModes)extra_arg_0);
                }
            }
        }


    }//end class    
}//end namespace