using System.Collections.Generic;
using MyFrameworkProject.Engine.Components;
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
        /// The collection of all entities currently managed by the game loop.
        /// Entities are updated and rendered in the order they were added.
        /// </summary>
        private readonly List<Entity> _entities = [];

        /// <summary>
        /// The collection of all tilemaps currently managed by the game loop.
        /// Tilemaps are rendered in the world-space context.
        /// </summary>
        private readonly List<Tilemap> _tilemaps = [];

        /// <summary>
        /// Pending entities to add, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly List<Entity> _entitiesToAdd = [];

        /// <summary>
        /// Pending game objects to add, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly List<GameObject> _gameObjectsToAdd = [];

        /// <summary>
        /// Pending entities to remove, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly HashSet<Entity> _entitiesToRemove = [];

        /// <summary>
        /// Pending game objects to remove, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly HashSet<GameObject> _gameObjectsToRemove = [];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLoop"/> class.
        /// Logs the creation of the game loop for debugging purposes.
        /// </summary>
        /// <param name="inputManager">The input manager to provide to all game objects.</param>
        public GameLoop(InputManager inputManager)
        {
            // Initialize static input reference for all GameObjects
            GameObject.InitializeInput(inputManager);
            
            Logger.Info("GameLoop created");
        }

        #endregion

        #region Public Methods - Entity Management

        /// <summary>
        /// Adds an entity to the game loop.
        /// The entity will be updated and rendered every frame until removed.
        /// Entities are processed in the order they were added.
        /// Addition is deferred until the end of the current frame.
        /// </summary>
        /// <param name="entity">The entity to add to the game loop.</param>
        public void AddEntity(Entity entity)
        {
            _entitiesToAdd.Add(entity);
        }

        /// <summary>
        /// Removes an entity from the game loop.
        /// Removal is deferred until the end of the current frame to avoid collection modification during iteration.
        /// </summary>
        /// <param name="entity">The entity to remove from the game loop.</param>
        public void RemoveEntity(Entity entity)
        {
            _entitiesToRemove.Add(entity);
        }

        /// <summary>
        /// Clears all entities from the game loop immediately.
        /// </summary>
        public void ClearEntities()
        {
            _entities.Clear();
            _entitiesToAdd.Clear();
            _entitiesToRemove.Clear();
        }

        /// <summary>
        /// Adds a game object to the game loop.
        /// Game objects are also added to the entity list for rendering.
        /// Addition is deferred until the end of the current frame.
        /// </summary>
        /// <param name="gameObject">The game object to add to the game loop.</param>
        public void AddGameObject(GameObject gameObject)
        {
            _gameObjectsToAdd.Add(gameObject);
            _entitiesToAdd.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the game loop.
        /// Removal is deferred until the end of the current frame to avoid collection modification during iteration.
        /// </summary>
        /// <param name="gameObject">The game object to remove from the game loop.</param>
        public void RemoveGameObject(GameObject gameObject)
        {
            _gameObjectsToRemove.Add(gameObject);
            _entitiesToRemove.Add(gameObject);
        }

        /// <summary>
        /// Clears all game objects from the game loop immediately.
        /// </summary>
        public void ClearGameObjects()
        {
            foreach (var gameObject in _gameObjects)
                _entities.Remove(gameObject);

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
            // Process removals first
            if (_entitiesToRemove.Count > 0)
            {
                foreach (var entity in _entitiesToRemove)
                    _entities.Remove(entity);
                
                _entitiesToRemove.Clear();
            }

            if (_gameObjectsToRemove.Count > 0)
            {
                foreach (var gameObject in _gameObjectsToRemove)
                    _gameObjects.Remove(gameObject);
                
                _gameObjectsToRemove.Clear();
            }

            // Process additions
            if (_entitiesToAdd.Count > 0)
            {
                _entities.AddRange(_entitiesToAdd);
                _entitiesToAdd.Clear();
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
            }

            // Update animations for all entities in a single pass
            int entityCount = _entities.Count;
            for (int i = 0; i < entityCount; i++)
            {
                _entities[i].UpdateAnimation(deltaTime);
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

            // Draw entities
            int entityCount = _entities.Count;
            for (int i = 0; i < entityCount; i++)
            {
                renderer.DrawEntity(_entities[i]);
            }

            renderer.EndWorld();

            renderer.BeginUI();

            renderer.EndUI();
        }

        #endregion
    }
}