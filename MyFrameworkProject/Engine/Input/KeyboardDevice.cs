using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    public sealed class KeyboardDevice : InputDevice<KeyboardState>
    {
        protected override KeyboardState GetState()
        {
            return Keyboard.GetState();
        }

        public bool IsDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        public bool IsPressed(Keys key)
        {
            return _currentState.IsKeyDown(key) &&
                   _previousState.IsKeyUp(key);
        }

        public bool IsReleased(Keys key)
        {
            return _currentState.IsKeyUp(key) &&
                   _previousState.IsKeyDown(key);
        }
    }
}