![Black Coat Banner](/src/Resources/Banner.png)

## What is it?
The Black Coat Engine is a simple, straightforward 2D game engine using [SFML.net](http://www.sfml-dev.org/) and C#.
The engine is designed to let programmers focus on their game without getting in the way.
However each feature is just one keyword away if you need it.

## Features
Some of the work the engine handles for you:
- Render device management
- Particles
- Frame & blitting animations
- Timing and Tweening
- Scene graph management
- Live object inspection during runtime
- Automated asset management (Textures, Fonts, Sounds & Music)
- Customizable game launchers
- Vector math
- Polygon collisions
- UI automation, Buttons, Dialogs
- Input mapping for mouse, keyboard and game pads/joysticks
- In-game console with customizable commands
- and more...

## Techstack
Black Coat is build with:
- NET 5.0 (Visual Studio 16.8 and later) or
- .Net Framework 4.7.*
- [SFML](http://www.sfml-dev.org)
- [SFML.net](https://www.nuget.org/packages/SFML.Net)

## Quick start
- Create a new Winform project
- Add SFML.net via Nuget
- Download and reference the [Black Coat DLL](/lib/BlackCoat.dll)

Too fast? No problem.
Take your time learning Black Coat, one tutorial at a time. [Lets get started!](/doc).

## Preview
See how easy it is to get the engine up and running:
```
using (var core = new Core(Device.Fullscreen))
{
    core.SceneManager.ChangeScene(new MyScene(core));
    core.Run();
}
```

## Contribution
If you want to contribute to the Black Coat project, be it additions or bug reports, your help is very welcome.

## License
The Black Coat Game Engine is licensed under the Apache 2.0 license.
You can read the full license [here](/LICENSE).

## Special Thanks
Special Thanks go out to the developers and contributors of the SFML Library and its .net Wrapper.