using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities.Shapes
{
    public class Line:BaseEntity
    {
        // Properties ######################################################################
        /// <summary>
        /// Line Color
        /// </summary>
        public override Color Color { get; set; }

        public Vector2f Start { get; set; }
        public Vector2f End { get; set; }

        /// <summary>
        /// Creates a new instance of the line class.
        /// </summary>
        /// <param name="core">Render Core</param>
        /// <param name="start">Optional start vector</param>
        /// <param name="end">Optional end vector</param>
        /// <param name="color">Optional line color</param>
        public Line(Core core, Vector2f start = default(Vector2f), Vector2f end = default(Vector2f), Color? color = null) : base(core)
        {
            Color = color ?? Color.Cyan;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Draws the Line respective to its paramters and view
        /// </summary>
        public override void Draw()
        {
            _Core.Draw(new[] { new Vertex(Start, Color), new Vertex(End, Color) }, PrimitiveType.Lines, RenderState);
        }

        /// <summary>
        /// Unused Function
        /// </summary>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            Draw();
        }
    }
}