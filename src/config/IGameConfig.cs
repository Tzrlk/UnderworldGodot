using System;
using System.Threading.Tasks;

namespace Underworld.config;

public interface IGameConfig : IGameConfigData
{

    public Task<IGameConfig> Load();

    public Task<IGameConfig> Load(string filePath);

    public Task<IGameConfig> Save();

    public Task<IGameConfig> Save(string filePath);

    public event EventHandler<GameConfigChanged> OnConfigChanged;

    // TODO: Replace this with appropriate uses of ConfigChangeHandler.
    protected void PerformUpdates()
    {

        // Update the camera FOV immediately if it's available.
        if (main.gamecam != null)
        {
            main.gamecam.Fov = Math.Max(50, FOV);
        }

        // Keep game selection in sync with UWClass properties.
        // TODO: Should this be done on any change to the properties? On save?
        UWClass._RES = (byte)GameToLoad;
        UWClass.BasePath = GameToLoad switch
        {
            GameVariant.DEMO => PathUW1,
            GameVariant.UW1 => PathUW1,
            GameVariant.UW2 => PathUW2,
            _ => throw new InvalidOperationException("Invalid Game Selected"),
        };

    }

}
