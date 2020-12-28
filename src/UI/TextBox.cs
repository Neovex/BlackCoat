using System;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities.Shapes;

namespace BlackCoat.UI
{
    public class TextBox : Label
    {
        // Events ##########################################################################        
        /// <summary>
        /// Occurs when the edit mode begins or ends.
        /// </summary>
        public event Action<TextBox> InEditChanged = tb => { };

        /// <summary>
        /// Initialization helper for the <see cref="InEditChanged"/> event.
        /// </summary>
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
        /// <summary>
        /// Determines whether this <see cref="TextBox"/> is currently being edited.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the background color during editing.
        /// </summary>
        public Color EditingBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the text color during editing.
        /// </summary>
        public Color EditingTextColor { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of this <see cref="TextBox" />.
        /// </summary>
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

        /// <summary>
        /// Gets the inner size of this <see cref="TextBox" />.
        /// </summary>
        public override Vector2f InnerSize
        {
            get
            {
                var realInnerSize = base.InnerSize;
                var minInnerSize = _MinSize + Padding.Position() + Padding.Size();
                return new Vector2f(Math.Max(realInnerSize.X, minInnerSize.X), Math.Max(realInnerSize.Y, minInnerSize.Y));
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBox"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="characterSize">Initial size of the texts characters.</param>
        /// <param name="font">Initial font. Null for default.</param>
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


        // Methods #########################################################################
        /// <summary>
        /// Occurs when the focus tries to move.
        /// </summary>
        /// <param name="direction">The movements direction.</param>
        protected override void HandleInputMove(float direction)
        {
            if (!InEdit) base.HandleInputMove(direction);
        }

        /// <summary>
        /// Occurs when the user confirms an operation. I.e.: Clicks on a button.
        /// </summary>
        protected override void HandleInputConfirm()
        {
            base.HandleInputConfirm();
            StartEdit();
        }

        /// <summary>
        /// Occurs when the user desires to enter an edit state. I.e.: Focusing a text field.
        /// </summary>
        protected override void HandleInputEdit()
        {
            base.HandleInputEdit();
            StartEdit();
        }

        /// <summary>
        /// Occurs when the user wants to chancel the current operation or dialog.
        /// </summary>
        protected override void HandleInputCancel()
        {
            base.HandleInputCancel();
            StopEdit();
        }

        /// <summary>
        /// Invokes the focus lost event.
        /// </summary>
        protected override void InvokeFocusLost()
        {
            base.InvokeFocusLost();
            StopEdit();
        }

        /// <summary>
        /// Starts the edit mode enabling the <see cref="TextBox"/>s text to be edited.
        /// </summary>
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

        /// <summary>
        /// Stops the edit mode and blocks further text input.
        /// </summary>
        public virtual void StopEdit()
        {
            if (!InEdit) return;
            InEdit = false;
            BackgroundColor = _BackgroundColorBackup;
            TextColor = _TextColorBackup;
            _Caret.Visible = false;
        }

        /// <summary>
        /// Occurs when the user enters text.
        /// </summary>
        /// <param name="tArgs"></param>
        protected override void HandleTextEntered(TextEnteredEventArgs tArgs)
        {
            if (InEdit)
            {
                Text = tArgs.Update(Text, ref _Index);
                UpdateCaretPosition();
                _CaretBlinkTimer = 0; // needed to keep caret visible when moving
            }
        }

        /// <summary>
        /// Updates the caret position.
        /// </summary>
        private void UpdateCaretPosition()
        {
            if (!InEdit) return;
            _Caret.Start.Position = _Text.FindCharacterPos(_Index) + Padding.Position() + new Vector2f(0.5f, 0); // + 0,5 to fix anti aliasing blur issue
            _Caret.End.Position = _Caret.Start.Position + new Vector2f(0, InnerSize.Y - Padding.Top - Padding.Height);
        }

        /// <summary>
        /// Updates the <see cref="TextBox" />. Used for animating the caret.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
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

        /// <summary>
        /// Invokes the text changed event.
        /// </summary>
        protected override void InvokeTextChanged()
        {
            base.InvokeTextChanged();
            _Index = Math.Min(_Index, (uint)Text.Length);
            UpdateCaretPosition();
        }

        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
        protected override void InvokeSizeChanged()
        {
            base.InvokeSizeChanged();
            UpdateCaretPosition();
        }
    }
}