using System;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Renders Text onto the Scene
    /// </summary>
    public class TextItem : EntityBase<Text>, ICollidable
    {
        // Variables #######################################################################
        private ICollisionShape _CollisionShape;


        // Properties ######################################################################
        /// <summary>
        /// Text to Render
        /// </summary>
        public String Text { get => Target.DisplayedString; set => Target.DisplayedString = value; }

        /// <summary>
        /// Font Size
        /// </summary>
        public uint CharacterSize { get => Target.CharacterSize; set => Target.CharacterSize = value; }

        /// <summary>
        /// Font used to display the text
        /// </summary>
        public Font Font { get => Target.Font; set => Target.Font = value; }

        /// <summary>
        /// Style of the text <see cref="Text.Styles"/>
        /// </summary>
        public Text.Styles Style { get => Target.Style; set => Target.Style = value; }

        /// <summary>
        /// Text Color
        /// </summary>
        public override Color Color { get => Target.FillColor; set => Target.FillColor = value.ApplyAlpha(GlobalAlpha); }

        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public ICollisionShape CollisionShape
        {
            get => _CollisionShape ?? (_CollisionShape = new BasicTextCollisionShape(_Core.CollisionSystem, this));
            set => _CollisionShape = value;
        }

        /// <summary>
        /// Absolute bounds of the <see cref="TextItem"/>
        /// </summary>
        public FloatRect GlobalBounds => Target.GetGlobalBounds();

        /// <summary>
        /// Local bounds of the <see cref="TextItem"/>
        /// </summary>
        public FloatRect LocalBounds => Target.GetLocalBounds();


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="TextItem" /> class.
        /// </summary>
        /// <param name="core">Black Coat Engine Core.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="characterSize">The size of the texts characters.</param>
        /// <param name="font">Initial font.</param>
        public TextItem(Core core, String text = "", uint characterSize = 16, Font font = null) :
                        base(core, new Text(text, font ?? core.DefaultFont, characterSize))
        {
        }


        // Methods #########################################################################
        /// <summary>
        /// Returns the visual position of the Index-th character of the text, in coordinates relative to the text.
        /// </summary>
        /// <remarks>The translation, origin, rotation and scale of the source instance are ignored.</remarks>
        /// <param name="index">Index of the character</param>
        /// <returns>Position of the Index-th character (end of text if Index is out of range)</returns>
        public Vector2f FindCharacterPos(uint index) => Target.FindCharacterPos(index);
    }
}