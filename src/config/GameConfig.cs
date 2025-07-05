using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;

namespace Underworld.config;

public class GameConfig : IGameConfig, IGameConfigData
{

    private static string DefaultFilePath => OS
            .GetExecutablePath()
            .GetBaseDir()
            .PathJoin("uwsettings.json");

    // <editor-fold desc="IGameConfigData instance" defaultstate="collapsed">
    private IGameConfigData configData = new GameConfigData();
    private IGameConfigData ConfigData
    {
        get { return configData; }
        set
        {
            IGameConfigData prev = configData;
            configData = value;
            OnConfigChanged(this, new GameConfigChanged(value, prev));
        }
    }
    // </editor-fold>

    // <editor-fold desc="IGameConfigData delegating implementation" defaultstate="collapsed">
    public string PathUW1 { get => configData.PathUW1; set => configData.PathUW1 = value; }
    public string PathUW2 { get => configData.PathUW2; set => configData.PathUW2 = value; }
    public GameVariant GameToLoad { get => configData.GameToLoad; set => configData.GameToLoad = value; }
    public int Level { get => configData.Level; set => configData.Level = value; }
    public float FOV { get => configData.FOV; set => configData.FOV = value; }
    public bool ShowColliders { get => configData.ShowColliders; set => configData.ShowColliders = value; }
    public int ShaderBandSize { get => configData.ShaderBandSize; set => configData.ShaderBandSize = value; }
    // </editor-fold>

    // <editor-fold desc="IGameConfig implementation" defaultstate="collapsed">

    public event EventHandler<GameConfigChanged> OnConfigChanged;

    public Task<IGameConfig> Load()
    {
        return Load(DefaultFilePath);
    }

    public async Task<IGameConfig> Load(string filePath)
    {
        try
        {
            Debug.Print($"Loading config from {filePath}.");
            using FileStream stream = File.OpenRead(filePath);
            ConfigData = await JsonSerializer.DeserializeAsync<GameConfig>(stream, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            });
            return this;
        }
        catch (FileNotFoundException)
        {
            Debug.Print("Unable to find existing config. Using defaults.");
            ConfigData = new GameConfigData();
            return this;
        }
        catch (Exception err)
        {
            throw new Exception($"Failed to load game config from {filePath}.", err);
        }
    }

    public Task<IGameConfig> Save()
    {
        return Save(DefaultFilePath);
    }

    public async Task<IGameConfig> Save(string filePath)
    {
        try
        {
            Debug.Print($"Saving current config to {filePath}");
            using FileStream stream = File.OpenWrite(filePath);
            await JsonSerializer.SerializeAsync(stream, ConfigData, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
            return this;
        }
        catch (Exception err)
        {
            throw new Exception($"Failed to save current settings to {filePath}", err);
        }
    }

    // </editor-fold>

}
