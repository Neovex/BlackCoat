using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SFML.System;
using SFML.Graphics;
using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Lights;
using BlackCoat.ParticleSystem;

namespace BlackCoat.Tools
{
    public partial class PropertyInspector : Form, IPropertyInspector
    {
        private const String _NAME = "Property Inspector";

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


        public event Action<object> InspectionItemChanged = o => { };

        private Core _Core;
        private TextureLoader _TextureLoader;
        private Boolean _Locked = true;

        protected override bool ShowWithoutActivation => true;

        public object InspectionItem
        {
            get => _Inspector.SelectedObject;
            set => _Inspector.SelectedObject = value;
        }

        public PropertyInspector(Core core, TextureLoader textureLoader)
        {
            _Core = core ?? throw new ArgumentNullException(nameof(core));
            _TextureLoader = textureLoader ?? throw new ArgumentNullException(nameof(textureLoader));
            InitializeComponent();
            _CoreUpdateWithoutFocusToolStripMenuItem.Checked = !_Core.PauseUpdateOnFocusLoss;
        }


        private void Add(IEnumerable<object> objects, TreeNode parent)
        {
            foreach (var item in objects) Add(item, parent);
        }
        public void Add(object item, TreeNode parent = null, string displayName = null)
        {
            if (item == null) return;

            var graphNode = new TreeNode(displayName ?? item.GetType().Name) { Tag = item };
            (parent?.Nodes ?? _SceneGraph.Nodes).Add(graphNode);

            // Handle Entity names
            if (item is IEntity e) graphNode.Text = $"{(String.IsNullOrWhiteSpace(e.Name) ? String.Empty : $"\"{e.Name}\" ")}{graphNode.Text}{(e.Position == default(Vector2f) ? String.Empty : $"{e.Position.X} x {e.Position.Y}")}";

            // Add SubItems of known collections / hierarchies
            switch (item)
            {
                case Gamestate state:
                    if (graphNode.Text != state.Name) graphNode.Text = $"{graphNode.Text} \"{state.Name}\"";
                    Add(state.TextureLoader, graphNode);
                    Add(state.MusicLoader, graphNode);
                    Add(state.FontLoader, graphNode);
                    Add(state.SfxLoader, graphNode);
                    Add(state.Layer_BG, graphNode, nameof(state.Layer_BG));
                    Add(state.Layer_Game, graphNode, nameof(state.Layer_Game));
                    Add(state.Layer_Overlay, graphNode, nameof(state.Layer_Overlay));
                    Add(state.Layer_Debug, graphNode, nameof(state.Layer_Debug));
                    Add(state.Layer_Cursor, graphNode, nameof(state.Layer_Cursor));
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

        // Dispose
        public void Destroy()
        {
            Text = nameof(Destroy);
            _Locked = false;
            Close();
            Dispose();
        }

        // Dont close - hide instead
        private void PropertyEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = _Locked;
            if (e.Cancel) Hide();
            else Clear();
        }

        // Clear Selection
        public void Clear()
        {
            Text = _NAME;
            _SceneGraph.Nodes.Clear();
            _Inspector.SelectedObject = null;
        }

        // Close / Hide
        private void HideToolStripMenuItem_Clicked(object sender, EventArgs e)
        {
            Hide();
        }

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

        private void RebuildSceneGraphToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var root = _SceneGraph.Nodes.OfType<TreeNode>().Select(n => n.Tag).ToArray();
            _SceneGraph.Nodes.Clear();
            Add(root, null);
        }

        public void RemoveCurrent() => RemoveEntitiyToolStripMenuItemClicked(null, null);
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

        private void RenderToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var prerenderedContainer = _Inspector.SelectedObject as PrerenderedContainer;
            prerenderedContainer.RedrawNow();
        }

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

        private void SaveLightMapToolStripMenuItemClicked(object sender, EventArgs e)
        {
            if (FindParent(n => n.Tag is Lightmap)?.Tag is Lightmap lightmap
               && _SaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                lightmap.Save(_SaveFileDialog.FileName);
            }
        }

        private void AddLightToolStripMenuItemClicked(object sender, EventArgs e)
        {
            var node = FindParent(n => n.Tag is Lightmap);
            if (node?.Tag is Lightmap lightmap)
            {
                var light = lightmap.AddLight(_TextureLoader, new Vector2f());
                Add(light, node.Nodes[0]);
            }
        }

        private void DuplicateLightToolStripMenuItemClicked(object sender, EventArgs e)
        {
            if (_SceneGraph.SelectedNode.Tag is IEntity sourceLight)
            {
                var node = FindParent(n => n.Tag is Lightmap);
                if (node.Tag is Lightmap lightmap)
                {
                    var light = lightmap.AddLight(_TextureLoader, sourceLight.Position, sourceLight.Color, sourceLight.Scale, sourceLight.Rotation);
                    Add(light, node.Nodes[0]);
                }
            }
        }

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

        private void CoreUpdateWithoutFocusToolStripMenuItemToolStripMenuItemClicked(object sender, EventArgs e)
        {
            _CoreUpdateWithoutFocusToolStripMenuItem.Checked = !(_Core.PauseUpdateOnFocusLoss = !_Core.PauseUpdateOnFocusLoss);
        }

        private void EmitterTriggerToolStripMenuItemClicked(object sender, EventArgs e)
        {
            if (_Inspector.SelectedObject is ITriggerEmitter emitter) emitter.Trigger();
        }

        private void Inspector_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            InspectionItemChanged.Invoke(_Inspector.SelectedObject);
        }
    }

    static class TreeExtensions
    {
        public static IEnumerable<TreeNode> FlattenTree(this TreeView tv)
        {
            return FlattenTree(tv.Nodes);
        }

        public static IEnumerable<TreeNode> FlattenTree(this TreeNodeCollection coll)
        {
            return coll.Cast<TreeNode>()
                        .Concat(coll.Cast<TreeNode>()
                                    .SelectMany(x => FlattenTree(x.Nodes)));
        }
    }
}