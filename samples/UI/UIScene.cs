using SFML.System;
using SFML.Graphics;

using BlackCoat;
using BlackCoat.UI;

namespace UI
{
    class UIScene : Scene
    {
        private FloatRect _TextPadding = new FloatRect(6, 4, 6, 4);
        private Canvas _UIRoot;
        private Canvas _Dialog;

        public UIScene(Core core) : base(core)
        { }


        protected override bool Load()
        {
            // UIs require their own input wrapper
            var uiInput = new UIInput(Input, true);

            // Create UI root entity
            _UIRoot = new Canvas(_Core, _Core.DeviceSize)
            {
                Input = uiInput, // never forget to add a uiInput to the root of any UI
                BackgroundColor = Color.Cyan, // All UI components support textures but lets keep it simple for now
                Init = new[]
                {
                    new OffsetContainer(_Core, Orientation.Vertical, 10)
                    {
                        Position = new Vector2f(50,50),
                        Init = new []
                        {
                            new Button(_Core, null, new Label(_Core, "Show Dialog") { Padding = _TextPadding })
                            {
                                Name = "Button 1",
                                BackgroundColor = Color.Blue,
                                InitPressed = b => b.ShowDialog(Layer_Overlay, _Dialog),
                                InitFocusGained = HandleFocusGained,
                                InitFocusLost = HandleFocusLost
                            },
                            new Button(_Core, null, new Label(_Core, "Exit") { Padding = _TextPadding })
                            {
                                Name = "Button 2",
                                BackgroundColor = Color.Blue,
                                InitPressed = b => _Core.Exit(),
                                InitFocusGained = HandleFocusGained,
                                InitFocusLost = HandleFocusLost
                            }
                        }
                    }
                }
            };
            Layer_Game.Add(_UIRoot);

            // Handle UI resize
            _Core.DeviceResized += _UIRoot.Resize;

            // Create the Dialog
            _Dialog = new Canvas(_Core, _Core.DeviceSize / 2)
            {
                Position = _Core.DeviceSize / 2,
                Origin = _Core.DeviceSize / 4,
                BackgroundColor = Color.Green,
                Init = new UIComponent[]
                {
                    new AlignedContainer(_Core, Alignment.Center,
                        new OffsetContainer(_Core, Orientation.Vertical,20,
                            new Label(_Core, "Dialog Title", 20),
                            new OffsetContainer(_Core, Orientation.Horizontal, 30,
                                new Button(_Core, null, new Label(_Core, "OK") { Padding = _TextPadding })
                                {
                                    Name = "Dialog Button OK",
                                    BackgroundColor = Color.Blue,
                                    InitPressed = b => b.CloseDialog(),
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                },
                                new Button(_Core, null, new Label(_Core, "Cancel") { Padding = _TextPadding })
                                {
                                    Name = "Dialog Button Cancel",
                                    BackgroundColor = Color.Blue,
                                    InitPressed = b => b.CloseDialog(),
                                    InitFocusGained = HandleFocusGained,
                                    InitFocusLost = HandleFocusLost
                                }
                            )
                        )
                    )
                }
            };

            OpenInspector();
            return true;
        }

        private void HandleFocusGained(UIComponent c)
        {
            c.BackgroundColor = Color.Red;
        }

        private void HandleFocusLost(UIComponent c)
        {
            c.BackgroundColor = Color.Blue;
        }

        protected override void Update(float deltaT)
        { }

        protected override void Destroy()
        {
            _Core.DeviceResized -= _UIRoot.Resize;
        }
    }
}