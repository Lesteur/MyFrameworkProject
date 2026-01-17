using System.Collections.Generic;

using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Core.Coroutines;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Manages the main game loop including entity updates and rendering.
    /// Maintains a collection of entities and coordinates their update and draw cycles.
    /// Separates rendering into world-space and UI-space contexts.
    /// Supports efficient GameObject lookup by tag for collision detection and object queries.
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
        /// Index of game objects organized by tag for efficient tag-based queries.
        /// Each tag maps to a list of game objects with that tag.
        /// </summary>
        private readonly Dictionary<string, List<GameObject>> _gameObjectsByTag = [];

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

        #region Properties - Entities

        /// <summary>
        /// Gets a read-only view of all GameObjects in the game loop.
        /// </summary>
        public IReadOnlyList<GameObject> GameObjects => _gameObjects;

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
        /// </summary>
        public GameLoop()
        {
            _coroutineManager = new CoroutineManager();
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
            if (gameObject == null)
            {
                Logger.Warning("Cannot add null GameObject to GameLoop");
                return;
            }

            _gameObjectsToAdd.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the game loop.
        /// Removal is deferred until the end of the current frame to avoid collection modification during iteration.
        /// </summary>
        /// <param name="gameObject">The game object to remove from the game loop.</param>
        public void RemoveGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Logger.Warning("Cannot remove null GameObject from GameLoop");
                return;
            }

            _gameObjectsToRemove.Add(gameObject);
        }

        /// <summary>
        /// Clears all game objects and tilemaps from the game loop immediately.
        /// </summary>
        public void ClearGameObjects()
        {
            _gameObjects.Clear();
            _gameObjectsByTag.Clear();
            _tilemaps.Clear();
            _gameObjectsToAdd.Clear();
            _gameObjectsToRemove.Clear();
        }

        /// <summary>
        /// Adds a tilemap to the game loop for rendering.
        /// </summary>
        /// <param name="tilemap">The tilemap to add to the game loop.</param>
        public void AddTilemap(Tilemap tilemap)
        {
            if (tilemap == null)
            {
                Logger.Warning("Cannot add null Tilemap to GameLoop");
                return;
            }

            _tilemaps.Add(tilemap);
        }

        #endregion

        #region Public Methods - Tag-Based Queries

        /// <summary>
        /// Gets all game objects with the specified tag.
        /// Returns an empty list if no objects have the tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>A read-only list of GameObjects with the specified tag.</returns>
        public IReadOnlyList<GameObject> GetGameObjectsByTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return System.Array.Empty<GameObject>();

            if (_gameObjectsByTag.TryGetValue(tag, out var objects))
                return objects;

            return System.Array.Empty<GameObject>();
        }

        /// <summary>
        /// Gets the first game object with the specified tag.
        /// Returns null if no object has the tag.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>The first GameObject with the specified tag, or null.</returns>
        public GameObject GetFirstGameObjectByTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            if (_gameObjectsByTag.TryGetValue(tag, out var objects) && objects.Count > 0)
                return objects[0];

            return null;
        }

        /// <summary>
        /// Checks if any game object with the specified tag exists.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>True if at least one GameObject with the tag exists; otherwise, false.</returns>
        public bool HasGameObjectWithTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return false;

            return _gameObjectsByTag.TryGetValue(tag, out var objects) && objects.Count > 0;
        }

        /// <summary>
        /// Updates the tag index when a GameObject's tag changes.
        /// Called internally by GameObject.SetTag().
        /// </summary>
        /// <param name="gameObject">The game object whose tag changed.</param>
        /// <param name="oldTag">The previous tag.</param>
        /// <param name="newTag">The new tag.</param>
        internal void UpdateGameObjectTag(GameObject gameObject, string oldTag, string newTag)
        {
            if (gameObject == null)
                return;

            // Remove from old tag list
            if (!string.IsNullOrEmpty(oldTag) && _gameObjectsByTag.TryGetValue(oldTag, out var oldList))
            {
                oldList.Remove(gameObject);
                if (oldList.Count == 0)
                    _gameObjectsByTag.Remove(oldTag);
            }

            // Add to new tag list
            if (!string.IsNullOrEmpty(newTag))
            {
                if (!_gameObjectsByTag.TryGetValue(newTag, out var newList))
                {
                    newList = [];
                    _gameObjectsByTag[newTag] = newList;
                }

                if (!newList.Contains(gameObject))
                    newList.Add(gameObject);
            }
        }

        #endregion

        #region Private Methods - Deferred Operations

        /// <summary>
        /// Processes all pending add and remove operations for entities and game objects.
        /// Called at the end of each frame to avoid modifying collections during iteration.
        /// </summary>
        private void ProcessPendingOperations()
        {
            // Process removals first
            if (_gameObjectsToRemove.Count > 0)
            {
                foreach (var gameObject in _gameObjectsToRemove)
                {
                    _gameObjects.Remove(gameObject);
                    RemoveFromTagIndex(gameObject);
                }
                
                _gameObjectsToRemove.Clear();
            }

            // Process additions
            if (_gameObjectsToAdd.Count > 0)
            {
                foreach (var gameObject in _gameObjectsToAdd)
                {
                    _gameObjects.Add(gameObject);
                    AddToTagIndex(gameObject);
                }
                
                _gameObjectsToAdd.Clear();
            }
        }

        /// <summary>
        /// Adds a game object to the tag index.
        /// </summary>
        /// <param name="gameObject">The game object to index.</param>
        private void AddToTagIndex(GameObject gameObject)
        {
            if (gameObject == null || string.IsNullOrEmpty(gameObject.Tag))
                return;

            if (!_gameObjectsByTag.TryGetValue(gameObject.Tag, out var list))
            {
                list = [];
                _gameObjectsByTag[gameObject.Tag] = list;
            }

            if (!list.Contains(gameObject))
                list.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the tag index.
        /// </summary>
        /// <param name="gameObject">The game object to remove from the index.</param>
        private void RemoveFromTagIndex(GameObject gameObject)
        {
            if (gameObject == null || string.IsNullOrEmpty(gameObject.Tag))
                return;

            if (_gameObjectsByTag.TryGetValue(gameObject.Tag, out var list))
            {
                list.Remove(gameObject);
                if (list.Count == 0)
                    _gameObjectsByTag.Remove(gameObject.Tag);
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
                var gameObject = _gameObjects[i];
                renderer.DrawEntity(gameObject);

                // Draw collision debug if enabled
                if (gameObject.DrawDebug && gameObject.HasCollisionMask)
                {
                    renderer.DrawCollisionMask(gameObject.CollisionMask, gameObject.X, gameObject.Y);
                }
            }

            renderer.EndWorld();

            renderer.BeginUI();

            renderer.EndUI();
        }

        #endregion
    }
}