using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Graphics;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Central rendering system responsible for managing sprite batch operations and camera transformations.
    /// Provides separate rendering contexts for world-space and UI-space objects with automatic virtual resolution scaling.
    /// </summary>
    public sealed class Renderer
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

        #region Properties

        /// <summary>
        /// Gets the camera used for rendering world-space entities.
        /// </summary>
        public Camera WorldCamera => _worldCamera;

        /// <summary>
        /// Gets the camera used for rendering UI-space elements.
        /// </summary>
        public Camera UICamera => _uiCamera;

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

            InitializeCameras();
            CalculateScalingMatrix();
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
        /// Must be followed by a call to <see cref="EndWorld"/> when world rendering is complete.
        /// </summary>
        public void BeginWorld()
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: _worldCamera.GetTransformMatrix() * _scalingMatrix
            );
        }

        /// <summary>
        /// Draws an entity to the screen using its sprite, position, rotation, scale, and color properties.
        /// The entity is rendered in world-space coordinates with the current world camera transformation applied.
        /// If the entity has no sprite assigned, the method returns without drawing anything.
        /// </summary>
        /// <param name="entity">The entity to render.</param>
        public void DrawEntity(Entity entity)
        {
            if (entity == null || !entity.Visible)
                return;

            var sprite = entity.Sprite;
            if (sprite == null)
                return;

            _spriteBatch.Draw(
                sprite.Texture.NativeTexture,
                new Vector2(entity.X, entity.Y),
                sprite.GetSourceRectangle(entity.FrameNumber),
                entity.Color,
                MathHelper.ToRadians(entity.Rotation),
                sprite.Origin,
                new Vector2(entity.ScaleX, entity.ScaleY),
                SpriteEffects.None,
                entity.LayerDepth
            );
        }

        /// <summary>
        /// Draws a tilemap to the screen by iterating over its grid and rendering each
        /// visible tile using the associated tileset texture.
        /// The tilemap is rendered in world-space coordinates with the current world camera transformation applied.
        /// </summary>
        /// <param name="tilemap">The tilemap to render.</param>
        public void DrawTilemap(Tilemap tilemap)
        {
            if (tilemap == null || !tilemap.Visible)
                return;

            for (int y = 0; y < tilemap.GridHeight; y++)
            {
                for (int x = 0; x < tilemap.GridWidth; x++)
                {
                    int tileIndex = tilemap.GetTile(x, y);
                    if (tileIndex < 0)
                        continue;

                    Rectangle sourceRect = tilemap.GetTileSourceRectangle(tileIndex);
                    Vector2 position = new(
                        tilemap.X + x * sourceRect.Width,
                        tilemap.Y + y * sourceRect.Height
                    );

                    _spriteBatch.Draw(
                        tilemap.Tileset.Texture.NativeTexture,
                        position,
                        sourceRect,
                        tilemap.Color,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        tilemap.LayerDepth
                    );
                }
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
        /// Must be followed by a call to <see cref="EndUI"/> when UI rendering is complete.
        /// </summary>
        public void BeginUI()
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
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
    }
}