using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Line Primitve
    /// </summary>
    public class Line:BaseEntity
    {
        private Color _Color;
        private Vertex[] _Verticies;


        // Properties ######################################################################
        /// <summary>
        /// Line Color
        /// </summary>
        public override Color Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
                Start.Color = value;
                End.Color = value;
            }
        }


        /// <summary>
        /// Point to start the line from
        /// </summary>
        public Vertex Start;

        /// <summary>
        /// Point to draw the line to
        /// </summary>
        public Vertex End;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a Line instance.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="start">Start Vertex</param>
        /// <param name="end">End Vertex</param>
        /// <param name="color">Optional line color - overrides Vertex color</param>
        public Line(Core core, Vertex start, Vertex end, Color? color = null) : base(core)
        {
            Start = start;
            End = end;
            Color = color ?? Color.Cyan;
            _Verticies = new[] { Start, End };
        }


        // Methods #########################################################################
        /// <summary>
        /// Draws the Line respective to its paramters and view
        /// </summary>
        public override void Draw()
        {
            _Core.Draw(_Verticies, PrimitiveType.Lines, RenderState);
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