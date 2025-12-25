namespace MyFrameworkProject.Engine.Graphics
{
    public class Tileset(Texture texture, int tileWidth, int tileHeight, int xMargin = 0, int yMargin = 0, int xSpacing = 0, int ySpacing = 0)
    {
        #region Fields - Texture

        /// <summary>
        /// The texture containing the sprite image data.
        /// </summary>
        protected Texture _texture = texture;

        #endregion

        #region Fields - Dimensions

        /// <summary>
        /// The total width of the sprite texture in pixels.
        /// </summary>
        protected int _width = texture.GetWidth();

        /// <summary>
        /// The total height of the sprite texture in pixels.
        /// </summary>
        protected int _height = texture.GetHeight();

        #endregion

        #region Fields - Tile

        /// <summary>
        /// The width of a single tile in pixels.
        /// </summary>
        protected int _tileWidth = tileWidth;

        /// <summary>
        /// The height of a single tile in pixels.
        /// </summary>
        protected int _tileHeight = tileHeight;

        /// </summary>
        /// The X-coordinate of the sprite's origin point
        /// </summary>
        protected int _xMargin = xMargin;

        /// <summary>
        /// The Y-coordinate of the sprite's origin point
        /// </summary>
        protected int _yMargin = yMargin;

        /// <summary>
        /// The X-spacing between tiles in pixels.
        /// </summary>
        protected int _xSpacing = xSpacing;

        /// <summary>
        /// The Y-spacing between tiles in pixels.
        /// </summary>
        protected int _ySpacing = ySpacing;

        #endregion

        public Texture GetTexture() => _texture;

        public int Width => _width;

        public int Height => _height;

        public int TileWidth => _tileWidth;

        public int TileHeight => _tileHeight;

        public int XMargin => _xMargin;

        public int YMargin => _yMargin;

        public int XSpacing => _xSpacing;

        public int YSpacing => _ySpacing;
    }
}
