using System;

namespace Underworld.config;

public class GameConfigChanged(
    IGameConfigData next,
    IGameConfigData prev
) : EventArgs {
    public IGameConfigData Next { get; } = next;
    public IGameConfigData Prev { get; } = prev;
}
