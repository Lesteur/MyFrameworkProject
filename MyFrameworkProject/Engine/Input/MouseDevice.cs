using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Represents a mouse input device that tracks mouse states across frames.
    /// Provides access to mouse position, button states, and scroll wheel changes.
    /// Inherits state tracking capabilities from <see cref="InputDevice{TState}"/>.
    /// </summary>
    public sealed class MouseDevice : InputDevice<MouseState>
    {
        #region Properties

        /// <summary>
        /// Gets the current position of the mouse cursor in screen coordinates.
        /// </summary>
        public Point Position => _currentState.Position;

        /// <summary>
        /// Gets the change in the scroll wheel value since the last frame.
        /// Positive values indicate scrolling up, negative values indicate scrolling down.
        /// </summary>
        public int ScrollDelta =>
            _currentState.ScrollWheelValue - _previousState.ScrollWheelValue;

        #endregion

        #region Protected Methods - State Retrieval

        /// <summary>
        /// Gets the current state of the mouse from the hardware.
        /// Called internally by the Update method to refresh mouse state.
        /// </summary>
        /// <returns>The current <see cref="MouseState"/> from the MonoGame framework.</returns>
        protected override MouseState GetState()
        {
            return Mouse.GetState();
        }

        #endregion

        #region Public Methods - Button State Queries

        /// <summary>
        /// Checks if a mouse button is currently pressed based on its button state.
        /// This is a static utility method that can be used to check any <see cref="ButtonState"/>.
        /// </summary>
        /// <param name="state">The button state to check.</param>
        /// <returns>True if the button state is Pressed; otherwise, false.</returns>
        public static bool IsButtonDown(ButtonState state)
        {
            return state == ButtonState.Pressed;
        }

        #endregion
    }
}
