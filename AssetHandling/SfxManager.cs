using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Audio;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Simplifies access and management of sound effects
    /// </summary>
    public class SfxManager
    {
        public static Int32 MAX_SOUNDS_PER_FRAME = 5; // TODO: check necessity of sound overlapping

        private SfxLoader _Loader;
        private Dictionary<String, Sound> _Sounds;
        private Dictionary<String, Int32> _ActiveSounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="SfxManager" /> class.
        /// </summary>
        /// <param name="loader">A <see cref="SfxLoader" /> instance to load the required sound effects</param>
        /// <exception cref="ArgumentNullException">loader</exception>
        public SfxManager(SfxLoader loader)
        {
            _Loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _Sounds = new Dictionary<String, Sound>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SfxManager" /> class.
        /// </summary>
        /// <param name="loader">A <see cref="SfxLoader" /> instance to load the required sound effects</param>
        /// <exception cref="ArgumentNullException">loader</exception>
        public SfxManager(Core core, SfxLoader loader)
        {
            if(core == null) throw new ArgumentNullException(nameof(core));
            _Loader = loader ?? throw new ArgumentNullException(nameof(loader));

            _Sounds = new Dictionary<String, Sound>();
            _ActiveSounds = new Dictionary<String, Int32>();

            core.OnUpdate += Update;
        }

        /// <summary>
        /// Loads the specified files into an internal buffer for faster access.
        /// </summary>
        /// <param name="files">The files to load.</param>
        public void LoadFromDirectory(string root = null)
        {
            var oldRoot = _Loader.RootFolder;
            if (root != null) _Loader.RootFolder = root;
            var sounds = _Loader.LoadAllFilesInDirectory();
            Log.Debug(sounds.Length, "Sounds loaded");
            _Loader.RootFolder = oldRoot;
        }

        /// <summary>
        /// Loads the specified files into an internal buffer for faster access.
        /// </summary>
        /// <param name="files">The files to load.</param>
        public void LoadFromFileList(IEnumerable<String> files)
        {
            foreach (var file in files) _Loader.Load(file);
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
        public Sound Create(String name, float volume = 100, bool spatial = false, Vector2f position = new Vector2f(), float volumeDropoffStartDistance = 500, float volumeDropoffFactor = 10)
        {
            var copy = false;
            if (_Sounds.TryGetValue(name, out Sound sound))
            {
                sound = new Sound(sound); // create copy from entry
            }
            else
            {
                sound = new Sound(_Loader.Load(name));// name conflicts & usage conflicts recheck this
                _Sounds.Add(name, sound);
                copy = true;
            }

            // Adjust sound
            sound.Volume = volume;
            sound.RelativeToListener = !spatial;
            sound.Position = ConvertTo3D(position);
            sound.MinDistance = volumeDropoffStartDistance;
            sound.Attenuation = volumeDropoffFactor;

            return copy ? new Sound(sound) : sound;
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="position">Optional position of the sound source. Only relevant when sound was created as a spatial sound.</param>
        public void Play(String name, Vector2f? position = null)
        {
            if (_Sounds.TryGetValue(name, out Sound sound))
            {
                sound = new Sound(sound);
                if (position.HasValue) sound.Position = ConvertTo3D(position.Value);
                //_ActiveSounds[name]++; // FIXME
                sound.Play();
            }
            else
            {
                Log.Warning("Skipped playing of uninitialized sound", name);
            }
        }
        
        private void Update(float obj)
        {
            _ActiveSounds.Clear();
        }

        /// <summary>
        /// Converts a 2d Vector into 3D space.
        /// </summary>
        /// <param name="v">The source vector.</param>
        /// <returns>Converted 3D vector</returns>
        private Vector3f ConvertTo3D(Vector2f v)
        {
            return new Vector3f(v.X, 0, v.Y);
        }
    }
}