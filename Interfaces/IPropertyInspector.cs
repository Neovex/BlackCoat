using System.Windows.Forms;
using SFML.System;

namespace BlackCoat
{
    public interface IPropertyInspector
    {
        void Add(object item, TreeNode parent = null, string displayName = null);
        void Destroy();
        void SetPosition(Vector2f position);
    }
}