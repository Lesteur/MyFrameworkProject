using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Graphics.Shaders;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Central rendering system responsible for managing sprite batch operations and camera transformations.
    /// Provides separate rendering contexts for world-space and UI-space objects with automatic virtual resolution scaling.
    /// </summary>
    public sealed class Renderer : IDisposable
    {
        #region Fields - Graphics

        /// <summary>
        /// The graphics device used for rendering operations and clearing the screen.
        /// </summary>
        private readonly GraphicsDevice _graphicsDevice;

        /// <summary>
        /// The sprite batch used for efficient 2D sprite rendering.
        /// </summary>
        private readonly SpriteBatch _spriteBatch;

        /// <summary>
        /// Collection of named textures for resource management.
        /// </summary>
        private readonly Dictionary<string, Graphics.Texture> _namedTextures;

        /// <summary>
        /// Collection of named sprites for resource management.
        /// </summary>
        private readonly Dictionary<string, Sprite> _namedSprites;

        /// <summary>
        /// Collection of named tilesets for resource management.
        /// </summary>
        private readonly Dictionary<string, Tileset> _namedTilesets;

        #endregion

        #region Fields - Cameras

        /// <summary>
        /// The camera used for rendering world-space entities.
        /// Supports position, rotation, and zoom transformations.
        /// </summary>
        private Camera _worldCamera;

        /// <summary>
        /// The camera used for rendering UI-space elements.
        /// Typically remains static without transformations.
        /// </summary>
        private Camera _uiCamera;

        #endregion

        #region Fields - Scaling

        /// <summary>
        /// The scaling matrix used to transform from virtual resolution to actual window resolution.
        /// Maintains aspect ratio using letterboxing to prevent distortion.
        /// </summary>
        private Matrix _scalingMatrix;

        #endregion

        #region Fields - Shaders

        /// <summary>
        /// The shader manager responsible for global and entity-specific shader effects.
        /// </summary>
        private readonly ShaderManager _shaderManager;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether the renderer has been disposed.
        /// </summary>
        private bool _disposed = false;

        #endregion

        #region Properties - Cameras

        /// <summary>
        /// Gets the camera used for rendering world-space entities.
        /// </summary>
        public Camera WorldCamera => _worldCamera;

        /// <summary>
        /// Gets the camera used for rendering UI-space elements.
        /// </summary>
        public Camera UICamera => _uiCamera;

        #endregion

        #region Properties - Shaders

        /// <summary>
        /// Gets the shader manager for managing global and entity-specific shaders.
        /// </summary>
        public ShaderManager ShaderManager => _shaderManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class with the specified graphics device.
        /// Automatically creates and configures cameras and calculates the scaling matrix for virtual resolution support.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used for rendering operations.</param>
        public Renderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _shaderManager = new ShaderManager();

            _namedTextures = [];
            _namedSprites = [];
            _namedTilesets = [];

            InitializeCameras();
            CalculateScalingMatrix();

            Logger.Info("Renderer initialized");
        }

        #endregion

        #region Private Methods - Initialization

        /// <summary>
        /// Initializes the world and UI cameras with the virtual resolution dimensions.
        /// Both cameras are centered at the origin (0, 0) with dimensions matching the engine's virtual resolution.
        /// </summary>
        private void InitializeCameras()
        {
            _worldCamera = new Camera(
                0,
                0,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );

            _uiCamera = new Camera(
                0,
                0,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );
        }

        /// <summary>
        /// Calculates the scaling matrix that transforms from virtual resolution to actual window resolution.
        /// Uses the smaller of the horizontal or vertical scaling factors to maintain aspect ratio (letterboxing).
        /// This ensures that the game's graphics are not distorted regardless of window size.
        /// </summary>
        private void CalculateScalingMatrix()
        {
            float scaleX = (float)EngineConfig.WindowWidth / EngineConfig.VirtualWidth;
            float scaleY = (float)EngineConfig.WindowHeight / EngineConfig.VirtualHeight;

            // Use the smaller scale to maintain aspect ratio (letterbox mode)
            float scale = MathHelper.Min(scaleX, scaleY);

            _scalingMatrix = Matrix.CreateScale(scale, scale, 1f);
        }

        #endregion

        #region Public Methods - Resource Management

        /// <summary>
        /// Registers a texture with the specified name for later retrieval.
        /// </summary>
        /// <param name="name">The unique name for the texture.</param>
        /// <param name="texture">The texture to register.</param>
        /// <returns>True if the texture was registered successfully; false if the name already exists.</returns>
        public bool RegisterTexture(string name, Graphics.Texture texture)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.Warning("Cannot register texture with null or empty name");
                return false;
            }

            if (texture == null)
            {
                Logger.Warning($"Cannot register null texture with name '{name}'");
                return false;
            }

            return _namedTextures.TryAdd(name, texture);
        }

        /// <summary>
        /// Retrieves a texture by its registered name.
        /// </summary>
        /// <param name="name">The name of the texture to retrieve.</param>
        /// <returns>The texture if found; otherwise, null.</returns>
        public Graphics.Texture GetTexture(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            _namedTextures.TryGetValue(name, out Graphics.Texture texture);
            return texture;
        }

        /// <summary>
        /// Registers a sprite with the specified name for later retrieval.
        /// </summary>
        /// <param name="name">The unique name for the sprite.</param>
        /// <param name="sprite">The sprite to register.</param>
        /// <returns>True if the sprite was registered successfully; false if the name already exists.</returns>
        public bool RegisterSprite(string name, Sprite sprite)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.Warning("Cannot register sprite with null or empty name");
                return false;
            }

            if (sprite == null)
            {
                Logger.Warning($"Cannot register null sprite with name '{name}'");
                return false;
            }

            return _namedSprites.TryAdd(name, sprite);
        }

        /// <summary>
        /// Retrieves a sprite by its registered name.
        /// </summary>
        /// <param name="name">The name of the sprite to retrieve.</param>
        /// <returns>The sprite if found; otherwise, null.</returns>
        public Sprite GetSprite(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            _namedSprites.TryGetValue(name, out Sprite sprite);
            return sprite;
        }

        /// <summary>
        /// Registers a tileset with the specified name for later retrieval.
        /// </summary>
        /// <param name="name">The unique name for the tileset.</param>
        /// <param name="tileset">The tileset to register.</param>
        /// <returns>True if the tileset was registered successfully; false if the name already exists.</returns>
        public bool RegisterTileset(string name, Tileset tileset)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.Warning("Cannot register tileset with null or empty name");
                return false;
            }

            if (tileset == null)
            {
                Logger.Warning($"Cannot register null tileset with name '{name}'");
                return false;
            }

            return _namedTilesets.TryAdd(name, tileset);
        }

        /// <summary>
        /// Retrieves a tileset by its registered name.
        /// </summary>
        /// <param name="name">The name of the tileset to retrieve.</param>
        /// <returns>The tileset if found; otherwise, null.</returns>
        public Tileset GetTileset(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            _namedTilesets.TryGetValue(name, out Tileset tileset);
            return tileset;
        }

        /// <summary>
        /// Clears all registered resources (textures, sprites, and tilesets).
        /// </summary>
        public void ClearResources()
        {
            _namedTextures.Clear();
            _namedSprites.Clear();
            _namedTilesets.Clear();
            Logger.Info("All renderer resources cleared");
        }

        #endregion

        #region Public Methods - Frame Management

        /// <summary>
        /// Begins a new rendering frame by clearing the screen with the default background color.
        /// Should be called once at the start of each frame before any rendering operations.
        /// </summary>
        public void BeginFrame()
        {
            _graphicsDevice.Clear(Color.SkyBlue);
        }

        #endregion

        #region Public Methods - World Rendering

        /// <summary>
        /// Begins the world rendering context with the world camera's transform and scaling matrix applied.
        /// All entities drawn after this call will be rendered in world-space coordinates.
        /// Uses point clamp sampling for pixel-perfect rendering and alpha blending for transparency.
        /// Applies global world shaders if any are registered.
        /// Must be followed by a call to <see cref="EndWorld"/> when world rendering is complete.
        /// </summary>
        public void BeginWorld()
        {
            Effect activeEffect = GetActiveWorldShader();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: activeEffect,
                transformMatrix: _worldCamera.GetTransformMatrix() * _scalingMatrix
            );
        }

        /// <summary>
        /// Draws an entity to the screen using its inherited rendering properties.
        /// The entity is rendered in world-space coordinates with the current world camera transformation applied.
        /// If the entity has a specific shader assigned, the batch is restarted with that shader.
        /// Automatically handles visibility checks and shader management.
        /// </summary>
        /// <param name="entity">The entity to render. Can be any type derived from Entity (GameObject, Tilemap, etc.).</param>
        public void DrawEntity(Entity entity)
        {
            if (entity == null || !entity.Visible)
                return;

            // Handle entity-specific shader if present
            if (entity.Shader != null)
            {
                DrawEntityWithShader(entity);
            }
            else
            {
                // Draw entity with current batch (uses global shader if any)
                entity.Draw(_spriteBatch);
            }
        }

        /// <summary>
        /// Draws a tilemap to the screen. This is a convenience method that calls DrawEntity internally.
        /// Tilemaps now inherit from Entity and use the same rendering pipeline.
        /// </summary>
        /// <param name="tilemap">The tilemap to render.</param>
        public void DrawTilemap(Tilemap tilemap)
        {
            DrawEntity(tilemap);
        }

        /// <summary>
        /// Ends the world rendering context and submits all batched world-space sprites for rendering.
        /// Must be called after <see cref="BeginWorld"/> when all world entities have been drawn.
        /// </summary>
        public void EndWorld()
        {
            _spriteBatch.End();
        }

        #endregion

        #region Public Methods - UI Rendering

        /// <summary>
        /// Begins the UI rendering context with the UI camera's transform and scaling matrix applied.
        /// All UI elements drawn after this call will be rendered in UI-space coordinates.
        /// Uses point clamp sampling for pixel-perfect rendering and alpha blending for transparency.
        /// Applies global UI shaders if any are registered.
        /// Must be followed by a call to <see cref="EndUI"/> when UI rendering is complete.
        /// </summary>
        public void BeginUI()
        {
            Effect activeEffect = GetActiveUIShader();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: activeEffect,
                transformMatrix: _uiCamera.GetTransformMatrix() * _scalingMatrix
            );
        }

        /// <summary>
        /// Ends the UI rendering context and submits all batched UI-space sprites for rendering.
        /// Must be called after <see cref="BeginUI"/> when all UI elements have been drawn.
        /// </summary>
        public void EndUI()
        {
            _spriteBatch.End();
        }

        #endregion

        #region Private Methods - Shader Management

        /// <summary>
        /// Gets the active shader for world rendering.
        /// Returns null if no global world shaders are registered.
        /// </summary>
        /// <returns>The active world shader effect, or null.</returns>
        private Effect GetActiveWorldShader()
        {
            return _shaderManager.GlobalWorldShaders.Count > 0
                ? _shaderManager.GlobalWorldShaders[0].NativeEffect
                : null;
        }

        /// <summary>
        /// Gets the active shader for UI rendering.
        /// Returns null if no global UI shaders are registered.
        /// </summary>
        /// <returns>The active UI shader effect, or null.</returns>
        private Effect GetActiveUIShader()
        {
            return _shaderManager.GlobalUIShaders.Count > 0
                ? _shaderManager.GlobalUIShaders[0].NativeEffect
                : null;
        }

        /// <summary>
        /// Draws an entity with its specific shader by temporarily switching the sprite batch context.
        /// </summary>
        /// <param name="entity">The entity with a custom shader to render.</param>
        private void DrawEntityWithShader(Entity entity)
        {
            // End current batch
            _spriteBatch.End();

            // Begin new batch with entity-specific shader
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: entity.Shader.NativeEffect,
                transformMatrix: _worldCamera.GetTransformMatrix() * _scalingMatrix
            );

            // Draw entity with its specific shader
            entity.Draw(_spriteBatch);

            // Resume normal batch with global shader
            _spriteBatch.End();
            BeginWorld();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the renderer.
        /// Disposes the sprite batch and shader manager.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _spriteBatch?.Dispose();
            _shaderManager?.Dispose();

            ClearResources();

            _disposed = true;
            Logger.Info("Renderer disposed");
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Finalizer to ensure resources are released if Dispose is not called.
        /// </summary>
        ~Renderer()
        {
            Dispose();
        }

        #endregion
    }
}