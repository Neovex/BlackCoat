using BlackCoat;

namespace ConwaysGameOfLife
{
    static class Program
    {
        static void Main()
        {
            using (var core = new Core(Device.Demo))
            {
                // Setup Scenes
                Scene startScene = new ConwayScene(core);
#if !DEBUG
                // Add intro for release builds
                startScene = new BlackCoatIntro(core, startScene);
#endif

                // Run
                core.SceneManager.ChangeScene(startScene);
                core.Run();
            }
        }
    }
}