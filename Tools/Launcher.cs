using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.Window;

namespace BlackCoat
{
    public partial class Launcher : Form
    {
        private class Resolution
        {
            public VideoMode VideoMode { get; }
            public Resolution(VideoMode videoMode) => VideoMode = videoMode;
            public override string ToString() => $"{VideoMode.Width} x {VideoMode.Height}";
        }


        public Image BannerImage
        {
            get => _Banner.Image;
            set => _Banner.Image = value;
        }
        public TabPage CustomSettings
        {
            get => _TapHost.TabPages[1];
            set => _TapHost.TabPages[1] = value;
        }

        public VideoMode VideoMode
        {
            get => ((Resolution)_ResolutionComboBox.SelectedItem).VideoMode;
            set => _ResolutionComboBox.SelectedIndex = _ResolutionComboBox.Items.IndexOf(_ResolutionComboBox.Items.Cast<Resolution>().FirstOrDefault(c => c.VideoMode.Equals(value)) ?? _ResolutionComboBox.Items[0]);
        }
        public uint AntiAliasing
        {
            get => _AAComboBox.SelectedIndex == 0 ? 0 : Convert.ToUInt32(_AAComboBox.SelectedItem.ToString());
            set => _AAComboBox.SelectedIndex = value == 0 ? 0 : _AAComboBox.Items.IndexOf(value.ToString());
        }
        public uint? FpsLimit
        {
            get
            {
                if (_FPSComboBox.SelectedIndex == 3) return null;
                else return Convert.ToUInt32(_FPSComboBox.SelectedItem.ToString());
            }
            set => _FPSComboBox.SelectedIndex = _FPSComboBox.Items.IndexOf(value.ToString());
        }
        public bool Windowed
        {
            get => _WindowedCheckBox.Checked;
            set => _WindowedCheckBox.Checked = true;
        }
        public bool Borderless
        {
            get => _BorderCheckBox.Checked;
            set => _BorderCheckBox.Checked = value;
        }


        public Launcher()
        {
            InitializeComponent();
            _ResolutionComboBox.Items.AddRange(VideoMode.FullscreenModes.Where(m => m.BitsPerPixel == 32).Select(vm => new Resolution(vm)).ToArray());
            _ResolutionComboBox.SelectedIndex = 0;
            AntiAliasing = 0;
            FpsLimit = 120;
            Windowed = true;
            Borderless = false;
        }


        private void StartButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void WindowedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _BorderCheckBox.Enabled = _WindowedCheckBox.Checked;
            _BorderCheckBox.Checked = false;
        }
    }
}