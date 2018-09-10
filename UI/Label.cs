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
        private FloatRect _InnerPadding;
        private bool _Centered;


        public override Vector2f InnerSize => _Text.GetGlobalBounds().Size() + _InnerPadding.Position() + _InnerPadding.Size();

        public string Text
        {
            get => _Text.DisplayedString;
            set
            {
                value = value ?? String.Empty;
                if (_Text.DisplayedString == value) return;
                _Text.DisplayedString = value;
                InvokeTextChanged();
                InvokeSizeChanged();
            }
        }

        public FloatRect InnerPadding
        {
            get => _InnerPadding;
            set
            {
                if (_InnerPadding == value) return;
                _InnerPadding = value;
                _Text.Position = _InnerPadding.Position();
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

        public bool Centered
        {
            get => _Centered;
            set
            {
                if (_Centered == value) return;
                _Centered = value;
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


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="core">Black Coat Engine Core.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="font">Initial font.</param>
        public Label(Core core, String text = "", Font font = null) : base(core)
        {
            Add(_Text = new TextItem(core, text, font));
            InvokeSizeChanged();
        }


        protected override void InvokeSizeChanged()
        {
            var bounds = _Text.GetGlobalBounds();
            _Text.Origin += bounds.Position() - _Text.Position;
            if (Centered) Origin = new Vector2f(bounds.Width / 2, Origin.Y);
            base.InvokeSizeChanged();
        }

        protected virtual void InvokeTextChanged() => TextChanged.Invoke(this);
        protected virtual void InvokeColorChanged() => ColorChanged.Invoke(this);
    }
}