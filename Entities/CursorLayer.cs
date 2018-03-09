using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities
{
    public class CursorLayer : Layer
    {
        private Graphic Cursor;

        // CTOR ############################################################################
        internal CursorLayer(Core core) : base(core)
        {
            AddChild(Cursor = new Graphic(_Core) { Visible = false });
        }


        // Methods #########################################################################
        public override void Update(Single deltaT)
        {
            base.Update(deltaT);
            Cursor.Position = Input.MousePosition;
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
                Cursor.Visible = false;
            }
            else
            {
                Cursor.Texture = texture;
                Cursor.Origin = origin;
                Input.MouseVisible = false;
                Cursor.Visible = true;
            }
        }
    }
}