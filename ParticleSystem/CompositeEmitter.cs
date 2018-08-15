using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Represents a composition of multiple emitters.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.BaseEmitter" />
    public class CompositeEmitter : BaseEmitter
    {
        protected List<BaseEmitter> _Emitters;
        private ParticleEmitterHost _Host;
        private Vector2f _Position;
        private Single _Rotation;

        /// <summary>
        /// Always returns an empty <see cref="Guid"/> because Composites don't have particles.
        /// </summary>
        public override Guid ParticleTypeGuid => Guid.Empty;

        /// <summary>
        /// Current emitter host managing this composite.
        /// </summary>
        public ParticleEmitterHost Host
        {
            get => _Host;
            set
            {
                if (_Host != null) foreach (var emitter in _Emitters) _Host.Remove(emitter);
                _Host = value;
                if (_Host != null) foreach (var emitter in _Emitters) _Host.AddEmitter(emitter);
            }
        }

        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        public override Vector2f Position
        {
            get => _Position;
            set
            {
                _Position = value;
                foreach (var emitter in _Emitters) emitter.Position = _Position;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of this instance.
        /// </summary>
        public override float Rotation
        {
            get => _Rotation;
            set
            {
                _Rotation = value;
                foreach (var emitter in _Emitters) emitter.Rotation = _Rotation;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeEmitter"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="emitters">Optional initial child emitters.</param>
        public CompositeEmitter(Core core, IEnumerable<BaseEmitter> emitters = null):base(core)
        {
            _Emitters = emitters?.ToList() ?? new List<BaseEmitter>();
        }


        /// <summary>
        /// Adds an emitter to the composition.
        /// </summary>
        /// <param name="emitter">The emitter to add</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        public void Add(PixelEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));
            emitter.Composition = this;
            _Emitters.Add(emitter);
            if (_Host != null) _Host.AddEmitter(emitter);
        }

        /// <summary>
        /// Removes the specified emitter.
        /// </summary>
        /// <param name="emitter">The emitter to remove</param>
        /// <exception cref="ArgumentNullException">emitter</exception>
        /// <exception cref="ArgumentException">emitter</exception>
        public void Remove(PixelEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));
            if (emitter.Composition != this) throw new ArgumentException(nameof(emitter));
            emitter.Composition = null;
            _Emitters.Remove(emitter);
            if (_Host != null) _Host.Remove(emitter);
        }

        internal override void UpdateInternal(float deltaT)
        {
            // not needed - child emitters are updated by the emitter host
        }

        protected override void Update(float deltaT)
        {
            // not needed - child emitters are updated by the emitter host
        }
    }
}