using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Represents a gamepad input device that tracks button and analog stick states across frames.
    /// Provides methods to check button presses, releases, holds, and analog stick positions.
    /// Supports multiple controllers through player index assignment.
    /// Inherits state tracking capabilities from <see cref="InputDevice{TState}"/>.
    /// </summary>
    public sealed class GamePadDevice : InputDevice<GamePadState>
    {
        #region Fields

        /// <summary>
        /// The player index identifying which gamepad this device represents (Player1, Player2, etc.).
        /// </summary>
        private readonly PlayerIndex _playerIndex;

        #endregion

        #region Properties - Connection

        /// <summary>
        /// Gets a value indicating whether the gamepad is currently connected.
        /// </summary>
        public bool IsConnected => _currentState.IsConnected;

        #endregion

        #region Properties - Analog Sticks

        /// <summary>
        /// Gets the current position of the left analog stick.
        /// X and Y values range from -1.0 (left/down) to 1.0 (right/up), with (0, 0) at center.
        /// </summary>
        public Vector2 LeftStick => _currentState.ThumbSticks.Left;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadDevice"/> class for the specified player.
        /// </summary>
        /// <param name="index">The player index identifying which gamepad to track (e.g., PlayerIndex.One).</param>
        public GamePadDevice(PlayerIndex index)
        {
            _playerIndex = index;
        }

        #endregion

        #region Protected Methods - State Retrieval

        /// <summary>
        /// Gets the current state of the gamepad from the hardware.
        /// Called internally by the Update method to refresh gamepad state.
        /// </summary>
        /// <returns>The current <see cref="GamePadState"/> for this player index from the MonoGame framework.</returns>
        protected override GamePadState GetState()
        {
            return GamePad.GetState(_playerIndex);
        }

        #endregion

        #region Public Methods - Button State Queries

        /// <summary>
        /// Checks if a button is currently held down in this frame.
        /// Returns true for the entire duration the button is held.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button is currently down; otherwise, false.</returns>
        public bool IsButtonDown(Buttons button)
        {
            return _currentState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if a button was just pressed this frame.
        /// Returns true only on the first frame the button transitions from up to down.
        /// Useful for detecting single button press events without repetition.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button was just pressed this frame; otherwise, false.</returns>
        public bool IsButtonPressed(Buttons button)
        {
            return _currentState.IsButtonDown(button) &&
                   _previousState.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if a button was just released this frame.
        /// Returns true only on the first frame the button transitions from down to up.
        /// Useful for detecting button release events.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button was just released this frame; otherwise, false.</returns>
        public bool IsButtonReleased(Buttons button)
        {
            return _currentState.IsButtonUp(button) &&
                   _previousState.IsButtonDown(button);
        }

        #endregion
    }
}