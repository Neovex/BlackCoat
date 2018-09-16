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
using BlackCoat.ParticleSystem;

namespace BlackCoat.Tools
{
    public partial class PropertyInspector : Form
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
        }

        private Boolean _Locked;

        public PropertyInspector()
        {
            _Locked = true;
            InitializeComponent();
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

        public void Destroy()
        {
            Text = nameof(Destroy);
            _Locked = false;
            Close();
            Dispose();
        }

        private void PropertyEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = _Locked;
            if (e.Cancel) Hide();
            else Clear();
        }

        public void Clear()
        {
            Text = _NAME;
            _SceneGraph.Nodes.Clear();
            _Inspector.SelectedObject = null;
        }

        private void HideToolStripMenuItem_Clicked(object sender, EventArgs e)
        {
            Hide();
        }

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

        private void SceneGraph_NodeSelected(object sender, TreeViewEventArgs e)
        {
            _Inspector.SelectedObject = e.Node.Tag;
            Text = $"{_NAME} {_Inspector.SelectedObject?.GetType().Name}";
        }

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

        private void SceneGraph_GotFocus(object sender, EventArgs e)
        {
            if (_SceneGraph.SelectedNode != null) SceneGraph_NodeSelected(this, new TreeViewEventArgs(_SceneGraph.SelectedNode));
        }

        private void TopMostToolStripMenuItem_Clicked(object sender, EventArgs e)
        {
            _TopMostToolStripMenuItem.Checked = TopMost = !TopMost;
        }
    }
}