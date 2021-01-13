using System;
using BlackCoat;

namespace UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var device = Device.Create("Black Coat UI Sample");
            if (device == null) return;
            using (var core = new Core(device))
            {
                core.SceneManager.ChangeScene(new UIScene(core));
                core.Run();
            }
        }
    }
}