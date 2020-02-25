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
    /// <summary>
    /// Represents a Launcher Window to easily acess and configure graphic settings.
    /// Custom application settings may be added as well
    /// </summary>
    public sealed partial class Launcher : Form
    {
        private class Resolution
        {
            public VideoMode VideoMode { get; }
            public Resolution(VideoMode videoMode) => VideoMode = videoMode;
            public override string ToString() => $"{VideoMode.Width} x {VideoMode.Height}";
        }
        public interface ISettings
        {
            (uint X, uint Y) Resolution { get; set; }
            uint AntiAliasing { get; set; }
            uint FpsLimit { get; set; }
            bool Windowed { get; set; }
            bool Borderless { get; set; }
            bool VSync { get; set; }
        }

        private ISettings _Settings;

        public Image BannerImage
        {
            get => _Banner.Image;
            set => _Banner.Image = value;
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
        public uint FpsLimit
        {
            get
            {
                if (_FPSComboBox.SelectedIndex == 3) return 0;
                return Convert.ToUInt32(_FPSComboBox.SelectedItem.ToString());
            }
            set
            {
                if (value == 0) _FPSComboBox.SelectedIndex = 3;
                else
                {
                    var index = _FPSComboBox.Items.IndexOf(value.ToString());
                    if (index == -1) _FPSComboBox.SelectedIndex = 0;
                    else _FPSComboBox.SelectedIndex = index;
                }
            }
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
        public bool VSync
        {
            get => _VsyncCheckBox.Checked;
            set => _VsyncCheckBox.Checked = value;
        }

        /// <summary>
        /// Creates new instance of the <see cref="Launcher"/> class
        /// </summary>
        /// <param name="settings">Optional instance of a <see cref="ISettings"/> implementation containing all required graphic initialization info</param>
        /// <param name="customSettings">Optional user <see cref="Control"/> for editing application  specific settings</param>
        public Launcher(ISettings settings = null, Control customSettings = null)
        {
            InitializeComponent();

            // Add all available videomodes to UI
            _ResolutionComboBox.Items.AddRange(VideoMode.FullscreenModes.Where(m => m.BitsPerPixel == 32).Select(vm => new Resolution(vm)).ToArray());

            // Initialize Default Values
            _ResolutionComboBox.SelectedIndex = 0;
            AntiAliasing = 0;
            FpsLimit = 120;
            Windowed = true;
            Borderless = false;
            VSync = false;
            
            if(settings != null) // load from settings
            {
                _Settings = settings;
                var vmode = new VideoMode(settings.Resolution.X, settings.Resolution.Y);
                if (vmode.IsValid()) VideoMode = vmode;
                else MessageBox.Show("Invalid video mode loaded - resetting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AntiAliasing = settings.AntiAliasing;
                FpsLimit = settings.FpsLimit;
                Windowed = settings.Windowed;
                Borderless = settings.Borderless;
                VSync = settings.VSync;
            }

            // Initialize custom settings tabpage
            if (customSettings != null)
            {
                customSettings.Dock = DockStyle.Fill;
                var newTab = new TabPage(customSettings.Text);
                newTab.Controls.Add(customSettings);
                _TabHost.TabPages.Add(newTab);
            }
        }


        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_Settings != null)
            {
                _Settings.Resolution = (VideoMode.Width, VideoMode.Height);
                _Settings.AntiAliasing = AntiAliasing;
                _Settings.FpsLimit = FpsLimit;
                _Settings.Windowed = Windowed;
                _Settings.Borderless = Borderless;
                _Settings.VSync = VSync;
            }
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

        private void VsyncCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _FPSComboBox.Enabled = !_VsyncCheckBox.Checked;
            if(!_FPSComboBox.Enabled) _FPSComboBox.SelectedIndex = 1;
        }
    }
}