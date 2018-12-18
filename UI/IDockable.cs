using SFML.System;
using SFML.Graphics;

namespace BlackCoat.UI
{
    public interface IDockable
    {
        bool DockX { get; set; }
        bool DockY { get; set; }

        Vector2f MinSize { get; }
        Vector2f OuterMinSize { get; }
        Vector2f DockedPosition { get; }

        void Resize(Vector2f size);
    }
}