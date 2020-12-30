using System;
using BlackCoat;

namespace Particles
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var core = new Core(Device.Demo))
            {
                core.SceneManager.ChangeScene(new MyScene(core));
                core.Run();
            }
        }
    }
}