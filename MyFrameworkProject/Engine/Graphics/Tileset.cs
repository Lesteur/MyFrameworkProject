using System;

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
    }
}
