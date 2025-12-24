using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Input
{
    public sealed class MouseDevice : InputDevice<MouseState>
    {
        protected override MouseState GetState()
        {
            return Mouse.GetState();
        }

        public bool IsButtonDown(ButtonState state)
        {
            return state == ButtonState.Pressed;
        }

        public Point Position => _currentState.Position;

        public int ScrollDelta =>
            _currentState.ScrollWheelValue - _previousState.ScrollWheelValue;
    }
}
