using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public class CompoundEmitter : BaseEmitter
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


        public CompoundEmitter(IEnumerable<BaseEmitter> emitters = null)
        {
            _Emitters = emitters?.ToList() ?? new List<BaseEmitter>();
        }


        public void Add(PixelEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));
            if (IsTriggered) throw new InvalidStateException();
            emitter.CompoundParent = this;
            _Emitters.Add(emitter);
        }

        public void Remove(PixelEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));
            if (emitter.CompoundParent != this) throw new ArgumentException(nameof(emitter));
            if (IsTriggered) throw new InvalidStateException();
            emitter.CompoundParent = null;
            _Emitters.Remove(emitter);
        }


        protected override void Triggered()
        {
            foreach (var emitter in _Emitters) emitter.Trigger();
        }

        protected override void UpdateInternal(float deltaT)
        {
            // not needed -  child emitters are updated by the emitter host
        }
    }
}