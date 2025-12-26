using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Input;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Main application class that extends MonoGame's Game class.
    /// Manages the game lifecycle including initialization, updating, rendering, and content loading.
    /// Provides singleton access and centralizes core engine systems like input, rendering, and game loop.
    /// </summary>
    public class Application : Game
    {
        #region Fields - Graphics

        /// <summary>
        /// The graphics device manager responsible for configuring display settings.
        /// </summary>
        private readonly GraphicsDeviceManager _graphics;

        /// <summary>
        /// The renderer responsible for all 2D sprite rendering operations.
        /// </summary>
        private Renderer _renderer;

        #endregion

        #region Fields - Game Systems

        /// <summary>
        /// The game loop that manages entity updates and rendering.
        /// </summary>
        private GameLoop _gameLoop;

        /// <summary>
        /// The game object that the camera is currently following.
        /// </summary>
        private GameObject _followingObject = null;

        #endregion

        #region Properties - Singleton

        /// <summary>
        /// Gets the singleton instance of the application.
        /// Provides global access to the application throughout the game.
        /// </summary>
        public static Application Instance { get; private set; }

        #endregion

        #region Properties - Core Systems

        /// <summary>
        /// Gets the input manager that handles all input devices (keyboard, gamepad, mouse).
        /// </summary>
        public InputManager Input { get; private set; }

        /// <summary>
        /// Gets the renderer responsible for all 2D sprite rendering operations.
        /// Provides access to world and UI cameras.
        /// </summary>
        public Renderer Renderer => _renderer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// Sets up the singleton instance, configures the graphics device, and initializes the content pipeline.
        /// </summary>
        public Application()
        {
            Instance = this;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ConfigureGraphics();
        }

        #endregion

        #region Private Methods - Configuration

        /// <summary>
        /// Configures graphics settings based on the engine configuration.
        /// Sets up window dimensions, VSync, fixed time step, and target frame rate.
        /// </summary>
        private void ConfigureGraphics()
        {
            _graphics.PreferredBackBufferWidth = EngineConfig.WindowWidth;
            _graphics.PreferredBackBufferHeight = EngineConfig.WindowHeight;
            _graphics.SynchronizeWithVerticalRetrace = EngineConfig.VSync;

            IsFixedTimeStep = EngineConfig.IsFixedTimeStep;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / EngineConfig.TargetFPS);
        }

        #endregion

        #region MonoGame Lifecycle - Initialize

        /// <summary>
        /// Initializes the application and all core engine systems.
        /// Called once after construction but before the first Update call.
        /// Sets up input manager, renderer, time system, and game loop.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Info("Application initialized");

            Input = new InputManager();
            _renderer = new Renderer(GraphicsDevice);

            Time.Initialize();
            _gameLoop = new GameLoop(Input);

            base.Initialize();
        }

        #endregion

        #region MonoGame Lifecycle - LoadContent

        /// <summary>
        /// Loads game content such as textures, sprites, and entities.
        /// Called once after Initialize and before the first Update call.
        /// This method contains temporary test code for loading and creating demo entities.
        /// </summary>
        protected override void LoadContent()
        {
            Logger.Info("Loading content...");

            // Loading
            Texture2D nativeTexture = Content.Load<Texture2D>("spr_jonathan");
            var texture = new Graphics.Texture(nativeTexture);
            var sprite = new Sprite(texture, 0, 0, 14);

            // GameObject creation using ObjectTest
            var testObject = new ObjectTest(sprite);
            testObject.SetPosition(10, 10);
            testObject.SetScale(1.0f, 1.0f);
            testObject.SetMoveSpeed(150f);
            testObject.EnableAnimation(0.05f, true);

            _followingObject = testObject;

            // Game loop registration
            _gameLoop.AddGameObject(testObject);

            Texture2D tilesetTexture = Content.Load<Texture2D>("Tileset");
            var tileset = new Graphics.Texture(tilesetTexture);
            var tileSprite = new Tileset(tileset, 16, 16);

            var tilemap = new Tilemap(tileSprite, 50, 50);
            tilemap.Fill(2);
            _gameLoop.AddTilemap(tilemap);
        }

        #endregion

        #region MonoGame Lifecycle - Update

        /// <summary>
        /// Updates the application state and all game systems.
        /// Called once per frame (or at fixed intervals if using fixed time step).
        /// Handles input processing, time updates, camera movement, and game loop updates.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values from MonoGame.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            Time.Update(gameTime);

            _gameLoop.Update();

            base.Update(gameTime);
        }

        #endregion

        #region MonoGame Lifecycle - Draw

        /// <summary>
        /// Renders the current frame to the screen.
        /// Called once per frame after Update.
        /// Begins the frame, delegates rendering to the game loop, and finalizes the frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values from MonoGame.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (_followingObject != null)
            {
                var position = _followingObject.Position;
                Renderer.WorldCamera.SetPosition(position.X, position.Y);
            }

            _renderer.BeginFrame();

            _gameLoop.Draw(_renderer);

            base.Draw(gameTime);
        }

        #endregion
    }
}