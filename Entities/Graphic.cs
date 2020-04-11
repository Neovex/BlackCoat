using SFML.Graphics;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Represents a single texture <see cref="IEntity"/> in the Scene
    /// </summary>
    public class Graphic : EntityBase<Sprite>, ICollidable
    {
        // Variables #######################################################################
        private ICollisionShape _CollisionShape;


        // Properties ######################################################################
        /// <summary>
        /// Tint Color
        /// </summary>
        public override Color Color { get => Target.Color; set => Target.Color = value.ApplyAlpha(GlobalAlpha); }
        /// <summary>
        /// Source texture displayed by the <see cref="Graphic"/>
        /// </summary>
        public Texture Texture { get => Target.Texture; set => Target.Texture = value; }
        /// <summary>
        /// Gets or sets the source texture rectangle.
        /// </summary>
        public IntRect TextureRect { get => Target.TextureRect; set => Target.TextureRect = value; }
        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public ICollisionShape CollisionShape
        {
            get => _CollisionShape ?? (_CollisionShape = new BasicGraphicCollisionShape(_Core.CollisionSystem, this));
            set => _CollisionShape = value;
        }
        /// <summary>
        /// Absolute bounds of the <see cref="Graphic"/>
        /// </summary>
        public FloatRect GlobalBounds => Target.GetGlobalBounds();


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        public Graphic(Core core, Texture texture = null):base(core, new Sprite(texture))
        {
        }
    }
}