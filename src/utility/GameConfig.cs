using System.IO;
using System.Text.Json;
using System;
using Godot;
using System.Diagnostics;

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

    public static GameConfig instance;

    // This initialises our instance as soon as the class is loaded.
    static GameConfig() => LoadSettings();

    public static void LoadSettings()
    {

        if (File.Exists(FilePath))
        {
            Debug.Print($"Loading settings from {FilePath}");
            using var stream = File.OpenRead(FilePath);
            instance = JsonSerializer.Deserialize<GameConfig>(stream, JsonOpts);
        }
        else
        {
            Debug.Print($"No existing settings at {FilePath}. Loading defaults.");
            instance = new();
        }

        if (main.gamecam != null)
        {
            main.gamecam.Fov = Math.Max(50, instance.FOV);
        }

    }

    public string pathuw1 { get; set; } = @"C:\Games\UW";
    public string pathuw2 { get; set; } = @"C:\Games\UW2";
    public string gametoload { get; set; } = "UW1";
    public int level { get; set; } = 0;
    public float FOV { get; set; } = 75;
    public bool showcolliders { get; set; }
    public int shaderbandsize { get; set; } = 8;

    public void Save()
    {
        Debug.Print($"Saving settings to {FilePath}");
        using var stream = File.OpenWrite(FilePath);
        JsonSerializer.Serialize(stream, this, JsonOpts);
    }

    public static Game GameSelected =>
	    instance.gametoload.ToUpper() switch {
		    "UW0" => Game.Uw0,
		    "UW1" => Game.Uw1,
		    "UW2" => Game.Uw2,
		    var game => throw new ApplicationException($"Unrecognised game selection: {game}")
	    };

    public static string GamePath
	    => GameSelected switch {
		    Game.Uw0 or Game.Uw1 => instance.pathuw1,
		    Game.Uw2 => instance.pathuw2,
		    var game => throw new ApplicationException($"Unrecognised game selection: {game}")
	    };

}
