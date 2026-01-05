using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Underworld;

// <summary>
// This script node backs-up the initial launch scene for the game, allowing
// the user to configure paths and pick between UW 1 & 2.
// </summary>
public partial class LaunchMenu : Control {

	private const string NextScene = "res://scenes/Underworld.tscn";

	[Export]
	public TextureRect SelectUW1;

	[Export]
	public TextEdit PathUW1;

	[Export]
	public TextureRect SelectUW2;

	[Export]
	public TextEdit PathUW2;

	[Export]
	public FileDialog GameFilesSelector;

	// Allows easy storage and retrieval of mouse mode overrides.
	private readonly Stack<Input.MouseModeEnum> _mouseModeHistory = new();

	// Only need one reference to the current settings.
	private readonly GameConfig _uwSettings = GameConfig.instance;

	public override void _Ready()
	{
		Debug.Print($"_Ready fired");

		// Load initial paths from settings.
		PathUW1.Text = _uwSettings.pathuw1;
		PathUW2.Text = _uwSettings.pathuw2;

		// Set the initial focus selection.
		switch (GameConfig.GameSelected)
		{
			case Game.Uw0:
			case Game.Uw1:
				SelectUW1.GrabFocus();
				break;
			case Game.Uw2:
				SelectUW2.GrabFocus();
				break;
			default:
				// Non-blocking at this point. Just notify and do no more.
				GD.PushError("Invalid game path selection: ", (byte)GameConfig.GameSelected);
				return;
		}

		// Start loading the main scene as soon as practicable.
		var error = ResourceLoader.LoadThreadedRequest(NextScene, "PackedScene", true);
		GD.PushError($"{Enum.GetName(error)} while preloading main scene.");

	}

	public void OnPathInput(InputEvent @event, int selection)
	{
		// Seems dumb at the moment, but this lets us process multiple
		// event types, such as InputEventKey. Feels like this should
		// be more abstract, though. Like focus and selection events
		// instead of binding to specific device inputs.
		switch (@event)
		{
			case InputEventMouseButton mouseButtonEvent:
				if (mouseButtonEvent.Pressed)
					break;
				return;
			default:
				return;
		}

		Debug.Print($"OnPathInput: {@event}");

		// Select which path we're going to edit.
		GameConfig.GameSelected = (Game)selection;
		switch (GameConfig.GameSelected)
		{
			case Game.Uw0:
			case Game.Uw1:
				GameFilesSelector.CurrentPath = PathUW1.Text;
				GameFilesSelector.CurrentDir = PathUW1.Text;
				GameFilesSelector.Filters = [
					"uw.exe;Stygian Abyss",
				];
				break;
			case Game.Uw2:
				GameFilesSelector.CurrentPath = PathUW2.Text;
				GameFilesSelector.CurrentDir = PathUW2.Text;
				GameFilesSelector.Filters = [
					"uw2.exe;Labyrinth of Worlds",
				];
				break;
			default:
				// Non-blocking at this point. Just notify and do no more.
				GD.PushError("Invalid game path selection: ", selection);
				return;
		}

		// Switch to the regular cursor, saving the previous state so we
		// can revert to it later.
		_mouseModeHistory.Push(Input.MouseMode);
		Input.MouseMode = Input.MouseModeEnum.Visible;

		// Finally display the selector
		GameFilesSelector.Show();

	}

	public void OnGameFilesSelectorSubmitted(string path)
	{
		Debug.Print($"GameFilesSelector submitted {path}");

		// Save the selected directory back, and clear the selection for
		// no good reason in particular.
		var selectedDir = System.IO.Path.GetDirectoryName(path);
		switch (GameConfig.GameSelected)
		{
			case Game.Uw0:
			case Game.Uw1:
				PathUW1.Text = selectedDir;
				_uwSettings.pathuw1 = selectedDir;
				_uwSettings.Save();
				break;
			case Game.Uw2:
				PathUW2.Text = selectedDir;
				_uwSettings.pathuw2 = selectedDir;
				_uwSettings.Save();
				break;
			default:
				// Non-blocking at this point. Just notify and do no more.
				GD.PushError("Invalid game selection: ", (byte)GameConfig.GameSelected);
				break;
		}

		// Revert the cursor. Yes I'm not checking that it's there first.
		Input.MouseMode = _mouseModeHistory.Pop();

	}

	public void OnLaunchInput(InputEvent @event, int selection)
	{

		// Filter appropriate event triggers.
		switch (@event)
		{
			case InputEventMouseButton mouseButtonEvent:
				if (mouseButtonEvent.Pressed)
					break;
				return;
			default:
				return;
		}

		Debug.Print($"OnPathInput: {@event}");

		// Update settings and the current state.
		GameConfig.GameSelected = (Game)selection;

		// Save any changes to our settings.
		_uwSettings.Save();

		// Switch scenes to start the game.
		var scene = (PackedScene)ResourceLoader.LoadThreadedGet(NextScene);
		GetTree().ChangeSceneToPacked(scene);

	}

}
