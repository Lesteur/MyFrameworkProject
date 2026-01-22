using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Core;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a tilemap instance based on a tileset, allowing for grid-based tile rendering.
    /// Extends Entity to inherit common rendering properties like position, color, visibility, and layer depth.
    /// Provides functionality for managing tile data, efficient grid-based rendering, and tile animations.
    /// </summary>
    public class Tilemap : Entity
    {
        #region Fields - Tileset

        /// <summary>
        /// The tileset associated with this tilemap.
        /// </summary>
        private Tileset _tileset;

        #endregion

        #region Fields - Grid

        /// <summary>
        /// The width of the tilemap grid in tiles.
        /// </summary>
        private readonly int _gridWidth;

        /// <summary>
        /// The height of the tilemap grid in tiles.
        /// </summary>
        private readonly int _gridHeight;

        /// <summary>
        /// 2D array storing tile indices. -1 represents an empty tile.
        /// </summary>
        private readonly int[,] _tileData;

        #endregion

        #region Fields - Animation

        /// <summary>
        /// Total elapsed time in milliseconds since the tilemap was created or reset.
        /// Used to calculate the current frame for all animated tiles.
        /// </summary>
        private double _totalElapsedTimeMs = 0.0;

        /// <summary>
        /// Indicates whether tile animations are enabled for this tilemap.
        /// When disabled, animated tiles display their base frame.
        /// </summary>
        private bool _animationsEnabled = true;

        #endregion

        #region Properties - Tileset

        /// <summary>
        /// Gets the tileset associated with this tilemap.
        /// </summary>
        public Tileset Tileset => _tileset;

        #endregion

        #region Properties - Grid

        /// <summary>
        /// Gets the width of the tilemap grid in tiles.
        /// </summary>
        public int GridWidth => _gridWidth;

        /// <summary>
        /// Gets the height of the tilemap grid in tiles.
        /// </summary>
        public int GridHeight => _gridHeight;

        #endregion

        #region Properties - Animation

        /// <summary>
        /// Gets whether tile animations are currently enabled.
        /// </summary>
        public bool AnimationsEnabled => _animationsEnabled;

        /// <summary>
        /// Gets the total elapsed time in milliseconds for animations.
        /// </summary>
        public double TotalElapsedTimeMs => _totalElapsedTimeMs;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tilemap"/> class with the specified tileset and grid dimensions.
        /// </summary>
        /// <param name="tileset">The tileset to use for this tilemap.</param>
        /// <param name="gridWidth">The width of the tilemap grid in tiles.</param>
        /// <param name="gridHeight">The height of the tilemap grid in tiles.</param>
        public Tilemap(Tileset tileset, int gridWidth, int gridHeight) : base()
        {
            ArgumentNullException.ThrowIfNull(tileset);

            if (gridWidth <= 0)
                throw new ArgumentException("Grid width must be greater than zero.", nameof(gridWidth));

            if (gridHeight <= 0)
                throw new ArgumentException("Grid height must be greater than zero.", nameof(gridHeight));

            _tileset = tileset;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _tileData = new int[gridWidth, gridHeight];

            // Initialize all tiles to -1 (empty)
            ClearAll();
        }

        #endregion

        #region Public Methods - Tileset Management

        /// <summary>
        /// Sets the tileset for this tilemap.
        /// </summary>
        /// <param name="tileset">The new tileset to assign.</param>
        public void SetTileset(Tileset tileset)
        {
            ArgumentNullException.ThrowIfNull(tileset);
            _tileset = tileset;
        }

        #endregion

        #region Public Methods - Tile Data Management

        /// <summary>
        /// Sets a tile at the specified grid coordinates.
        /// </summary>
        /// <param name="gridX">The X-coordinate in the grid.</param>
        /// <param name="gridY">The Y-coordinate in the grid.</param>
        /// <param name="tileIndex">The tile index from the tileset. Use -1 for empty tiles.</param>
        public void SetTile(int gridX, int gridY, int tileIndex)
        {
            if (IsValidGridPosition(gridX, gridY))
            {
                _tileData[gridX, gridY] = tileIndex;
            }
        }

        /// <summary>
        /// Gets the tile index at the specified grid coordinates.
        /// </summary>
        /// <param name="gridX">The X-coordinate in the grid.</param>
        /// <param name="gridY">The Y-coordinate in the grid.</param>
        /// <returns>The tile index at the specified position, or -1 if out of bounds.</returns>
        public int GetTile(int gridX, int gridY)
        {
            if (IsValidGridPosition(gridX, gridY))
            {
                return _tileData[gridX, gridY];
            }
            return -1;
        }

        /// <summary>
        /// Clears the tile at the specified grid coordinates (sets it to -1).
        /// </summary>
        /// <param name="gridX">The X-coordinate in the grid.</param>
        /// <param name="gridY">The Y-coordinate in the grid.</param>
        public void ClearTile(int gridX, int gridY)
        {
            SetTile(gridX, gridY, -1);
        }

        /// <summary>
        /// Clears all tiles in the tilemap by setting them to -1 (empty).
        /// </summary>
        public void ClearAll()
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    _tileData[x, y] = -1;
                }
            }
        }

        /// <summary>
        /// Fills the entire tilemap with the specified tile index.
        /// </summary>
        /// <param name="tileIndex">The tile index to fill with.</param>
        public void Fill(int tileIndex)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    _tileData[x, y] = tileIndex;
                }
            }
        }

        /// <summary>
        /// Fills a rectangular region with the specified tile index.
        /// Coordinates are clamped to grid boundaries automatically.
        /// </summary>
        /// <param name="startX">The starting X-coordinate in the grid.</param>
        /// <param name="startY">The starting Y-coordinate in the grid.</param>
        /// <param name="width">The width of the region in tiles.</param>
        /// <param name="height">The height of the region in tiles.</param>
        /// <param name="tileIndex">The tile index to fill with.</param>
        public void FillRegion(int startX, int startY, int width, int height, int tileIndex)
        {
            int endX = Math.Min(startX + width, _gridWidth);
            int endY = Math.Min(startY + height, _gridHeight);

            for (int x = Math.Max(0, startX); x < endX; x++)
            {
                for (int y = Math.Max(0, startY); y < endY; y++)
                {
                    _tileData[x, y] = tileIndex;
                }
            }
        }

        #endregion

        #region Public Methods - Animation

        /// <summary>
        /// Updates the tilemap's animation state based on elapsed time.
        /// Overrides Entity.UpdateAnimation to handle tile-specific animations.
        /// This method is automatically called by the game loop.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public sealed override void UpdateAnimation(float deltaTime)
        {
            // Only update if animations are enabled and the tileset has animations
            if (_animationsEnabled && _tileset != null && _tileset.HasAnimations)
            {
                _totalElapsedTimeMs += deltaTime * 1000.0; // Convert seconds to milliseconds
            }
        }

        /// <summary>
        /// Enables tile animations for this tilemap.
        /// </summary>
        public void EnableAnimations()
        {
            _animationsEnabled = true;
        }

        /// <summary>
        /// Disables tile animations for this tilemap.
        /// Animated tiles will display their base frame.
        /// </summary>
        public void DisableAnimations()
        {
            _animationsEnabled = false;
        }

        /// <summary>
        /// Resets the animation timer to zero, restarting all tile animations.
        /// </summary>
        public void ResetAnimations()
        {
            _totalElapsedTimeMs = 0.0;
        }

        #endregion

        #region Public Methods - Rendering

        /// <summary>
        /// Draws the tilemap to the screen using the provided sprite batch.
        /// Iterates through all tiles and renders only non-empty tiles.
        /// Automatically handles animated tiles if animations are enabled.
        /// Uses the tilemap's position, color, layer depth, and visibility inherited from Entity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for rendering.</param>
        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            if (_tileset == null)
                return;

            // Cache values for performance
            bool hasAnimations = _animationsEnabled && _tileset.HasAnimations;
            double currentTimeMs = _totalElapsedTimeMs;
            Color color = Color;
            float layerDepth = LayerDepth;
            SpriteEffects spriteEffects = SpriteEffects;

            for (int gridY = 0; gridY < _gridHeight; gridY++)
            {
                for (int gridX = 0; gridX < _gridWidth; gridX++)
                {
                    int tileIndex = _tileData[gridX, gridY];

                    // Skip empty tiles
                    if (tileIndex < 0)
                        continue;

                    // Get the actual tile to render (animated or static)
                    int renderTileIndex = hasAnimations 
                        ? _tileset.GetAnimatedTileId(tileIndex, currentTimeMs)
                        : tileIndex;

                    Rectangle sourceRect = GetTileSourceRectangle(renderTileIndex);
                    Vector2 position = new(
                        _x + gridX * _tileset.TileWidth,
                        _y + gridY * _tileset.TileHeight
                    );

                    spriteBatch.Draw(
                        _tileset.Texture.NativeTexture,
                        position,
                        sourceRect,
                        color,
                        0f,
                        Vector2.Zero,
                        1f,
                        spriteEffects,
                        layerDepth
                    );
                }
            }
        }

        #endregion

        #region Private Methods - Helpers

        /// <summary>
        /// Checks if the specified grid coordinates are within valid bounds.
        /// </summary>
        /// <param name="gridX">The X-coordinate in the grid.</param>
        /// <param name="gridY">The Y-coordinate in the grid.</param>
        /// <returns>True if the coordinates are valid, false otherwise.</returns>
        private bool IsValidGridPosition(int gridX, int gridY)
        {
            return gridX >= 0 && gridX < _gridWidth && gridY >= 0 && gridY < _gridHeight;
        }

        /// <summary>
        /// Calculates the source rectangle for a specific tile index in the tileset.
        /// </summary>
        /// <param name="tileIndex">The tile index.</param>
        /// <returns>A <see cref="Rectangle"/> representing the source area in the tileset texture.</returns>
        private Rectangle GetTileSourceRectangle(int tileIndex)
        {
            if (_tileset == null || tileIndex < 0)
                return Rectangle.Empty;

            // Calculate the number of tiles per row, accounting for margins and spacing
            int tilesPerRow = _tileset.GetTilesPerRow();

            // Calculate row and column from tile index
            int row = tileIndex / tilesPerRow;
            int col = tileIndex % tilesPerRow;

            // Calculate the pixel position of the tile in the texture
            int x = _tileset.XMargin + col * (_tileset.TileWidth + _tileset.XSpacing);
            int y = _tileset.YMargin + row * (_tileset.TileHeight + _tileset.YSpacing);

            return new Rectangle(x, y, _tileset.TileWidth, _tileset.TileHeight);
        }

        #endregion
    }
}