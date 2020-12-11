using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    public class CursorLayer : Layer
    {
        // Variables #######################################################################
        private readonly Graphic _Cursor;


        // Properties ######################################################################
        public Input Input { get; set; }


        // CTOR ############################################################################
        internal CursorLayer(Core core, Input input) : base(core)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Add(_Cursor = new Graphic(_Core) { Visible = false });
        }


        // Methods #########################################################################
        public override void Update(Single deltaT)
        {
            base.Update(deltaT);
            _Cursor.Position = Input.MousePosition;
        }

        /// <summary>
        /// Replaces the system cursor with a texture or restores the original.
        /// </summary>
        /// <param name="texture">The texture to replace the cursor or null to restore system default.</param>
        /// <param name="origin">The optional origin of the texture.</param>
        public void SetCursor(Texture texture, Vector2f origin = new Vector2f())
        {
            if (texture == null)
            {
                Input.MouseVisible = true;
                _Cursor.Visible = false;
            }
            else
            {
                _Cursor.Texture = texture;
                _Cursor.Origin = origin;
                Input.MouseVisible = false;
                _Cursor.Visible = true;
            }
        }
    }
}