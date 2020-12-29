# Black Coat Engine First Steps

## Project Setup
1. In Visual Studio create a new Windows Forms Project. While .NetFramework is supported I recommend using the .Net Variant for .net 5.0 (Visual Studio 16.8 and later).
2. If you choose to use .net 5.0 make sure your project targets this version by verifying its properties. To do so Right click your project and choose options, then select .NET 5.0 as the Target framework.
3. Right Click dependencies and select `Manage NuGet Packages...`
4. Click on the Browse Tab and search for `SFML`
5. Install the NuGet Package `SFML.Net` by Laurent Gomila then close NuGet.
6. Add the Black Coat Engine either by
A) Adding its appropriate DLL to the list of dependencies (recommended) or
B) Adding its project to your solution and reference it via project reference.
7. Delete the default Form1 file. The engine will provide the necessary window.
8. Open the Programm.cs and clear out the `Main()` method.

## Creating a Stage
1. Create a new Class called `MyFirstScene`
2. Add `using BlackCoat;` to the list of namespaces.
3. Inherit the new class from `Scene`
4. Press Alt+Enter to bring up the quick actions then select `Implement abstract class`
5. Press Alt+Enter again then select `Generate Constructor`
6. Delete all occurrences of `throw new NotImplementedException();`
Optional: grab the destroy method and move it to the bottom of your class.

## Starting the Engine
1. Open `Programm.cs`
If you haven´t cleared the contents of your `Main()` method do so now.
2. Add `using BlackCoat;` to the list of namespaces
3. Inside your `Main()` create a new using block and define the engine core and device:
```
using (var core = new Core(Device.Demo))
{
}
```
4. Inside the using block we now want to use the stage we created earlier:
`core.SceneManager.ChangeScene(new MyFirstScene(core));`
5. Finally lets start the engine itself:
`core.Run();`