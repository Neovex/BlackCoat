# Drawing all the Things™!

Now that you're all ready to go lets dive into getting some content into your scene.
Got no scene? Check back on the First Steps tutorial [here](FirstSteps.md).

## Understanding the Scene Graph

A scene graph defines the hierarchy of entities - sounds complicated but is super easy. Imagine a tank game with a top down view. Your tank would consist of the body and its turret. Since the turret can rotate independently from the tanks body you'll need to use 2 graphics - one for each. Now when your tanks body moves forward it would make sense if the turret follows accordingly. Of course you could move both graphics by the same amount but what if your tank reaches level 200, has 5 turrets 4 rocket launcher and 2 shields? You'd go mad keeping track of all the movement. This is where the scene graph comes in.

In our example we can add the turret as a child to the body of the tank. Now whenever the tank moves or rotates _all_ child entities will be moved / rotated accordingly. Even better: when adding a child entity it will have its own local coordinate system so you can completely ignore the orientation of its parent when moving stuff around - neat huh?
```
// Load textures
var bodyTex = TextureLoader.Load("tracks");
var turretTex = TextureLoader.Load("turret");

// Build tank
var tank = new Container(_Core) { Texture = bodyTex };
var turret = new Graphic(_Core) { Texture = turretTex };

// Add to scene graph
tankBody.Add(turret);
Layer_Game.Add(tank);
```
Now you also know why you don't see a thing when you forget to add an entity to the scene: Entities that are not part of the scene graph hierarchy are neither drawn nor updated. This is useful for managing stuff in the background. You can always remove something from the scene change it around and add it at a different location within the scene graph. You can imagine this as your "backstage area" - VIPs (aka. coders) only. :wink:

Keep in mind: you cannot add an entity nor an entity container to the scene directly. Instead use one of the scenes predefined layers:
- Layer_Background:  Lowest layer for background graphics or animations.
- Layer_Game: Default layer for your games entities.
- Layer_Overlay: Top layer for all kinds of special effects overlays like UIs, Lightmaps or ParticleEmitterHosts.
- Layer_Cursor: You cannot place entities on this layer directly but it allows for custom cursors. Checkout `Layer_Cursor.SetCursor()`

## Drawing Shapes

If you went through the First Steps tutorial [here](FirstSteps.md) you are already familiar with rectangle shapes. All the other shapes in `BlackCoat.Entites.Shapes` work mostly the same. Create your instance, provide the necessary information to the constructor for creating the entity and add it to the scene graph.
```
// Circle Example
var circle = new Circle(_Core, 100, Color.Cyan);
Layer_Game.Add(circle);
```
For more sophisticated shapes you may use the `Polygon` type. However keep in mind that when you want to use the polygon in combination with the default Black Coat Collision System you must adhere to the following rules:
1. Always define the points that make up your polygons in a clockwise order.
2. Never define a concave shape. If you need it to be concave split it into 2 convex shapes.

If that is still to high-level for you the `FreeShape` entity got you covered. With a `FreeShape` you can define your own vertices along with vertex color and texture coordinates giving you low level access to every single vertex as you see fit. Since a free shape only creates a single draw call per instance you can use it to cover resource intensive scenarios like tile maps or custom special effects.

## Drawing Textures

While shapes are fun they often lack details. This is where textured graphics come into play. However before you can place your latest holiday photo album in your scene you need to load it first.

### Loading assets

To load textures, or any asset for that matter you need the appropriate asset loader. Your scene already has loaders for all the types of assets you might need:
- TextureLoader: loads graphics from a wide variety of file types thanks to SFML
- MusicLoader: loads music buffers to be played in the background
- FontLoader: loads fonts to display Text - more on this later
- SfxLoader: loads short audio clips for sound effects like laser beams or jump sounds

All asset loaders work the same, simply call its `Load(fileName)` method where fileName is the desired file you want to load **without** its file ending. It is important to notice that multiple calls to `Load(fileName)` will not result in multiple load operations. Once a file is loaded it will be cached and can be accessed very quickly. This way you can preload assets and use them at a later time without delay. You can even load an entire folders worth of assets with just one call to `LoadAllFilesInDirectory()`. It will then automatically cache all compatible files for later.


When creating a scene you can define an asset root path in its base constructor call. This will be used as the folder where all asset loaders will look for their respective files. This way you could separate your assets into one folder per scene.
Additionally you can alter an asset loaders root folder separately for even more fine grained asset management. This way your asset folder may end up looking like this:
- Assets
  - MenueScene
    - Textures
    - Fonts
  - Level1Scene
    - Textures
    - Music
    - Sounds

### Drawing Textures (for real this time)

A texture is not an entity hence cannot be placed in the scene graph directly. To do so anyway simply wrap your texture in a `Graphic` which is the default entity type to represent textures in your scene:
```
var myTex = TextureLoader.Load("myTexture");
var myGraphic = new Graphic(_Core, myTex);
Layer_Game.Add(myGraphic);
// You can alternatively chain it all together if you like:
Layer_Game.Add(new Graphic(_Core, TextureLoader.Load("myTexture")));
```
<span style="color:blue">
As always don't forget to add your entity to a layer or container.
</span>
As Black Coat is build on top of SFML you may want to take a look at SFMLs own tutorial on loading and displaying textures [here](https://www.sfml-dev.org/tutorials/2.5/graphics-sprite.php).

## Drawing Text

Getting some sweet letters into your scene is easy as well. As always, all you need the right entity type for the job. For text this is the aptly named `TextItem`:
```
var myText = new TextItem(_Core, "Hello World");
Layer_Game.Add(myText);
```
Wile you don't have to assign an initial text string to display, it is generally a good idea to do it anyway as it prevents you from creating "invisible" text instances. There are two more optional constructor arguments you may define: character size and font. However all these arguments may be modified any time via their respective properties.

### Known issues
While rare it may happen that a font, while perfectly loaded can suddenly "loose" letters - yeah its weired I know. To counteract this behavior the Core provides a method to work around this issue: `_Core.InitializeFontHack(myfont);`. Should you run into this issue call this method immediately after a font is loaded but be aware this method is very resource intensive and should be used a sparingly as possible.