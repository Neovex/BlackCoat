using System;
using System.Collections.Generic;

using SFML.Graphics;

using BlackCoat.Collision;


namespace BlackCoat.Entities
{
    /// <summary>
    /// Renders Text onto the Scene
    /// </summary>
    public class TextItem : Text, IEntity, ICollidable
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Boolean _Visible;
        private View _View;
        private float _Alpha;


        // Properties ######################################################################
        /// <summary>
        /// Text to Render
        /// </summary>
        public String Text
        {
            get { return DisplayedString; }
            set { DisplayedString = value; }
        }

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
        public Boolean Visible
        {
            get { return _Visible && (_Parent == null || _Parent.Visible); }
            set { _Visible = value; }
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public View View
        {
            get { return _View ?? _Parent?.View; }
            set { _View = value; }
        }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public Single Alpha
        {
            get { return _Alpha * (Parent == null ? 1 : _Parent.Alpha); }
            set { _Alpha = value; }
        }

        /// <summary>
        /// Renderstate of the entity
        /// </summary>
        public RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Blend method for Rendering
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

        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public ICollisionShape CollisionShape { get; set; }


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="TextItem"/> class.
        /// </summary>
        /// <param name="core">Black Coat Engine Core.</param>
        /// <param name="font">Initial font.</param>
        public TextItem(Core core, Font font = null)
        {
            _Core = core;
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;
            Font = font ?? _Core.DefaultFont;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="TextItem"/>.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game time</param>
        public virtual void Update(Single deltaT)
        {
        }

        /// <summary>
        /// Draws the Text
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }
    }
}