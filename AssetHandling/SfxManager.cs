using System;
using System.Linq;
using System.Collections.Generic;
using SFML.System;
using SFML.Audio;

namespace BlackCoat.AssetHandling
{
    /// <summary>
    /// Simplifies access and management of sound effects
    /// </summary>
    public class SfxManager
    {
        private readonly SfxLoader _Loader;
        private readonly Dictionary<String, ManagedSound> _SoundLibrary;
        private readonly Func<int> _ReadVolume;


        /// <summary>
        /// Gets or sets the global listener position for spatial sounds.
        /// </summary>
        public Vector2f ListenerPosition
        {
            get => Listener.Position.ToVector2f();
            set => Listener.Position = value.ToVector3f();
        }
        /// <summary>
        /// Gets or sets the initial volume drop-off start distance for spatial sounds.
        /// This defines the maximum distance a sound can still be heard at max volume.
        /// </summary>
        public float VolumeDropoffStartDistance { get; set; }
        /// <summary>
        /// Gets or sets the initial volume drop-off factor for spatial sounds.
        /// Defines how fast the volume drops beyond the <see cref="VolumeDropoffStartDistance"/>
        /// </summary>
        public float VolumeDropoffFactor { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="SfxManager" /> class.
        /// </summary>
        /// <param name="loader">A <see cref="SfxLoader" /> instance to load the required sound effects</param>
        /// <exception cref="ArgumentNullException">loader</exception>
        public SfxManager(SfxLoader loader, Func<int> volume)
        {
            _Loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _SoundLibrary = new Dictionary<String, ManagedSound>();
            _ReadVolume = volume;

            VolumeDropoffStartDistance = 500;
            VolumeDropoffFactor = 10;
        }


        /// <summary>
        /// Loads all compatible files from a folder into the sound library.
        /// </summary>
        /// <param name="root">Optional: root folder path when different from the default asset root.</param>
        /// <param name="parallelSounds">The amount of times each sound can be played in parallel.</param>
        public void LoadFromDirectory(string root = null, int parallelSounds = 2)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            var oldRoot = _Loader.RootFolder;
            if (root != null) _Loader.RootFolder = root;
            var sounds = _Loader.LoadAllFilesInDirectory();
            Log.Debug(sounds.Length, "Sounds loaded");
            _Loader.RootFolder = oldRoot;

            LoadFromFileList(sounds, parallelSounds);
        }

        /// <summary>
        /// Loads the specified files into the sound library.
        /// </summary>
        /// <param name="files">The files to load.</param>
        /// <param name="parallelSounds">The amount of times each sound can be played in parallel.</param>
        public void LoadFromFileList(IEnumerable<String> files, int parallelSounds)
        {
            foreach (var file in files) AddToLibrary(file, parallelSounds);
        }

        /// <summary>
        /// Loads a new entry into the sound library.
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="parallelSounds">The amount of times this sound can be played in parallel.</param>
        public void AddToLibrary(String name, int parallelSounds)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            if (_SoundLibrary.TryGetValue(name, out ManagedSound sound))
            {
                // Replace ManagedSound
                _SoundLibrary.Remove(name);
                sound.Dispose();
                AddToLibrary(name, parallelSounds);
            }
            else
            {
                // Add new ManagedSound
                sound = new ManagedSound(name, _Loader.Load(name), parallelSounds);
                _SoundLibrary.Add(name, sound);
            }
        }

        /// <summary>
        /// Retrieves a sound effect when currently available.
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="position">Defines whether the sound is supposed to be spatial.</param>
        public Sound GetSound(String name, bool spatial = false)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            if (_SoundLibrary.TryGetValue(name, out ManagedSound managedSound))
            {
                var sound = managedSound.GetSound();
                if (sound != null)
                {
                    sound.Volume = _ReadVolume.Invoke();
                    sound.RelativeToListener = !spatial;
                    if (spatial)
                    {
                        sound.MinDistance = VolumeDropoffStartDistance;
                        sound.Attenuation = VolumeDropoffFactor;
                    }
                }
                return sound;
            }

            throw new ArgumentException($"There is no sound named '{name}'");
        }

        /// <summary>
        /// Plays a sound effect when currently available.
        /// </summary>
        /// <param name="name">The name of the sound effect.</param>
        /// <param name="position">Optional position of the sound. Only relevant when sound is supposed to be spatial.</param>
        public void Play(String name, Vector2f? position = null)
        {
            if (_Loader.Disposed) throw new ObjectDisposedException(nameof(_Loader));

            var sound = GetSound(name, position.HasValue);
            if (sound == null) return;
            if (position.HasValue) sound.Position = position.Value.ToVector3f();
            sound.Play();
        }
    }
}