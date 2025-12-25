namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Abstract base class for input devices that tracks current and previous state.
    /// Provides a generic framework for implementing input handling with state comparison capabilities.
    /// This enables detection of button presses, releases, and holds across different input devices.
    /// </summary>
    /// <typeparam name="TState">The type representing the state of the input device (e.g., KeyboardState, GamePadState).</typeparam>
    public abstract class InputDevice<TState>
    {
        #region Fields - State

        /// <summary>
        /// The current state of the input device for this frame.
        /// Updated every frame by calling <see cref="Update"/>.
        /// </summary>
        protected TState _currentState;

        /// <summary>
        /// The previous state of the input device from the last frame.
        /// Used to detect state changes such as button presses and releases.
        /// </summary>
        protected TState _previousState;

        #endregion

        #region Public Methods - Lifecycle

        /// <summary>
        /// Updates the input device state by saving the current state as previous and retrieving the new current state.
        /// This method should be called once per frame before checking input.
        /// Virtual to allow derived classes to add additional update logic.
        /// </summary>
        public virtual void Update()
        {
            _previousState = _currentState;
            _currentState = GetState();
        }

        /// <summary>
        /// Resets the input device state by setting both current and previous states to the current hardware state.
        /// This effectively clears any state change detection until the next Update call.
        /// Useful for initialization or when input state needs to be cleared.
        /// </summary>
        public virtual void Reset()
        {
            _currentState = GetState();
            _previousState = _currentState;
        }

        #endregion

        #region Protected Methods - Abstract

        /// <summary>
        /// Gets the current state of the input device from the hardware.
        /// Must be implemented by derived classes to retrieve device-specific state.
        /// </summary>
        /// <returns>The current state of the input device.</returns>
        protected abstract TState GetState();

        #endregion
    }
}