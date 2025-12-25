using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Centralized input management system that handles all input devices and action mappings.
    /// Provides unified access to keyboard, mouse, and gamepad inputs through logical action queries.
    /// Supports multiple gamepads and automatic input mapping across different device types.
    /// </summary>
    public sealed class InputManager
    {
        #region Fields - Devices

        /// <summary>
        /// The keyboard input device.
        /// </summary>
        private readonly KeyboardDevice _keyboard;

        /// <summary>
        /// The mouse input device.
        /// </summary>
        private readonly MouseDevice _mouse;

        /// <summary>
        /// The collection of gamepad input devices, supporting multiple players.
        /// </summary>
        private readonly List<GamePadDevice> _gamePads;

        #endregion

        #region Fields - Mappings

        /// <summary>
        /// The array of input mappings that associate logical actions with physical device inputs.
        /// Each mapping defines keyboard keys and gamepad buttons for a specific action.
        /// </summary>
        private readonly InputMapping[] _mappings =
        {
            new() { Action = InputAction.Left,    KeyboardKey = Keys.Left,   GamePadButton = Buttons.DPadLeft },
            new() { Action = InputAction.Right,   KeyboardKey = Keys.Right,  GamePadButton = Buttons.DPadRight },
            new() { Action = InputAction.Up,      KeyboardKey = Keys.Up,     GamePadButton = Buttons.DPadUp },
            new() { Action = InputAction.Down,    KeyboardKey = Keys.Down,   GamePadButton = Buttons.DPadDown },
            new() { Action = InputAction.Confirm, KeyboardKey = Keys.Enter,  GamePadButton = Buttons.A },
            new() { Action = InputAction.Cancel,  KeyboardKey = Keys.Escape, GamePadButton = Buttons.B }
        };

        #endregion

        #region Properties - Devices

        /// <summary>
        /// Gets the keyboard input device.
        /// Provides direct access for device-specific queries.
        /// </summary>
        public KeyboardDevice Keyboard => _keyboard;

        /// <summary>
        /// Gets the mouse input device.
        /// Provides direct access for device-specific queries.
        /// </summary>
        public MouseDevice Mouse => _mouse;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// Creates keyboard, mouse, and gamepad devices, then resets their states.
        /// By default, supports one gamepad for Player One.
        /// </summary>
        public InputManager()
        {
            _keyboard = new KeyboardDevice();
            _mouse = new MouseDevice();

            _gamePads =
            [
                new GamePadDevice(PlayerIndex.One)
            ];

            Reset();
        }

        #endregion

        #region Public Methods - Lifecycle

        /// <summary>
        /// Updates all input devices by polling their current states.
        /// Should be called once per frame before querying input.
        /// Updates keyboard, mouse, and all gamepad states.
        /// </summary>
        public void Update()
        {
            _keyboard.Update();
            _mouse.Update();

            foreach (var pad in _gamePads)
                pad.Update();
        }

        /// <summary>
        /// Resets all input device states, clearing any detected state changes.
        /// Useful for initialization or when input state needs to be cleared.
        /// Resets keyboard, mouse, and all gamepad states.
        /// </summary>
        public void Reset()
        {
            _keyboard.Reset();
            _mouse.Reset();

            foreach (var pad in _gamePads)
                pad.Reset();
        }

        #endregion

        #region Public Methods - Device Access

        /// <summary>
        /// Gets a gamepad device by its index.
        /// </summary>
        /// <param name="index">The zero-based index of the gamepad (0 for Player One, 1 for Player Two, etc.).</param>
        /// <returns>The gamepad device at the specified index, or null if the index is out of range.</returns>
        public GamePadDevice GetGamePad(int index)
        {
            return index >= 0 && index < _gamePads.Count ? _gamePads[index] : null;
        }

        #endregion

        #region Public Methods - Action Queries

        /// <summary>
        /// Checks if a logical action is currently active (held down) on any mapped input device.
        /// Returns true if either the mapped keyboard key or gamepad button is currently down.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <returns>True if the action is active on any device; otherwise, false.</returns>
        public bool IsDown(InputAction action)
        {
            foreach (var m in _mappings)
                if (m.Action == action &&
                    (_keyboard.IsDown(m.KeyboardKey) ||
                     _gamePads[0].IsButtonDown(m.GamePadButton)))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if a logical action was just activated (pressed) this frame on any mapped input device.
        /// Returns true only on the first frame the action transitions from inactive to active.
        /// Useful for detecting single action events without repetition.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <returns>True if the action was just pressed this frame on any device; otherwise, false.</returns>
        public bool IsPressed(InputAction action)
        {
            foreach (var m in _mappings)
                if (m.Action == action &&
                    (_keyboard.IsPressed(m.KeyboardKey) ||
                     _gamePads[0].IsButtonPressed(m.GamePadButton)))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if a logical action was just deactivated (released) this frame on any mapped input device.
        /// Returns true only on the first frame the action transitions from active to inactive.
        /// Useful for detecting action release events.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <returns>True if the action was just released this frame on any device; otherwise, false.</returns>
        public bool IsReleased(InputAction action)
        {
            foreach (var m in _mappings)
                if (m.Action == action &&
                    (_keyboard.IsReleased(m.KeyboardKey) ||
                     _gamePads[0].IsButtonReleased(m.GamePadButton)))
                    return true;

            return false;
        }

        #endregion
    }
}