using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Represents a keyboard input device that tracks key states across frames.
    /// Provides methods to check if keys are currently held down, were just pressed, or were just released.
    /// Inherits state tracking capabilities from <see cref="InputDevice{TState}"/>.
    /// </summary>
    public sealed class KeyboardDevice : InputDevice<KeyboardState>
    {
        #region Protected Methods - State Retrieval

        /// <summary>
        /// Gets the current state of the keyboard from the hardware.
        /// Called internally by the Update method to refresh keyboard state.
        /// </summary>
        /// <returns>The current <see cref="KeyboardState"/> from the MonoGame framework.</returns>
        protected override KeyboardState GetState()
        {
            return Keyboard.GetState();
        }

        #endregion

        #region Public Methods - Key State Queries

        /// <summary>
        /// Checks if a key is currently held down in this frame.
        /// Returns true for the entire duration the key is held.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is currently down; otherwise, false.</returns>
        public bool IsDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if a key was just pressed this frame.
        /// Returns true only on the first frame the key transitions from up to down.
        /// Useful for detecting single key press events without repetition.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just pressed this frame; otherwise, false.</returns>
        public bool IsPressed(Keys key)
        {
            return _currentState.IsKeyDown(key) &&
                   _previousState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a key was just released this frame.
        /// Returns true only on the first frame the key transitions from down to up.
        /// Useful for detecting key release events.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just released this frame; otherwise, false.</returns>
        public bool IsReleased(Keys key)
        {
            return _currentState.IsKeyUp(key) &&
                   _previousState.IsKeyDown(key);
        }

        #endregion
    }
}