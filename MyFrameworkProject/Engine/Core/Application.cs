using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using MyFrameworkProject.Engine.Input;

namespace MyFrameworkProject.Engine.Core
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private GameLoop _gameLoop;

        public static Application Instance { get; private set; }
        public InputManager Input { get; private set; }

        public Application()
        {
            Instance = this;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ConfigureGraphics();
        }

        private void ConfigureGraphics()
        {
            _graphics.PreferredBackBufferWidth = EngineConfig.WindowWidth;
            _graphics.PreferredBackBufferHeight = EngineConfig.WindowHeight;
            _graphics.SynchronizeWithVerticalRetrace = EngineConfig.VSync;

            IsFixedTimeStep = EngineConfig.IsFixedTimeStep;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / EngineConfig.TargetFPS);
        }

        protected override void Initialize()
        {
            Logger.Info("Application initialized");

            Input = new InputManager();

            Time.Initialize();
            _gameLoop = new GameLoop();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            Time.Update(gameTime);

            if (Input.IsPressed(InputAction.Left))
                Logger.Info("Left action triggered");

            if (Input.IsPressed(InputAction.Right))
                Logger.Info("Right action triggered");

            if (Input.IsDown(InputAction.Confirm))
                Exit();

            _gameLoop.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _gameLoop.Draw(GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}