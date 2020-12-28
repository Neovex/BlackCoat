using System;
using System.Linq;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;

namespace BlackCoat.UI
{
    /// <summary>
    /// Base class of all UI elements
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.Container" />
    public abstract class UIComponent : Container
    {
        // Statics #########################################################################
        /// <summary>
        /// Gets the currently active/focused <see cref="UIComponent"/>.
        /// </summary>
        public static UIComponent ACTIVE_COMPONENT { get; internal set; }

        /// <summary>
        /// Gets or sets the allowed focus movement deviation. Default: 30.
        /// This angle defines how far a component can deviate from the direction of a <see cref="ChangeFocus(float)"/> call and still be focused as valid target.
        /// </summary>
        public static UInt32 FOCUS_MOVEMENT_DEVIATION { get; set; } = 30;

        /// <summary>
        /// Holds a <see cref="DialogContainer"/> when a dialog is active.
        /// </summary>
        internal static DialogContainer DIALOG { set; get; }


        // Events ##########################################################################
        /// <summary>
        /// Occurs when the <see cref="UIComponent"/> has gained input focus.
        /// </summary>
        public event Action<UIComponent> FocusGained = c => { };

        /// <summary>
        /// Initialization helper for the <see cref="FocusGained"/> event.
        /// </summary>
        public Action<UIComponent> InitFocusGained { set => FocusGained += value; }

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/> has lost input focus.
        /// </summary>
        public event Action<UIComponent> FocusLost = c => { };

        /// <summary>
        /// Initialization helper for the <see cref="FocusLost"/> event.
        /// </summary>
        public Action<UIComponent> InitFocusLost { set => FocusLost += value; }

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/>s <see cref="Enabled"/> property changes.
        /// </summary>
        public event Action<UIComponent> EnabledChanged = c => { };

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/>s <see cref="Margin"/> property changes.
        /// </summary>
        public event Action<UIComponent> MarginChanged = c => { };

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/>s size changes.
        /// </summary>
        public event Action<UIComponent> SizeChanged = c => { };

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/>s <see cref="Position"/> property changes.
        /// </summary>
        public event Action<UIComponent> PositionChanged = c => { };

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/>s <see cref="Origin"/> property changes.
        /// </summary>
        public event Action<UIComponent> OriginChanged = c => { };

        /// <summary>
        /// Occurs when the <see cref="UIComponent"/>s <see cref="Container"/> property changes.
        /// </summary>
        public event Action<UIComponent> ContainerChanged = c => { };


        // Variables #######################################################################
        protected Rectangle _Background;
        private Boolean _HasFocus;
        private Boolean _GotFocusThisFrame;
        private Boolean _Enabled;
        private FloatRect _Margin;
        private UIInput _Input;
        private UIContainer _Container;


        // Properties ######################################################################
        /// <summary>
        /// Nonfunctional payload of this <see cref="UIComponent"/>. Default: <see langword="null"/>.
        /// </summary>
        public Object Tag { get; set; }

        /// <summary>
        /// Current Input event source.
        /// </summary>
        public virtual UIInput Input
        {
            get => _Input ?? Container?.Input;
            set
            {
                if (value == _Input) return;
                Unsubscribe(_Input);
                _Input = value;
                Subscribe(_Input);
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="UIContainer"/>.
        /// </summary>
        public virtual UIContainer Container
        {
            get => _Container;
            set
            {
                _Container = value;
                Input = Input;
                InvokeContainerChanged();
            }
        }

        /// <summary>
        /// Determines whether this <see cref="UIComponent"/> can be focused.
        /// </summary>
        public bool CanFocus { get; protected set; }

        /// <summary>
        /// Determines whether this <see cref="UIComponent"/> currently has the input focus.
        /// </summary>
        public virtual bool HasFocus
        {
            get => _HasFocus;
            set
            {
                if (!CanFocus || _HasFocus == value) return;
                if (value)
                {
                    if (!Visible || !Enabled) return;
                    InvokeFocusGained();
                    if (ACTIVE_COMPONENT != null) ACTIVE_COMPONENT.HasFocus = false;
                    ACTIVE_COMPONENT = this;
                }
                else
                {
                    if (ACTIVE_COMPONENT == this) ACTIVE_COMPONENT = null;
                    InvokeFocusLost();
                }
                _HasFocus = value;
            }
        }

        /// <summary>
        /// Gets the inner size of this <see cref="UIComponent"/>.
        /// </summary>
        public abstract Vector2f InnerSize { get; }

        /// <summary>
        /// Gets the outer size of this <see cref="UIComponent"/> including position and margin values.
        /// </summary>
        public virtual Vector2f OuterSize => Margin.Position() + InnerSize + Margin.Size();

        /// <summary>
        /// Gets the relative size of this <see cref="UIComponent"/> including position, origin and margin values.
        /// </summary>
        public virtual Vector2f RelativeSize => (Position - Origin) + InnerSize + Margin.Size();

        /// <summary>
        /// Determines whether this <see cref="UIComponent"/> is currently enabled.
        /// </summary>
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                InvokeEnabledChanged();
            }
        }

        /// <summary>
        /// Gets or sets the margin which is the space demand around the outside of the component.
        /// </summary>
        public FloatRect Margin
        {
            get => _Margin;
            set
            {
                if (_Margin == value) return;
                _Margin = value;
                InvokeMarginChanged();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="UIComponent"/>s position in relation to its parent.
        /// </summary>
        public new virtual Vector2f Position
        {
            get => base.Position;
            set
            {
                if (base.Position == value) return;
                base.Position = value;
                InvokePositionChanged();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="UIComponent"/>s point of origin.
        /// </summary>
        public new Vector2f Origin
        {
            get => base.Origin;
            set
            {
                if (base.Origin == value) return;
                base.Origin = value;
                InvokeOriginChanged();
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        public Color BackgroundColor
        {
            get => _Background.FillColor;
            set { _Background.FillColor = value; _Background.Filled = value.A != 0; }
        }

        /// <summary>
        /// Gets or sets the background alpha.
        /// </summary>
        public float BackgroundAlpha
        {
            get => _Background.Alpha;
            set { _Background.Alpha = value; _Background.Filled = value != 0; }
        }

        /// <summary>
        /// Determines the Visibility of the <see cref="UIComponent"/>.
        /// </summary>
        public override bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                if (ACTIVE_COMPONENT != null)
                {
                    ACTIVE_COMPONENT.HasFocus = ACTIVE_COMPONENT.Visible;
                }
            }
        }

        /// <summary>
        /// Retrieves the bottom most <see cref="UIContainer"/> of the scene graphs branch this <see cref="UIComponent"/> is part of.
        /// </summary>
        public UIContainer Root
        {
            get
            {
                if (Container == null) return null;
                var root = Container;
                while (root.Container != null) root = root.Container;
                return root;
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="UIComponent"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        public UIComponent(Core core) : base(core)
        {
            _Enabled = true;
            CollisionShape = new UICollisionShape(_Core.CollisionSystem, this);
            Add(_Background = new Rectangle(_Core, new Vector2f()));
        }


        // Methods #########################################################################
        /// <summary>
        /// Gives the focus to this <see cref="UIComponent"/>.
        /// </summary>
        /// <returns>Whether the <see cref="UIComponent"/> got the focus within the same frame.</returns>
        public virtual bool GiveFocus()
        {
            HasFocus = true;
            _GotFocusThisFrame = HasFocus;
            return _GotFocusThisFrame;
        }

        /// <summary>
        /// Requests a focus change towards a specific direction.
        /// </summary>
        /// <param name="direction">The direction to move the focus.</param>
        protected virtual void ChangeFocus(float direction)
        {
            if (ACTIVE_COMPONENT == null)
            {
                GiveFocus();
                return;
            }

            if (!HasFocus || _GotFocusThisFrame || Container == null) return;

            // Prepare search
            Vector2f _CalculateGlobalCenter(UIComponent component) => component.GlobalPosition + component.InnerSize / 2;
            var globalCenter = _CalculateGlobalCenter(this);
            var allComponents = Root.ComponentsFlattened;
            // Find the focus-able component closest to this one that is within the desired direction (+-FocusMovementSpectrum)
            var target = allComponents.Where(c => c != this && c.CanFocus && c.Visible && c.Enabled).
                                       Select(c => new { component = c, globalCenter = _CalculateGlobalCenter(c) }).
                                       Where(cp => _Core.CollisionSystem.IntersectAngles(globalCenter.AngleTowards(cp.globalCenter),
                                                                                         MathHelper.ValidateAngle(direction - FOCUS_MOVEMENT_DEVIATION),
                                                                                         MathHelper.ValidateAngle(direction + FOCUS_MOVEMENT_DEVIATION))).
                                       OrderBy(cp => cp.globalCenter.DistanceBetweenSquared(globalCenter)).
                                       Select(cp => cp.component).
                                       FirstOrDefault();

            if (target != null) target.GiveFocus();
        }

        /// <summary>
        /// Updates the <see cref="UIComponent" /> and all its entities along the scene graph.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
        public override void Update(float deltaT)
        {
            _GotFocusThisFrame = false;
            base.Update(deltaT);
        }

        #region Dialog Handling
        /// <summary>
        /// Shows a dialog.
        /// </summary>
        /// <param name="dialogLayer">The layer to be used for the dialog. Usually the Overlay layer.</param>
        /// <param name="dialogContent">Content of the dialog.</param>
        /// <exception cref="ArgumentNullException">dialogLayer or dialogContent</exception>
        public void ShowDialog(Layer dialogLayer, UIContainer dialogContent)
        {
            if (dialogLayer == null) throw new ArgumentNullException(nameof(dialogLayer));
            if (dialogContent == null) throw new ArgumentNullException(nameof(dialogContent));

            CloseDialog();
            DIALOG = new DialogContainer(_Core, this, dialogContent) { Input = Input };
            dialogLayer.Add(DIALOG);
            dialogContent.GiveFocus();
        }

        /// <summary>
        /// Closes the current dialog (if any).
        /// </summary>
        public void CloseDialog()
        {
            if (DIALOG != null) DIALOG.Close();
        }
        #endregion

        #region Input Management
        /// <summary>
        /// Subscribes all UI events to the specified input source.
        /// </summary>
        /// <param name="input">The input source.</param>
        private void Subscribe(UIInput input)
        {
            if (input == null) return;
            input.Input.MouseMoved += HandleMouseMoved;

            input.Move += HandleInputMove;
            input.BeforeConfirm += ValidateBeforeConfirm;
            input.Confirm += ValidateConfirm;
            input.Cancel += HandleInputCancel;
            input.Edit += ValidateEdit;
            input.TextEntered += ValidateTextEntered;
            input.Scroll += HandleMouseWheelScrolled;
        }

        /// <summary>
        /// Unsubscribes all UI events from the specified input source.
        /// </summary>
        /// <param name="input">The input source.</param>
        private void Unsubscribe(UIInput input)
        {
            if (input == null) return;
            input.Input.MouseMoved -= HandleMouseMoved;

            input.Move -= HandleInputMove;
            input.BeforeConfirm -= ValidateBeforeConfirm;
            input.Confirm -= ValidateConfirm;
            input.Cancel -= HandleInputCancel;
            input.Edit -= ValidateEdit;
            input.TextEntered -= ValidateTextEntered;
            input.Scroll -= HandleMouseWheelScrolled;
        }

        // Mouse Input
        /// <summary>
        /// Handles the mouse moved event.
        /// </summary>
        /// <param name="pos">The current mouse position.</param>
        protected virtual void HandleMouseMoved(Vector2f pos)
        {
            if (CanFocus && Visible && Enabled && CollisionShape.CollidesWith(pos)) GiveFocus();
        }
        /// <summary>
        /// Handles the mouse wheel scrolled event.
        /// </summary>
        /// <param name="direction">The scroll direction.</param>
        protected virtual void HandleMouseWheelScrolled(float direction) { }


        // Input Validation
        /// <summary>
        /// Validates whether this <see cref="UIComponent"/> is currently valid for receiving input.
        /// </summary>
        /// <returns></returns>
        private bool ValidateInput() => HasFocus && !_GotFocusThisFrame && Visible && Enabled && (!Input.MouseEventActive || CollisionShape.CollidesWith(Input.Input.MousePosition));
        private void ValidateBeforeConfirm() { if (ValidateInput()) HandleInputBeforeConfirm(); }
        private void ValidateConfirm() { if (ValidateInput()) HandleInputConfirm(); }
        private void ValidateEdit() { if (ValidateInput()) HandleInputEdit(); }
        private void ValidateTextEntered(TextEnteredEventArgs tArgs) { if (ValidateInput()) HandleTextEntered(tArgs); }

        // Mapped Input Handlers
        /// <summary>
        /// Occurs when the focus tries to move.
        /// </summary>
        /// <param name="direction">The movements direction.</param>
        protected virtual void HandleInputMove(float direction) => ChangeFocus(direction);

        /// <summary>
        /// Occurs before a confirm event.
        /// </summary>
        protected virtual void HandleInputBeforeConfirm() { }

        /// <summary>
        /// Occurs when the user confirms an operation. I.e.: Clicks on a button.
        /// </summary>
        protected virtual void HandleInputConfirm() { }

        /// <summary>
        /// Occurs when the user wants to chancel the current operation or dialog.
        /// </summary>
        protected virtual void HandleInputCancel() { }

        /// <summary>
        /// Occurs when the user desires to enter an edit state. I.e.: Focusing a text field.
        /// </summary>
        protected virtual void HandleInputEdit() { }

        /// <summary>
        /// Occurs when the user enters text.
        /// </summary>
        protected virtual void HandleTextEntered(TextEnteredEventArgs tArgs) { }
        #endregion

        #region Event Invocation Methods
        /// <summary>
        /// Invokes the focus gained event.
        /// </summary>
        protected virtual void InvokeFocusGained() => FocusGained.Invoke(this);

        /// <summary>
        /// Invokes the focus lost event.
        /// </summary>
        protected virtual void InvokeFocusLost() => FocusLost.Invoke(this);

        /// <summary>
        /// Invokes the enabled changed event.
        /// </summary>
        protected virtual void InvokeEnabledChanged() => EnabledChanged.Invoke(this);

        /// <summary>
        /// Invokes the margin changed event.
        /// </summary>
        protected virtual void InvokeMarginChanged() => MarginChanged.Invoke(this);

        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
        protected virtual void InvokeSizeChanged()
        {
            SizeChanged.Invoke(this);
            _Background.Size = InnerSize;
        }

        /// <summary>
        /// Invokes the position changed event.
        /// </summary>
        protected virtual void InvokePositionChanged() => PositionChanged.Invoke(this);

        /// <summary>
        /// Invokes the origin changed event.
        /// </summary>
        protected virtual void InvokeOriginChanged() => OriginChanged.Invoke(this);

        /// <summary>
        /// Invokes the container changed event.
        /// </summary>
        protected virtual void InvokeContainerChanged() => ContainerChanged.Invoke(this);
        #endregion

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManaged)
        {
            if (ACTIVE_COMPONENT == this) ACTIVE_COMPONENT = null;
            Input = null;
            base.Dispose(disposeManaged);
        }
    }
}