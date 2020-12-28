using System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Specialized container for handling dialogs.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    internal sealed class DialogContainer : Canvas
    {
        // Variables #######################################################################
        private UIComponent _CallingComponent;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="callingComponent">The <see cref="UIComponent"/> which caused the dialog.</param>
        /// <param name="dialogContent">The content container for this dialog.</param>
        /// <exception cref="ArgumentNullException">callingComponent</exception>
        internal DialogContainer(Core core, UIComponent callingComponent, UIContainer dialogContent) :
                            base(core, core.DeviceSize, dialogContent)
        {
            _CallingComponent = callingComponent ?? throw new ArgumentNullException(nameof(callingComponent));
        }


        // Methods #########################################################################
        /// <summary>
        /// Closes the dialog.
        /// </summary>
        internal void Close()
        {
            if (Disposed) return;
            DIALOG = null;
            _CallingComponent.GiveFocus();
            _CallingComponent = null;
            Clear();
            Dispose();
        }

        /// <summary>
        /// Occurs when the user wants to chancel the current operation or dialog.
        /// </summary>
        protected override void HandleInputCancel() => Close();
    }
}