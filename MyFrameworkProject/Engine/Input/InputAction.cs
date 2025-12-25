namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Defines logical input actions that can be mapped to physical input devices.
    /// These abstract actions decouple game logic from specific hardware inputs,
    /// allowing flexible input remapping and multi-device support.
    /// </summary>
    public enum InputAction
    {
        /// <summary>
        /// Represents a leftward movement or navigation action.
        /// </summary>
        Left,

        /// <summary>
        /// Represents a rightward movement or navigation action.
        /// </summary>
        Right,

        /// <summary>
        /// Represents an upward movement or navigation action.
        /// </summary>
        Up,

        /// <summary>
        /// Represents a downward movement or navigation action.
        /// </summary>
        Down,

        /// <summary>
        /// Represents a confirmation or accept action (e.g., select, interact, jump).
        /// </summary>
        Confirm,

        /// <summary>
        /// Represents a cancellation or back action (e.g., close menu, go back).
        /// </summary>
        Cancel,

        /// <summary>
        /// Represents no action or an unbound state.
        /// Used as a default or null value.
        /// </summary>
        None
    }
}