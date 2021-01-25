using SFML.Window;
using BlackCoat;

namespace UI
{
    static class Program
    {
        static void Main()
        {
            var mode = new VideoMode(800, 600);
            var device = Device.Create(mode, "Black Coat UI Sample", Styles.Default, 0, false, 120);
            using var core = new Core(device);
            core.SceneManager.ChangeScene(new UIScene(core));
            core.Run();
        }
    }
}