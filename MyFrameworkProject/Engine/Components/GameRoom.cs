using Microsoft.Xna.Framework.Content;
using MyFrameworkProject.Engine.Core;
using System;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Components
{
    /// <summary>
    /// Represents a game room (scene) that manages its own resources, entities, and lifecycle.
    /// Provides automatic resource loading and cleanup when the room becomes active or inactive.
    /// </summary>
    /// <param name="width">The width of the room in pixels.</param>
    /// <param name="height">The height of the room in pixels.</param>
    public abstract class GameRoom(int width, int height)
    {
        #region Fields - Dimensions

        protected readonly int _width = width;
        protected readonly int _height = height;

        #endregion

        #region Fields - Resources

        /// <summary>
        /// Collection of disposable resources loaded by this room.
        /// Resources are automatically disposed when the room is unloaded.
        /// </summary>
        private readonly List<IDisposable> _disposableResources = [];

        #endregion

        #region Properties

        /// <summary>
        /// Gets the width of this game room in pixels.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the height of this game room in pixels.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Gets the game loop associated with this room.
        /// </summary>
        protected GameLoop GameLoop { get; private set; }

        /// <summary>
        /// Gets the content manager for loading resources.
        /// </summary>
        protected ContentManager Content { get; private set; }

        /// <summary>
        /// Gets whether this room is currently active.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the game object that the camera should follow in this room.
        /// </summary>
        public GameObject CameraTarget { get; protected set; }

        #endregion
        #region Constructors

        #endregion

        #region Internal Methods - Lifecycle

        /// <summary>
        /// Initializes the room with required systems and loads resources.
        /// Called internally by the Application when the room becomes active.
        /// </summary>
        /// <param name="gameLoop">The game loop to use for this room.</param>
        /// <param name="content">The content manager for loading resources.</param>
        internal void Initialize(GameLoop gameLoop, ContentManager content)
        {
            if (IsLoaded)
            {
                Logger.Warning($"Room {GetType().Name} is already loaded");
                return;
            }

            GameLoop = gameLoop;
            Content = content;

            Logger.Info($"Loading room: {GetType().Name}");

            Load();

            IsLoaded = true;
        }

        /// <summary>
        /// Unloads the room and disposes all loaded resources.
        /// Called internally by the Application when the room becomes inactive.
        /// </summary>
        internal void Cleanup()
        {
            if (!IsLoaded)
            {
                Logger.Warning($"Room {GetType().Name} is not loaded");
                return;
            }

            Logger.Info($"Unloading room: {GetType().Name}");

            Unload();

            // Clear all entities and game objects from the game loop
            GameLoop.ClearGameObjects();
            GameLoop.ClearEntities();

            // Dispose all tracked resources
            foreach (var resource in _disposableResources)
            {
                resource?.Dispose();
            }
            _disposableResources.Clear();

            IsLoaded = false;
            CameraTarget = null;

            Logger.Info($"Room {GetType().Name} unloaded successfully");
        }

        #endregion

        #region Abstract/Virtual Methods - Room Lifecycle

        /// <summary>
        /// Called when the room is loaded. Override this to load room-specific resources and create entities.
        /// Use <see cref="Content"/> to load assets and <see cref="GameLoop"/> to add entities.
        /// </summary>
        protected abstract void Load();

        /// <summary>
        /// Called when the room is unloaded. Override this to perform custom cleanup logic.
        /// Resources are automatically disposed, but you can use this for custom cleanup.
        /// </summary>
        protected virtual void Unload()
        {
            // Custom cleanup logic
        }

        #endregion

        #region Protected Methods - Resource Management

        /// <summary>
        /// Registers a disposable resource for automatic cleanup when the room is unloaded.
        /// </summary>
        /// <typeparam name="T">The type of resource to track.</typeparam>
        /// <param name="resource">The resource to track.</param>
        /// <returns>The same resource for fluent API usage.</returns>
        protected T TrackResource<T>(T resource) where T : IDisposable
        {
            _disposableResources.Add(resource);
            return resource;
        }

        #endregion
    }
}