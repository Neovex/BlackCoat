using System;
using System.Collections.Generic;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// Represents a single Sound Effect
    /// </summary>
    /// <seealso cref="SFML.Audio.Sound" />
    public class ManagedSound : Sound
    {
        public const Int32 MAX_INSTANCES = 100;
        private static Queue<ManagedSound> _INSTANCES = new Queue<ManagedSound>(MAX_INSTANCES);
        private static void MANAGE_INSTANCE(ManagedSound newInstance)
        {
            while (_INSTANCES.Count >= MAX_INSTANCES)
            {
                var oldInstance = _INSTANCES.Dequeue();
                if (oldInstance.Unmanaged) _INSTANCES.Enqueue(oldInstance);
                else oldInstance.Dispose();
            }
            _INSTANCES.Enqueue(newInstance);
        }


        /// <summary>
        /// Determines whether this <see cref="ManagedSound"/> has been destroyed.
        /// </summary>
        public bool Destroyed { get; private set; }

        /// <summary>
        /// Determines whether this <see cref="ManagedSound"/> is unmanaged.
        /// </summary>
        public bool Unmanaged { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedSound"/> class.
        /// </summary>
        /// <param name="buffer">Sound buffer containing the audio data to play with the sound instance</param>
        internal ManagedSound(SoundBuffer buffer) : base(buffer)
        {
            MANAGE_INSTANCE(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedSound"/> class.
        /// </summary>
        /// <param name="sound">The sound this instance should be based on</param>
        internal ManagedSound(Sound sound) : base(sound)
        {
            MANAGE_INSTANCE(this);
        }


        /// <summary>
        /// Handle the destruction of the object
        /// </summary>
        /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
        protected override void Destroy(bool disposing)
        {
            Destroyed = true;
            base.Destroy(disposing);
        }
    }
}