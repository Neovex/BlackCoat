using SFML.System;
using SFML.Graphics;

namespace BlackCoat.UI
{
    /// <summary>
    /// Represents a simple graphic object within a UI.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIComponent" />
    public class UIGraphic : UIComponent
    {
        // Properties ######################################################################
        /// <summary>
        /// Gets the inner size of this <see cref="UIComponent"/>.
        /// </summary>
        public override Vector2f InnerSize => Texture == null ? default : Texture.Size.ToVector2f().MultiplyBy(Scale);


        // CTOR ############################################################################
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