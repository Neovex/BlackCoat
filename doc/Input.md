# Input

Now that you're all ready to go you might wanna expand on the interactive side of your project. Not that there is anything wrong with floating rectangles but it might get a little stale over time when there is no way to control any of it. Therefore lets have a closer look on how BlackCoat handles user input.

## Mouse & Keyboard

While you can create your own instance of the `Input` class it is usually much easier to use your own scenes `Input` property. With it you get live access to all input events and its current status. For small input requests you can simply test against one of the Inputs properties like so:
```
Vector2f mPos = Input.MousePosition;
bool lmb = Input.LeftMouseButtonPressed;
bool alt = Input.AltKeyPressed;
bool esc = Input.IsKeyDown(Keyboard.Key.Escape);
```
This way you can quickly check whether a specific a button is down in your `Update(float deltaT)` method:
```
protected override void Update(float deltaT)
{
    if (Input.IsKeyDown(Keyboard.Key.Space)) Jump();
}
```
For more sophisticated input handling you could alternatively use the event approach. While you can always use lambdas to handle events it is highly recommended to use classic handlers instead since you should always clean up your events at the end of you scene which is not possible with lambdas:
```
protected override bool Load()
{
    Input.KeyPressed += Input_KeyPressed;
    return true;
}

private void Input_KeyPressed(Keyboard.Key key)
{
    switch (key)
    {
        case Keyboard.Key.Space:
            Jump();
        break;
        [...]
    }
}

protected override void Destroy()
{
    Input.KeyPressed -= Input_KeyPressed;
}
```
Pro-Tip: avoid using both techniques at the same time, instead choose one that fits the scope of your project and then stick with it.

### Handling Text

Since handling raw text input is usually only relevant within a UI component like a text field, Black Coats UI framework will handle all the text management for you. However if you want to do it yourself you'll want to subscribe to the `TextEntered` event. The `Modification` property of the provided `TextEnteredEventArgs` will let you know what kind of operation the user has performed, as the `TextEntered` event does not only handle text input but also text navigation i.e. when the user presses the End button. If you are only interested in the raw entered text you'll find it in the arguments `Text` property, however when you want to actually edit a string it's a lot easier to let the arguments `Update` method handle it for you:
```
private void Input_TextEntered(TextEnteredEventArgs args)
{
    myString = args.Update(myString, ref myCaretPosition);
}
```

## Joysticks & Gamepads

When working with non-standardized input hardware (joysticks, gamepads, pedals etc.) you'll notice 2 differences compared to handling mouse/keyboard input:
1. There is no enumeration for any of the buttons, instead all buttons are simply numbered
2. Each interaction either requires or provides an extra number that defines WHICH device is the target or source

Apart form that the work flow is the same as it is with keyboard and mouse. Example:
```
protected override void Update(float deltaT)
{
    if (Input.IsJoystickButtonDown(0, 0) Jump();
    var movement = new Vector2f(
        Input.GetJoystickPositionFor(0, Joystick.Axis.X),
        Input.GetJoystickPositionFor(0, Joystick.Axis.Y)
    );
}
```
As you can see, each call to input requires an extra ID that defines which device to request the information from. Similarly, when either requesting pressed buttons or listening to button presses via event, you'll only get a number to identify a button. Since this can get very confusing when all you have to work with is numbers, it is therefore recommended to always use BlackCoats input mapping as described below.

Pro-Tip: When handling gamepads, joysticks and so on, you should always subscribe to the `JoystickConnected` and `JoystickDisconnected` events to keep track whether a new player joins or someone fell over a cable. You can then either fall back to a mouse/keyboard based input or pause the game until a new device connects.

## Input Mapping

Ever wanted to have "jump" on a mouse button or "dash" on F8? No? Good! However, somebody might still disagree with you on that and therefore want to remap your projects interaction options onto different keys. So far, when you use the `Input` property of your scene to create a bridge between an input event and an action within your code that binding is final. For small projects, demos or tests this is perfectly fine but as soon as your project grows bigger you'll want to give your users more freedom in how they control your projects actions. For that matter Black Coat provides you with ways to create editable input mappings.

### Simple Input Map

The `SimpleInputMap` is the fastest and easiest way to create an editable mapping between key presses on different devices and a custom set of actions defined by your project. To get started you need to define all possible actions via enumeration.
```
enum MyGameAction
{
    Left,
    Right,
    Jump,
    Fire
}
```
With your actions defined you can now create your first mapping:
```
var myInputMap = new SimpleInputMap<MyGameAction>(Input);
myInputMap.AddKeyboardMapping(Keyboard.Key.A, MyGameAction.Left);
myInputMap.AddKeyboardMapping(Keyboard.Key.D, MyGameAction.Right);
myInputMap.AddKeyboardMapping(Keyboard.Key.Space, MyGameAction.Jump);
myInputMap.AddMouseMapping(Mouse.Button.Left, MyGameAction.Fire);
```
All that is left is subscribing to the `MappedOperationInvoked` event and provide it with a handler. From this moment forward you are exclusively working with your own set of actions having the input completely abstracted away:
```
myInputMap.MappedOperationInvoked += HandleMappedInput;
[...]
private void HandleMappedInput(MyGameAction action, bool activate)
{
    switch (action)
    {
        case MyGameAction.Jump:
            if(activate) Jump();
        break;
        [...]
    }
}
```
The `activate` parameter determines whether a button was pressed `true` or released `false` and thats it, now you have all you need to let a player decide what each button will do, be it keyboard, mouse or joystick. Please note: when using multiple input devices you are required to use one instance of `SimpleInputMap` per device. To do so you must define a filter in the constructor to bind a specific device ID to each instance, this way each player can even have his own custom mapping.