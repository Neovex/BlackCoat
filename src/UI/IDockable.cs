using SFML.System;

namespace BlackCoat.UI
{
    public interface IDockable
    {
        bool DockX { get; set; }
        bool DockY { get; set; }

        Vector2f MinSize { get; }
        Vector2f OuterBounds { get; }
        Vector2f DockedPosition { get; }

        void Resize(Vector2f size);
    }
}