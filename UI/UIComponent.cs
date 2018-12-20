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
        /// Gets the currently active/focused component.
        /// </summary>
        public static UIComponent ActiveComponent { get; internal set; }
        /// <summary>
        /// Gets or sets the focus movement spectrum.
        /// This angle defines how far a component can deviate from the direction of a <see cref="ChangeFocus(float)"/> call and still be focused as valid target.
        /// </summary>
        public static UInt32 FocusMovementSpectrum { get; set; } = 30;


        // Events ##########################################################################
        public event Action<UIComponent> FocusGained = c => { };
        public Action<UIComponent> InitFocusGained { set => FocusGained += value; }
        public event Action<UIComponent> FocusLost = c => { };
        public Action<UIComponent> InitFocusLost { set => FocusLost += value; }
        public event Action<UIComponent> EnabledChanged = c => { };
        public event Action<UIComponent> PaddingChanged = c => { };
        public event Action<UIComponent> SizeChanged = c => { };
        public event Action<UIComponent> PositionChanged = c => { };
        public event Action<UIComponent> OriginChanged = c => { };
        public event Action<UIComponent> ContainerChanged = c => { };


        // Variables #######################################################################
        protected Rectangle _Background;

        private Boolean _HasFocus;
        private Boolean _GotFocusThisFrame;
        private Boolean _Enabled;
        private FloatRect _Padding;
        private UIInput _Input;
        private UIContainer _Container;


        // Properties ######################################################################
        public Object Tag { get; set; }

        public virtual UIInput Input
        {
            get { return _Input; }
            set
            {
                Unsubscribe(_Input);
                _Input = value;
                Subscribe(_Input);
            }
        }

        public virtual UIContainer Container
        {
            get => _Container;
            set { _Container = value; Input = _Container?.Input; InvokeContainerChanged(); }
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
                    if (ActiveComponent != null) ActiveComponent.HasFocus = false;
                    ActiveComponent = this;
                }
                else
                {
                    if (ActiveComponent == this) ActiveComponent = null;
                    InvokeFocusLost();
                }
                _HasFocus = value;
            }
        }

        public abstract Vector2f InnerSize { get; }
        public virtual Vector2f OuterSize => Padding.Position() + InnerSize + Padding.Size();
        public virtual Vector2f RelativeSize => (Position - Origin) + InnerSize + Padding.Size();

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

        public FloatRect Padding
        {
            get => _Padding;
            set
            {
                if (_Padding == value) return;
                _Padding = value;
                InvokePaddingChanged();
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
                if (ActiveComponent != null)
                {
                    ActiveComponent.HasFocus = ActiveComponent.Visible;
                }
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
            if (ActiveComponent == null)
            {
                GiveFocus();
                return;
            }

            if (!HasFocus || _GotFocusThisFrame || Container == null) return;

            // Find UI root
            var root = Container;
            while (root.Container != null) root = root.Container;

            // Prepare search
            Vector2f _CalculateGlobalCenter(UIComponent component) => component.GlobalPosition + component.InnerSize / 2;
            var globalCenter = _CalculateGlobalCenter(this);
            var allComponents = root.ComponentsFlattened;
            // Find the focus-able component closest to this one that is within the desired direction (+-FocusMovementSpectrum)
            var target = allComponents.Where(c => c != this && c.CanFocus).
                                       Select(c => new { component = c, globalCenter = _CalculateGlobalCenter(c) }).
                                       Where(cp => _Core.CollisionSystem.IntersectAngles(globalCenter.AngleTowards(cp.globalCenter),
                                                                                         MathHelper.ValidateAngle(direction - FocusMovementSpectrum),
                                                                                         MathHelper.ValidateAngle(direction + FocusMovementSpectrum))).
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
            if (HasFocus && !CollisionShape.Collide(pos)) HasFocus = false;
            else if (CanFocus && Visible && Enabled && CollisionShape.Collide(pos)) GiveFocus();
        }
        protected virtual void HandleMouseWheelScrolled(float delta) { }


        // Input Validation
        private bool ValidateInput(bool fromMouse) => HasFocus && !_GotFocusThisFrame && Visible && Enabled && 
                                                    (!fromMouse || CollisionShape.Collide(Input.Input.MousePosition));
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
        protected virtual void InvokePaddingChanged() => PaddingChanged.Invoke(this);
        protected virtual void InvokeSizeChanged() { SizeChanged.Invoke(this); _Background.Size = InnerSize; }
        protected virtual void InvokePositionChanged() => PositionChanged.Invoke(this);
        protected virtual void InvokeOriginChanged() => OriginChanged.Invoke(this);
        protected virtual void InvokeContainerChanged() => ContainerChanged.Invoke(this);

        protected override void Destroy(bool disposing)
        {
            if (ActiveComponent == this) ActiveComponent = null;
            Input = null;
            base.Destroy(disposing);
        }
    }
}