using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.UI
{
    /// <summary>
    /// A dialog of selectable items for use in combination with a <see cref="TextBox"/> to create a comboBox.
    /// </summary>
    /// <example><code>myTextBox.ShowDialog(Layer_Overlay, new ComboBoxDialog(_Core, myTextBox, [items]));</code></example>
    /// <seealso cref="BlackCoat.UI.OffsetContainer" />
    public class ComboBoxDialog : OffsetContainer
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when an item was selected.
        /// </summary>
        public event Action<String> ItemSelected = s => { };


        // Variables #######################################################################
        private TextBox _Target;


        // Properties ######################################################################
        public Color HighlightColor { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxDialog"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="target">The partner <see cref="TextBox"/>.</param>
        /// <param name="items">A <see cref="string"/> collection representing the items of the dropdown list.</param>
        public ComboBoxDialog(Core core, TextBox target, IEnumerable<String> items) : base(core, Orientation.Vertical)
        {
            _Target = target;

            _Background.OutlineColor = Color.Black;
            _Background.OutlineThickness = 1;
            BackgroundColor = Color.White;
            HighlightColor =  Color.Cyan;

            foreach (var item in items)
            {
                var button = new Button(_Core, _Target.InnerSize)
                {
                    InitReleased = ButtonClicked,
                    InitFocusGained = ButtonFocusGained,
                    InitFocusLost = ButtonFocusLost,
                    Tag = item,
                    Init = new[]
                    {
                        new AlignedContainer(_Core, Alignment.CenterLeft, 
                            new Label(_Core, item, target.CharacterSize, target.Font)
                            {
                                TextColor = Color.Black,
                                Padding = new FloatRect(5,0,0,0)
                            }
                        )
                    }
                };
                Add(button);
            }
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="UIComponent" /> and all its entities along the scene graph.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            Position = _Target.GlobalPosition + new Vector2f(0, _Target.InnerSize.Y);
        }

        /// <summary>
        /// Handles when an item was selected
        /// </summary>
        /// <param name="button">The selected button.</param>
        private void ButtonClicked(UIComponent button)
        {
            button.CloseDialog();
            _Target.Text = button.Tag.ToString();
            ItemSelected.Invoke(button.Tag.ToString());
        }

        // Color changes
        private void ButtonFocusGained(UIComponent button) => button.BackgroundColor = HighlightColor;
        private void ButtonFocusLost(UIComponent button) => button.BackgroundColor = BackgroundColor;
    }
}