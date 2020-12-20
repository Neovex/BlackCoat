using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Basic abstract implementation of IEntity
    /// </summary>
    public abstract class EntityBase : BlackCoatBase, IEntity
    {
        // Variables #######################################################################
        private Container _Parent;
        private Boolean _Visible;
        private float _Alpha;
        private View _View;
        private RenderTarget _RenderTarget;


        // Properties ######################################################################
        public abstract Vector2f Position { get; set; }
        /// <summary>
        /// Gets the position of this <see cref="IEntity"/> independent from scene graph and view.
        /// </summary>
        public Vector2f GlobalPosition => Position - Origin.MultiplyBy(Scale) + (Parent == null ? default : Parent.GlobalPosition);
        public abstract float Rotation { get; set; }
        public abstract Vector2f Scale { get; set; }
        public abstract Vector2f Origin { get; set; }

        /// <summary>
        /// Determines the Visibility of the Entity
        /// </summary>
        public virtual Boolean Visible
        {
            get => _Visible && (_Parent == null || _Parent.Visible);
            set => _Visible = value;
        }

        /// <summary>
        /// Tint Color
        /// </summary>
        public abstract Color Color { get; set; }
        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public Single Alpha
        {
            get => _Alpha;
            set
            {
                _Alpha = MathHelper.Clamp(value, 0, 1);
                AlphaChanged();
            }
        }

        /// <summary>
        /// Global Alpha Visibility according to the scene graph
        /// </summary>
        public virtual Single GlobalAlpha => _Alpha * (Parent == null ? 1 : _Parent.GlobalAlpha);

        /// <summary>
        /// Name of the <see cref="IEntity" />
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        public Container Parent
        {
            get => _Parent;
            set => _Parent = value == null || !value.Contains(this) ? value : _Parent;
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public virtual View View
        {
            get => _View ?? (Parent is PrerenderedContainer ? null : _Parent?.View);
            set => _View = value;
        }

        /// <summary>
        /// Renderstate of the entity
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget
        {
            get => _RenderTarget ?? _Parent?.RenderTarget;
            set => _RenderTarget = value;
        }

        /// <summary>
        /// Blending method used for Rendering
        /// </summary>
        public virtual BlendMode BlendMode
        {
            get => RenderState.BlendMode;
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
            get => RenderState.Shader;
            set
            {
                var state = RenderState;
                state.Shader = value;
                RenderState = state;
            }
        }

        /// <summary>
        /// Determines whether this <see cref="IEntity" /> is destroyed.
        /// </summary>
        public abstract bool Disposed { get; }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        protected EntityBase(Core core) : base(core)
        {
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;
        }
        ~EntityBase()
        {
            Dispose(false);
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT) { }

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

        /// <summary>
        /// Called when the alpha value has changed.
        /// </summary>
        protected virtual void AlphaChanged() => Color = Color;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (!Disposed)
            {
                if (disposeManaged && Parent != null && !Parent.Disposed)
                {
                    Parent.Remove(this);
                }
                Parent = null;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.CreateIdString();
    }







    /// <summary>
    /// Basic abstract implementation of IEntity
    /// </summary>
    public abstract class EntityBase<T> : EntityBase where T : Transformable, Drawable
    {
        // Variables #######################################################################
        private readonly T _Target;


        // Properties ######################################################################
        protected T Target => Disposed ? throw new ObjectDisposedException(nameof(EntityBase<T>)) : _Target;
        public override Vector2f Position { get => Target.Position; set => Target.Position = value; }
        public override float Rotation { get => Target.Rotation; set => Target.Rotation = value; }
        public override Vector2f Scale { get => Target.Scale; set => Target.Scale = value; }
        public override Vector2f Origin { get => Target.Origin; set => Target.Origin = value; }

        /// <summary>
        /// Determines whether this <see cref="IEntity" /> is destroyed.
        /// </summary>
        public override bool Disposed => _Target.CPointer == IntPtr.Zero;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase{T}"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="target">The SFML target.</param>
        protected EntityBase(Core core, T target) : base(core)
        {
            _Target = target ?? throw new ArgumentNullException(nameof(target));
            Color = Color.White;
        }


        // Methods #########################################################################
        /// <summary>
        /// Renders the <see cref="IEntity"/> into the scene.
        /// </summary>
        /// <param name="target">Render device</param>
        /// <param name="states">Additional render information</param>
        public override void Draw(RenderTarget target, RenderStates states) => target.Draw(Target, states);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManaged)
        {
            base.Dispose(disposeManaged);
            if (!Disposed) Target.Dispose();
        }
    }
}