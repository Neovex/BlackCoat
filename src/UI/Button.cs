using System;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Represents a simple, empty UI Button
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    public class Button : Canvas
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when the <see cref="Button"/> is pressed down.
        /// </summary>
        public event Action<Button> Pressed = b => { };

        /// <summary>
        /// Initialization helper for the <see cref="Pressed"/> event.
        /// </summary>
        public Action<Button> InitPressed { set => Pressed += value; }

        /// <summary>
        /// Occurs when the <see cref="Button"/> is released.
        /// </summary>
        public event Action<Button> Released = b => { };

        /// <summary>
        /// Initialization helper for the <see cref="Released"/> event.
        /// </summary>
        public Action<Button> InitReleased { set => Released += value; }


        // CTOR #############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="size">Optional initial size of the <see cref="Button" />.</param>
        /// <param name="components">Optional <see cref="UIComponent" />s for functional construction.</param>
        public Button(Core core, Vector2f? size = null, params UIComponent[] components) : base(core, size, components)
        {
            CanFocus = true;
        }


        // Methods #########################################################################
        /// <summary>
        /// Invokes the released event.
        /// </summary>
        protected virtual void InvokeReleased() => Released.Invoke(this);

        /// <summary>
        /// Invokes the pressed event.
        /// </summary>
        protected virtual void InvokePressed() => Pressed.Invoke(this);

        /// <summary>
        /// Occurs before a confirm event.
        /// </summary>
        protected override void HandleInputBeforeConfirm() => InvokePressed();

        /// <summary>
        /// Occurs when the user confirms an operation. I.e.: Clicks on a button.
        /// </summary>
        protected override void HandleInputConfirm() => InvokeReleased();
    }
}