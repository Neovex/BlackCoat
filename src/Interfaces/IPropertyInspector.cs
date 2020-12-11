using System;
using System.Windows.Forms;
using SFML.System;

namespace BlackCoat
{
    public interface IPropertyInspector
    {
        event Action<object> InspectionItemChanged;
        object InspectionItem { get; set; }
        void Add(object item, TreeNode parent = null, string displayName = null);
        void RemoveCurrent();
    }
}