using SFML.Graphics;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>Represents a simple graphic object within a UI.</summary>
    /// <seealso cref="BlackCoat.UI.UIComponent" />
    public class UIGraphic : UIComponent
    {
        /// <summary>
        /// Gets the inner size of this <see cref="UIComponent"/>.
        /// </summary>
        public override Vector2f InnerSize => Texture != null ? Texture.Size.ToVector2f().MultiplyBy(Scale) : default(Vector2f);

        /// <summary>
        /// Initializes a new instance of the <see cref="UIGraphic"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        /// <param name="texture"><see cref="Texture"/> to be displayed with this instance</param>
        public UIGraphic(Core core, Texture texture) : base(core)
        {
            Texture = texture;
        }
    }
}