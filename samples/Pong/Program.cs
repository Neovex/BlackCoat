using BlackCoat;

namespace Pong
{
    static class Program
    {
        static void Main()
        {
            using (var core = new Core(Device.Demo))
            {
                core.SceneManager.ChangeScene(new PongScene(core));
                core.Run();
            }
        }
    }
}