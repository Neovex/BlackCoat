﻿using System;
using SFML.Window;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Represents, and provides methods to create a Render Device aka a Render Window.
    /// </summary>
    /// <seealso cref="SFML.Graphics.RenderWindow" />
    public sealed class Device : RenderWindow
    {
        private const uint _DEFAULT_FRAMERATE_LIMIT = 120;


        /// <summary>
        /// Gets a value indicating whether this instance is a full screen device.
        /// </summary>
        public bool IsFullscreen
        {
            get
            {
                var desktop = VideoMode.DesktopMode;
                return Size.X == desktop.Width && Size.Y == desktop.Height;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        /// <param name="style">Window style (Resize | Close by default)</param>
        private Device(VideoMode mode, string title, Styles style) : base(mode, title, style)
        {
            SetFramerateLimit(_DEFAULT_FRAMERATE_LIMIT);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        /// <param name="style">Window style (Resize | Close by default)</param>
        /// <param name="settings">Creation parameters</param>
        private Device(VideoMode mode, string title, Styles style, ContextSettings settings) : base(mode, title, style, settings)
        {
            SetFramerateLimit(_DEFAULT_FRAMERATE_LIMIT);
        }

        /// <summary>
        /// Initializes a new Graphic Device based on a window or control handle
        /// </summary>
        /// <param name="handle">Handle to create the device on</param>
        /// <param name="antialiasing">Determines the Anti-aliasing</param>
        /// <param name="framerateLimit">Optional frame rate limit</param>
        /// <exception cref="ArgumentOutOfRangeException">Window handle must not be Zero</exception>
        private Device(IntPtr handle, UInt32 antialiasing, UInt32? framerateLimit = null) : base(handle, new ContextSettings(24, 8, antialiasing))
        {
            SetFramerateLimit(framerateLimit ?? _DEFAULT_FRAMERATE_LIMIT);
        }


        /// <summary>
        /// Sets the icon of the render window and the associated task bar button.
        /// </summary>
        /// <param name="icon">Texture used to set the icon from</param>
        public void SetIcon(Texture icon)
        {
            if (icon == null) throw new ArgumentNullException("icon");
            using (var img = icon.CopyToImage())
            {
                SetIcon(img.Size.X, img.Size.Y, img.Pixels);
            }
        }


        // STATICS

        /// <summary>
        /// Initializes a new Graphic Device window in 800x600.
        /// </summary>
        /// <returns>The default device</returns>
        public static Device Default => new Device(new VideoMode(800, 600), nameof(BlackCoat), Styles.Titlebar);

        /// <summary>
        /// Initializes a new Render Device as a windowed full screen window.
        /// </summary>
        /// <returns>The full screen device</returns>
        public static Device Fullscreen => new Device(VideoMode.DesktopMode, nameof(BlackCoat), Styles.None);

        /// <summary>
        /// Initializes a new Graphic Device
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the /Window</param>
        /// <param name="style">Display Style of the Device/Window</param>
        /// <param name="antialiasing">Determines the Anti-aliasing</param>
        /// <param name="framerateLimit">Optional frame rate limit.</param>
        /// <param name="skipValidityCheck">Skips the device validation (not recommended but required for non-standard resolutions)</param>
        /// <returns>
        /// The Initialized Device/Window or null if the Device could not be created
        /// </returns>
        public static Device Create(VideoMode videoMode, String title, Styles style, UInt32 antialiasing, UInt32? framerateLimit = null, Boolean skipValidityCheck = false)
        {
            var settings = new ContextSettings(24, 8, antialiasing);
            if (skipValidityCheck || videoMode.IsValid())
            {
                var window = new Device(videoMode, title, style, settings);
                window.SetFramerateLimit(framerateLimit ?? _DEFAULT_FRAMERATE_LIMIT);
                return window;
            }
            return null;
        }

        /// <summary>
        /// Initializes a new Graphic Device based on a window or control handle
        /// </summary>
        /// <param name="handle">Handle to create the device on</param>
        /// <param name="antialiasing">Determines the Anti-aliasing</param>
        /// <param name="framerateLimit">Optional frame rate limit</param>
        /// <returns>The Initialized Device/Window or null if the Device could not be created</returns>
        /// <exception cref="ArgumentOutOfRangeException">Window handle must not be Zero</exception>
        public static Device Create(IntPtr handle, UInt32 antialiasing, UInt32? framerateLimit = null) => new Device(handle, antialiasing, framerateLimit);
    }
}