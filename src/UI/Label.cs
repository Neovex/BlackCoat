using System;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities;

namespace BlackCoat.UI
{
    /// <summary>
    /// Represents a simple piece of text within the UI.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIComponent" />
    public class Label : UIComponent
    {
        // Events ##########################################################################        
        /// <summary>
        /// Occurs when text changes.
        /// </summary>
        public event Action<Label> TextChanged = l => { };

        /// <summary>
        /// Occurs when text color changes.
        /// </summary>
        public event Action<Label> ColorChanged = l => { };


        // Variables #######################################################################
        protected TextItem _Text;
        private FloatRect _Padding;
        private TextAlignment _Alignment;


        // Properties ######################################################################
        /// <summary>
        /// Gets the inner size of this <see cref="Label" />.
        /// </summary>
        public override Vector2f InnerSize => _Text.LocalBounds.Size() + _Padding.Position() + _Padding.Size();

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        public string Text
        {
            get => _Text.Text;
            set
            {
                value = value ?? String.Empty;
                if (_Text.Text == value) return;
                _Text.Text = value;
                InvokeTextChanged();
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the padding which is the space demand on the inside of the component.
        /// </summary>
        public FloatRect Padding
        {
            get => _Padding;
            set
            {
                if (_Padding == value) return;
                _Padding = value;
                _Text.Position = _Padding.Position();
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Font used to display the text
        /// </summary>
        public Font Font
        {
            get => _Text.Font;
            set
            {
                if (_Text.Font == value) return;
                _Text.Font = value;
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Font Size
        /// </summary>
        public uint CharacterSize
        {
            get => _Text.CharacterSize;
            set
            {
                if (_Text.CharacterSize == value) return;
                _Text.CharacterSize = value;
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Style of the text <see cref="Text.Styles"/>
        /// </summary>
        public Text.Styles Style
        {
            get => _Text.Style;
            set
            {
                if (_Text.Style == value) return;
                _Text.Style = value;
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Label"/>s text alignment.
        /// </summary>
        public TextAlignment Alignment
        {
            get => _Alignment;
            set
            {
                if (_Alignment == value) return;
                _Alignment = value;
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color TextColor
        {
            get => _Text.Color;
            set
            {
                if (_Text.Color == value) return;
                _Text.Color = value;
                InvokeColorChanged();
            }
        }

        /// <summary>
        /// Initialization helper for the <see cref="TextChanged"/> event.
        /// </summary>
        public Action<Label> InitTextChanged { set => TextChanged += value; }


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="core">Black Coat Engine Core.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="characterSize">Initial size of the texts characters.</param>
        /// <param name="font">Initial font. Null for default.</param>
        public Label(Core core, String text = "", UInt32 characterSize = 16, Font font = null) : base(core)
        {
            Add(_Text = new TextItem(core, text, characterSize, font));
            InvokeSizeChanged();
        }


        // Methods #########################################################################
        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
        protected override void InvokeSizeChanged()
        {
            var bounds = _Text.GlobalBounds;
            _Text.Origin += bounds.Position() - _Text.Position;
            switch (Alignment)
            {
                case TextAlignment.Left:
                    Origin = new Vector2f(0, Origin.Y);
                break;
                case TextAlignment.Centered:
                    Origin = new Vector2f(bounds.Width / 2, Origin.Y);
                break;
                case TextAlignment.Right:
                    Origin = new Vector2f(bounds.Width, Origin.Y);
                break;
            }
            base.InvokeSizeChanged();
        }

        /// <summary>
        /// Invokes the text changed event.
        /// </summary>
        protected virtual void InvokeTextChanged() => TextChanged.Invoke(this);

        /// <summary>
        /// Invokes the color changed event.
        /// </summary>
        protected virtual void InvokeColorChanged() => ColorChanged.Invoke(this);
    }

    /// <summary>
    /// Defines the alignment for text based elements of the UI System.
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Centered,
        Right
    }
}