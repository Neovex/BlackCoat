using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Audio;
using SFML.System;

namespace BlackCoat
{
    class SfxManager
    {
        private SfxLoader _Loader;
        private List<String> _AvailableSfx;
        private Dictionary<String, Sound> _Sounds;

        public SfxManager(SfxLoader loader)
        {
            _Loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _AvailableSfx = new List<String>();
            _Sounds = new Dictionary<String, Sound>();
        }

        public void LoadBuffers(IEnumerable<String> files)
        {
            // Files are loaded and managed by the loader
            _AvailableSfx.AddRange(files.Where(f => _Loader.Load(f) != null));
        }

        public Sound Load(String name, float volume = 100, bool spatial = false, Vector2f position = new Vector2f(), float volumeDropoffStartDistance = 500, float volumeDropoffFactor = 10)
        {


            var sound = new Sound(_Loader.Load(name))
            {
                Volume = volume,
                RelativeToListener = !spatial,
                Position = Convert(position),
                MinDistance = volumeDropoffStartDistance,
                Attenuation = volumeDropoffFactor
            };
            _Sounds.Add(name, sound);
            sound.Dispose sounds müssen wie buffers behandelt werden hmk... still open: externally managed sounds or sounds with regular updates - idee: managed sound für id/name & event Extensions?
        }

        public void Play(String name, Vector2f? position = null)
        {
            var sound = new Sound(_Sounds[name]);
            if (position.HasValue) sound.Position = Convert(position.Value);
            sound.Play();
            sound.
        }
        public Sound Get(String name)
        {
            return new Sound(_Sounds[name]);
        }

        private Vector3f Convert(Vector2f v)
        {
            return new Vector3f(v.X, 0, v.Y);
        }
    }
}