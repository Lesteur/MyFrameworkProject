using System;
using System.Collections.Generic;
using MyFrameworkProject.Engine.Core;
using MyFrameworkProject.Engine.Serialization;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a tileset texture with configurable tile dimensions, margins, and spacing.
    /// Provides metadata for grid-based tile extraction from a texture atlas.
    /// Used in conjunction with <see cref="Tilemap"/> for tile-based rendering.
    /// </summary>
    /// <param name="texture">The texture containing the tileset image data.</param>
    /// <param name="tileWidth">The width of a single tile in pixels.</param>
    /// <param name="tileHeight">The height of a single tile in pixels.</param>
    /// <param name="xMargin">The horizontal margin in pixels from the texture edge before the first tile. Default is 0.</param>
    /// <param name="yMargin">The vertical margin in pixels from the texture edge before the first tile. Default is 0.</param>
    /// <param name="xSpacing">The horizontal spacing in pixels between adjacent tiles. Default is 0.</param>
    /// <param name="ySpacing">The vertical spacing in pixels between adjacent tiles. Default is 0.</param>
    public class Tileset(Texture texture, int tileWidth, int tileHeight, int xMargin = 0, int yMargin = 0, int xSpacing = 0, int ySpacing = 0)
    {
        #region Fields - Texture

        /// <summary>
        /// The texture containing the tileset image data.
        /// </summary>
        protected Texture _texture = texture ?? throw new ArgumentNullException(nameof(texture));

        #endregion

        #region Fields - Dimensions

        /// <summary>
        /// The total width of the tileset texture in pixels.
        /// </summary>
        protected int _width = texture?.Width ?? throw new ArgumentNullException(nameof(texture));

        /// <summary>
        /// The total height of the tileset texture in pixels.
        /// </summary>
        protected int _height = texture?.Height ?? throw new ArgumentNullException(nameof(texture));

        #endregion

        #region Fields - Tile

        /// <summary>
        /// The width of a single tile in pixels.
        /// </summary>
        protected int _tileWidth = tileWidth > 0 ? tileWidth : throw new ArgumentException("Tile width must be greater than zero.", nameof(tileWidth));

        /// <summary>
        /// The height of a single tile in pixels.
        /// </summary>
        protected int _tileHeight = tileHeight > 0 ? tileHeight : throw new ArgumentException("Tile height must be greater than zero.", nameof(tileHeight));

        /// <summary>
        /// The horizontal margin in pixels from the left edge of the texture before the first tile begins.
        /// Used to account for padding or borders in tileset textures.
        /// </summary>
        protected int _xMargin = xMargin >= 0 ? xMargin : throw new ArgumentException("X margin cannot be negative.", nameof(xMargin));

        /// <summary>
        /// The vertical margin in pixels from the top edge of the texture before the first tile begins.
        /// Used to account for padding or borders in tileset textures.
        /// </summary>
        protected int _yMargin = yMargin >= 0 ? yMargin : throw new ArgumentException("Y margin cannot be negative.", nameof(yMargin));

        /// <summary>
        /// The horizontal spacing in pixels between adjacent tiles.
        /// Accounts for gaps or separator lines in tileset textures.
        /// </summary>
        protected int _xSpacing = xSpacing >= 0 ? xSpacing : throw new ArgumentException("X spacing cannot be negative.", nameof(xSpacing));

        /// <summary>
        /// The vertical spacing in pixels between adjacent tiles.
        /// Accounts for gaps or separator lines in tileset textures.
        /// </summary>
        protected int _ySpacing = ySpacing >= 0 ? ySpacing : throw new ArgumentException("Y spacing cannot be negative.", nameof(ySpacing));

        #endregion

        #region Fields - Animation

        /// <summary>
        /// Dictionary mapping tile IDs to their animation data for O(1) lookup.
        /// Null if no animations are present in this tileset.
        /// </summary>
        protected Dictionary<int, TileAnimation> _animatedTiles;

        #endregion

        #region Properties - Texture

        /// <summary>
        /// Gets the texture containing the tileset image data.
        /// </summary>
        public Texture Texture => _texture;

        #endregion

        #region Properties - Dimensions

        /// <summary>
        /// Gets the total width of the tileset texture in pixels.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the total height of the tileset texture in pixels.
        /// </summary>
        public int Height => _height;

        #endregion

        #region Properties - Tile

        /// <summary>
        /// Gets the width of a single tile in pixels.
        /// </summary>
        public int TileWidth => _tileWidth;

        /// <summary>
        /// Gets the height of a single tile in pixels.
        /// </summary>
        public int TileHeight => _tileHeight;

        /// <summary>
        /// Gets the horizontal margin in pixels from the left edge of the texture.
        /// </summary>
        public int XMargin => _xMargin;

        /// <summary>
        /// Gets the vertical margin in pixels from the top edge of the texture.
        /// </summary>
        public int YMargin => _yMargin;

        /// <summary>
        /// Gets the horizontal spacing in pixels between adjacent tiles.
        /// </summary>
        public int XSpacing => _xSpacing;

        /// <summary>
        /// Gets the vertical spacing in pixels between adjacent tiles.
        /// </summary>
        public int YSpacing => _ySpacing;

        #endregion

        #region Properties - Animation

        /// <summary>
        /// Gets whether this tileset contains any animated tiles.
        /// </summary>
        public bool HasAnimations => _animatedTiles != null && _animatedTiles.Count > 0;

        #endregion

        #region Public Methods - Tile Calculations

        /// <summary>
        /// Calculates the maximum number of tiles that can fit horizontally in the tileset.
        /// Takes into account margins and spacing.
        /// </summary>
        /// <returns>The number of tiles per row.</returns>
        public int GetTilesPerRow()
        {
            int availableWidth = _width - (2 * _xMargin);
            return (availableWidth + _xSpacing) / (_tileWidth + _xSpacing);
        }

        /// <summary>
        /// Calculates the maximum number of tiles that can fit vertically in the tileset.
        /// Takes into account margins and spacing.
        /// </summary>
        /// <returns>The number of tiles per column.</returns>
        public int GetTilesPerColumn()
        {
            int availableHeight = _height - (2 * _yMargin);
            return (availableHeight + _ySpacing) / (_tileHeight + _ySpacing);
        }

        /// <summary>
        /// Calculates the total number of tiles in the tileset.
        /// </summary>
        /// <returns>The total tile count.</returns>
        public int GetTotalTileCount()
        {
            return GetTilesPerRow() * GetTilesPerColumn();
        }

        #endregion

        #region Public Methods - Animation

        /// <summary>
        /// Sets the animation data for the tileset from Tiled tileset data.
        /// Creates an optimized lookup dictionary for animated tiles.
        /// </summary>
        /// <param name="tileData">The tile data containing animation information.</param>
        public void SetAnimations(TiledTileData[] tileData)
        {
            if (tileData == null || tileData.Length == 0)
            {
                _animatedTiles = null;
                return;
            }

            _animatedTiles = new Dictionary<int, TileAnimation>();

            foreach (var tile in tileData)
            {
                if (tile.Animation != null && tile.Animation.Length > 0)
                {
                    _animatedTiles[tile.Id] = new TileAnimation(tile.Animation);
                }
            }

            // Free memory if no animations were actually found
            if (_animatedTiles.Count == 0)
            {
                _animatedTiles = null;
            }
        }

        /// <summary>
        /// Checks if a specific tile ID is animated.
        /// </summary>
        /// <param name="tileId">The local tile ID to check.</param>
        /// <returns>True if the tile has animation data; otherwise, false.</returns>
        public bool IsTileAnimated(int tileId)
        {
            return _animatedTiles != null && _animatedTiles.ContainsKey(tileId);
        }

        /// <summary>
        /// Gets the current frame tile ID for an animated tile based on elapsed time.
        /// </summary>
        /// <param name="tileId">The local tile ID.</param>
        /// <param name="elapsedTimeMs">The total elapsed time in milliseconds.</param>
        /// <returns>The tile ID to render for the current frame, or the original tile ID if not animated.</returns>
        public int GetAnimatedTileId(int tileId, double elapsedTimeMs)
        {
            if (_animatedTiles != null && _animatedTiles.TryGetValue(tileId, out var animation))
            {
                return animation.GetCurrentFrameTileId(elapsedTimeMs);
            }

            return tileId;
        }

        #endregion
    }

    /// <summary>
    /// Represents animation data for a single tile.
    /// Manages frame sequences and timing for animated tiles.
    /// Immutable after construction for thread safety and optimization.
    /// </summary>
    public class TileAnimation
    {
        private readonly TiledAnimationFrame[] _frames;
        private readonly int _totalDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileAnimation"/> class.
        /// </summary>
        /// <param name="frames">The animation frames from Tiled data.</param>
        /// <exception cref="ArgumentNullException">Thrown when frames is null.</exception>
        /// <exception cref="ArgumentException">Thrown when frames array is empty.</exception>
        public TileAnimation(TiledAnimationFrame[] frames)
        {
            _frames = frames ?? throw new ArgumentNullException(nameof(frames));

            if (_frames.Length == 0)
            {
                throw new ArgumentException("Animation must have at least one frame.", nameof(frames));
            }

            // Precalculate total duration for modulo operation
            _totalDuration = 0;
            foreach (var frame in _frames)
            {
                _totalDuration += frame.Duration;
            }
        }

        /// <summary>
        /// Gets the total duration of the animation loop in milliseconds.
        /// </summary>
        public int TotalDuration => _totalDuration;

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount => _frames.Length;

        /// <summary>
        /// Gets the tile ID to display for the current frame based on elapsed time.
        /// Uses modulo for seamless looping.
        /// </summary>
        /// <param name="elapsedTimeMs">The total elapsed time in milliseconds.</param>
        /// <returns>The tile ID for the current frame.</returns>
        public int GetCurrentFrameTileId(double elapsedTimeMs)
        {
            // Loop the animation using modulo
            int currentTime = (int)(elapsedTimeMs % _totalDuration);
            int accumulatedTime = 0;

            // Find the current frame
            for (int i = 0; i < _frames.Length; i++)
            {
                accumulatedTime += _frames[i].Duration;
                if (currentTime < accumulatedTime)
                {
                    return _frames[i].TileId;
                }
            }

            // Fallback to last frame (should not happen with correct logic)
            return _frames[_frames.Length - 1].TileId;
        }
    }
}
