using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Abstract base class of all texture based particles.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.ParticleBase" />
    public abstract class TextureParticleBase : ParticleBase
    {
        // Variables #######################################################################
        protected Vector2u _TextureSize;
        protected IntRect _TextureRect;
        protected Vector2f _Origin;
        protected Vector2f _Scale;
        protected Single _Rotation;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureParticleBase"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public TextureParticleBase(Core core) : base(core)
        {
            _Scale.X = _Scale.Y = 1f;
        }


        // Methods #########################################################################
        /// <summary>
        /// Resets the used vertices into a neutral/reusable state.
        /// </summary>
        /// <param name="vPtr">First vertex of this particle</param>
        override protected unsafe void Clear(Vertex* vPtr)
        {
            vPtr->Color = Color.Transparent;
            vPtr++;
            vPtr->Color = Color.Transparent;
            vPtr++;
            vPtr->Color = Color.Transparent;
            vPtr++;
            vPtr->Color = Color.Transparent;
        }

        /// <summary>
        /// Updates the particle with the behavior defined by inherited classes.
        /// </summary>
        /// <param name="deltaT">Current Frame Time.</param>
        /// <param name="vPtr">First vertex of this particle</param>
        /// <returns>True if the particle needs to be removed otherwise false.</returns>
        override protected unsafe bool UpdateInternal(float deltaT, Vertex* vPtr)
        {
            var scaledSize = _Scale;
            scaledSize.X *= _TextureSize.X;
            scaledSize.Y *= _TextureSize.Y;

            var offset = -_Origin;
            offset.X *=_Scale.X;
            offset.Y *=_Scale.Y;

            float cos = MathHelper.Cos(_Rotation);
            float sin = MathHelper.Sin(_Rotation);

            // Update Vertices
            vPtr->Position.X = offset.X * cos - offset.Y * sin + _Position.X;
            vPtr->Position.Y = offset.X * sin + offset.Y * cos + _Position.Y;
            vPtr->TexCoords.X = _TextureRect.Left;
            vPtr->TexCoords.Y = _TextureRect.Top;
            vPtr->Color = _Color;
            vPtr++;

            offset.X += scaledSize.X;
            vPtr->Position.X = offset.X * cos - offset.Y * sin + _Position.X;
            vPtr->Position.Y = offset.X * sin + offset.Y * cos + _Position.Y;
            vPtr->TexCoords.X = _TextureRect.Left + _TextureRect.Width;
            vPtr->TexCoords.Y = _TextureRect.Top;
            vPtr->Color = _Color;
            vPtr++;

            offset.Y += scaledSize.Y;
            vPtr->Position.X = offset.X * cos - offset.Y * sin + _Position.X;
            vPtr->Position.Y = offset.X * sin + offset.Y * cos + _Position.Y;
            vPtr->TexCoords.X = _TextureRect.Left + _TextureRect.Width;
            vPtr->TexCoords.Y = _TextureRect.Top +_TextureRect.Height;
            vPtr->Color = _Color;
            vPtr++;

            offset.X -= scaledSize.X;
            vPtr->Position.X = offset.X * cos - offset.Y * sin + _Position.X;
            vPtr->Position.Y = offset.X * sin + offset.Y * cos + _Position.Y;
            vPtr->TexCoords.X = _TextureRect.Left;
            vPtr->TexCoords.Y = _TextureRect.Top + _TextureRect.Height;
            vPtr->Color = _Color;

            return false;
        }
    }
}