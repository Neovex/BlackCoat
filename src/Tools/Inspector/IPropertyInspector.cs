using System;
using System.Windows.Forms;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Facade interface to give simplified access to the <see cref="PropertyInspector"/>
    /// </summary>
    public interface IPropertyInspector
    {
        // Events ##########################################################################        
        /// <summary>
        /// Occurs when item is selected for inspection.
        /// </summary>
        event Action<object> InspectionItemChanged;


        // Properties ######################################################################        
        /// <summary>
        /// Gets or sets the currently inspected item.
        /// </summary>
        object InspectionItem { get; set; }


        // Methods #########################################################################        
        /// <summary>
        /// Adds an item to the tree of inspectable items.
        /// </summary>
        /// <param name="item">The new item.</param>
        /// <param name="parent">Optional parent item.</param>
        /// <param name="displayName">Optional display name.</param>
        void Add(object item, TreeNode parent = null, string displayName = null);

        /// <summary>
        /// Removes the currently inspected item from the scene.
        /// </summary>
        void RemoveCurrent();
    }
}