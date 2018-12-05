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

        public virtual Vector2f OuterSize => InnerSize + new Vector2f(Padding.Left + Padding.Width, Padding.Top + Padding.Height);
        public abstract Vector2f InnerSize { get; }

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
            set => _Background.Color = value;
        }
        public float BackgroundAlpha
        {
            get => _Background.Alpha;
            set => _Background.Alpha = value;
        }


        // CTOR ############################################################################
        public UIComponent(Core core) : base(core)
        {
            _Enabled = true;
            CollisionShape = new UICollisionShape(_Core.CollisionSystem, this);
            Add(_Background = new Rectangle(_Core) { Alpha = 0 }); //?
        }


        // Methods #########################################################################
        public virtual bool GiveFocus() => HasFocus = true;

        protected virtual void ChangeFocus(float direction)
        {
            if (!HasFocus || Container == null) return;

            // Find UI root
            var root = Container;
            while (root.Container != null) root = root.Container;

            // Prepare search
            Vector2f _CalculateGlobalCenter(UIComponent component) => component.GlobalPosition - component.Origin + component.InnerSize / 2;
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

        private void Subscribe(UIInput input)
        {
            if (input == null) return;
            input.Input.MouseMoved += HandleMouseMoved;
            input.Input.MouseWheelScrolled += HandleMouseWheelScrolled;

            input.Move += HandleInputMove;
            input.BeforeConfirm += HandleInputBeforeConfirm;
            input.Confirm += HandleInputConfirm;
            input.Cancel += HandleInputCancel;
            input.Edit += HandleInputEdit;
            input.TextEntered += HandleTextEntered;
        }
        private void Unsubscribe(UIInput input)
        {
            if (input == null) return;
            input.Input.MouseMoved -= HandleMouseMoved;
            input.Input.MouseWheelScrolled -= HandleMouseWheelScrolled;

            input.Move -= HandleInputMove;
            input.BeforeConfirm -= HandleInputBeforeConfirm;
            input.Confirm -= HandleInputConfirm;
            input.Cancel -= HandleInputCancel;
            input.Edit -= HandleInputEdit;
            input.TextEntered -= HandleTextEntered;
        }

        // Mouse Input
        protected virtual void HandleMouseMoved(Vector2f pos)
        {
            if (CanFocus && Visible && Enabled && CollisionShape.Collide(pos)) GiveFocus();
        }
        protected virtual void HandleMouseWheelScrolled(float delta) { }

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
        protected virtual void InvokeSizeChanged() { SizeChanged.Invoke(this); _Background.Size = InnerSize; } // FIXME?
        protected virtual void InvokePositionChanged() => PositionChanged.Invoke(this);
        protected virtual void InvokeOriginChanged() => OriginChanged.Invoke(this);
        protected virtual void InvokeContainerChanged() => ContainerChanged.Invoke(this);

        protected override void Destroy(bool disposing)
        {
            Input = null;
            base.Destroy(disposing);
        }
    }
}