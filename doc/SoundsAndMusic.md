# Sounds and Music

Now that you got the scenes and entities under your control it is time to add some feel to your project. While modern games often focus too much on graphics,  music and sound is the secret to bring even the most plain scene to life. Lets get started:

## Music

Apart from the asset management Black Coat was not required to add much to the sound capabilities that are already provided by SFML. When your familiar with loading textures, creating `Music` instances is a breeze:
```
var myMusic = MusicLoader.Load("musicFile");
myMusic.Play();
```
The `Volume` property of your `Music` instance can be changed at any time. Values range from 0 - complete silence, to 100 - maximum volume of the original file. If you just want to keep the music going, i.e: for the background music in a level, set the `Loop` property to `true`.

## Sounds

Sounds or sound effects are short snippets of sound and used mostly for non musical purposes like steps, gunfire, acoustic notifications or UI feedback. In Black Coat you can either manage all sounds yourself or use the `SfxManager`.

### Manually managing sounds

If you only have a few sounds or simply like to keep tabs on your sounds yourself you can easily do so.
```
var soundBuffer = SfxLoader.Load("mySoundFile");
var sound = new Sound(soundBuffer);
sound.Play();
```
The relationship between `SoundBuffer` and `Sound` is similar to that between a `Texture` and a `Graphic`. While the first always defines the "what" the latter defines the "how". This way you can play one and the same buffer at a different pitch and volume defined by each sound envelope. For more information on the internal workings on sounds have a look at the original tutorial on Sound and Music for SFML [here](https://www.sfml-dev.org/tutorials/2.5/audio-sounds.php).

### Automatic sound management

If the manual approach does not match your taste you can make your life easier by utilizing Black Coats `SfxManager`. It manages all `SoundBuffer` and `Sound` instances internally leaving you with simple method calls for control like so:
```
var soundManager = new SfxManager(SfxLoader, () => 100);
soundManager.AddToLibrary("mySoundFile", 1);
soundManager.Play("mySoundFile");
```
As you can see the constructor of an `SfxManager` requires a callback from where it can read the default volume for each sound. In this example it is a simple constant value but it might as well be read from the settings like so: `() => Settings.Default.SfxVolume`. 

To play a sound it must be added to the managers sound library so make sure to load everything you need first. Using the `LoadFromDirectory()` method or the `LoadFromFileList()` method can help to prevent missing files.

Apart from making your life easier, the job of the `SfxManager` is to prevent sound stacking. Sound stacking is the effect of a single sound played multiple times within a single frame cycle. If this happens the sounds amplitudes are added together by your sound hardware creating a deafening noise. This is why you have to provide the `SfxManager` with the maximum amount of parallel sounds whenever you add one to the library. The higher the probability of a sound the lower the amount of parallel sounds should be. My recommendation is to set all sound effects to a parallel value of 1 and only raise it when you hear a sound not properly playing.

### 3D Sound

Thanks to SFML you can place your sound effects inside an artificial listening space. By either moving sounds or the listener inside it you can create full 3D sound effects.

#### Manual

To use 3d sound on manually managed sounds make sure your sound is mono then simply set the `RelativeToListener` property to `true` and place it with its `Position` property. The `Listener` is a static SFML class which has a `Position` property as well. Since this is the unmodified functionality of SFML I recommend taking a look at its original tutorial on audio spatialization [here](https://www.sfml-dev.org/tutorials/2.5/audio-spatialization.php).

#### Managed

When using the `SfxManager` any sound that is mono can be made spatial by simply defining its position when calling `Play()` like so:
```
soundManager.ListenerPosition = new Vector2f(0, 5);
soundManager.Play("mySoundFile", new Vector2f(10, 10));
```
As you may notice, both positions are only 2 dimensional although one could define spatial sound in 3 dimensions. Since Black Coat is a 2D engine all positions are being mapped automatically into 2D space for you. When you need the 3rd dimension or need to change the position of a sound while it is playing you have to resort to the manual approach at this time.