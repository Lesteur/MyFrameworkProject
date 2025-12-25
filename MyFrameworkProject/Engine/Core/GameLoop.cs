using System.Collections.Generic;

using MyFrameworkProject.Engine.Graphics;

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
        /// The collection of all entities currently managed by the game loop.
        /// Entities are updated and rendered in the order they were added.
        /// </summary>
        private readonly List<Entity> _entities = [];

        /// <summary>
        /// The collection of all tilemaps currently managed by the game loop.
        /// Tilemaps are rendered in the world-space context.
        /// </summary>
        private readonly List<Tilemap> _tilemaps = [];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLoop"/> class.
        /// Logs the creation of the game loop for debugging purposes.
        /// </summary>
        public GameLoop()
        {
            Logger.Info("GameLoop created");
        }

        #endregion

        #region Public Methods - Entity Management

        /// <summary>
        /// Adds an entity to the game loop.
        /// The entity will be updated and rendered every frame until removed.
        /// Entities are processed in the order they were added.
        /// </summary>
        /// <param name="entity">The entity to add to the game loop.</param>
        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public void AddTilemap(Tilemap tilemap)
        {
            _tilemaps.Add(tilemap);
        }

        #endregion

        #region Public Methods - Update

        /// <summary>
        /// Updates all entities in the game loop.
        /// Called once per frame by the application.
        /// Iterates through all entities and updates them with the current delta time.
        /// </summary>
        public void Update()
        {
            foreach (var entity in _entities)
            {
                entity.Update(Time.DeltaTime);
            }
        }

        #endregion

        #region Public Methods - Draw

        /// <summary>
        /// Renders all entities in the game loop.
        /// Called once per frame after Update by the application.
        /// Separates rendering into two passes: world-space entities and UI-space elements.
        /// </summary>
        /// <param name="renderer">The renderer used to draw entities and UI elements.</param>
        public void Draw(Renderer renderer)
        {
            renderer.BeginWorld();

            foreach (var tilemap in _tilemaps)
            {
                renderer.DrawTilemap(tilemap);
            }

            foreach (var entity in _entities)
            {
                renderer.DrawEntity(entity);
            }

            renderer.EndWorld();

            renderer.BeginUI();

            renderer.EndUI();
        }

        #endregion
    }
}