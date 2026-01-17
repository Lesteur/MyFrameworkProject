using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Graphics.Shaders;
using MyFrameworkProject.Engine.Components.Collisions;

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

        #region Fields - Debug Rendering

        /// <summary>
        /// A 1x1 white texture used for drawing debug shapes.
        /// </summary>
        private Texture2D _pixelTexture;

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
            InitializeDebugTexture();

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

        /// <summary>
        /// Initializes a 1x1 white pixel texture for drawing debug shapes.
        /// </summary>
        private void InitializeDebugTexture()
        {
            _pixelTexture = new Texture2D(_graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
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
        /// Draws a collision mask for debugging purposes.
        /// Uses different colors for different collision types.
        /// </summary>
        /// <param name="collisionMask">The collision mask to draw.</param>
        /// <param name="x">The world X-coordinate of the mask.</param>
        /// <param name="y">The world Y-coordinate of the mask.</param>
        public void DrawCollisionMask(CollisionMask collisionMask, float x, float y)
        {
            if (collisionMask == null)
                return;

            switch (collisionMask)
            {
                case PointCollision point:
                    DrawPointCollision(point, x, y);
                    break;
                case LineCollision line:
                    DrawLineCollision(line, x, y);
                    break;
                case CircleCollision circle:
                    DrawCircleCollision(circle, x, y);
                    break;
                case RectangleCollision rectangle:
                    DrawRectangleCollision(rectangle, x, y);
                    break;
            }
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

        #region Private Methods - Debug Collision Rendering

        /// <summary>
        /// Draws a point collision mask as a small cross.
        /// </summary>
        /// <param name="point">The point collision mask.</param>
        /// <param name="x">The world X-coordinate.</param>
        /// <param name="y">The world Y-coordinate.</param>
        private void DrawPointCollision(PointCollision point, float x, float y)
        {
            float worldX = x + point.OffsetX;
            float worldY = y + point.OffsetY;
            Color color = Color.Red;
            int size = 3;

            // Draw a cross
            DrawDebugLine(worldX - size, worldY, worldX + size, worldY, color);
            DrawDebugLine(worldX, worldY - size, worldX, worldY + size, color);
        }

        /// <summary>
        /// Draws a line collision mask.
        /// </summary>
        /// <param name="line">The line collision mask.</param>
        /// <param name="x">The world X-coordinate.</param>
        /// <param name="y">The world Y-coordinate.</param>
        private void DrawLineCollision(LineCollision line, float x, float y)
        {
            float x1 = x + line.OffsetX;
            float y1 = y + line.OffsetY;
            float x2 = x + line.X2;
            float y2 = y + line.Y2;

            DrawDebugLine(x1, y1, x2, y2, Color.Yellow);
        }

        /// <summary>
        /// Draws a circle collision mask as an outlined circle.
        /// </summary>
        /// <param name="circle">The circle collision mask.</param>
        /// <param name="x">The world X-coordinate.</param>
        /// <param name="y">The world Y-coordinate.</param>
        private void DrawCircleCollision(CircleCollision circle, float x, float y)
        {
            float centerX = x + circle.OffsetX;
            float centerY = y + circle.OffsetY;
            float radius = circle.Radius;
            Color color = Color.Lime;
            int segments = 32;

            // Draw circle using line segments
            for (int i = 0; i < segments; i++)
            {
                float angle1 = (float)(2 * Math.PI * i / segments);
                float angle2 = (float)(2 * Math.PI * (i + 1) / segments);

                float x1 = centerX + radius * MathF.Cos(angle1);
                float y1 = centerY + radius * MathF.Sin(angle1);
                float x2 = centerX + radius * MathF.Cos(angle2);
                float y2 = centerY + radius * MathF.Sin(angle2);

                DrawDebugLine(x1, y1, x2, y2, color);
            }
        }

        /// <summary>
        /// Draws a rectangle collision mask as an outlined rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle collision mask.</param>
        /// <param name="x">The world X-coordinate.</param>
        /// <param name="y">The world Y-coordinate.</param>
        private void DrawRectangleCollision(RectangleCollision rectangle, float x, float y)
        {
            float left = x + rectangle.OffsetX;
            float top = y + rectangle.OffsetY;
            float right = left + rectangle.Width;
            float bottom = top + rectangle.Height;
            Color color = Color.Cyan;

            // Draw four edges
            DrawDebugLine(left, top, right, top, color);        // Top
            DrawDebugLine(right, top, right, bottom, color);    // Right
            DrawDebugLine(right, bottom, left, bottom, color);  // Bottom
            DrawDebugLine(left, bottom, left, top, color);      // Left
        }

        /// <summary>
        /// Draws a debug line between two points.
        /// </summary>
        /// <param name="x1">Starting X-coordinate.</param>
        /// <param name="y1">Starting Y-coordinate.</param>
        /// <param name="x2">Ending X-coordinate.</param>
        /// <param name="y2">Ending Y-coordinate.</param>
        /// <param name="color">Line color.</param>
        private void DrawDebugLine(float x1, float y1, float x2, float y2, Color color)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            float angle = MathF.Atan2(dy, dx);

            _spriteBatch.Draw(
                _pixelTexture,
                new Vector2(x1, y1),
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, 1f),
                SpriteEffects.None,
                0f
            );
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
            _pixelTexture?.Dispose();

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