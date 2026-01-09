using System.Collections.Generic;
using MyFrameworkProject.Engine.Audio;
using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Core.Coroutines;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Input;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Manages the main game loop including entity updates and rendering.
    /// Maintains a collection of entities and coordinates their update and draw cycles.
    /// Separates rendering into world-space and UI-space contexts.
    /// </summary>
    public class GameLoop
    {
        #region Fields - Entities

        /// <summary>
        /// The collection of all game objects currently managed by the game loop.
        /// Game objects are updated and rendered in the order they were added.
        /// </summary>
        private readonly List<GameObject> _gameObjects = [];

        /// <summary>
        /// The collection of all tilemaps currently managed by the game loop.
        /// Tilemaps are rendered in the world-space context.
        /// </summary>
        private readonly List<Tilemap> _tilemaps = [];

        /// <summary>
        /// Pending game objects to add, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly List<GameObject> _gameObjectsToAdd = [];

        /// <summary>
        /// Pending game objects to remove, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly HashSet<GameObject> _gameObjectsToRemove = [];

        #endregion

        #region Fields - Coroutines

        /// <summary>
        /// The coroutine manager that handles all coroutine execution.
        /// </summary>
        private readonly CoroutineManager _coroutineManager;

        #endregion

        #region Properties - Coroutines

        /// <summary>
        /// Gets the coroutine manager for starting and stopping coroutines.
        /// </summary>
        public CoroutineManager Coroutines => _coroutineManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLoop"/> class.
        /// Logs the creation of the game loop for debugging purposes.
        /// </summary>
        /// <param name="inputManager">The input manager to provide to all game objects.</param>
        public GameLoop(InputManager inputManager, AudioManager audioManager)
        {
            // Initialize static input reference for all GameObjects
            GameObject.InitializeInput(inputManager);
            GameObject.InitializeAudio(audioManager);

            // Initialize coroutine manager
            _coroutineManager = new CoroutineManager();
            GameObject.InitializeCoroutines(_coroutineManager);

            Logger.Info("GameLoop created");
        }

        #endregion

        #region Public Methods - Entity Management

        /// <summary>
        /// Adds a game object to the game loop.
        /// Game objects are also added to the entity list for rendering.
        /// Addition is deferred until the end of the current frame.
        /// </summary>
        /// <param name="gameObject">The game object to add to the game loop.</param>
        public void AddGameObject(GameObject gameObject)
        {
            _gameObjectsToAdd.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the game loop.
        /// Removal is deferred until the end of the current frame to avoid collection modification during iteration.
        /// </summary>
        /// <param name="gameObject">The game object to remove from the game loop.</param>
        public void RemoveGameObject(GameObject gameObject)
        {
            _gameObjectsToRemove.Add(gameObject);
        }

        /// <summary>
        /// Clears all game objects from the game loop immediately.
        /// </summary>
        public void ClearGameObjects()
        {
            _gameObjects.Clear();
            _gameObjectsToAdd.Clear();
            _gameObjectsToRemove.Clear();
        }

        /// <summary>
        /// Adds a tilemap to the game loop for rendering.
        /// </summary>
        /// <param name="tilemap">The tilemap to add to the game loop.</param>
        public void AddTilemap(Tilemap tilemap)
        {
            _tilemaps.Add(tilemap);
        }

        #endregion

        #region Private Methods - Deferred Operations

        /// <summary>
        /// Processes all pending add and remove operations for entities and game objects.
        /// Called at the end of each frame to avoid modifying collections during iteration.
        /// </summary>
        private void ProcessPendingOperations()
        {
            if (_gameObjectsToRemove.Count > 0)
            {
                foreach (var gameObject in _gameObjectsToRemove)
                    _gameObjects.Remove(gameObject);
                
                _gameObjectsToRemove.Clear();
            }

            if (_gameObjectsToAdd.Count > 0)
            {
                _gameObjects.AddRange(_gameObjectsToAdd);
                _gameObjectsToAdd.Clear();
            }
        }

        #endregion

        #region Public Methods - Update

        /// <summary>
        /// Updates all entities in the game loop.
        /// Called once per frame by the application.
        /// Optimized to minimize iterations and cache delta time for better performance.
        /// </summary>
        public void Update()
        {
            float deltaTime = Time.DeltaTime;

            // Update coroutines
            _coroutineManager.Update(deltaTime);

            // Single optimized pass for game objects lifecycle
            int gameObjectCount = _gameObjects.Count;
            for (int i = 0; i < gameObjectCount; i++)
            {
                var gameObject = _gameObjects[i];
                if (!gameObject.Active)
                    continue;

                gameObject.BeforeUpdate(deltaTime);
                gameObject.Update(deltaTime);
                gameObject.AfterUpdate(deltaTime);

                gameObject.UpdateAnimation(deltaTime);
            }

            // Process pending add/remove operations
            ProcessPendingOperations();
        }

        #endregion

        #region Public Methods - Draw

        /// <summary>
        /// Renders all entities in the game loop.
        /// Called once per frame after Update by the application.
        /// Separates rendering into two passes: world-space entities and UI-space elements.
        /// Optimized using for loops instead of foreach to reduce allocations.
        /// </summary>
        /// <param name="renderer">The renderer used to draw entities and UI elements.</param>
        public void Draw(Renderer renderer)
        {
            renderer.BeginWorld();

            // Draw tilemaps
            int tilemapCount = _tilemaps.Count;
            for (int i = 0; i < tilemapCount; i++)
            {
                renderer.DrawTilemap(_tilemaps[i]);
            }

            // Draw game objects
            int gameObjectCount = _gameObjects.Count;
            for (int i = 0; i < gameObjectCount; i++)
            {
                renderer.DrawEntity(_gameObjects[i]);
            }

            renderer.EndWorld();

            renderer.BeginUI();

            renderer.EndUI();
        }

        #endregion
    }
}