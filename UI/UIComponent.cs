using System;
using System.Linq;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;

namespace BlackCoat.UI
{
    public abstract class UIComponent : Container
    {
        // Statics #########################################################################
        /// <summary>
        /// Gets the currently active/focused <see cref="UIComponent"/>.
        /// </summary>
        public static UIComponent ACTIVE_COMPONENT { get; internal set; }
        /// <summary>
        /// Gets or sets the focus movement spectrum. Default: 30.
        /// This angle defines how far a component can deviate from the direction of a <see cref="ChangeFocus(float)"/> call and still be focused as valid target.
        /// </summary>
        public static UInt32 FOCUS_MOVEMENT_SPECTRUM { get; set; } = 30;
        /// <summary>
        /// Holds a <see cref="DialogContainer"/> when a dialog is active.
        /// </summary>
        internal static DialogContainer DIALOG { set; get; }


        // Events ##########################################################################
        public event Action<UIComponent> FocusGained = c => { };
        public Action<UIComponent> InitFocusGained { set => FocusGained += value; }
        public event Action<UIComponent> FocusLost = c => { };
        public Action<UIComponent> InitFocusLost { set => FocusLost += value; }
        public event Action<UIComponent> EnabledChanged = c => { };
        public event Action<UIComponent> MarginChanged = c => { };
        public event Action<UIComponent> SizeChanged = c => { };
        public event Action<UIComponent> PositionChanged = c => { };
        public event Action<UIComponent> OriginChanged = c => { };
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
        public Object Tag { get; set; }

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

        public bool CanFocus { get; protected set; }

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

        public abstract Vector2f InnerSize { get; }
        public virtual Vector2f OuterSize => Margin.Position() + InnerSize + Margin.Size();
        public virtual Vector2f RelativeSize => (Position - Origin) + InnerSize + Margin.Size();

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

        public Color BackgroundColor
        {
            get => _Background.Color;
            set => _Background.Visible = (_Background.Color = value).A != 0;
        }
        public float BackgroundAlpha
        {
            get => _Background.Alpha;
            set => _Background.Visible = (_Background.Alpha = value) != 0;
        }

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
        public UIComponent(Core core) : base(core)
        {
            _Enabled = true;
            CollisionShape = new UICollisionShape(_Core.CollisionSystem, this);
            Add(_Background = new Rectangle(_Core) { Alpha = 0 });
        }


        // Methods #########################################################################
        public virtual bool GiveFocus()
        {
            HasFocus = true;
            _GotFocusThisFrame = HasFocus;
            return _GotFocusThisFrame;
        }

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
                                                                                         MathHelper.ValidateAngle(direction - FOCUS_MOVEMENT_SPECTRUM),
                                                                                         MathHelper.ValidateAngle(direction + FOCUS_MOVEMENT_SPECTRUM))).
                                       OrderBy(cp => cp.globalCenter.DistanceBetweenSquared(globalCenter)).
                                       Select(cp => cp.component).
                                       FirstOrDefault();

            if (target != null) target.GiveFocus();
        }

        public override void Update(float deltaT)
        {
            _GotFocusThisFrame = false;
            base.Update(deltaT);
        }

        // Dialog Handling
        public void ShowDialog(Layer dialogLayer, UIComponent dialogContent)
        {
            if (dialogLayer == null) throw new ArgumentNullException(nameof(dialogLayer));
            if (dialogContent == null) throw new ArgumentNullException(nameof(dialogContent));

            CloseDialog();
            DIALOG = new DialogContainer(_Core, this) { Input = Input };
            DIALOG.Add(dialogContent);
            dialogLayer.Add(DIALOG);
            dialogContent.GiveFocus();
        }
        public void CloseDialog()
        {
            if (DIALOG != null) DIALOG.Close();
        }

        // Input Management
        private void Subscribe(UIInput input)
        {
            if (input == null) return;
            input.Input.MouseMoved += HandleMouseMoved;
            input.Input.MouseWheelScrolled += HandleMouseWheelScrolled;

            input.Move += HandleInputMove;
            input.BeforeConfirm += ValidateBeforeConfirm;
            input.Confirm += ValidateConfirm;
            input.Cancel += HandleInputCancel;
            input.Edit += ValidateEdit;
            input.TextEntered += ValidateTextEntered;
        }
        private void Unsubscribe(UIInput input)
        {
            if (input == null) return;
            input.Input.MouseMoved -= HandleMouseMoved;
            input.Input.MouseWheelScrolled -= HandleMouseWheelScrolled;

            input.Move -= HandleInputMove;
            input.BeforeConfirm -= ValidateBeforeConfirm;
            input.Confirm -= ValidateConfirm;
            input.Cancel -= HandleInputCancel;
            input.Edit -= ValidateEdit;
            input.TextEntered -= ValidateTextEntered;
        }

        // Mouse Input
        protected virtual void HandleMouseMoved(Vector2f pos)
        {
            if (CanFocus && Visible && Enabled && CollisionShape.CollidesWith(pos)) GiveFocus();
        }
        protected virtual void HandleMouseWheelScrolled(float delta) { }


        // Input Validation
        private bool ValidateInput(bool fromMouse) => HasFocus && !_GotFocusThisFrame && Visible && Enabled &&
                                                    (!fromMouse || CollisionShape.CollidesWith(Input.Input.MousePosition));
        private void ValidateBeforeConfirm(bool fromMouse)
        {
            if (ValidateInput(fromMouse)) HandleInputBeforeConfirm();
        }
        private void ValidateConfirm(bool fromMouse)
        {
            if (ValidateInput(fromMouse)) HandleInputConfirm();
        }
        private void ValidateEdit()
        {
            if (ValidateInput(false)) HandleInputEdit();
        }
        private void ValidateTextEntered(TextEnteredEventArgs tArgs)
        {
            if (ValidateInput(false)) HandleTextEntered(tArgs);
        }

        // Generalized Mapped Input
        protected virtual void HandleInputMove(float direction) => ChangeFocus(direction);
        protected virtual void HandleInputBeforeConfirm() { }
        protected virtual void HandleInputConfirm() { }
        protected virtual void HandleInputCancel() { }
        protected virtual void HandleInputEdit() { }
        protected virtual void HandleTextEntered(TextEnteredEventArgs tArgs) { }


        // Event Invocation Methods ########################################################
        protected virtual void InvokeFocusGained() => FocusGained.Invoke(this);
        protected virtual void InvokeFocusLost() => FocusLost.Invoke(this);
        protected virtual void InvokeEnabledChanged() => EnabledChanged.Invoke(this);
        protected virtual void InvokeMarginChanged() => MarginChanged.Invoke(this);
        protected virtual void InvokeSizeChanged() { SizeChanged.Invoke(this); _Background.Size = InnerSize; }
        protected virtual void InvokePositionChanged() => PositionChanged.Invoke(this);
        protected virtual void InvokeOriginChanged() => OriginChanged.Invoke(this);
        protected virtual void InvokeContainerChanged() => ContainerChanged.Invoke(this);

        protected override void Destroy(bool disposing)
        {
            if (ACTIVE_COMPONENT == this) ACTIVE_COMPONENT = null;
            Input = null;
            base.Destroy(disposing);
        }
    }
}