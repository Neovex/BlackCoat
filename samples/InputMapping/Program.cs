using BlackCoat;

namespace InputMapping
{
    static class Program
    {
        static void Main()
        {
            using (var core = new Core(Device.Demo))
            {
                core.SceneManager.ChangeScene(new InputScene(core));
                core.Run();
            }
        }
    }
}