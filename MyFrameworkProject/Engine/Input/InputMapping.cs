using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Defines a mapping between a logical input action and physical input device controls.
    /// Associates a single action with both keyboard keys and gamepad buttons,
    /// enabling unified input handling across multiple input devices.
    /// </summary>
    internal struct InputMapping
    {
        /// <summary>
        /// The logical input action this mapping represents.
        /// </summary>
        public InputAction Action;

        /// <summary>
        /// The keyboard key mapped to this action.
        /// </summary>
        public Keys KeyboardKey;

        /// <summary>
        /// The gamepad button mapped to this action.
        /// </summary>
        public Buttons GamePadButton;
    }
}