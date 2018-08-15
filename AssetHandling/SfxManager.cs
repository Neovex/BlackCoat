using System;
using System.Linq;
using System.Collections.Generic;
using SFML.System;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// Simplifies access and management of sound effects
    /// </summary>
    public class SfxManager
    {
        //public static Int32 MAX_SOUNDS_PER_FRAME = 5; // TODO: check necessity of sound overlapping

        private SfxLoader _Loader;
        private Dictionary<String, Sound> _SoundLibrary;


        /// <summary>
        /// Gets or sets the global listener position for spatial sound.
        /// </summary>
        public Vector2f ListenerPosition
        {
            get => new Vector2f(Listener.Position.X, Listener.Position.Z);
            set => Listener.Position = new Vector3f(value.X, 0, value.Y);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SfxManager" /> class.
        /// </summary>
        /// <param name="loader">A <see cref="SfxLoader" /> instance to load the required sound effects</param>
        /// <exception cref="ArgumentNullException">loader</exception>
        public SfxManager(SfxLoader loader)
        {
            _Loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _SoundLibrary = new Dictionary<String, Sound>();
        }


        /// <summary>
        /// Loads all compatible files from a folder into the sound library.
        /// </summary>
        /// <param name="root">Optional: root folder path when different from the default asset root.</param>
        public void LoadFromDirectory(string root = null)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            var oldRoot = _Loader.RootFolder;
            if (root != null) _Loader.RootFolder = root;
            var sounds = _Loader.LoadAllFilesInDirectory();
            Log.Debug(sounds.Length, "Sounds loaded");
            _Loader.RootFolder = oldRoot;

            LoadFromFileList(sounds);
        }

        /// <summary>
        /// Loads the specified files into the sound library.
        /// </summary>
        /// <param name="files">The files to load.</param>
        public void LoadFromFileList(IEnumerable<String> files)
        {
            foreach (var file in files) AddToLibrary(file);
        }

        /// <summary>
        /// Creates a new sound effect.
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="volume">The sounds volume 0-100.</param>
        /// <param name="spatial">Determines if the sound should be handled spatial aka. as a 3D sound</param>
        /// <param name="position">The position of the sound source.</param>
        /// <param name="volumeDropoffStartDistance">The distance where the sounds volume diminishes in PX.</param>
        /// <param name="volumeDropoffFactor">The factor how much the volume will loose strength after passing the drop-off distance 0-100.</param>
        /// <returns>A copy of the loaded sound</returns>
        public ManagedSound Create(String name, float volume = 100, bool spatial = false, Vector2f position = new Vector2f(), float volumeDropoffStartDistance = 500, float volumeDropoffFactor = 10)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            if (!_SoundLibrary.TryGetValue(name, out Sound sound)) // Sound not in library?
            {
                sound = new Sound(_Loader.Load(name)); // -> create
                _SoundLibrary.Add(name, sound);
            }

            return new ManagedSound(sound) // create managed instance from lib sound
            {
                Volume = volume,
                RelativeToListener = !spatial,
                Position = position.ToVector3f(),
                MinDistance = volumeDropoffStartDistance,
                Attenuation = volumeDropoffFactor
            };
        }

        /// <summary>
        /// Adds or updates and entry in  the 
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="volume">The sounds volume 0-100.</param>
        /// <param name="spatial">Determines if the sound should be handled spatial aka. as a 3D sound</param>
        /// <param name="position">The position of the sound source.</param>
        /// <param name="volumeDropoffStartDistance">The distance where the sounds volume diminishes in PX.</param>
        /// <param name="volumeDropoffFactor">The factor how much the volume will loose strength after passing the drop-off distance 0-100.</param>
        /// <returns>A copy of the loaded sound</returns>
        public void AddToLibrary(String name, float volume = 100, bool spatial = false, Vector2f position = new Vector2f(), float volumeDropoffStartDistance = 500, float volumeDropoffFactor = 10)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            if (!_SoundLibrary.TryGetValue(name, out Sound sound)) // Sound not in library?
            {
                sound = new Sound(_Loader.Load(name)); // -> create
                _SoundLibrary.Add(name, sound);
            }

            sound.Volume = volume;
            sound.RelativeToListener = !spatial;
            sound.Position = position.ToVector3f();
            sound.MinDistance = volumeDropoffStartDistance;
            sound.Attenuation = volumeDropoffFactor;
        }

        /// <summary>
        /// Gets the specified sound effect.
        /// </summary>
        /// <param name="name">The name that identifies the sound effect</param>
        /// <returns>The sound effect</returns>
        public ManagedSound Get(String name)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));
            return new ManagedSound(_SoundLibrary[name]);
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="position">Optional position of the sound. Only relevant when sound was created as a spatial sound.</param>
        public void Play(String name, Vector2f? position = null)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            if (_SoundLibrary.TryGetValue(name, out Sound sound))
            {
                sound = new ManagedSound(sound);
                sound.Position = !sound.RelativeToListener && position.HasValue ? position.Value.ToVector3f() : new Vector3f();
                sound.Play();
            }
            else
            {
                AddToLibrary(name, spatial: position.HasValue);
                Play(name, position);
            }
        }
    }
}