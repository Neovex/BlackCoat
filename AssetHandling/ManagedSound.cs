using System;
using SFML.Audio;

namespace BlackCoat.AssetHandling
{
    /// <summary>
    /// Represents a single Sound Effect
    /// </summary>
    public class ManagedSound : IDisposable
    {
        private SoundBuffer _Buffer;
        private Sound[] _Sounds;

        /// <summary>
        /// Gets the name of this <see cref="ManagedSound"/>.
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Determines whether this <see cref="ManagedSound"/> has been disposed.
        /// </summary>
        public bool Disposed { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedSound" /> class.
        /// </summary>
        /// <param name="name">The sounds name</param>
        /// <param name="soundBuffer">Sound buffer containing the audio data to play with the sound instance</param>
        /// <param name="parallelSounds">The maximum number of parallel playing sounds.</param>
        public ManagedSound(String name, SoundBuffer soundBuffer, int parallelSounds)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException($"Invalid {nameof(name)}:{name}");
            Name = name;
            _Buffer = soundBuffer ?? throw new ArgumentNullException(nameof(soundBuffer));
            _Sounds = new Sound[MathHelper.Clamp(parallelSounds, 1, 25)];
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="ManagedSound" /> class.
        /// </summary>
        ~ManagedSound()
        {
            Dispose(false);
        }


        /// <summary>
        /// Plays the sound.
        /// </summary>
        public Sound GetSound()
        {
            if (Disposed) throw new ObjectDisposedException(Name);

            for (int i = 0; i < _Sounds.Length; i++)
            {
                var sound = _Sounds[i];
                if (sound == null) _Sounds[i] = sound = new Sound(_Buffer);
                if (sound.Status != SoundStatus.Stopped) continue;
                return sound;
            }
            return null; // when all sounds are busy none shall be added
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < _Sounds.Length; i++)
                    {
                        if (_Sounds[i] != null)
                        {
                            _Sounds[i].Dispose();
                            _Sounds[i] = null;
                        }
                    }
                }
                _Sounds = null;
                _Buffer = null;
                Disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}