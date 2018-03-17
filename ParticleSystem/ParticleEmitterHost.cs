using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackCoat.Entities;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public class ParticleEmitterHost : BaseEntity
    {
        private List<BaseEmitter> _Emitters;
        private SortedList<int, ParticleDepthLayer> _DepthLayers;


        public override Color Color { get; set; }


        public ParticleEmitterHost(Core core) : base(core)
        {
            _Emitters = new List<BaseEmitter>();
            _DepthLayers = new SortedList<int, ParticleDepthLayer>();
        }


        public void AddEmitter(BaseEmitter emitter)
        {
            if (emitter == null) throw new ArgumentNullException(nameof(emitter));

            _Emitters.Add(emitter);
            if (emitter is CompoundEmitter)
            {
                foreach (var e in (emitter as CompoundEmitter).Emitters)
                {
                    AddEmitter(e);
                }
            }
            else if (emitter is PixelEmitter)
            {
                var pix = emitter as PixelEmitter;
                if (!_DepthLayers.ContainsKey(pix.Depth))
                {
                    _DepthLayers.Add(pix.Depth, new ParticleDepthLayer(pix.Depth));
                }
                _DepthLayers[pix.Depth].Add(pix);
            }
        }

        public void Remove(BaseEmitter emitter)
        {
            if(emitter == null) throw new ArgumentNullException(nameof(emitter));
            _Emitters.Remove(emitter);
            if (emitter is CompoundEmitter)
            {
                foreach (var e in (emitter as CompoundEmitter).Emitters)
                {
                    Remove(e);
                }
            }
            else if (emitter is PixelEmitter)
            {
                var pix = emitter as PixelEmitter;
                _DepthLayers[pix.Depth].Remove(pix);
                if (_DepthLayers[pix.Depth].IsEmpty)
                {
                    _DepthLayers.Remove(pix.Depth);
                }
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var layer in _DepthLayers.Values)
            {
                layer.Draw(target, states);
            }
        }

        public override void Update(float deltaT)
        {
            foreach (var emitter in _Emitters)
            {
                emitter.Update(deltaT);
            }
        }
    }
}