using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public abstract class BaseEmitter
    {
        protected static Dictionary<Guid, Stack<BasicParticle>> INSTANCE_POOL { get; }
        static BaseEmitter() => INSTANCE_POOL = new Dictionary<Guid, Stack<BasicParticle>>();


        protected List<BasicParticle> _Particles;


        /// <summary>
        /// Determines if the Emitter has been triggered
        /// </summary>
        public virtual Boolean IsTriggered { get; protected set; }
        
        public CompoundEmitter CompoundParent { get; internal set; }
        public abstract Vector2f Position { get; set; }
        public abstract float Rotation { get; set; }


        public BaseEmitter()
        {
            _Particles = new List<BasicParticle>();
        }

        /// <summary>
        /// Notifies the Emitter to begin emitting particles.
        /// </summary>
        public void Trigger()
        {
            IsTriggered = true;
            Triggered();
        }

        /// <summary>
        /// Updates the Emitter.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public void Update(Single deltaT)
        {
            UpdateInternal(deltaT);
            /*for (int i = _Particles.Count - 1; i >= 0; i--)
            {
                if (_Particles[i].Update(deltaT))
                {
                    //AddToCache(_Particles[i]);
                    _Particles[i].Free();
                    _Particles.RemoveAt(i);
                }
            }*/
            for (int i = 0; i < _Particles.Count; i++)
            {
                var particle = _Particles[i];
                if (particle.TTL > 0) particle.Update(deltaT);
            }
        }
        
        protected void Destroy()
        {
            for (int i = 0; i < _Particles.Count; i++)
            {
                AddToCache(_Particles[i]);
            }
            _Particles.Clear();
        }

        protected abstract void Triggered();
        protected abstract void UpdateInternal(float deltaT);


        // Cache Handling
        protected void AddToCache(BasicParticle particle)
        {
            Guid typeId = GetType().GUID;
            if (!INSTANCE_POOL.ContainsKey(typeId))
            {
                INSTANCE_POOL.Add(typeId, new Stack<BasicParticle>());
            }
            INSTANCE_POOL[typeId].Push(particle);
            particle.Free();
        }
        protected BasicParticle RetrieveFromCache()
        {
            Guid typeId = GetType().GUID;
            if (!INSTANCE_POOL.ContainsKey(typeId)) return null;
            var pool = INSTANCE_POOL[typeId];
            if (pool.Count == 0) return null;
            return pool.Pop();
        }
    }
}