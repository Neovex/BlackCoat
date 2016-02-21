using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Line Primitve
    /// </summary>
    public class Line:BaseEntity
    {
        // Properties ######################################################################
        /// <summary>
        /// Line Color
        /// </summary>
        public override Color Color { get; set; }

        /// <summary>
        /// Point to start the line from
        /// </summary>
        public Vector2f Start { get; set; }
        
        /// <summary>
        /// Point to draw the line to
        /// </summary>
        public Vector2f End { get; set; }

        /// <summary>
        /// Position of Startpoint and Endpoint in relation to each other
        /// </summary>
        public override Vector2f Position
        {
            get
            {
                return Start;
            }
            set
            {
                End = new Vector2f(End.X + value.X - Start.X, End.Y + value.Y - Start.Y);
                Start = value;
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a Line instance.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="start">Optional start vector</param>
        /// <param name="end">Optional end vector</param>
        /// <param name="color">Optional line color</param>
        public Line(Core core, Vector2f start = default(Vector2f), Vector2f end = default(Vector2f), Color? color = null) : base(core)
        {
            Color = color ?? Color.Cyan;
            Start = start;
            End = end;
        }


        // Methods #########################################################################
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