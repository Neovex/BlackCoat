using SFML.System;
using BlackCoat;
using BlackCoat.ParticleSystem;

namespace Particles
{
    class FireworkSpawnInfo : ParticleSpawnInfo
    {
        private Core _Core;

        public override Vector2f Velocity
        {
            get => Create.Vector2fFromAngleLookup(_Core.Random.NextFloat(0, 359), 
                                                  _Core.Random.NextFloat(5,  50));
            set => base.Velocity = value;
        }

        public FireworkSpawnInfo(Core core) => _Core = core;
    }
}