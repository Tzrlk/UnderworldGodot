namespace Underworld.config;

public struct GameConfigData : IGameConfigData
{

    public string PathUW1 { get; set; } = "C:\\Games\\UW1";

    public string PathUW2 { get; set; } = "C:\\Games\\UW2";

    public GameVariant GameToLoad { get; set; } = GameVariant.UW1;

    public int Level { get; set; } = 0;

    public float FOV { get; set; } = 75.0f;

    public bool ShowColliders { get; set; } = false;

    public int ShaderBandSize { get; set; } = 7;

    public GameConfigData() { }

}
