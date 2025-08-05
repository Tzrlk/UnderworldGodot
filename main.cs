using Godot;
using System;
using System.Diagnostics;
using Underworld;


/// <summary>
/// Node to initialise the game
/// </summary>
public partial class main : Node3D
{

	static bool EnablePositionDebug = false;
	/// <summary>
	/// Blocks input for certain modes
	/// </summary>
	public static bool blockmouseinput
	{
		get
		{
			return
			 ConversationVM.InConversation
			 ||
			 uimanager.InAutomap
			 ||
			 MessageDisplay.WaitingForTypedInput
			 ||
			 MessageDisplay.WaitingForMore
			 ||
			 MessageDisplay.WaitingForYesOrNo
			 ||
			 musicalinstrument.PlayingInstrument
			 ||
			 uimanager.InteractionMode == uimanager.InteractionModes.ModeOptions
			 ;

			; //TODO and other menu modes that will stop input
		}
	}
	public static main instance;

	// Called when the node enters the scene tree for the first time.
	[Export] public Camera3D cam;
	public static Camera3D gamecam; //static ref to the above camera
	[Export] public AudioStreamPlayer audioplayer;
	[Export] public RichTextLabel lblPositionDebug;
	//[Export] public uimanager uwUI;

	[Export] public SubViewport secondarycameras;

	double gameRefreshTimer = 0f;

	double cycletime = 0;

	public static bool DoRedraw = false;


	//DOS INT8 (PIT) timer interupt. updates 18.2 times a second.
	double Pit = 0f;
	uint PitTimer = 0;
	uint LastPitTimer = 0;
	static byte EasyMoveFrameIncrement = 0;

	static byte ThisFrameDelta = 0;
	static byte PreviousFrameDelta = 0;

	public override void _Ready()
	{
		instance = this;
		gamecam = cam;

		//uimanager.instance = uwUI;	
		if (uwsettings.instance != null)
		{
			GetTree().DebugCollisionsHint = uwsettings.instance.showcolliders;
		}

		// var exe = System.IO.File.ReadAllBytes("C:\\Games\\UW2\\uw2.exe");
		// int addr_ptr = 0x63FC2;
		// for (long x = 0; x <= 320; x++)
		// {
		// 	Debug.Print($"{x}={(short)Loader.getAt(exe, addr_ptr, 16)}");
		// 	addr_ptr += 2;
		// }
	}

	public static void StartGame()
	{
		if (gamecam == null)
		{
			if (instance.cam == null)
			{
				Debug.Print("Main Cam instance is null. trying to find it's node");
				instance.cam = (Camera3D)instance.GetNode("/root/Underworld/WorldViewContainer/SubViewport/Camera3D");
			}
			gamecam = instance.cam;
			if (gamecam == null)
			{
				Debug.Print("Gamecam is still null!");
			}
		}
		if (uimanager.instance == null)
		{
			Debug.Print("UI Manager is null");
			//UI/uiManager
			uimanager.instance = (uimanager)instance.GetNode("/root/Underworld/UI/uiManager");
			if (uimanager.instance == null)
			{
				Debug.Print("UIManager is still null!!");
			}
		}
		gamecam.Fov = Math.Max(50, uwsettings.instance.FOV);
		uimanager.EnableDisable(instance.lblPositionDebug, EnablePositionDebug);
		ObjectCreator.grObjects = new GRLoader(GRLoader.OBJECTS_GR, GRLoader.GRShaderMode.BillboardSpriteShader);
		ObjectCreator.grObjects.UseRedChannel = true;
		ObjectCreator.grObjects.UseCropping = true;
		Palette.CurrentPalette = 0;
		uimanager.instance.InitUI();
		uimanager.AddToMessageScroll(GameStrings.GetString(1, 13));//welcome message
	}


	/// <summary>
	/// Draws a debug marker sprite on game load to show where the character is positioned
	/// </summary>
	/// <param name="gr"></param>
	public static void DrawPlayerPositionSprite(GRLoader gr)
	{
		int spriteNo = 127;
		var a_sprite = new MeshInstance3D(); //new Sprite3D();
		a_sprite.Name = "player";
		a_sprite.Mesh = new QuadMesh();
		Vector2 NewSize;
		var img = gr.LoadImageAt(spriteNo);
		if (img != null)
		{
			a_sprite.Mesh.SurfaceSetMaterial(0, gr.GetMaterial(spriteNo));
			NewSize = new Vector2(
					ArtLoader.SpriteScale * img.GetWidth(),
					ArtLoader.SpriteScale * img.GetHeight()
					);
			a_sprite.Mesh.Set("size", NewSize);
			Node3D worldobjects = instance.GetNode<Node3D>("/root/Underworld/worldobjects");
			worldobjects.AddChild(a_sprite);
			a_sprite.Position = gamecam.Position;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if ((uimanager.InGame) || (uimanager.AtMainMenu))
		{
			cycletime += delta;
			if (cycletime > 0.2)
			{
				cycletime = 0;
				PaletteLoader.UpdatePaletteCycles();
			}
		}

		//DOS interupt 8
		Pit += (delta*5);  //seem smoother
		//
		if (Pit >= 0.054945) // DOS PIT Timer interupt 8 is 18.2 times a second
		{
			PitTimer +=  (uint)(Pit / 0.054945);
			Pit = 0;
			//Debug.Print($"{Pit}, {PitTimer}, {delta}");
		}


		if ((uimanager.InGame) && (!blockmouseinput))
		{
			

			byte AnimationFrameDeltaIncrement = 0;

			var ClockIncrement = PitTimer - LastPitTimer;
			if ((ClockIncrement < 0) || (ClockIncrement > 0x40))
			{
				ClockIncrement = 0x40;
				AnimationFrameDeltaIncrement = 1;
				EasyMoveFrameIncrement += 4;
			}
			else
			{
				//Debug.Print($"{PitTimer - LastPitTimer}");
				EasyMoveFrameIncrement += (byte)((PitTimer >> 4) - (LastPitTimer >> 4));  //every 16 pits?
				AnimationFrameDeltaIncrement = (byte)((PitTimer >> 6) - (LastPitTimer >> 6));//every 63 pits?


				//HACK the above appears to be what should be happening in vanilla code but is very slow to process, but the below gives the appearance of normal movement but may cause frame rate issues. 
				//This whole section will need to be fixed in the future.
				//A clock increment of 1 will cause strafing to fail when moving to the east!
				// EasyMoveFrameIncrement = 1;
				// AnimationFrameDeltaIncrement = 1;
				//ClockIncrement = 0xF;
				//ClockIncrement = ClockIncrement * 4;
			}

			if (ClockIncrement != 0)
			{
				ClockIncrement = Math.Max(ClockIncrement, 2);
				ProcessMotionInputs();
				if (AnimationFrameDeltaIncrement != 0)
				{
					//if animations enabled
					if ((uimanager.InGame) && (!blockmouseinput))
					{
						AnimationOverlay.UpdateAnimationOverlays();
						timers.RunTimerTriggers(AnimationFrameDeltaIncrement);
					}
				}
				playerdat.ClockValue += (int)ClockIncrement;
				LastPitTimer = PitTimer;
				AnimationFrameDeltaIncrement = EasyMoveFrameIncrement;
				if (playerdat.SpeedEnchantment)
				{
					EasyMoveFrameIncrement = (byte)((AnimationFrameDeltaIncrement >> 1) & 0x1);
				}
				else
				{
					EasyMoveFrameIncrement = 0;
				}

				GameObjectLoop((byte)ClockIncrement, AnimationFrameDeltaIncrement, false);
			}

		}


		//Other updates
		if (uimanager.InGame)
		{
			RefreshWorldState();//handles teleports, tile redraws
			uimanager.UpdateCompass();
			combat.CombatInputHandler(delta);
			playerdat.PlayerTimedLoop(delta);

			int tileX = -(int)(cam.Position.X / 1.2f);
			int tileY = (int)(cam.Position.Z / 1.2f);
			tileX = Math.Max(Math.Min(tileX, 63), 0);
			tileY = Math.Max(Math.Min(tileY, 63), 0);
			int xposvecto = -(int)(((cam.Position.X % 1.2f) / 1.2f) * 8);
			int yposvecto = (int)(((cam.Position.Z % 1.2f) / 1.2f) * 8);
			int newzpos = (int)(((((cam.Position.Y) * 100) / 32f) / 15f) * 128f) - commonObjDat.height(127);
			if (EnablePositionDebug)
			{
				var fps = Engine.GetFramesPerSecond();
				lblPositionDebug.Text = $"FPS:{fps} Time:{playerdat.game_time}\nL:{playerdat.dungeon_level} X:{tileX} Y:{tileY}\n{uimanager.instance.uwsubviewport.GetMousePosition()}\n {motion.playerMotionParams.x_0} {motion.playerMotionParams.y_2} {motion.playerMotionParams.z_4}";
			}

			if ((MessageDisplay.WaitingForTypedInput) || (MessageDisplay.WaitingForYesOrNo))
			{
				if (!uimanager.instance.TypedInput.HasFocus())
				{
					uimanager.instance.TypedInput.GrabFocus();
				}
				uimanager.instance.scroll.UpdateMessageDisplay();
			}
		}



		return;

		if (uimanager.InGame)
		{
			RefreshWorldState();//handles teleports, tile redraws

			int tileX = -(int)(cam.Position.X / 1.2f);
			int tileY = (int)(cam.Position.Z / 1.2f);
			tileX = Math.Max(Math.Min(tileX, 63), 0);
			tileY = Math.Max(Math.Min(tileY, 63), 0);
			int xposvecto = -(int)(((cam.Position.X % 1.2f) / 1.2f) * 8);
			int yposvecto = (int)(((cam.Position.Z % 1.2f) / 1.2f) * 8);
			int newzpos = (int)(((((cam.Position.Y) * 100) / 32f) / 15f) * 128f) - commonObjDat.height(127);

			// newzpos = Math.Max(Math.Min(newzpos, 127), 0);
			// var tmp = cam.Rotation;
			// tmp.Y = (float)(tmp.Y - Math.PI);
			// playerdat.heading_major = (int)Math.Round(-(tmp.Y * 127) / Math.PI);//placeholder track these values for projectile calcs.
			// playerdat.playerObject.heading = (short)((playerdat.headingMinor >> 0xD) & 0x7);
			// playerdat.playerObject.npc_heading = (short)((playerdat.headingMinor>>8) & 0x1F);
			uimanager.UpdateCompass();
			combat.CombatInputHandler(delta);
			playerdat.PlayerTimedLoop(delta);
			if (EnablePositionDebug)
			{
				var fps = Engine.GetFramesPerSecond();
				lblPositionDebug.Text = $"FPS:{fps} Time:{playerdat.game_time}\nL:{playerdat.dungeon_level} X:{tileX} Y:{tileY}\n{uimanager.instance.uwsubviewport.GetMousePosition()}\n{cam.Rotation} {playerdat.heading_major} {(playerdat.heading_major >> 4) % 4} {xposvecto} {yposvecto} {newzpos}";
			}


			// if (UWTileMap.ValidTile(tileX, tileY))//((tileX < 64) && (tileX >= 0) && (tileY < 64) && (tileY >= 0))
			// {
			// 	if ((playerdat.tileX != tileX) || (playerdat.tileY != tileY))
			// 	{

			// 		var tileExited = UWTileMap.current_tilemap.Tiles[playerdat.tileX, playerdat.tileY];
			// 		if (UWClass._RES == UWClass.GAME_UW2)
			// 		{
			// 			//find exit triggers.
			// 			if (tileExited.indexObjectList != 0)
			// 			{
			// 				var next = tileExited.indexObjectList;
			// 				while (next != 0)
			// 				{
			// 					var nextObj = UWTileMap.current_tilemap.LevelObjects[next];
			// 					trigger.RunTrigger(character: 1,
			// 							ObjectUsed: nextObj,
			// 							TriggerObject: nextObj,
			// 							triggerType: (int)triggerObjectDat.triggertypes.EXIT,
			// 							objList: UWTileMap.current_tilemap.LevelObjects);
			// 					next = nextObj.next;
			// 				}
			// 			}
			// 		}
			// 		//player has changed tiles. move them to their new tile
			// 		var oldTileX = playerdat.tileX; var oldTileY = playerdat.tileY;

			// 		playerdat.tileX = Math.Min(Math.Max(tileX, 0), 63);
			// 		playerdat.tileY = Math.Min(Math.Max(tileY, 0), 63);
			// 		playerdat.PlacePlayerInTile(playerdat.tileX, playerdat.tileY, oldTileX, oldTileY);
			// 		playerdat.xpos = Math.Min(Math.Max(0, xposvecto), 8);
			// 		playerdat.ypos = Math.Min(Math.Max(0, yposvecto), 8);
			// 		playerdat.zpos = newzpos;
			// 		// if( UWTileMap.current_tilemap.Tiles[playerdat.tileX, playerdat.tileY].tileType != 0)
			// 		// {//TMP put player Zpos at tile height
			// 		// 	playerdat.zpos = UWTileMap.current_tilemap.Tiles[playerdat.tileX, playerdat.tileY].floorHeight<<3;
			// 		// }


			// 		//tmp update the player object to keep in sync with other values
			// 		playerdat.playerObject.item_id = 127;
			// 		playerdat.playerObject.xpos = (short)playerdat.xpos;
			// 		playerdat.playerObject.ypos = (short)playerdat.ypos;
			// 		playerdat.playerObject.zpos = (short)playerdat.zpos;
			// 		playerdat.playerObject.tileX = playerdat.tileX;
			// 		playerdat.playerObject.npc_xhome = (short)tileX;
			// 		playerdat.playerObject.tileY = playerdat.tileY;
			// 		playerdat.playerObject.npc_yhome = (short)tileY;
			// 		var tileEntered = UWTileMap.current_tilemap.Tiles[playerdat.tileX, playerdat.tileY];
			// 		playerdat.PlayerStatusUpdate();
			// 		if (UWClass._RES == UWClass.GAME_UW2)
			// 		{
			// 			//find enter triggers.
			// 			//find exit triggers.
			// 			if (tileEntered.indexObjectList != 0)
			// 			{
			// 				var next = tileEntered.indexObjectList;
			// 				while (next != 0)
			// 				{
			// 					var nextObj = UWTileMap.current_tilemap.LevelObjects[next];
			// 					trigger.RunTrigger(character: 1,
			// 							ObjectUsed: nextObj,
			// 							TriggerObject: nextObj,
			// 							triggerType: (int)triggerObjectDat.triggertypes.ENTER,
			// 							objList: UWTileMap.current_tilemap.LevelObjects);

			// 					next = nextObj.next; //risk of infinite loop where while player motion is being reworked

			// 				}
			// 			}
			// 			//Debug.Print($"{playerdat.zpos} vs {(tileEntered.floorHeight << 3)}");
			// 			// If grounded try and find pressure triggers. for the moment ground is just zpos less than floorheight.
			// 			if (playerdat.zpos <= (tileEntered.floorHeight << 3))//Janky temp implementation. player must be on/below the height before changing tiles.
			// 			{
			// 				if (tileEntered.indexObjectList != 0)
			// 				{
			// 					var next = tileEntered.indexObjectList;
			// 					while (next != 0)
			// 					{
			// 						var nextObj = UWTileMap.current_tilemap.LevelObjects[next];
			// 						trigger.RunTrigger(character: 1,
			// 								ObjectUsed: nextObj,
			// 								TriggerObject: nextObj,
			// 								triggerType: (int)triggerObjectDat.triggertypes.PRESSURE,
			// 								objList: UWTileMap.current_tilemap.LevelObjects);
			// 						next = nextObj.next;
			// 					}
			// 				}
			// 			}
			// 		}
			// 	}
			// }
			// if (playerdat.playerObject != null)
			// {//temp crash fix
			// 	if (
			// 		(playerdat.playerObject.tileX != playerdat.tileX)
			// 		||
			// 		(playerdat.playerObject.tileY != playerdat.tileY)
			// 	)
			// 	{
			// 		playerdat.playerObject.tileX = playerdat.tileX;
			// 		playerdat.playerObject.tileY = playerdat.tileY;
			// 	}
			// }

			gameRefreshTimer += delta;
			if (gameRefreshTimer >= 0.1)
			{
				gameRefreshTimer = 0;
				if (!blockmouseinput)
				{
					//Player motion.
					if (
						(motion.MotionInputPressed != 0)
						||
						(motion.playerMotionParams.unk_14 != 0)
						||
						(motion.playerMotionParams.unk_a_pitch != 0)
						||
						(motion.playerMotionParams.unk_10_Z != 0)
						||
						(motion.playerMotionParams.unk_e_Y != 0)
						||
						(motion.playerMotionParams.unk_c_X != 0)
						||
						(motion.Examine_dseg_D3 != 0)
					)
					{
						//when any forced movement or player input is not 0
						motion.PlayerMotion(0xF); //todo confirm increments
					}


					for (int i = 0; i < UWTileMap.current_tilemap.NoOfActiveMobiles; i++)
					{
						var index = UWTileMap.current_tilemap.GetActiveMobileAtIndex(i);
						if ((index > 1) && (index < 256))
						{
							var obj = UWTileMap.current_tilemap.LevelObjects[index];
							if (obj.majorclass == 1)
							{
								if (UWTileMap.ValidTile(obj.tileX, obj.tileY))
								{
									//This is an NPC on the map	
									var n = (npc)obj.instance;

									npc.NPCInitialProcess(obj);
									if (n != null)
									{
										if (obj.instance != null)
										{
											var CalcedFacing = npc.CalculateFacingAngleToNPC(obj);
											n.SetAnimSprite(obj.npc_animation, obj.AnimationFrame, CalcedFacing);
										}
									}
								}
								else
								{
									Debug.Print($"{obj.a_name} {obj.index} is off map");
								}
							}
							else
							{
								if (motion.MotionSingleStepEnabled)
								{
									//This is a projectile
									motion.MotionProcessing(obj);
								}
							}
						}
					}
					//motion.MotionSingleStepEnabled = false;

					AnimationOverlay.UpdateAnimationOverlays();
					timers.RunTimerTriggers(1);
				}
			}

			if ((MessageDisplay.WaitingForTypedInput) || (MessageDisplay.WaitingForYesOrNo))
			{
				if (!uimanager.instance.TypedInput.HasFocus())
				{
					uimanager.instance.TypedInput.GrabFocus();
				}
				uimanager.instance.scroll.UpdateMessageDisplay();
			}
		}
	}


	static void GameObjectLoop(byte ClockIncrement, byte AnimationFrameDelta, bool EasyMove)
	{
		motion.RelatedToSwimDmg_dseg_67d6_33CE = 0;
		motion.RelatedToClockIncrement_67d6_742 += ClockIncrement;
		motion.dseg_67d6_33c6 = false;

		if (motion.MotionInputPressed == 0)
		{
			if (playerdat.RoamingSightEnchantment == false)
			{
				//ProcessMotionInputs
			}
		}
		if (
			(motion.MotionInputPressed != 0)
			||
			(motion.playerMotionParams.unk_14 != 0)
			||
			(motion.playerMotionParams.unk_a_pitch != 0)
			||
			(motion.playerMotionParams.unk_10_Z != 0)
			||
			(motion.playerMotionParams.unk_e_Y != 0)
			||
			(motion.playerMotionParams.unk_c_X != 0)
			||
			(motion.Examine_dseg_D3 != 0)
		)
		{
			if (EasyMove == false)
			{
				//when any forced movement or player input is not 0
				motion.PlayerMotion(ClockIncrement); //todo confirm increments
			}

		}
		if (playerdat.FreezeTimeEnchantment == false)
		{
			if (AnimationFrameDelta != 0)
			{
				ProcessMobileObjects(AnimationFrameDelta);
			}
		}
		if (playerdat.TileState != 0)
		{
			motion.WalkOnSpecialTerrain();
		}
		//playerdat.ApplyPlayerSneakScore(EasyMove);

		//Footsteps()
	}

	static void ProcessMotionInputs()
	{
		motion.PlayerMotionWalk_77C = 0;
		motion.PlayerMotionHeading_77E = 0;
		if (Input.IsKeyPressed(Key.W))
		{
			if (Input.IsKeyPressed(Key.Shift))//forwards
			{
				//walk forwards
				motion.PlayerMotionWalk_77C = 0x32;
			}
			else
			{
				//run forwards
				motion.PlayerMotionWalk_77C = 0x70;
			}
			motion.MotionInputPressed = 1;
		}
		if (Input.IsKeyPressed(Key.Q))//turn left
		{
			motion.PlayerMotionHeading_77E = -90;
			motion.MotionInputPressed = 1;
		}
		if (Input.IsKeyPressed(Key.E))//turn right
		{
			motion.PlayerMotionHeading_77E = +90;
			motion.MotionInputPressed = 1;
		}
		if (Input.IsKeyPressed(Key.S))//walk backwards
		{
			motion.PlayerMotionWalk_77C = 0;
			motion.PlayerMotionHeading_77E = 0;
			motion.MotionInputPressed = 8;
		}
		if (Input.IsKeyPressed(Key.A))//slide left
		{
			motion.PlayerMotionWalk_77C = 0;
			motion.PlayerMotionHeading_77E = 0;
			motion.MotionInputPressed = 9;
		}
		if (Input.IsKeyPressed(Key.D))//slide right
		{
			motion.PlayerMotionWalk_77C = 0;
			motion.PlayerMotionHeading_77E = 0;
			motion.MotionInputPressed = 0xA;
		}
		if (Input.IsKeyPressed(Key.J))//jump.
		{
			if (Input.IsKeyPressed(Key.Shift))
			{
				//long jump
				motion.MotionInputPressed = 6;
			}
			else
			{
				//jump
				//todo: Do a test that the player is grounded.
				motion.MotionInputPressed = 7;
			}
		}
		if (Input.IsKeyPressed(Key.R))//fly up
		{
			if ((playerdat.MagicalMotionAbilities & 0x14) != 0)
			{
				motion.MotionInputPressed = 0xC;
			}
			else
			{
				motion.MotionInputPressed = 0;
			}
		}
		if (Input.IsKeyPressed(Key.F))//fly down
		{
			if ((playerdat.MagicalMotionAbilities & 0x14) != 0)
			{
				motion.MotionInputPressed = 0xD;
			}
			else
			{
				motion.MotionInputPressed = 0;
			}
		}
	}

	static void ProcessMobileObjects(byte AnimationFrameDelta)
	{
		ThisFrameDelta = (byte)((PreviousFrameDelta + AnimationFrameDelta) & 0xF);
		for (int i = 0; i < UWTileMap.current_tilemap.NoOfActiveMobiles; i++)
		{
			var index = UWTileMap.current_tilemap.GetActiveMobileAtIndex(i);
			if ((index > 1) && (index < 256))
			{
				var obj = UWTileMap.current_tilemap.LevelObjects[index];
				//Loop update for a many times as the frame deltas require the object to be updated in this loop.
				var initialnextframe = obj.NextFrame_0XA_Bit0123;
				while (CheckIfUpdateNeeded(nextFrame: obj.NextFrame_0XA_Bit0123))
				{
					if (obj.majorclass == 1)
					{
						if (UWTileMap.ValidTile(obj.tileX, obj.tileY))
						{
							//This is an NPC on the map	
							var n = (npc)obj.instance;

							var result = npc.NPCInitialProcess(obj);
							if (n != null)
							{
								if (obj.instance != null)
								{
									var CalcedFacing = npc.CalculateFacingAngleToNPC(obj);
									n.SetAnimSprite(obj.npc_animation, obj.AnimationFrame, CalcedFacing);
								}
							}
							if (result == false)
							{
								break;
							}
						}
						else
						{
							Debug.Print($"{obj.a_name} {obj.index} is off map");
						}
					}
					else
					{
						//if (motion.MotionSingleStepEnabled)
						//{
						//This is a projectile
						if (motion.MotionProcessing(obj) == false)
						{
							break;
						}
						//}
					}
					if (initialnextframe == obj.NextFrame_0XA_Bit0123)
					{
						Debug.Print($"{obj.a_name} {obj.index} has bugged out in ProcessMobileObjects(), probably needs to be made static.");
					}

				}
			}
		}
		PreviousFrameDelta = ThisFrameDelta;
	}

	/// <summary>
	/// Checks if object/npc needs to move based on their nextFrame value and the current Animation Frame Delta
	/// </summary>
	/// <param name="nextFrame"></param>
	/// <param name="AnimationFrameDelta"></param>
	/// <returns></returns>
	static bool CheckIfUpdateNeeded(int nextFrame)
	{
		//if (AnimationFrameDelta)
		if (ThisFrameDelta > nextFrame)
		{
			if (nextFrame + 4 >= ThisFrameDelta)
			{
				return true;
			}
		}
		//seg007_17A2_357A
		if (ThisFrameDelta + 0x10 <= nextFrame)
		{
			return false;
		}
		if (PreviousFrameDelta > nextFrame)
		{
			return false;
		}
		if (PreviousFrameDelta <= ThisFrameDelta)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if ((@event is InputEventMouseButton eventMouseButton)
			&&
			((eventMouseButton.ButtonIndex == MouseButton.Left) || (eventMouseButton.ButtonIndex == MouseButton.Right)))
		{
			if (eventMouseButton.Pressed)
			{
				if (MessageDisplay.WaitingForMore)
				{
					//Debug.Print("End wait due to click");
					MessageDisplay.WaitingForMore = false;
					return; //don't process any more clicks here.
				}
				if (!blockmouseinput)
				{
					if (uimanager.IsMouseInViewPort())
					{
						uimanager.ClickOnViewPort(eventMouseButton);
					}

				}
			}
		}


		if ((!blockmouseinput) && (uimanager.InGame))
		{
			if (@event is InputEventKey keyinput)
			{
				if (keyinput.Pressed)
				{
					switch (keyinput.Keycode)
					{
						// case Key.W:
						// 	{
						// 		// if (Input.IsKeyPressed(Key.Shift))
						// 		// {
						// 		// 	//walk forwards
						// 		// 	motion.PlayerMotionWalk_77C = 0x32;
						// 		// 	motion.PlayerMotionHeading_77E = 0;
						// 		// }
						// 		// else
						// 		// {
						// 		// 	//run forwards
						// 		// 	motion.PlayerMotionWalk_77C = 0x70;
						// 		// 	motion.PlayerMotionHeading_77E = 0;
						// 		// }
						// 		// motion.MotionInputPressed = 1;

						// 		break;
						// 	}
						// case Key.Q: //not vanilla key
						// 	{
						// 		// motion.PlayerMotionWalk_77C = 0;
						// 		// //turn left
						// 		// motion.PlayerMotionHeading_77E = -90;
						// 		// motion.MotionInputPressed = 1;
						// 		break;
						// 	}
						// case Key.E: //not vanilla key
						// 	{
						// 		//motion.PlayerMotionWalk_77C = 0;
						// 		//turn right
						// 		// motion.PlayerMotionHeading_77E = +90;
						// 		// motion.MotionInputPressed = 1;
						// 		break;
						// 	}
						// case Key.S: //not vanilla key
						// 	{
						// 		//move backwards
						// 		// motion.PlayerMotionWalk_77C = 0;
						// 		// motion.PlayerMotionHeading_77E = 0;
						// 		// motion.MotionInputPressed = 8;
						// 		break;
						// 	}
						// case Key.A: //not vanilla key
						// 	{
						// 		//slide left
						// 		motion.PlayerMotionWalk_77C = 0;
						// 		motion.PlayerMotionHeading_77E = 0;
						// 		motion.MotionInputPressed = 9;
						// 		break;
						// 	}
						// case Key.D: //not vanilla key
						// 	{
						// 		//slide right
						// 		motion.PlayerMotionWalk_77C = 0;
						// 		motion.PlayerMotionHeading_77E = 0;
						// 		motion.MotionInputPressed = 0xA;
						// 		break;
						// 	}
						// case Key.J://jumps
						// 	{
						// 		motion.PlayerMotionHeading_77E = 0;//cancel turn movement when jumping
						// 		if (Input.IsKeyPressed(Key.Shift))
						// 		{
						// 			//long jump
						// 			motion.MotionInputPressed = 6;

						// 		}
						// 		else
						// 		{
						// 			//jump
						// 			//todo: Do a test that the player is grounded.
						// 			motion.MotionInputPressed = 7;
						// 		}
						// 		break;
						// 	}
						case Key.T:
							var mouselook = (bool)gamecam.Get("MOUSELOOK");
							if (mouselook)
							{//toggle to free curso
								Input.MouseMode = Input.MouseModeEnum.Hidden;
							}
							else
							{//toogle to mouselook
								Input.MouseMode = Input.MouseModeEnum.Captured;
							}
							gamecam.Set("MOUSELOOK", !mouselook);
							break;
						// case Key.R: //fly up (not vanilla)
						// 	if ((playerdat.MagicalMotionAbilities & 0x14) != 0)
						// 	{
						// 		motion.MotionInputPressed = 0xC;
						// 	}
						// 	else
						// 	{
						// 		motion.MotionInputPressed = 0;
						// 	}
						// 	break;
						// case Key.F: //fly down (not vanilla)
						// 	if ((playerdat.MagicalMotionAbilities & 0x14) != 0)
						// 	{
						// 		motion.MotionInputPressed = 0xD;
						// 	}
						// 	else
						// 	{
						// 		motion.MotionInputPressed = 0;
						// 	}
						// 	break;
						case Key.F1: //open options menu
							uimanager.InteractionModeToggle(0); break;
						case Key.F2: //talk
							uimanager.InteractionModeToggle((uimanager.InteractionModes)1); break;
						case Key.F3://pickup
							uimanager.InteractionModeToggle((uimanager.InteractionModes)2); break;
						case Key.F4://look
							uimanager.InteractionModeToggle((uimanager.InteractionModes)3); break;
						case Key.F5://attack
							uimanager.InteractionModeToggle((uimanager.InteractionModes)4); break;
						case Key.F6://use
							uimanager.InteractionModeToggle((uimanager.InteractionModes)5); break;
						case Key.F7://toggle panel
							uimanager.ChangePanels(); break;
						case Key.F8: //cast spell
							RunicMagic.CastRunicSpell(); break;
						case Key.F9://track skill
							tracking.DetectMonsters(8, playerdat.Track); break;
						case Key.F10: // make camp
							{
								Debug.Print("Make camp");
								//Try and find a bedroll in player inventory.
								var bedroll = objectsearch.FindMatchInFullObjectList(
									majorclass: 4, minorclass: 2, classindex: 1,
									objList: playerdat.InventoryObjects);
								if (bedroll != null)
								{
									sleep.Sleep(1);
								}
								else
								{
									sleep.Sleep(0);
								}
								break;
							}


						case Key.F11://toggle position label
							{
								EnablePositionDebug = !EnablePositionDebug;
								uimanager.EnableDisable(lblPositionDebug, EnablePositionDebug);
								break;
							}
						case Key.F12://debug
							{
								//cutsplayer.PlayCutscene(0);//test  
								//trigger.RunTimerTriggers();
								if (UWClass._RES == UWClass.GAME_UW2)
								{
									scd.ProcessSCDArk(1);
								}
								//trigger.RunNextScheduledTrigger();
								break;
							}
						case Key.Pagedown:
							{
								motion.MotionSingleStepEnabled = true;//for stepping through motion processing.
								break;
							}
						case Key.Apostrophe:
							{
								//give full mage abilities
								playerdat.max_mana = 60;
								playerdat.play_mana = 60;
								playerdat.Casting = 30;
								playerdat.ManaSkill = 30;
								playerdat.play_level = 16;
								for (int r = 0; r < 24; r++)
								{
									playerdat.SetRune(r, true);
								}
								playerdat.PlayerStatusUpdate();
								break;
							}
					}
				}
			}
		}

		if (ConversationVM.WaitingForInput
			&& !uimanager.MessageScrollIsTemporary
			&& !MessageDisplay.WaitingForTypedInput)
		{
			switch (@event)
			{
				//Click to select options in conversation. Ensure we only allow left &right click to adhere to the original UW implementation. 
				case InputEventMouseButton mouseEvent:
					if (uimanager.CursorOverMessageScroll)
					{
						if (mouseEvent.Pressed && (mouseEvent.ButtonIndex == MouseButton.Left || mouseEvent.ButtonIndex == MouseButton.Right))
						{
							int result = uimanager.instance.HandleMessageScrollClick(mouseEvent);
							if ((result > 0) && (result <= ConversationVM.MaxAnswer))
							{
								ConversationVM.PlayerNumericAnswer = result;
								ConversationVM.WaitingForInput = false;
							}
							//GD.Print("Mouse Clicked in conversation");
						}
					}

					break;

				//Using keyboard numbers to select options in conversation
				case InputEventKey keyinput:
					{

						if (keyinput.Pressed)
						{
							if (int.TryParse(keyinput.AsText(), out int result))
							{
								if ((result > 0) && (result <= ConversationVM.MaxAnswer))
								{
									ConversationVM.PlayerNumericAnswer = result;
									ConversationVM.WaitingForInput = false;
								}
							}
						}

						break;
					}
			}
		}

		if (MessageDisplay.WaitingForMore)
		{
			if (@event is InputEventKey keyinput)
			{
				Debug.Print("End wait due to key inputclick");
				MessageDisplay.WaitingForMore = false;
			}
		}

		if ((MessageDisplay.WaitingForTypedInput) && (!chargen.ChargenWaitForInput))
		{
			if (@event is InputEventKey keyinput)
			{
				if (keyinput.Pressed)
				{
					bool stop = false;
					switch (keyinput.Keycode)
					{
						case Key.Enter:
							stop = true;
							break;
						case Key.Escape:
							stop = true;
							uimanager.instance.TypedInput.Text = "";
							break;
					}
					if (stop)
					{//end typed input
						uimanager.instance.scroll.Clear();
						MessageDisplay.WaitingForTypedInput = false;
						if (ConversationVM.InConversation == false)
						{
							gamecam.Set("MOVE", true);//re-enable movement
						}
					}
				}
			}
		}

		if ((MessageDisplay.WaitingForTypedInput) && (chargen.ChargenWaitForInput))
		{//handles character name input
			if (@event is InputEventKey keyinput)
			{
				if (keyinput.Pressed)
				{
					bool stop = false;
					//var keyin = keyinput.GetKeycodeWithModifiers();
					switch (keyinput.Keycode)
					{
						case Key.Enter:
							stop = true;
							break;
						case Key.Backspace:
							{
								var text = uimanager.instance.ChargenNameInput.Text;
								if (text.Length > 0)
								{
									text = text.Remove(text.Length - 1);
									uimanager.instance.ChargenNameInput.Text = text;
								}
								break;
							}
						case >= Key.Space and <= Key.Z:
							{
								string inputed;// = (char)keyinput.Unicode;
								if (Input.IsPhysicalKeyPressed(Key.Shift))
								{
									inputed = ((char)keyinput.Unicode).ToString().ToUpper();
								}
								else
								{
									inputed = ((char)keyinput.Unicode).ToString().ToLower();
								}
								var text = uimanager.instance.ChargenNameInput.Text;
								if (text.Length < 16)
								{
									text += inputed;
									uimanager.instance.ChargenNameInput.Text = text;
								}
								break;
							}
					}
					if (stop)
					{//end typed input						
						MessageDisplay.WaitingForTypedInput = false;
						chargen.ChargenWaitForInput = false;
					}
				}
			}
		}

		if (musicalinstrument.PlayingInstrument)
		{
			if (@event is InputEventKey keyinput)
			{
				if (keyinput.Pressed)
				{
					switch (keyinput.Keycode)
					{
						case >= Key.Key0 and <= Key.Key9:
						case >= Key.Kp0 and <= Key.Kp9:
							musicalinstrument.notesplayed += keyinput.AsText();
							Debug.Print($"Imagine musical note {keyinput.AsText()}");
							break;
						case Key.Escape:
							musicalinstrument.StopPlaying();
							break;
					}
				}
			}
		}

		if (MessageDisplay.WaitingForYesOrNo)
		{
			if (@event is InputEventKey keyinput)
			{
				if (keyinput.Pressed)
				{
					bool stop = false;
					switch (keyinput.Keycode)
					{
						case Key.Enter:
							stop = true;
							break;
						case Key.Escape:
							stop = true;
							uimanager.instance.TypedInput.Text = "No";
							break;
						case Key.Y:
							uimanager.instance.TypedInput.Text = "Yes"; break;
						default:
							uimanager.instance.TypedInput.Text = "No"; break;
					}
					if (stop)
					{//end typed input
						uimanager.instance.scroll.Clear();
						MessageDisplay.WaitingForYesOrNo = false;
						gamecam.Set("MOVE", true);//re-enable movement
					}
				}
			}
		}
	}

	/// <summary>
	/// Handles the end of chain events.
	/// </summary>
	public static void RefreshWorldState()
	{
		if (DoRedraw)
		{
			//update tile faces
			UWTileMap.SetTileMapWallFacesUW();
			//UWTileMap.current_tilemap.CleanUp();
			//Handle tile changes after all else is done
			foreach (var t in UWTileMap.current_tilemap.Tiles)
			{
				if (t.Redraw)
				{
					UWTileMap.RemoveTile(
						tileX: t.tileX,
						tileY: t.tileY,
						removeWall: (t.tileType >= 2 && t.tileType <= 5));
					tileMapRender.RenderTile(tileMapRender.worldnode, t.tileX, t.tileY, t);
					t.Redraw = false;
				}
			}
		}

		//Handle level transitions now since it's possible for further traps to be called after the teleport trap
		Teleportation.HandleTeleportation();
	}

}//end class
