using System.ComponentModel;

namespace Underworld.config;

/// <summary>
/// These are the individual games possible to load with this program.
/// Their values are directly linked to `UWClass.GAME_*` in order to
/// enforce the sync between them.
/// </summary>
public enum GameVariant
{

    [Description("Ultima Underworld Demo")]
    DEMO = UWClass.GAME_UWDEMO,

    [Description("Ultima Underworld: The Stygian Abyss")]
    UW1 = UWClass.GAME_UW1,

    [Description("Ultima Underworld 2: Labyrinth of Worlds")]
    UW2 = UWClass.GAME_UW2,

}
