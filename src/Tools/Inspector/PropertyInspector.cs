using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Entities;
using BlackCoat.Entities.Lights;
using BlackCoat.ParticleSystem;

namespace BlackCoat.Tools
{
    public partial class PropertyInspector : Form, IPropertyInspector
    {
        // CONSTANTS #######################################################################
        private const String _NAME = "Property Inspector";


        // STATICS #########################################################################
        /// <summary>
        /// Initializes the <see cref="PropertyInspector"/> class.
        /// </summary>
        static PropertyInspector()
        {
            // Adding Conversion Attributes to 3rd Party Components
            TypeDescriptor.AddAttributes(typeof(Vector2f),  new TypeConverterAttribute(typeof(Vector2Converter<Vector2f>)));
            TypeDescriptor.AddAttributes(typeof(Vector2i),  new TypeConverterAttribute(typeof(Vector2Converter<Vector2i>)));
            TypeDescriptor.AddAttributes(typeof(Vector2u),  new TypeConverterAttribute(typeof(Vector2Converter<Vector2u>)));
            TypeDescriptor.AddAttributes(typeof(Color),     new TypeConverterAttribute(typeof(ColorConverter)));
            TypeDescriptor.AddAttributes(typeof(IntRect),   new TypeConverterAttribute(typeof(RectangleConverter<IntRect>)));
            TypeDescriptor.AddAttributes(typeof(FloatRect), new TypeConverterAttribute(typeof(RectangleConverter<FloatRect>)));
            TypeDescriptor.AddAttributes(typeof(BlendMode), new TypeConverterAttribute(typeof(BlendmodeConverter)));
        }


        // Events ##########################################################################        
        /// <summary>
        /// Occurs when item is selected for inspection.
        /// </summary>
        public event Action<object> InspectionItemChanged = o => { };


        // Variables #######################################################################
        private Core _Core;
        private TextureLoader _TextureLoader;
        private Boolean _Locked = true;


        // Properties ######################################################################
        /// <summary>
        /// Gets a value indicating whether the window will be activated when it is shown.
        /// </summary>
        protected override bool ShowWithoutActivation => true;

        /// <summary>
        /// Gets or sets the currently inspected item.
        /// </summary>
        public object InspectionItem
        {
            get => _Inspector.SelectedObject;
            set => _Inspector.SelectedObject = value;
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInspector"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="textureLoader">(Deprecated) A texture loader for the light editor.</param>
        /// <exception cref="System.ArgumentNullException">core or textureLoader</exception>
        public PropertyInspector(Core core, TextureLoader textureLoader)
        {
            _Core = core ?? throw new ArgumentNullException(nameof(core));
            _TextureLoader = textureLoader ?? throw new ArgumentNullException(nameof(textureLoader));
            InitializeComponent();
            _CoreUpdateWithoutFocusToolStripMenuItem.Checked = !_Core.PauseUpdateOnFocusLoss;
        }


        // Methods #########################################################################
        /// <summary>
        /// Adds the specified objects to the inspection tree.
        /// </summary>
        /// <param name="objects">The objects to add.</param>
        /// <param name="parent">The parent tree node.</param>
        private void Add(IEnumerable<object> objects, TreeNode parent)
        {
            foreach (var item in objects) Add(item, parent);
        }

        /// <summary>
        /// Adds an item to the tree of inspectable items.
        /// </summary>
        /// <param name="item">The new item.</param>
        /// <param name="parent">Optional parent item.</param>
        /// <param name="displayName">Optional display name.</param>
        public void Add(object item, TreeNode parent = null, string displayName = null)
        {
            if (item == null) return;

            var graphNode = new TreeNode(displayName ?? item.GetType().Name) { Tag = item };
            (parent?.Nodes ?? _SceneGraph.Nodes).Add(graphNode);

            // Handle Entity names
            if (item is IEntity e) graphNode.Text = displayName ?? e.ToString();

            // Add SubItems of known collections / hierarchies
            switch (item)
            {
                case Scene scene:
                    if (graphNode.Text != scene.Name) graphNode.Text = $"{graphNode.Text} \"{scene.Name}\"";
                    Add(scene.TextureLoader, graphNode);
                    Add(scene.MusicLoader, graphNode);
                    Add(scene.FontLoader, graphNode);
                    Add(scene.SfxLoader, graphNode);
                    Add(scene.Layer_Background, graphNode, nameof(scene.Layer_Background));
                    Add(scene.Layer_Game, graphNode, nameof(scene.Layer_Game));
                    Add(scene.Layer_Overlay, graphNode, nameof(scene.Layer_Overlay));
                    Add(scene.Layer_Debug, graphNode, nameof(scene.Layer_Debug));
                    Add(scene.Layer_Cursor, graphNode, nameof(scene.Layer_Cursor));
                break;
                case Entities.Container container:
                    Add(container._Entities, graphNode);
                break;
                case EmitterComposition composite:
                    Add(composite.Emitters, graphNode);
                break;
                case PixelEmitter pix:
                    Add(pix.ParticleInfo, graphNode);
                break;
                case TextureEmitter tex:
                    Add(tex.ParticleInfo, graphNode);
                break;
                case ParticleEmitterHost host:
                    Add(host.DepthLayers, graphNode);
                    Add(host.Emitters, graphNode);
                break;
            }

            _SceneGraph.Nodes[0].Expand();
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        public void Destroy()
        {
            Text = nameof(Destroy);
            _Locked = false;
            Close();
            Dispose();
        }

        // Don't close window - hide it instead
        private void PropertyEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = _Locked;
            if (e.Cancel) Hide();
            else Clear();
        }

        /// <summary>
        /// Clears this instance of inspectable items.
        /// </summary>
        public void Clear()
        {
            Text = _NAME;
            _SceneGraph.Nodes.Clear();
            _Inspector.SelectedObject = null;
        }

        // Hide Window
        private void HideToolStripMenuItem_Clicked(object sender, EventArgs e) => Hide();

        // Export all properties of the currently selected object to the clipboard
        private void ExportTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(_Inspector.SelectedObject.GetType().Name);
            int i = 0;
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(_Inspector.SelectedObject))
            {
                b.AppendLine($"{descriptor.Name}={descriptor.GetValue(_Inspector.SelectedObject) ?? "[null]"}");
                i++;
            }
            Clipboard.SetText(b.ToString());
            MessageBox.Show($"{i} Properties now in Clipboard.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Inspect an object chosen from the tree
        private void SceneGraph_NodeSelected(object sender, TreeViewEventArgs e)
        {
            _Inspector.SelectedObject = e.Node.Tag;
            Text = $"{_NAME} {_Inspector.SelectedObject?.GetType().Name}";
        }

        // Inspect an object chosen from the grid instead of the tree
        private void Inspector_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                _Inspector.SelectedGridItemChanged -= Inspector_SelectedGridItemChanged;
                _Inspector.SelectedObject = _Inspector.SelectedGridItem.Value;
                _Inspector.SelectedGridItemChanged += Inspector_SelectedGridItemChanged;

                Text = $"{_NAME} {_Inspector.SelectedObject?.GetType().Name}";
            }
        }

        // Restore highlight to selected node
        private void SceneGraph_GotFocus(object sender, EventArgs e)
        {
            if (_SceneGraph.SelectedNode != null) SceneGraph_NodeSelected(this, new TreeViewEventArgs(_SceneGraph.SelectedNode));
        }

        // Toggle top most
        private void TopMostToolStripMenuItem_Clicked(object sender, EventArgs e)
        {
            _TopMostToolStripMenuItem.Checked = TopMost = !TopMost;
        }

        // Adapt the Inspectors Extras Menu to the selected object
        private void InspectorSelectedObjectsChanged(object sender, EventArgs e)
        {
            _EmitterTriggerToolStripMenuItem.Enabled = _Inspector.SelectedObject is ITriggerEmitter;
            _RemoveEntitiyToolStripMenuItem.Enabled = _Inspector.SelectedObject is IEntity;
            _RenderToolStripMenuItem.Enabled = _Inspector.SelectedObject is PrerenderedContainer;
            _LightsToolStripMenuItem.Enabled = FindParent(n => n?.Tag is Lightmap) != null;

            InspectionItemChanged.Invoke(_Inspector.SelectedObject);
        }

        // Refreshes the tree
        private void RebuildSceneGraphToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var root = _SceneGraph.Nodes.OfType<TreeNode>().Select(n => n.Tag).ToArray();
            _SceneGraph.Nodes.Clear();
            Add(root, null);
        }

        /// <summary>
        /// Removes the currently inspected item from the scene.
        /// </summary>
        public void RemoveCurrent() => RemoveEntitiyToolStripMenuItemClicked(null, null);

        // Removes an entity from the scene
        private void RemoveEntitiyToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var entity = _Inspector.SelectedObject as IEntity;
            entity.Parent.Remove(entity);
            var node = _SceneGraph.FlattenTree().FirstOrDefault(n => n.Tag == entity);
            if (node != null)
            {
                _SceneGraph.SelectedNode = node.Parent;
                node.Remove();
            }
        }

        // Forces a prerendered container to immidately redraw its contents
        private void RenderToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var prerenderedContainer = _Inspector.SelectedObject as PrerenderedContainer;
            prerenderedContainer.RedrawNow();
        }

        // Load light map file
        private void LoadLightMapToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var node = FindParent(n => n.Tag is Lightmap);
            if(node?.Tag is Lightmap lightmap
               &&  _OpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                lightmap.Load(_TextureLoader, _OpenFileDialog.FileName);

                var parent = node.Parent;
                node.Remove();
                Add(lightmap, parent);
            }
        }

        // Save light map file
        private void SaveLightMapToolStripMenuItemClicked(object sender, EventArgs e)
        {
            if (FindParent(n => n.Tag is Lightmap)?.Tag is Lightmap lightmap
               && _SaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                lightmap.Save(_SaveFileDialog.FileName);
            }
        }

        // Add a new light
        private void AddLightToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var node = FindParent(n => n.Tag is Lightmap);
            if (node?.Tag is Lightmap lightmap)
            {
                var light = lightmap.AddLight(_TextureLoader, new Vector2f());
                Add(light, node.Nodes[0]);
            }
        }

        // Duplicate a light
        private void DuplicateLightToolStripMenuItemClicked(object sender, EventArgs e)
        {
            if (_Inspector.SelectedObject is IEntity sourceLight)
            {
                var node = FindParent(n => n.Tag is Lightmap);
                if (node.Tag is Lightmap lightmap)
                {
                    var light = lightmap.AddLight(_TextureLoader, sourceLight.Position, sourceLight.Color, sourceLight.Scale, sourceLight.Rotation);
                    Add(light, node.Nodes[0]);
                }
            }
        }

        // Find parent of currently selected node that satisfies a condition
        private TreeNode FindParent(Func<TreeNode, bool> validator)
        {
            var node = _SceneGraph.SelectedNode;
            while (node != null)
            {
                if (validator.Invoke(node)) return node;
                node = node.Parent;
            }
            return null;
        }

        // Toggle Update without Focus
        private void CoreUpdateWithoutFocusToolStripMenuItemToolStripMenuItemClicked(object sender, EventArgs e)
        {
            _CoreUpdateWithoutFocusToolStripMenuItem.Checked = !(_Core.PauseUpdateOnFocusLoss = !_Core.PauseUpdateOnFocusLoss);
        }

        // Triggers an emitter
        private void EmitterTriggerToolStripMenuItemClicked(object sender, EventArgs e)
        {
            if (_Inspector.SelectedObject is ITriggerEmitter emitter) emitter.Trigger();
        }

        // Raise InspectionItemChanged Event
        private void Inspector_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            InspectionItemChanged.Invoke(_Inspector.SelectedObject);
        }
    }

    /// <summary>
    /// Tree item extension methods
    /// </summary>
    static class TreeExtensions
    {
        /// <summary>
        /// Flattens the tree.
        /// </summary>
        /// <returns>All tree elements disregarding their hierarchy</returns>
        public static IEnumerable<TreeNode> FlattenTree(this TreeView tv) => tv.Nodes.FlattenTree();

        /// <summary>
        /// Flattens the tree.
        /// </summary>
        /// <returns>All tree elements disregarding their hierarchy</returns>
        public static IEnumerable<TreeNode> FlattenTree(this TreeNodeCollection coll)
        {
            return coll.Cast<TreeNode>()
                       .Concat(coll.Cast<TreeNode>()
                                   .SelectMany(x => FlattenTree(x.Nodes)));
        }
    }
}