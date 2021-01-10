using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Window;

using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.InputMapping;

namespace InputMapping
{
    enum MyGameAction
    {
        Left,
        Right,
        Jump,
        Fire
    }

    class InputScene : Scene
    {
        private SimpleInputMap<MyGameAction> _MyInput;
        private TextItem _Display;
        private List<MyGameAction> _Actions;

        public InputScene(Core core) : base(core)
        { }


        protected override bool Load()
        {
            _MyInput = new SimpleInputMap<MyGameAction>(Input);
            _MyInput.AddKeyboardMapping(Keyboard.Key.A, MyGameAction.Left);
            _MyInput.AddKeyboardMapping(Keyboard.Key.D, MyGameAction.Right);
            _MyInput.AddKeyboardMapping(Keyboard.Key.Space, MyGameAction.Jump);
            _MyInput.AddMouseMapping(Mouse.Button.Left, MyGameAction.Fire);
            _MyInput.MappedOperationInvoked += HandleMappedInput;

            _Display = new TextItem(_Core, "Press one of the defined input keys.");
            Layer_Game.Add(_Display);

            _Actions = new List<MyGameAction>();
            return true;
        }

        private void HandleMappedInput(MyGameAction action, bool activate)
        {
            if (activate) _Actions.Add(action);
            else _Actions.Remove(action);

            _Display.Text = String.Join(Constants.NEW_LINE, _Actions.Select(a => a.ToString()));
            if (String.IsNullOrWhiteSpace(_Display.Text)) _Display.Text = "No Input";
        }

        protected override void Update(float deltaT)
        { }

        protected override void Destroy()
        {
            if (_MyInput != null) _MyInput.MappedOperationInvoked -= HandleMappedInput;
        }
    }
}