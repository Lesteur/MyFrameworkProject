using System;

using Microsoft.Xna.Framework;

using MyFrameworkProject.Engine.Audio;
using MyFrameworkProject.Engine.Input;
using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Assets;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Main application class that extends MonoGame's Game class.
    /// Manages the game lifecycle including initialization, updating, rendering, and room management.
    /// Provides singleton access and centralizes core engine systems like input, rendering, and game loop.
    /// </summary>
    public class Application : Game
    {
        #region Fields - Singleton

        /// <summary>
        /// The singleton instance of the application.
        /// </summary>
        private static Application _instance;

        #endregion

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
        /// The scene manager that handles room transitions and lifecycle.
        /// </summary>
        private SceneManager _sceneManager;

        /// <summary>
        /// The input manager that handles all input devices (keyboard, gamepad, mouse).
        /// </summary>
        private InputManager _inputManager;

        /// <summary>
        /// The audio manager that handles sound playback and music.
        /// </summary>
        private AudioManager _audioManager;

        #endregion

        #region Properties - Singleton

        /// <summary>
        /// Gets the singleton instance of the application.
        /// Provides global access to the application throughout the game.
        /// </summary>
        public static Application Instance => _instance;

        #endregion

        #region Properties - Core Systems

        /// <summary>
        /// Gets the main game loop that handles updates and rendering.
        /// </summary>
        public GameLoop GameLoop => _gameLoop;

        /// <summary>
        /// Gets the scene manager that handles room transitions.
        /// </summary>
        public SceneManager SceneManager => _sceneManager;

        /// <summary>
        /// Gets the input manager that handles all input devices (keyboard, gamepad, mouse).
        /// Provides unified access to all input systems.
        /// </summary>
        public InputManager Input => _inputManager;

        /// <summary>
        /// Gets the audio manager that handles sound playback and music.
        /// </summary>
        public AudioManager Audio => _audioManager;

        /// <summary>
        /// Gets the renderer responsible for all 2D sprite rendering operations.
        /// Provides access to world and UI cameras.
        /// </summary>
        public Renderer Renderer => _renderer;

        #endregion

        #region Properties - Room Management (Backward Compatibility)

        /// <summary>
        /// Gets the currently active game room.
        /// </summary>
        public GameRoom CurrentRoom => _sceneManager?.CurrentRoom;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// Sets up the singleton instance, configures the graphics device, and initializes the content pipeline.
        /// </summary>
        public Application()
        {
            _instance = this;

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

        #region Protected Methods - MonoGame Lifecycle

        /// <summary>
        /// Initializes the application and all core engine systems.
        /// Called once after construction but before the first Update call.
        /// Sets up input manager, audio manager, renderer, time system, and game loop.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Info("Initializing application...");

            _inputManager = new InputManager();
            _audioManager = new AudioManager();
            _renderer = new Renderer(GraphicsDevice);
            _gameLoop = new GameLoop();

            Time.Initialize();

            // Initialize scene manager with coroutine support
            _sceneManager = new SceneManager(_gameLoop, Content, _gameLoop.Coroutines);

            Logger.Info("Application initialized successfully");

            base.Initialize();
        }

        /// <summary>
        /// Loads initial game content.
        /// Called once after Initialize and before the first Update call.
        /// Override this method in a derived class to load the initial room or perform initial setup.
        /// </summary>
        protected override void LoadContent()
        {
            Logger.Info("Loading initial content...");

            // Override this in derived class to load initial room
            LoadRoom(new RoomTest());
        }

        /// <summary>
        /// Updates the application state and all game systems.
        /// Called once per frame (or at fixed intervals if using fixed time step).
        /// Handles input processing, audio updates, time updates, and game loop updates.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values from MonoGame.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            Audio.Update();
            Time.Update(gameTime);

            if (CurrentRoom != null && CurrentRoom.IsLoaded)
            {
                _gameLoop.Update();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Renders the current frame to the screen.
        /// Called once per frame after Update.
        /// Begins the frame, updates camera position if following a target, delegates rendering to the game loop.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values from MonoGame.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Update camera to follow target if set
            if (CurrentRoom?.CameraTarget != null)
            {
                var target = CurrentRoom.CameraTarget;
                var camera = _renderer.WorldCamera;

                float halfViewportWidth = camera.ViewportWidth / 2f;
                float halfViewportHeight = camera.ViewportHeight / 2f;

                float clampedX = Math.Clamp(
                    target.Position.X,
                    halfViewportWidth,
                    CurrentRoom.Width - halfViewportWidth
                );
                float clampedY = Math.Clamp(
                    target.Position.Y,
                    halfViewportHeight,
                    CurrentRoom.Height - halfViewportHeight
                );

                camera.SetPosition(clampedX, clampedY);
            }

            _renderer.BeginFrame();

            if (CurrentRoom != null && CurrentRoom.IsLoaded)
            {
                _gameLoop.Draw();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Disposes resources when the application is closed.
        /// Ensures proper cleanup of the current room before disposal.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Logger.Info("Disposing application...");
                _sceneManager?.UnloadCurrentRoom();
                _renderer?.Dispose();
                Audio?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Public Methods - Room Management

        /// <summary>
        /// Loads and activates a new game room, unloading the current room if one exists.
        /// </summary>
        /// <param name="room">The room to load and activate.</param>
        public void LoadRoom(GameRoom room)
        {
            _sceneManager?.LoadRoom(room);
        }

        /// <summary>
        /// Loads a new room asynchronously with optional transition effects.
        /// </summary>
        /// <param name="room">The room to load and activate.</param>
        /// <param name="transitionEffect">Optional transition effect coroutine.</param>
        public void LoadRoomAsync(GameRoom room, Func<System.Collections.IEnumerator> transitionEffect = null)
        {
            _sceneManager?.LoadRoomAsync(room, transitionEffect);
        }

        /// <summary>
        /// Unloads the current room without loading a new one.
        /// Useful for returning to a menu state or cleaning up before application exit.
        /// </summary>
        public void UnloadCurrentRoom()
        {
            _sceneManager?.UnloadCurrentRoom();
        }

        #endregion
    }
}