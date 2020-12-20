using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Abstract base class of all Shape based Entities
    /// </summary>
    public abstract class ShapeEntity<T> : EntityBase<T>, ICollidable, ICollisionShape where T : Shape
    {
        // Variables #######################################################################
        private bool _Filled;
        private Color _FillColor;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ShapeEntity"/> is filled.
        /// </summary>
        public bool Filled { get => _Filled; set => Target.FillColor = (_Filled = value) ? _FillColor : Color.Transparent; }

        /// <summary>
        /// Gets or sets the fill color of the <see cref="ShapeEntity"/>.
        /// </summary>
        public Color FillColor
        {
            get => _FillColor;
            set
            {
                _FillColor = value.ApplyAlpha(GlobalAlpha);
                if (Filled) Target.FillColor = _FillColor;
            }
        }

        /// <summary>
        /// Gets or sets the color of the <see cref="ShapeEntity"/>.
        /// </summary>
        public override Color Color
        {
            get => Filled ? FillColor : Outlined ? OutlineColor : Color.Transparent;
            set => FillColor = OutlineColor = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ShapeEntity"/> is outlined.
        /// </summary>
        public bool Outlined { get => OutlineThickness != 0; set => OutlineThickness = !value ? 0 : !Outlined ? 1 : OutlineThickness; }

        /// <summary>
        /// Gets or sets the outline color of the <see cref="ShapeEntity"/>.
        /// </summary>
        public Color OutlineColor { get => Target.OutlineColor; set => Target.OutlineColor = value.ApplyAlpha(GlobalAlpha); }

        /// <summary>
        /// Gets or sets the outline thickness of the <see cref="ShapeEntity"/>.
        /// </summary>
        public float OutlineThickness { get => Target.OutlineThickness; set => Target.OutlineThickness = value; }

        /// <summary>
        /// Determines the collision shape for collision detection
        /// </summary>
        public virtual ICollisionShape CollisionShape => this;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public abstract Geometry CollisionGeometry { get; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="ShapeEntity" /> base
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="target">The target shape</param>
        /// <param name="fillColor">Color of the shapes area.</param>
        /// <param name="outlineColor">Color of the shapes outline.</param>
        public ShapeEntity(Core core, T target, Color? fillColor = null, Color? outlineColor = null) : base(core, target)
        {
            if (Filled = fillColor.HasValue) FillColor = fillColor.Value;
            if (Outlined = outlineColor.HasValue) OutlineColor = outlineColor.Value;
        }


        // Methods #########################################################################
        /// <summary>
        /// Called when the alpha value has changed.
        /// </summary>
        protected override void AlphaChanged()
        {
            OutlineColor = OutlineColor;
            FillColor = FillColor;
        }


        // Collision Implementation ########################################################
        /// <summary>
        /// Determines if this <see cref="Circle"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="Circle"/></returns>
        public virtual bool CollidesWith(Vector2f point) => _Core.CollisionSystem.CheckCollision(point, this);

        /// <summary>
        /// Determines if this <see cref="Circle"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool CollidesWith(ICollisionShape other) => _Core.CollisionSystem.CheckCollision(this, other);
    }
}