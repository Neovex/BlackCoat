using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    public class CursorLayer : Layer
    {
        // Variables #######################################################################
        private Graphic _Cursor;
        private Input _Input;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="CursorLayer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        internal CursorLayer(Core core) : base(core)
        { }


        // Methods #########################################################################
        /// <summary>
        /// Keeps the cursor replacement graphic in sync with the actual cursor position.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
        public override void Update(Single deltaT)
        {
            if (_Cursor != null) _Cursor.Position = _Input.MousePosition;
        }

        /// <summary>
        /// Replaces the system cursor with a custom graphic.
        /// </summary>
        /// <param name="input">The current input source.</param>
        /// <param name="texture">The texture to replace the cursor.</param>
        /// <param name="origin">The optional origin of the texture.</param>
        /// <exception cref="ObjectDisposedException">CursorLayer</exception>
        /// <exception cref="ArgumentNullException">input, texture</exception>
        public void SetCursor(Input input, Texture texture, Vector2f origin = default)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(CursorLayer));
            if (input == null) throw new ArgumentNullException(nameof(input));
            else if (input != _Input) RestoreSystemCursor();
            _Input = input;

            if (texture == null) throw new ArgumentNullException(nameof(texture));
            if(_Cursor == null) Add(_Cursor = new Graphic(_Core));

            _Cursor.Texture = texture;
            _Cursor.Origin = origin;
            _Input.MouseVisible = false;
            _Cursor.Visible = true;
        }

        /// <summary>
        /// Restores the system cursor.
        /// </summary>
        public void RestoreSystemCursor()
        {
            if (_Input != null) _Input.MouseVisible = true;
            if (_Cursor != null) _Cursor.Visible = false;
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManaged)
        {
            if (disposeManaged) RestoreSystemCursor();
            base.Dispose(disposeManaged);
        }
    }
}