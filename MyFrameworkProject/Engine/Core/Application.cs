using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Input;

namespace MyFrameworkProject.Engine.Core
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private GameLoop _gameLoop;
        private Renderer _renderer;

        public static Application Instance { get; private set; }
        public InputManager Input { get; private set; }

        public Renderer Renderer => _renderer;

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
            _renderer = new Renderer(GraphicsDevice);

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
            _renderer.BeginFrame();

            _gameLoop.Draw(_renderer);

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            Logger.Info("Loading content...");

            // Chargement de la texture depuis le Content Pipeline
            Texture2D nativeTexture = Content.Load<Texture2D>("spr_jonathan");

            // Encapsulation moteur
            var texture = new Graphics.Texture(nativeTexture);
            var sprite = new Sprite(texture, 14);

            // Création d'une entité de test
            var entity = new Entity(sprite);
            entity.SetPosition(0, 0);
            entity.EnableAnimation(0.05f, true);

            // On transmet l'entité au GameLoop
            _gameLoop.AddEntity(entity);
        }

    }
}