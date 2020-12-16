using BlackCoat;

namespace Minimal
{
    static class Program
    {
        static void Main()
        {
            using (var core = new Core(Device.Demo))
            {
                core.SceneManager.ChangeScene(new MinimalScene(core));
                core.Run();
            }
        }
    }
}