using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    public sealed class GamePadDevice(PlayerIndex index) : InputDevice<GamePadState>
    {
        private readonly PlayerIndex _playerIndex = index;

        protected override GamePadState GetState()
        {
            return GamePad.GetState(_playerIndex);
        }

        public bool IsConnected => _currentState.IsConnected;

        public bool IsButtonDown(Buttons button)
        {
            return _currentState.IsButtonDown(button);
        }

        public bool IsButtonPressed(Buttons button)
        {
            return _currentState.IsButtonDown(button) &&
                   _previousState.IsButtonUp(button);
        }

        public bool IsButtonReleased(Buttons button)
        {
            return _currentState.IsButtonUp(button) &&
                   _previousState.IsButtonDown(button);
        }

        public Vector2 LeftStick => _currentState.ThumbSticks.Left;
    }
}