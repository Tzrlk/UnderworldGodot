using System.IO;
using System.Text.Json;
using System;
using Godot;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Underworld;

public class GameConfig
{

	private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        IgnoreReadOnlyProperties = true,
        PropertyNameCaseInsensitive = true,
    };

	private static readonly string FilePath
		= ProjectSettings.GlobalizePath("user://settings.json");

    public static GameConfig Instance;

    // This initialises our instance as soon as the class is loaded.
    static GameConfig() {
	    if (File.Exists(FilePath))
	    {
		    Debug.Print($"Loading settings from {FilePath}");
		    using var stream = File.OpenRead(FilePath);
		    Instance = JsonSerializer.Deserialize<GameConfig>(stream, JsonOpts);
	    }
	    else
	    {
		    Debug.Print($"No existing settings at {FilePath}. Loading defaults.");
		    Instance = new();
	    }

	    if (main.gamecam != null)
	    {
		    main.gamecam.Fov = Math.Max(50, Instance.Fov);
	    }
    }

    [JsonPropertyName("pathuw1")]
    public string PathUw1 { get; set; } = @"C:\Games\UW";
    
    [JsonPropertyName("pathuw2")]
    public string PathUw2 { get; set; } = @"C:\Games\UW2";
    
    [JsonPropertyName("gametoload")]
    public string GameToLoad { get; set; } = "UW1";
    
    [JsonPropertyName("level")]
    public int GameLevel { get; set; } = 0;
    
    [JsonPropertyName("FOV")]
    public float Fov { get; set; } = 75;
    
    [JsonPropertyName("showcolliders")]
    public bool ShowColliders { get; set; }
    
    [JsonPropertyName("shaderbandsize")]
    public int ShaderBandSize { get; set; } = 8;

    public void Save()
    {
        Debug.Print($"Saving settings to {FilePath}");
        using var stream = File.OpenWrite(FilePath);
        JsonSerializer.Serialize(stream, this, JsonOpts);
    }

    public static Game GameSelected {
	    set => Instance.GameToLoad = Enum.GetName(value)!.ToUpper();
	    get => Instance.GameToLoad.ToUpper() switch {
		    "UW0" => Game.Uw0,
		    "UW1" => Game.Uw1,
		    "UW2" => Game.Uw2,
		    var game => throw new ApplicationException($"Unrecognised game selection: {game}")
	    };
    }

    public static string GamePath
	    => GameSelected switch {
		    Game.Uw0 or Game.Uw1 => Instance.PathUw1,
		    Game.Uw2 => Instance.PathUw2,
		    var game => throw new ApplicationException($"Unrecognised game selection: {game}")
	    };

}
