using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities
{
    public class CursorLayer : Layer
    {
        private Graphic _Pointer;

        // CTOR ############################################################################
        internal CursorLayer(Core core) : base(core)
        {
            AddChild(_Pointer = new Graphic(_Core) { Visible = false });
        }


        // Methods #########################################################################
        public override void Update(Single deltaT)
        {
            _Pointer.Position = Input.MousePosition;
            base.Update(deltaT);
        }

        /// <summary>
        /// Replaces the pointer with a texture or restores the original system pointer.
        /// </summary>
        /// <param name="texture">The texture to replace the pointer or null to restore system default.</param>
        /// <param name="origin">The optional origin of the texture.</param>
        public void SetPointerTexture(Texture texture, Vector2f origin = new Vector2f())
        {
            if (texture == null)
            {
                Input.MouseVisible = true;
                _Pointer.Visible = false;
            }
            else
            {
                _Pointer.Texture = texture;
                _Pointer.Origin = origin;
                Input.MouseVisible = false;
                _Pointer.Visible = true;
            }
        }
    }
}