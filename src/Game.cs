using System;
using System.Runtime.InteropServices;

namespace Underworld;

/// <summary>
/// Represents a game that can be run using this engine.
/// </summary>
public enum Game : byte {

	/// <summary>
	/// The Ultima Underworld demo. For the most part identical in behaviour to
	/// the full version.
	/// </summary>
	Uw0 = 0,

	/// <summary>
	/// Ultima Underworld: The Stygian Abyss
	/// </summary>
	Uw1 = 1, 

	/// <summary>
	/// Ultima Underworld 2: Labyrinth of Worlds
	/// </summary>
	Uw2 = 2,

}

public static class GameExtensions {

	/// <summary>
	/// Shortcut to checking if the given game is currently selected.
	/// </summary>
	/// <param name="game">The game in question.</param>
	/// <returns>true if the game is selected in config.</returns>
	public static bool IsSelected(this Game game)
		=> GameConfig.GameSelected == game;

}