using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class CompositeEmitter : BaseEmitter
    {
        private List<BaseEmitter> _Emitters;

        private Vector2f _Position;
        private float _Rotation;


        public IEnumerable<BaseEmitter> Emitters => _Emitters;
        public override Vector2f Position
        {
            get => _Position;
            set
            {
                _Position = value;
                foreach (var emitter in _Emitters) emitter.Position = _Position;
            }
        }
        public override float Rotation
        {
            get => _Rotation;
            set
            {
                _Rotation = value;
                foreach (var emitter in _Emitters) emitter.Rotation = _Rotation;
            }
        }

        public override Guid ParticleTypeGuid => throw new InvalidOperationException();

        public CompositeEmitter(Core core, IEnumerable<BaseEmitter> emitters = null):base(core)
        {
            _Emitters = emitters?.ToList() ?? new List<BaseEmitter>();
        }


        public void Add(PixelEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));
            emitter.Composition = this;
            _Emitters.Add(emitter);
        }

        public void Remove(PixelEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));
            if (emitter.Composition != this) throw new ArgumentException(nameof(emitter));
            emitter.Composition = null;
            _Emitters.Remove(emitter);
        }

        protected override void UpdateInternal(float deltaT)
        {
            // not needed -  child emitters are updated by the emitter host
        }
    }
}