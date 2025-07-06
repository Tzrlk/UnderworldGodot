using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace Underworld.config;

[TestSuite]
public class GameConfigTest
{

    [TestCase]
    public async Task LoadConfigAsync()
    {
        var config = await new GameConfig().Load();
        AssertString(config.PathUW1).IsEqual("C:\\Games\\UW1");
    }

}
