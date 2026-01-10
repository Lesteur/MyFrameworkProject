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
        #region Properties - Position

        /// <summary>
        /// Gets the current position of the mouse cursor in screen coordinates.
        /// </summary>
        public Point Position => _currentState.Position;

        /// <summary>
        /// Gets the current X-coordinate of the mouse cursor in screen coordinates.
        /// </summary>
        public int X => _currentState.X;

        /// <summary>
        /// Gets the current Y-coordinate of the mouse cursor in screen coordinates.
        /// </summary>
        public int Y => _currentState.Y;

        #endregion

        #region Properties - Scroll Wheel

        /// <summary>
        /// Gets the current scroll wheel value.
        /// This is a cumulative value that increases when scrolling up and decreases when scrolling down.
        /// </summary>
        public int ScrollWheelValue => _currentState.ScrollWheelValue;

        /// <summary>
        /// Gets the change in the scroll wheel value since the last frame.
        /// Positive values indicate scrolling up, negative values indicate scrolling down.
        /// </summary>
        public int ScrollDelta => _currentState.ScrollWheelValue - _previousState.ScrollWheelValue;

        /// <summary>
        /// Gets whether the mouse was scrolled up this frame.
        /// </summary>
        public bool ScrolledUp => ScrollDelta > 0;

        /// <summary>
        /// Gets whether the mouse was scrolled down this frame.
        /// </summary>
        public bool ScrolledDown => ScrollDelta < 0;

        #endregion

        #region Properties - Movement

        /// <summary>
        /// Gets the change in X-coordinate since the last frame.
        /// Positive values indicate movement to the right, negative values indicate movement to the left.
        /// </summary>
        public int DeltaX => _currentState.X - _previousState.X;

        /// <summary>
        /// Gets the change in Y-coordinate since the last frame.
        /// Positive values indicate movement down, negative values indicate movement up.
        /// </summary>
        public int DeltaY => _currentState.Y - _previousState.Y;

        /// <summary>
        /// Gets whether the mouse moved since the last frame.
        /// </summary>
        public bool HasMoved => DeltaX != 0 || DeltaY != 0;

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
        /// Checks if the left mouse button is currently held down.
        /// </summary>
        /// <returns>True if the left button is currently down; otherwise, false.</returns>
        public bool IsLeftButtonDown()
        {
            return _currentState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the left mouse button was just pressed this frame.
        /// </summary>
        /// <returns>True if the left button was just pressed; otherwise, false.</returns>
        public bool IsLeftButtonPressed()
        {
            return _currentState.LeftButton == ButtonState.Pressed &&
                   _previousState.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the left mouse button was just released this frame.
        /// </summary>
        /// <returns>True if the left button was just released; otherwise, false.</returns>
        public bool IsLeftButtonReleased()
        {
            return _currentState.LeftButton == ButtonState.Released &&
                   _previousState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the right mouse button is currently held down.
        /// </summary>
        /// <returns>True if the right button is currently down; otherwise, false.</returns>
        public bool IsRightButtonDown()
        {
            return _currentState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the right mouse button was just pressed this frame.
        /// </summary>
        /// <returns>True if the right button was just pressed; otherwise, false.</returns>
        public bool IsRightButtonPressed()
        {
            return _currentState.RightButton == ButtonState.Pressed &&
                   _previousState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the right mouse button was just released this frame.
        /// </summary>
        /// <returns>True if the right button was just released; otherwise, false.</returns>
        public bool IsRightButtonReleased()
        {
            return _currentState.RightButton == ButtonState.Released &&
                   _previousState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the middle mouse button is currently held down.
        /// </summary>
        /// <returns>True if the middle button is currently down; otherwise, false.</returns>
        public bool IsMiddleButtonDown()
        {
            return _currentState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the middle mouse button was just pressed this frame.
        /// </summary>
        /// <returns>True if the middle button was just pressed; otherwise, false.</returns>
        public bool IsMiddleButtonPressed()
        {
            return _currentState.MiddleButton == ButtonState.Pressed &&
                   _previousState.MiddleButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the middle mouse button was just released this frame.
        /// </summary>
        /// <returns>True if the middle button was just released; otherwise, false.</returns>
        public bool IsMiddleButtonReleased()
        {
            return _currentState.MiddleButton == ButtonState.Released &&
                   _previousState.MiddleButton == ButtonState.Pressed;
        }

        #endregion

        #region Public Methods - Position Queries

        /// <summary>
        /// Checks if the mouse cursor is within the specified rectangular bounds.
        /// </summary>
        /// <param name="bounds">The rectangular bounds to check.</param>
        /// <returns>True if the mouse is within the bounds; otherwise, false.</returns>
        public bool IsInBounds(Rectangle bounds)
        {
            return bounds.Contains(Position);
        }

        /// <summary>
        /// Checks if the mouse cursor is within the specified rectangular area.
        /// </summary>
        /// <param name="x">The X-coordinate of the top-left corner.</param>
        /// <param name="y">The Y-coordinate of the top-left corner.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns>True if the mouse is within the area; otherwise, false.</returns>
        public bool IsInBounds(int x, int y, int width, int height)
        {
            return IsInBounds(new Rectangle(x, y, width, height));
        }

        #endregion
    }
}
