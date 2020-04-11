using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat.Entities;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.UI
{
    public class Label : UIComponent
    {
        public event Action<Label> TextChanged = l => { };
        public event Action<Label> ColorChanged = l => { };


        protected TextItem _Text;
        private FloatRect _Padding;
        private TextAlignment _Alignment;


        public override Vector2f InnerSize => _Text.LocalBounds.Size() + _Padding.Position() + _Padding.Size();

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

        public Action<Label> InitTextChanged { set => TextChanged += value; }


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="core">Black Coat Engine Core.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="characterSize">Initial size of the texts characters.</param>
        /// <param name="font">Initial font.</param>
        public Label(Core core, String text = "", UInt32 characterSize = 16, Font font = null) : base(core)
        {
            Add(_Text = new TextItem(core, text, characterSize, font));
            InvokeSizeChanged();
        }


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

        protected virtual void InvokeTextChanged() => TextChanged.Invoke(this);

        protected virtual void InvokeColorChanged() => ColorChanged.Invoke(this);
    }

    public enum TextAlignment
    {
        Left,
        Centered,
        Right
    }
}