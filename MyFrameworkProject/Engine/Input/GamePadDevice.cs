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
        #region Fields - Configuration

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

        /// <summary>
        /// Gets the player index identifying which gamepad this device represents.
        /// </summary>
        public PlayerIndex PlayerIndex => _playerIndex;

        #endregion

        #region Properties - Analog Sticks

        /// <summary>
        /// Gets the current position of the left analog stick.
        /// X and Y values range from -1.0 (left/down) to 1.0 (right/up), with (0, 0) at center.
        /// </summary>
        public Vector2 LeftStick => _currentState.ThumbSticks.Left;

        /// <summary>
        /// Gets the current position of the right analog stick.
        /// X and Y values range from -1.0 (left/down) to 1.0 (right/up), with (0, 0) at center.
        /// </summary>
        public Vector2 RightStick => _currentState.ThumbSticks.Right;

        #endregion

        #region Properties - Triggers

        /// <summary>
        /// Gets the current position of the left trigger.
        /// Values range from 0.0 (not pressed) to 1.0 (fully pressed).
        /// </summary>
        public float LeftTrigger => _currentState.Triggers.Left;

        /// <summary>
        /// Gets the current position of the right trigger.
        /// Values range from 0.0 (not pressed) to 1.0 (fully pressed).
        /// </summary>
        public float RightTrigger => _currentState.Triggers.Right;

        #endregion

        #region Properties - D-Pad

        /// <summary>
        /// Gets whether the D-Pad Up direction is currently pressed.
        /// </summary>
        public bool DPadUp => _currentState.DPad.Up == ButtonState.Pressed;

        /// <summary>
        /// Gets whether the D-Pad Down direction is currently pressed.
        /// </summary>
        public bool DPadDown => _currentState.DPad.Down == ButtonState.Pressed;

        /// <summary>
        /// Gets whether the D-Pad Left direction is currently pressed.
        /// </summary>
        public bool DPadLeft => _currentState.DPad.Left == ButtonState.Pressed;

        /// <summary>
        /// Gets whether the D-Pad Right direction is currently pressed.
        /// </summary>
        public bool DPadRight => _currentState.DPad.Right == ButtonState.Pressed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadDevice"/> class for the specified player.
        /// </summary>
        /// <param name="playerIndex">The player index identifying which gamepad to track (e.g., PlayerIndex.One).</param>
        public GamePadDevice(PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;
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
        /// Checks if a button is currently up (not pressed) in this frame.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button is currently up; otherwise, false.</returns>
        public bool IsButtonUp(Buttons button)
        {
            return _currentState.IsButtonUp(button);
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
            return _currentState.IsButtonDown(button) && _previousState.IsButtonUp(button);
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
            return _currentState.IsButtonUp(button) && _previousState.IsButtonDown(button);
        }

        #endregion

        #region Public Methods - Multiple Button Queries

        /// <summary>
        /// Checks if any of the specified buttons are currently held down.
        /// </summary>
        /// <param name="buttons">The buttons to check.</param>
        /// <returns>True if any of the buttons are currently down; otherwise, false.</returns>
        public bool IsAnyButtonDown(params Buttons[] buttons)
        {
            foreach (var button in buttons)
            {
                if (IsButtonDown(button))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if all of the specified buttons are currently held down.
        /// </summary>
        /// <param name="buttons">The buttons to check.</param>
        /// <returns>True if all of the buttons are currently down; otherwise, false.</returns>
        public bool IsAllButtonsDown(params Buttons[] buttons)
        {
            foreach (var button in buttons)
            {
                if (!IsButtonDown(button))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if any of the specified buttons were just pressed this frame.
        /// </summary>
        /// <param name="buttons">The buttons to check.</param>
        /// <returns>True if any of the buttons were just pressed; otherwise, false.</returns>
        public bool IsAnyButtonPressed(params Buttons[] buttons)
        {
            foreach (var button in buttons)
            {
                if (IsButtonPressed(button))
                    return true;
            }
            return false;
        }

        #endregion

        #region Public Methods - Vibration

        /// <summary>
        /// Sets the vibration motor speeds on the gamepad.
        /// </summary>
        /// <param name="leftMotor">The speed of the left motor (0.0 to 1.0).</param>
        /// <param name="rightMotor">The speed of the right motor (0.0 to 1.0).</param>
        /// <returns>True if the vibration was set successfully; otherwise, false.</returns>
        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return GamePad.SetVibration(_playerIndex, leftMotor, rightMotor);
        }

        /// <summary>
        /// Stops all vibration on the gamepad.
        /// </summary>
        /// <returns>True if the vibration was stopped successfully; otherwise, false.</returns>
        public bool StopVibration()
        {
            return GamePad.SetVibration(_playerIndex, 0f, 0f);
        }

        #endregion

        #region Public Methods - Trigger Queries

        /// <summary>
        /// Checks if the left trigger is pressed beyond a specified threshold.
        /// </summary>
        /// <param name="threshold">The threshold value (0.0 to 1.0). Default is 0.5.</param>
        /// <returns>True if the left trigger exceeds the threshold; otherwise, false.</returns>
        public bool IsLeftTriggerDown(float threshold = 0.5f)
        {
            return LeftTrigger > threshold;
        }

        /// <summary>
        /// Checks if the right trigger is pressed beyond a specified threshold.
        /// </summary>
        /// <param name="threshold">The threshold value (0.0 to 1.0). Default is 0.5.</param>
        /// <returns>True if the right trigger exceeds the threshold; otherwise, false.</returns>
        public bool IsRightTriggerDown(float threshold = 0.5f)
        {
            return RightTrigger > threshold;
        }

        #endregion
    }
}