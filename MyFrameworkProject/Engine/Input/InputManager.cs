using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyFrameworkProject.Engine.Input
{
    /// <summary>
    /// Centralized input management system that handles all input devices and action mappings.
    /// Provides unified access to keyboard, mouse, and gamepad inputs through logical action queries.
    /// Supports multiple gamepads and configurable input mapping across different device types.
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
        /// The dictionary of input mappings that associate logical actions with physical device inputs.
        /// Each mapping defines keyboard keys and gamepad buttons for a specific action.
        /// </summary>
        private readonly Dictionary<InputAction, InputMapping> _mappings;

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

        /// <summary>
        /// Gets the number of gamepad devices managed by this input manager.
        /// </summary>
        public int GamePadCount => _gamePads.Count;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// Creates keyboard, mouse, and gamepad devices, then initializes default input mappings.
        /// By default, supports four gamepads for up to four players.
        /// </summary>
        public InputManager()
        {
            _keyboard = new KeyboardDevice();
            _mouse = new MouseDevice();

            _gamePads =
            [
                new GamePadDevice(PlayerIndex.One),
                new GamePadDevice(PlayerIndex.Two),
                new GamePadDevice(PlayerIndex.Three),
                new GamePadDevice(PlayerIndex.Four)
            ];

            _mappings = [];
            InitializeDefaultMappings();
            Reset();
        }

        #endregion

        #region Private Methods - Initialization

        /// <summary>
        /// Initializes the default input mappings for common actions.
        /// Can be overridden by calling SetMapping to customize controls.
        /// </summary>
        private void InitializeDefaultMappings()
        {
            SetMapping(InputAction.Left, Keys.Left, Buttons.DPadLeft);
            SetMapping(InputAction.Right, Keys.Right, Buttons.DPadRight);
            SetMapping(InputAction.Up, Keys.Up, Buttons.DPadUp);
            SetMapping(InputAction.Down, Keys.Down, Buttons.DPadDown);
            SetMapping(InputAction.Confirm, Keys.Enter, Buttons.A);
            SetMapping(InputAction.Cancel, Keys.Escape, Buttons.B);
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

            int gamepadCount = _gamePads.Count;
            for (int i = 0; i < gamepadCount; i++)
            {
                _gamePads[i].Update();
            }
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

            int gamepadCount = _gamePads.Count;
            for (int i = 0; i < gamepadCount; i++)
            {
                _gamePads[i].Reset();
            }
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

        /// <summary>
        /// Gets the first connected gamepad, if any.
        /// </summary>
        /// <returns>The first connected gamepad, or null if no gamepads are connected.</returns>
        public GamePadDevice GetFirstConnectedGamePad()
        {
            foreach (var gamePad in _gamePads)
            {
                if (gamePad.IsConnected)
                    return gamePad;
            }
            return null;
        }

        #endregion

        #region Public Methods - Mapping Configuration

        /// <summary>
        /// Sets or updates an input mapping for a specific action.
        /// </summary>
        /// <param name="action">The logical action to map.</param>
        /// <param name="keyboardKey">The keyboard key to bind to this action.</param>
        /// <param name="gamePadButton">The gamepad button to bind to this action.</param>
        public void SetMapping(InputAction action, Keys keyboardKey, Buttons gamePadButton)
        {
            _mappings[action] = new InputMapping
            {
                Action = action,
                KeyboardKey = keyboardKey,
                GamePadButton = gamePadButton
            };
        }

        /// <summary>
        /// Gets the input mapping for a specific action.
        /// </summary>
        /// <param name="action">The logical action to retrieve the mapping for.</param>
        /// <returns>The input mapping if found, or null if the action is not mapped.</returns>
        public InputMapping GetMapping(InputAction action)
        {
            return _mappings.TryGetValue(action, out var mapping) ? mapping : default;
        }

        /// <summary>
        /// Clears all input mappings.
        /// </summary>
        public void ClearMappings()
        {
            _mappings.Clear();
        }

        #endregion

        #region Public Methods - Action Queries

        /// <summary>
        /// Checks if a logical action is currently active (held down) on any mapped input device.
        /// Returns true if either the mapped keyboard key or gamepad button is currently down.
        /// Checks the first connected gamepad by default.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <returns>True if the action is active on any device; otherwise, false.</returns>
        public bool IsDown(InputAction action)
        {
            if (!_mappings.TryGetValue(action, out var mapping))
                return false;

            // Check keyboard
            if (_keyboard.IsDown(mapping.KeyboardKey))
                return true;

            // Check first connected gamepad
            var gamePad = GetFirstConnectedGamePad();
            if (gamePad != null && gamePad.IsButtonDown(mapping.GamePadButton))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a logical action was just activated (pressed) this frame on any mapped input device.
        /// Returns true only on the first frame the action transitions from inactive to active.
        /// Useful for detecting single action events without repetition.
        /// Checks the first connected gamepad by default.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <returns>True if the action was just pressed this frame on any device; otherwise, false.</returns>
        public bool IsPressed(InputAction action)
        {
            if (!_mappings.TryGetValue(action, out var mapping))
                return false;

            // Check keyboard
            if (_keyboard.IsPressed(mapping.KeyboardKey))
                return true;

            // Check first connected gamepad
            var gamePad = GetFirstConnectedGamePad();
            if (gamePad != null && gamePad.IsButtonPressed(mapping.GamePadButton))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a logical action was just deactivated (released) this frame on any mapped input device.
        /// Returns true only on the first frame the action transitions from active to inactive.
        /// Useful for detecting action release events.
        /// Checks the first connected gamepad by default.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <returns>True if the action was just released this frame on any device; otherwise, false.</returns>
        public bool IsReleased(InputAction action)
        {
            if (!_mappings.TryGetValue(action, out var mapping))
                return false;

            // Check keyboard
            if (_keyboard.IsReleased(mapping.KeyboardKey))
                return true;

            // Check first connected gamepad
            var gamePad = GetFirstConnectedGamePad();
            if (gamePad != null && gamePad.IsButtonReleased(mapping.GamePadButton))
                return true;

            return false;
        }

        #endregion

        #region Public Methods - Player-Specific Action Queries

        /// <summary>
        /// Checks if a logical action is currently active for a specific player.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <param name="playerIndex">The player index (0-based).</param>
        /// <returns>True if the action is active for the specified player; otherwise, false.</returns>
        public bool IsDown(InputAction action, int playerIndex)
        {
            if (!_mappings.TryGetValue(action, out var mapping))
                return false;

            var gamePad = GetGamePad(playerIndex);
            return gamePad != null && gamePad.IsConnected && gamePad.IsButtonDown(mapping.GamePadButton);
        }

        /// <summary>
        /// Checks if a logical action was just pressed for a specific player.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <param name="playerIndex">The player index (0-based).</param>
        /// <returns>True if the action was just pressed for the specified player; otherwise, false.</returns>
        public bool IsPressed(InputAction action, int playerIndex)
        {
            if (!_mappings.TryGetValue(action, out var mapping))
                return false;

            var gamePad = GetGamePad(playerIndex);
            return gamePad != null && gamePad.IsConnected && gamePad.IsButtonPressed(mapping.GamePadButton);
        }

        /// <summary>
        /// Checks if a logical action was just released for a specific player.
        /// </summary>
        /// <param name="action">The logical action to check.</param>
        /// <param name="playerIndex">The player index (0-based).</param>
        /// <returns>True if the action was just released for the specified player; otherwise, false.</returns>
        public bool IsReleased(InputAction action, int playerIndex)
        {
            if (!_mappings.TryGetValue(action, out var mapping))
                return false;

            var gamePad = GetGamePad(playerIndex);
            return gamePad != null && gamePad.IsConnected && gamePad.IsButtonReleased(mapping.GamePadButton);
        }

        #endregion
    }
}