using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using SFML.Window;

namespace BlackCoat
{
    /// <summary>
    /// Represents a Launcher Window to easily access and configure engine settings.
    /// Custom, application specific setting pages can be added as well.
    /// </summary>
    public sealed partial class Launcher : Form
    {
        #region Nested Types
        /// <summary>
        /// Wrapper class for the <see cref="SFML.Window.VideoMode"/>.
        /// </summary>
        private class Resolution
        {
            public VideoMode VideoMode { get; }
            public Resolution(VideoMode videoMode) => VideoMode = videoMode;
            public override string ToString() => $"{VideoMode.Width} x {VideoMode.Height}";
        }

        /// <summary>
        /// Interface for classes used for communicating with the <see cref="Launcher"/>.
        /// </summary>
        public interface ISettingsAdapter
        {
            /// <summary>
            /// Gets or sets the resolutions width and height. Default 800x600.
            /// </summary>
            (uint Width, uint Height) Resolution { get; set; }

            /// <summary>
            /// Gets or sets the anti aliasing level. Default 0.
            /// </summary>
            uint AntiAliasing { get; set; }

            /// <summary>
            /// Gets or sets the FPS limit. Default 120.
            /// </summary>
            uint FpsLimit { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ISettingsAdapter"/> is windowed. Default true.
            /// </summary>
            bool Windowed { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ISettingsAdapter"/> is running in a window without borders. Default false.
            /// </summary>
            bool Borderless { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the renderer will try to sync with the screens refresh rate. Default false.
            /// </summary>
            bool VSync { get; set; }

            /// <summary>
            /// Gets or sets the music volume. Default 100 (max).
            /// </summary>
            int MusicVolume { get; set; }

            /// <summary>
            /// Gets or sets the sound effects volume. Default 100 (max).
            /// </summary>
            int EffectVolume { get; set; }
        }
        #endregion


        // Variables #######################################################################
        private readonly ISettingsAdapter _Settings;


        // Properties ######################################################################        
        /// <summary>
        /// Image to be used as banner for the launcher window.
        /// Optimal resolution: 510x100 (72)
        /// </summary>
        public Image BannerImage
        {
            get => _Banner.Image;
            set => _Banner.Image = value;
        }

        /// <summary>
        /// Gets or sets the video mode for rendering.
        /// </summary>
        public VideoMode VideoMode
        {
            get => ((Resolution)_ResolutionComboBox.SelectedItem).VideoMode;
            set => _ResolutionComboBox.SelectedIndex = _ResolutionComboBox.Items.IndexOf(_ResolutionComboBox.Items.Cast<Resolution>().FirstOrDefault(c => c.VideoMode.Equals(value)) ?? _ResolutionComboBox.Items[0]);
        }

        /// <summary>
        /// Gets or sets the anti aliasing level. Default 0.
        /// </summary>
        public uint AntiAliasing
        {
            get => _AAComboBox.SelectedIndex == 0 ? 0 : Convert.ToUInt32(_AAComboBox.SelectedItem.ToString());
            set => _AAComboBox.SelectedIndex = value == 0 ? 0 : _AAComboBox.Items.IndexOf(value.ToString());
        }

        /// <summary>
        /// Gets or sets the FPS limit. Default 120.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISettingsAdapter"/> is windowed. Default true.
        /// </summary>
        public bool Windowed
        {
            get => _WindowedCheckBox.Checked;
            set => _WindowedCheckBox.Checked = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISettingsAdapter"/> is running in a window without borders. Default false.
        /// </summary>
        public bool Borderless
        {
            get => _BorderCheckBox.Checked;
            set => _BorderCheckBox.Checked = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the renderer will try to sync with the screens refresh rate. Default false.
        /// </summary>
        public bool VSync
        {
            get => _VsyncCheckBox.Checked;
            set => _VsyncCheckBox.Checked = value;
        }

        /// <summary>
        /// Gets or sets the music volume. Default 100 (max).
        /// </summary>
        public int MusicVolume
        {
            get => _MusicVolumeTrackBar.Value;
            set => _MusicVolumeNumericUpDown.Value = _MusicVolumeTrackBar.Value = MathHelper.Clamp(value, _MusicVolumeTrackBar.Minimum, _MusicVolumeTrackBar.Maximum);
        }

        /// <summary>
        /// Gets or sets the sound effects volume. Default 100 (max).
        /// </summary>
        public int EffectVolume
        {
            get => _EffectVolumeTrackBar.Value;
            set => _EffectVolumeNumericUpDown.Value = _EffectVolumeTrackBar.Value = MathHelper.Clamp(value, _EffectVolumeTrackBar.Minimum, _EffectVolumeTrackBar.Maximum);
        }


        // CTOR ############################################################################
        /// <summary>
        /// Creates new instance of the <see cref="Launcher"/> class
        /// </summary>
        /// <param name="settings">Optional instance of a <see cref="ISettingsAdapter"/> implementation containing all required graphic initialization info</param>
        /// <param name="customSettings">Optional user <see cref="Control"/> for editing application  specific settings</param>
        public Launcher(ISettingsAdapter settings = null, Control customSettings = null)
        {
            InitializeComponent();

            // Add all available video modes to UI
            _ResolutionComboBox.Items.AddRange(VideoMode.FullscreenModes.Where(m => m.BitsPerPixel == 32).Select(vm => new Resolution(vm)).ToArray());

            // Initialize Default Values
            _ResolutionComboBox.SelectedIndex = 0;
            AntiAliasing = 0;
            FpsLimit = 120;
            Windowed = true;
            Borderless = false;
            VSync = false;
            MusicVolume = 100;
            EffectVolume = 100;

            // Initialize settings from settings adapter
            if (settings != null)
            {
                _Settings = settings;
                var vmode = new VideoMode(settings.Resolution.Width, settings.Resolution.Height);
                if (vmode.IsValid()) VideoMode = vmode;
                else MessageBox.Show("Invalid video mode loaded -> resetting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AntiAliasing = settings.AntiAliasing;
                FpsLimit = settings.FpsLimit;
                Windowed = settings.Windowed;
                Borderless = settings.Borderless;
                VSync = settings.VSync;
                MusicVolume = settings.MusicVolume;
                EffectVolume = settings.EffectVolume;
            }

            // Initialize custom settings tab page
            if (customSettings != null)
            {
                customSettings.Dock = DockStyle.Fill;
                var newTab = new TabPage(customSettings.Text);
                newTab.Controls.Add(customSettings);
                _TabHost.TabPages.Add(newTab);
            }
        }


        // Methods #########################################################################
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_Settings != null) // Write all settings back to the adapter
            {
                _Settings.Resolution = (VideoMode.Width, VideoMode.Height);
                _Settings.AntiAliasing = AntiAliasing;
                _Settings.FpsLimit = FpsLimit;
                _Settings.Windowed = Windowed;
                _Settings.Borderless = Borderless;
                _Settings.VSync = VSync;
                _Settings.MusicVolume = MusicVolume;
                _Settings.EffectVolume = EffectVolume;
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

        private void MusicVolumeTrackBar_Scroll(object sender, EventArgs e)
        {
            MusicVolume = _MusicVolumeTrackBar.Value;
        }

        private void MusicVolumeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            MusicVolume = (int)_MusicVolumeNumericUpDown.Value;
        }

        private void EffectVolumeTrackBar_Scroll(object sender, EventArgs e)
        {
            EffectVolume = _EffectVolumeTrackBar.Value;
        }

        private void EffectVolumeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            EffectVolume = (int)_EffectVolumeNumericUpDown.Value;
        }
    }
}