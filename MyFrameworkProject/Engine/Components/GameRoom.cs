using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MyFrameworkProject.Engine.Core;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Serialization;

namespace MyFrameworkProject.Engine.Components
{
    /// <summary>
    /// Represents a game room (scene) that manages its own resources, entities, and lifecycle.
    /// Provides automatic resource loading and cleanup when the room becomes active or inactive.
    /// Supports Tiled map integration for level design and automatic tilemap generation.
    /// </summary>
    /// <param name="width">The width of the room in pixels.</param>
    /// <param name="height">The height of the room in pixels.</param>
    public abstract class GameRoom(int width, int height) : IDisposable
    {
        #region Fields - Dimensions

        /// <summary>
        /// The width of the room in pixels.
        /// Can be updated dynamically when loading Tiled maps.
        /// </summary>
        private int _width = width > 0 ? width : throw new ArgumentException("Width must be greater than zero.", nameof(width));

        /// <summary>
        /// The height of the room in pixels.
        /// Can be updated dynamically when loading Tiled maps.
        /// </summary>
        private int _height = height > 0 ? height : throw new ArgumentException("Height must be greater than zero.", nameof(height));

        #endregion

        #region Fields - Resources

        /// <summary>
        /// Collection of disposable resources loaded by this room.
        /// Resources are automatically disposed when the room is unloaded.
        /// </summary>
        private readonly List<IDisposable> _disposableResources = [];

        #endregion

        #region Properties - Dimensions

        /// <summary>
        /// Gets the width of this game room in pixels.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the height of this game room in pixels.
        /// </summary>
        public int Height => _height;

        #endregion

        #region Properties - Systems

        /// <summary>
        /// Gets the game loop associated with this room.
        /// Provides access to entity and tilemap management.
        /// </summary>
        protected GameLoop GameLoop { get; private set; }

        /// <summary>
        /// Gets the content manager for loading resources.
        /// Use this to load textures, sounds, and other game assets.
        /// </summary>
        protected ContentManager Content { get; private set; }

        #endregion

        #region Properties - State

        /// <summary>
        /// Gets whether this room is currently loaded and active.
        /// </summary>
        public bool IsLoaded { get; private set; }

        #endregion

        #region Properties - Camera

        /// <summary>
        /// Gets or sets the game object that the camera should follow in this room.
        /// Set to null to disable camera following.
        /// </summary>
        public GameObject CameraTarget { get; protected set; }

        #endregion

        #region Internal Methods - Lifecycle

        /// <summary>
        /// Initializes the room with required systems and loads resources.
        /// Called internally by the SceneManager when the room becomes active.
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

            OnBeforeLoad();
            Load();
            OnAfterLoad();

            IsLoaded = true;
        }

        /// <summary>
        /// Unloads the room and disposes all loaded resources.
        /// Called internally by the SceneManager when the room becomes inactive.
        /// </summary>
        internal void Cleanup()
        {
            if (!IsLoaded)
            {
                Logger.Warning($"Room {GetType().Name} is not loaded");
                return;
            }

            Logger.Info($"Unloading room: {GetType().Name}");

            OnBeforeUnload();
            Unload();

            // Clear all entities and game objects from the game loop
            GameLoop.ClearGameObjects();

            // Dispose all tracked resources
            foreach (var resource in _disposableResources)
            {
                resource?.Dispose();
            }
            _disposableResources.Clear();

            OnAfterUnload();

            IsLoaded = false;
            CameraTarget = null;

            Logger.Info($"Room {GetType().Name} unloaded successfully");
        }

        #endregion

        #region Protected Methods - Room Lifecycle

        /// <summary>
        /// Called before the Load method. Override this to perform pre-loading setup.
        /// </summary>
        protected virtual void OnBeforeLoad() { }

        /// <summary>
        /// Called when the room is loaded. Override this to load room-specific resources and create entities.
        /// Use <see cref="Content"/> to load assets and <see cref="GameLoop"/> to add entities.
        /// </summary>
        protected abstract void Load();

        /// <summary>
        /// Called after the Load method. Override this to perform post-loading setup.
        /// </summary>
        protected virtual void OnAfterLoad() { }

        /// <summary>
        /// Called before the Unload method. Override this to perform pre-unload cleanup.
        /// </summary>
        protected virtual void OnBeforeUnload() { }

        /// <summary>
        /// Called when the room is unloaded. Override this to perform custom cleanup logic.
        /// Resources are automatically disposed, but you can use this for custom cleanup.
        /// </summary>
        protected virtual void Unload() { }

        /// <summary>
        /// Called after the Unload method. Override this to perform post-unload cleanup.
        /// </summary>
        protected virtual void OnAfterUnload() { }

        #endregion

        #region Protected Methods - Tiled Map Loading

        /// <summary>
        /// Loads a Tiled map from the content pipeline and creates all associated tilemaps and game objects.
        /// Automatically updates room dimensions to match the map size.
        /// </summary>
        /// <param name="assetPath">Relative path to the Tiled map asset (e.g., "Maps/Level1").</param>
        /// <param name="objectFactory">Optional factory function to create GameObjects from Tiled objects.</param>
        protected void LoadTiledMap(string assetPath, Func<TiledObject, GameObject> objectFactory = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Logger.Error("Cannot load Tiled map with null or empty asset path");
                return;
            }

            // Load the Tiled map from the content pipeline
            var tiledMap = Content.Load<TiledMap>(assetPath);
            if (tiledMap == null)
            {
                Logger.Error($"Failed to load Tiled map: {assetPath}");
                return;
            }

            Logger.Info($"Loading Tiled map: {assetPath}");

            // Update room dimensions based on the map size
            _width = tiledMap.Width * tiledMap.TileWidth;
            _height = tiledMap.Height * tiledMap.TileHeight;

            // Load tilesets referenced by the map
            var tilesets = LoadTilesets(tiledMap);

            // Process all layers in the map
            foreach (var layer in tiledMap.Layers)
            {
                if (!layer.Visible)
                    continue;

                switch (layer.Type)
                {
                    case "tilelayer":
                        ProcessTileLayer(layer, tilesets);
                        break;

                    case "objectgroup":
                        ProcessObjectLayer(layer, objectFactory);
                        break;
                }
            }

            Logger.Info($"Tiled map '{assetPath}' loaded successfully with {tiledMap.Layers.Count} layers");
        }

        /// <summary>
        /// Creates a default GameObject from a Tiled object.
        /// Override this method to provide custom object creation logic.
        /// </summary>
        /// <param name="tiledObject">The Tiled object definition.</param>
        /// <returns>A GameObject instance or null.</returns>
        protected virtual GameObject CreateDefaultGameObject(TiledObject tiledObject)
        {
            Logger.Warning($"No factory defined for object type: {tiledObject.Type}");
            return null;
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
            if (resource != null)
            {
                _disposableResources.Add(resource);
            }
            return resource;
        }

        #endregion

        #region Private Methods - Tiled Map Processing

        /// <summary>
        /// Loads all tilesets referenced by a Tiled map.
        /// </summary>
        /// <param name="tiledMap">The Tiled map containing tileset references.</param>
        /// <returns>Dictionary mapping FirstGid to Tileset instances.</returns>
        private Dictionary<int, Tileset> LoadTilesets(TiledMap tiledMap)
        {
            var tilesets = new Dictionary<int, Tileset>();

            foreach (var tilesetRef in tiledMap.Tilesets)
            {
                try
                {
                    // Extract tileset asset name from source path (remove extension)
                    string tilesetAssetName = Path.ChangeExtension(tilesetRef.Source, null);

                    // Load the tileset from the content pipeline
                    var tiledTileset = Content.Load<TiledTileset>(tilesetAssetName);
                    if (tiledTileset == null)
                    {
                        Logger.Warning($"Failed to load tileset: {tilesetAssetName}");
                        continue;
                    }

                    // Create the engine tileset from Tiled tileset data
                    var tileset = CreateTileset(tiledTileset);
                    if (tileset != null)
                    {
                        tilesets[tilesetRef.FirstGid] = tileset;
                        Logger.Info($"Loaded tileset: {tiledTileset.Name} (FirstGid: {tilesetRef.FirstGid})");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error loading tileset '{tilesetRef.Source}': {ex.Message}");
                }
            }

            return tilesets;
        }

        /// <summary>
        /// Creates an engine Tileset from Tiled tileset data.
        /// </summary>
        /// <param name="tiledTileset">The Tiled tileset data.</param>
        /// <returns>A Tileset ready for rendering, or null if creation fails.</returns>
        private Tileset CreateTileset(TiledTileset tiledTileset)
        {
            try
            {
                // Extract texture name from image path (remove path and extension)
                string textureName = Path.GetFileNameWithoutExtension(tiledTileset.Image);

                // Load the texture from the content pipeline
                Texture2D nativeTexture = Content.Load<Texture2D>($"Textures/{textureName}");
                var texture = new Graphics.Texture(nativeTexture);

                // Create the tileset with margin and spacing
                var tileset = new Tileset(
                    texture,
                    tiledTileset.TileWidth,
                    tiledTileset.TileHeight,
                    tiledTileset.Margin,
                    tiledTileset.Margin,
                    tiledTileset.Spacing,
                    tiledTileset.Spacing
                );

                tileset.SetAnimations(tiledTileset.Tiles);

                return tileset;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create tileset '{tiledTileset.Name}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Processes a tile layer from a Tiled map and adds it to the game loop.
        /// Supports multiple tilesets by determining the correct tileset for each tile.
        /// </summary>
        /// <param name="layer">The tile layer to process.</param>
        /// <param name="tilesets">Dictionary of available tilesets mapped by FirstGid.</param>
        private void ProcessTileLayer(TiledLayer layer, Dictionary<int, Tileset> tilesets)
        {
            if (layer.Data == null || layer.Data.Count == 0)
            {
                Logger.Warning($"Tile layer '{layer.Name}' has no data");
                return;
            }

            if (tilesets.Count == 0)
            {
                Logger.Warning($"No tilesets available for layer: {layer.Name}");
                return;
            }

            // For simplicity, use the first tileset (can be extended for multi-tileset support)
            Tileset tileset = null;
            int firstGid = 0;

            foreach (var kvp in tilesets)
            {
                tileset = kvp.Value;
                firstGid = kvp.Key;
                break;
            }

            if (tileset == null)
            {
                Logger.Warning($"No valid tileset found for layer: {layer.Name}");
                return;
            }

            // Create the tilemap
            var tilemap = new Tilemap(tileset, layer.Width, layer.Height);

            // Fill the tilemap with tile data
            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int index = y * layer.Width + x;
                    if (index >= layer.Data.Count)
                        continue;

                    int tileId = layer.Data[index];
                    
                    // Skip empty tiles (Tiled uses 0 for empty)
                    if (tileId == 0)
                        continue;

                    // Convert from Tiled GID to tileset index
                    int tileIndex = tileId - firstGid;
                    
                    if (tileIndex >= 0)
                    {
                        tilemap.SetTile(x, y, tileIndex);
                    }
                }
            }

            // Set tilemap properties
            tilemap.SetLayerDepth(0.5f);
            tilemap.SetVisible(layer.Visible);

            // Add to game loop for rendering
            GameLoop.AddTilemap(tilemap);

            Logger.Info($"Created tilemap: {layer.Name} ({layer.Width}x{layer.Height})");
        }

        /// <summary>
        /// Processes an object layer from a Tiled map and creates game objects.
        /// </summary>
        /// <param name="layer">The object layer to process.</param>
        /// <param name="objectFactory">Factory function to create GameObjects from Tiled objects.</param>
        private void ProcessObjectLayer(TiledLayer layer, Func<TiledObject, GameObject> objectFactory)
        {
            if (layer.Objects == null || layer.Objects.Count == 0)
                return;

            foreach (var obj in layer.Objects)
            {
                if (!obj.Visible)
                    continue;

                GameObject gameObject;

                // Use custom factory if provided
                if (objectFactory != null)
                {
                    gameObject = objectFactory(obj);
                }
                else
                {
                    // Use default object creation
                    gameObject = CreateDefaultGameObject(obj);
                }

                if (gameObject != null)
                {
                    gameObject.SetPosition((int)obj.X, (int)obj.Y);
                    GameLoop.AddGameObject(gameObject);

                    Logger.Info($"Created object: {obj.Name} ({obj.Type}) at ({obj.X}, {obj.Y})");
                }
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes the room and its resources.
        /// </summary>
        public void Dispose()
        {
            if (IsLoaded)
            {
                Cleanup();
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}