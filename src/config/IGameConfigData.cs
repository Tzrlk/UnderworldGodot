namespace Underworld.config;

public interface IGameConfigData
{

    public string PathUW1 { get; set; }

    public string PathUW2 { get; set; }

    public GameVariant GameToLoad { get; set; }

    public int Level { get; set; }

    public float FOV { get; set; }

    public bool ShowColliders { get; set; }

    public int ShaderBandSize { get; set; }

}
