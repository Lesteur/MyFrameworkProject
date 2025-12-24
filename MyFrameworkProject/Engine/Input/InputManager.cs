using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Input
{
    public sealed class InputManager
    {
        private readonly KeyboardDevice _keyboard;
        private readonly MouseDevice _mouse;
        private readonly List<GamePadDevice> _gamePads;

        private readonly InputMapping[] _mappings =
        {
            new() { Action = InputAction.Left,    KeyboardKey = Keys.Left,  GamePadButton = Buttons.DPadLeft },
            new() { Action = InputAction.Right,   KeyboardKey = Keys.Right, GamePadButton = Buttons.DPadRight },
            new() { Action = InputAction.Up,      KeyboardKey = Keys.Up,    GamePadButton = Buttons.DPadUp },
            new() { Action = InputAction.Down,    KeyboardKey = Keys.Down,  GamePadButton = Buttons.DPadDown },
            new() { Action = InputAction.Confirm, KeyboardKey = Keys.Enter, GamePadButton = Buttons.A },
            new() { Action = InputAction.Cancel,  KeyboardKey = Keys.Escape,GamePadButton = Buttons.B }
        };

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

        public void Update()
        {
            _keyboard.Update();
            _mouse.Update();

            foreach (var pad in _gamePads)
                pad.Update();
        }

        public void Reset()
        {
            _keyboard.Reset();
            _mouse.Reset();

            foreach (var pad in _gamePads)
                pad.Reset();
        }

        public KeyboardDevice Keyboard => _keyboard;
        public MouseDevice Mouse => _mouse;

        public GamePadDevice GetGamePad(int index)
        {
            return index >= 0 && index < _gamePads.Count ? _gamePads[index] : null;
        }

        public bool IsDown(InputAction action)
        {
            foreach (var m in _mappings)
                if (m.Action == action &&
                    (_keyboard.IsDown(m.KeyboardKey) ||
                     _gamePads[0].IsButtonDown(m.GamePadButton)))
                    return true;

            return false;
        }

        public bool IsPressed(InputAction action)
        {
            foreach (var m in _mappings)
                if (m.Action == action &&
                    (_keyboard.IsPressed(m.KeyboardKey) ||
                     _gamePads[0].IsButtonPressed(m.GamePadButton)))
                    return true;

            return false;
        }

        public bool IsReleased(InputAction action)
        {
            foreach (var m in _mappings)
                if (m.Action == action &&
                    (_keyboard.IsReleased(m.KeyboardKey) ||
                     _gamePads[0].IsButtonReleased(m.GamePadButton)))
                    return true;

            return false;
        }
    }
}