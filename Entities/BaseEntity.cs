using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Basic abstract implementation of IEntity
    /// </summary>
    public abstract class BaseEntity: Transformable, IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Boolean _Visible;
        private View _View;
        private float _Alpha;


        // Properties ######################################################################
        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.HasChild(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the Visibility of the Entity
        /// </summary>
        public virtual Boolean Visible
        {
            get { return _Visible && (_Parent == null ? true : _Parent.Visible); }
            set { _Visible = value; }
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public virtual View View
        {
            get { return (_View ?? _Parent?.View) ?? _View; }
            set { _View = value; }
        }

        /// <summary>
        /// Renderstate of the entity
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public virtual RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Entity Color or Tint
        /// </summary>
        public abstract Color Color { get; set; } // TODO: consider removal

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public Single Alpha // TODO: consider removal
        {
            get { return _Alpha; }
            set
            {
                _Alpha = value = Math.Max(0, value);
                if (_Parent != null) value *= _Parent.Alpha;
                var color = Color;
                color.A = (Byte)(value * Byte.MaxValue);
                Color = color;
            }
        }

        /// <summary>
        /// Blending method used for Rendering
        /// </summary>
        public virtual BlendMode BlendMode
        {
            get { return RenderState.BlendMode; }
            set
            {
                var state = RenderState;
                state.BlendMode = value;
                RenderState = state;
            }
        }

        /// <summary>
        /// Shader for Rendering
        /// </summary>
        public virtual Shader Shader
        {
            get { return RenderState.Shader; }
            set
            {
                var state = RenderState;
                state.Shader = value;
                RenderState = state;
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        protected BaseEntity(Core core)
        {
            _Core = core;
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;
            Color = Color.White;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public abstract void Update(Single deltaT);

        /// <summary>
        /// Renders the <see cref="IEntity"/> into the scene.
        /// </summary>
        public virtual void Draw() => _Core.Draw(this);

        /// <summary>
        /// Renders the <see cref="IEntity"/> into the scene.
        /// </summary>
        /// <param name="target">Render device</param>
        /// <param name="states">Additional render information</param>
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}