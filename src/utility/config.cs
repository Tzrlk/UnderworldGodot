using System.IO;
using System.Text.Json;
using System;
using Godot;
using System.Diagnostics;

namespace Underworld
{
    public class uwsettings
    {
        public string pathuw1 { get; set; } = "C:\\Games\\UW1";
        public string pathuw2 { get; set; } = "C:\\Games\\UW2";
        public string gametoload { get; set; } = "UW1";
        public int level { get; set; } = 0;

        public float FOV { get; set; } = 75.0f;

        public bool showcolliders { get; set; } = false;
        public int shaderbandsize { get; set; } = 8;

        public static uwsettings instance;

        static uwsettings()
        {
            instance = LoadSettings();
            
            // Update the camera FOV immediately if it's available. 
            if (main.gamecam != null)
            {
                main.gamecam.Fov = Math.Max(50, instance.FOV);
            }

            SetGame(instance.gametoload);

            UWClass.BasePath = UWClass._RES switch
            {
                UWClass.GAME_UW1 => instance.pathuw1,
                UWClass.GAME_UW2 => instance.pathuw2,
                _ => throw new InvalidOperationException("Invalid Game Selected"),
            };

        }

        private static string CfgFolder => Path.GetDirectoryName(OS.GetExecutablePath());
        private static string CfgFile => Path.Combine(CfgFolder, "uwsettings.json");
        private static JsonSerializerOptions CfgJson => new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };

        public static void SaveSettings()
        {
            string cfgFile = CfgFile;
            try
            {
                Debug.Print($"Saving current config to {cfgFile}");
                using FileStream stream = File.OpenWrite(cfgFile);
                JsonSerializer.Serialize(stream, instance, CfgJson);
            }
            catch (Exception err)
            {
                throw new Exception($"Failed to save current settings to {cfgFile}", err);
            }
        }

        public static uwsettings LoadSettings()
        {
            string cfgFile = CfgFile;
            try
            {
                Debug.Print($"Loading config from {cfgFile}.");
                using FileStream stream = File.OpenRead(cfgFile);
                return JsonSerializer.Deserialize<uwsettings>(stream, CfgJson);
            }
            catch (FileNotFoundException)
            {
                Debug.Print("Unable to find existing config. Using defaults.");
                return new uwsettings();
            }
            catch (Exception err)
            {
                throw new Exception($"Failed to load game config from {cfgFile}.", err);
            }
        }

        public static void SetGame(string gamemode)
        {
            switch (gamemode.ToUpper())
            {
                case "UW2":
                case "2":
                    UWClass._RES = UWClass.GAME_UW2; break;
                case "UW1":
                case "1":
                    UWClass._RES = UWClass.GAME_UW1; break;                
                case "UWDEMO":
                case "0":
                    UWClass._RES = UWClass.GAME_UWDEMO; break;
            }
        }


    } //end class
}//end namespace