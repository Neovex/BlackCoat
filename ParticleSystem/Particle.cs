using System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public class Particle : GraphicItem
    {
        // Constants #######################################################################
        private const double DEG_TO_RAD = Math.PI / -180;


        // Variables #######################################################################
        // System
        protected Emitter _Emitter;
        protected Single _TTL = 0;
        // Movement
        protected Single _Speed = 0;
        protected Single _SpeedVelocity = 0;
        protected Single _Direction = 0;
        protected Single _DirectionVelocity = 0;
        // Blending
        protected Single _Blend = 0;
        // Rotation
        protected Single _RotationRate = 0;
        // Scaling
        protected Single _Transform = 0;


        // Properties ######################################################################
        /// <summary>
        /// Particle Image
        /// </summary>
        public new Image Image
        {
            set
            {
                base.Image = value;
                if(value != null) Center = new Vector2(value.Width / 2, value.Height / 2);
            }
        }

        /// <summary>
        /// Determines if the Particle is alive. Read Only.
        /// </summary>
        public virtual Boolean Alive { get { return _TTL > 0; } }

        /// <summary>
        /// Remaining Lifetime of the Particle. Read and Write.
        /// </summary>
        public virtual Single TTL
        {
            get { return _TTL; }
            set { _TTL = value; }
        }

        /// <summary>
        /// Movement Speed of the Particle. Read and Write.
        /// </summary>
        public virtual Single Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        /// <summary>
        /// Speed Modifier of the Particle. Read and Write.
        /// </summary>
        public virtual Single SpeedVelocity
        {
            get { return _SpeedVelocity; }
            set { _SpeedVelocity = value; }
        }

        /// <summary>
        /// Movement Direction of the Particle. Read and Write.
        /// </summary>
        public virtual Single Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        /// <summary>
        /// Direction Modifier of the Particle. Read and Write.
        /// </summary>
        public virtual Single DirectionVelocity
        {
            get { return _DirectionVelocity; }
            set { _DirectionVelocity = value; }
        }

        /// <summary>
        /// Alphablending of the Particle. Read and Write.
        /// </summary>
        public virtual Single Blend
        {
            get { return _Blend; }
            set { _Blend = value; }
        }

        /// <summary>
        /// Particle Rotation. Read and Write.
        /// </summary>
        public virtual Single RotationRate
        {
            get { return _RotationRate; }
            set { _RotationRate = value; }
        }

        /// <summary>
        /// Particle Scale. Read and Write.
        /// </summary>
        public virtual Single Transform
        {
            get { return _Transform; }
            set { _Transform = value; }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new  Instance of the Particle class.
        /// </summary>
        /// <param name="emitter">Parent Emitter of the Particle</param>
        /// <param name="core">Black Coat Engine Core</param>
        public Particle(Emitter emitter, Core core):base(core)
        {
            if (emitter == null) throw new ArgumentNullException("emitter");
            _Emitter = emitter;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Particle
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        public override void Update(Single deltaT)
        {
            if (Alive)
            {
                // TTL
                _TTL -= deltaT;
                // Movement
                Vector2 pos = Position;
                pos.X += (float)(Math.Cos(_Direction * DEG_TO_RAD) * _Speed * deltaT);
                pos.Y += (float)(Math.Sin(_Direction * DEG_TO_RAD) * _Speed * deltaT);
                Position = pos;

                //Blending
                Alpha += _Blend * deltaT;

                //Rotation
                Rotation += _RotationRate * deltaT;

                //Scaleing
                Vector2 scale = Scale; //check
                scale.X += _Transform * deltaT;
                scale.Y += _Transform * deltaT;
                Scale = scale;

                //Velocity calculation
                _Speed += SpeedVelocity * deltaT;
                _Direction += _DirectionVelocity * deltaT;
            }
            else
            {
                _Emitter.DeactivateParticle(this);
            }
        }
    }
}