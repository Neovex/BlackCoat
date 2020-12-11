using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat.Entities.Shapes;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.UI
{
    public class TextBox : Label
    {
        // Events ##########################################################################
        public event Action<TextBox> InEditChanged = tb => { };
        public Action<TextBox> InitInEditChanged { set => InEditChanged += value; }


        // Variables #######################################################################
        private uint _Index;
        private Line _Caret;
        private float _CaretBlinkTimer;
        private Color _BackgroundColorBackup;
        private Color _TextColorBackup;
        private Vector2f _MinSize;
        private bool _InEdit;


        // Properties ######################################################################
        public Boolean InEdit
        {
            get => _InEdit;
            private set
            {
                if (_InEdit == value) return;
                _InEdit = value;
                InEditChanged.Invoke(this);
            }
        }
        public Color EditingBackgroundColor { get; set; }
        public Color EditingTextColor { get; set; }
        public Vector2f MinSize
        {
            get => _MinSize;
            set
            {
                if (_MinSize == value) return;
                _MinSize = value;
                InvokeSizeChanged();
            }
        }
        public override Vector2f InnerSize
        {
            get
            {
                var realInnerSize = base.InnerSize;
                var minInnerSize = _MinSize + Padding.Position() + Padding.Size();
                return new Vector2f(Math.Max(realInnerSize.X, minInnerSize.X), Math.Max(realInnerSize.Y, minInnerSize.Y));
            }
        }


        public TextBox(Core core, Vector2f? minSize = null, uint characterSize = 16, Font font = null) : base(core, String.Empty, characterSize, font)
        {
            CanFocus = true;
            MinSize = minSize ?? new Vector2f(60, 15);
            BackgroundAlpha = 1;
            BackgroundColor = Color.White;
            EditingBackgroundColor = Color.Cyan;
            TextColor = Color.Black;
            EditingTextColor = Color.Black;
            _Caret = new Line(_Core, new Vector2f(), new Vector2f(0, InnerSize.Y), EditingBackgroundColor.Invert()) { Visible = false };
            Add(_Caret);
        }


        protected override void HandleInputMove(float direction)
        {
            if (!InEdit) base.HandleInputMove(direction);
        }

        protected override void HandleInputConfirm()
        {
            base.HandleInputConfirm();
            StartEdit();
        }
        protected override void HandleInputEdit()
        {
            base.HandleInputEdit();
            StartEdit();
        }
        protected override void HandleInputCancel()
        {
            base.HandleInputCancel();
            StopEdit();
        }
        protected override void InvokeFocusLost()
        {
            base.InvokeFocusLost();
            StopEdit();
        }

        public virtual void StartEdit()
        {
            if (!HasFocus || !Visible || !Enabled) return;

            // Update move caret to mouse
            var mpos = Input.Input.MousePosition;
            if (CollisionShape.CollidesWith(mpos))
            {
                var averageCharSize = _Text.FindCharacterPos(1).X;
                if (averageCharSize == 0) _Index = 0;
                else _Index = (uint)Math.Min(Text.Length, (mpos.ToLocal(GlobalPosition).X - Padding.Left) / averageCharSize);
            }

            // Enable Edit mode
            if (!InEdit)
            {
                _BackgroundColorBackup = BackgroundColor;
                _TextColorBackup = TextColor;
                BackgroundColor = EditingBackgroundColor;
                TextColor = EditingTextColor;
                _Caret.Color = EditingTextColor;
                InEdit = true;
            }
            UpdateCaretPosition();
        }
        public virtual void StopEdit()
        {
            if (!InEdit) return;
            InEdit = false;
            BackgroundColor = _BackgroundColorBackup;
            TextColor = _TextColorBackup;
            _Caret.Visible = false;
        }

        protected override void HandleTextEntered(TextEnteredEventArgs tArgs)
        {
            if (InEdit)
            {
                Text = tArgs.Update(Text, ref _Index);
                UpdateCaretPosition();
                _CaretBlinkTimer = 0; // needed to keep caret visible when moving
            }
        }

        private void UpdateCaretPosition()
        {
            if (!InEdit) return;
            _Caret.Start.Position = _Text.FindCharacterPos(_Index) + Padding.Position() + new Vector2f(0.5f, 0); // + 0,5 to fix anti aliasing blur issue
            _Caret.End.Position = _Caret.Start.Position + new Vector2f(0, InnerSize.Y - Padding.Top - Padding.Height);
        }

        public override void Update(float deltaT)
        {
            base.Update(deltaT);

            if (HasFocus && Visible && Enabled && InEdit)
            {
                _Caret.Visible = _CaretBlinkTimer % 1f < 0.5f;
                _CaretBlinkTimer += deltaT;
                if (_CaretBlinkTimer > 2) _CaretBlinkTimer -= 2;
            }
        }

        protected override void InvokeTextChanged()
        {
            base.InvokeTextChanged();
            _Index = Math.Min(_Index, (uint)Text.Length);
            UpdateCaretPosition();
        }
        protected override void InvokeSizeChanged()
        {
            base.InvokeSizeChanged();
            UpdateCaretPosition();
        }
    }
}