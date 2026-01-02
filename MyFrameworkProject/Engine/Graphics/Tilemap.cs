using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFrameworkProject.Engine.Serialization;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a tilemap instance based on a tileset, allowing for grid-based tile rendering.
    /// Provides functionality for managing tile data, positioning, and rendering properties.
    /// </summary>
    public class Tilemap
    {
        #region Static Fields

        /// <summary>
        /// Global counter used to generate unique identifiers for each tilemap instance.
        /// </summary>
        private static int _counter = 0;

        #endregion

        #region Fields - Identity

        /// <summary>
        /// Unique identifier for this tilemap instance.
        /// </summary>
        private readonly uint _id;

        #endregion

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
        private int[,] _tileData;

        #endregion

        #region Fields - Transform

        /// <summary>
        /// The X-coordinate of the tilemap's position.
        /// </summary>
        protected int _x = 0;

        /// <summary>
        /// The Y-coordinate of the tilemap's position.
        /// </summary>
        protected int _y = 0;

        #endregion

        #region Fields - Rendering

        /// <summary>
        /// The layer depth used for sorting during rendering (0.0 = front, 1.0 = back).
        /// </summary>
        private float _layerDepth = 0f;

        /// <summary>
        /// The color tint applied to the tilemap when rendering.
        /// </summary>
        private Color _color = Color.White;

        /// <summary>
        /// Indicates whether the tilemap should be rendered.
        /// </summary>
        private bool _visible = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tilemap"/> class with a unique identifier.
        /// </summary>
        public Tilemap()
        {
            _id = (uint)++_counter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tilemap"/> class with the specified tileset and grid dimensions.
        /// </summary>
        /// <param name="tileset">The tileset to use for this tilemap.</param>
        /// <param name="gridWidth">The width of the tilemap grid in tiles.</param>
        /// <param name="gridHeight">The height of the tilemap grid in tiles.</param>
        public Tilemap(Tileset tileset, int gridWidth, int gridHeight) : this()
        {
            _tileset = tileset;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _tileData = new int[gridWidth, gridHeight];

            // Initialize all tiles to -1 (empty)
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    _tileData[x, y] = -1;
                }
            }
        }

        #endregion

        #region Public Methods - Tileset

        /// <summary>
        /// Sets the tileset for this tilemap.
        /// </summary>
        /// <param name="tileset">The new tileset to assign.</param>
        public void SetTileset(Tileset tileset)
        {
            _tileset = tileset;
        }

        /// <summary>
        /// Gets the tileset associated with this tilemap.
        /// </summary>
        /// <returns>The current tileset.</returns>
        public Tileset Tileset => _tileset;

        #endregion

        #region Public Methods - Tile Data

        /// <summary>
        /// Sets a tile at the specified grid coordinates.
        /// </summary>
        /// <param name="gridX">The X-coordinate in the grid.</param>
        /// <param name="gridY">The Y-coordinate in the grid.</param>
        /// <param name="tileIndex">The tile index from the tileset. Use -1 for empty tiles.</param>
        public void SetTile(int gridX, int gridY, int tileIndex)
        {
            if (gridX >= 0 && gridX < _gridWidth && gridY >= 0 && gridY < _gridHeight)
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
            if (gridX >= 0 && gridX < _gridWidth && gridY >= 0 && gridY < _gridHeight)
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
        /// Clears all tiles in the tilemap.
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
        /// </summary>
        /// <param name="startX">The starting X-coordinate in the grid.</param>
        /// <param name="startY">The starting Y-coordinate in the grid.</param>
        /// <param name="width">The width of the region in tiles.</param>
        /// <param name="height">The height of the region in tiles.</param>
        /// <param name="tileIndex">The tile index to fill with.</param>
        public void FillRegion(int startX, int startY, int width, int height, int tileIndex)
        {
            for (int x = startX; x < startX + width && x < _gridWidth; x++)
            {
                for (int y = startY; y < startY + height && y < _gridHeight; y++)
                {
                    if (x >= 0 && y >= 0)
                    {
                        _tileData[x, y] = tileIndex;
                    }
                }
            }
        }

        #endregion

        #region Public Methods - Grid

        /// <summary>
        /// Gets the width of the tilemap grid in tiles.
        /// </summary>
        /// <returns>The grid width.</returns>
        public int GridWidth => _gridWidth;

        /// <summary>
        /// Gets the height of the tilemap grid in tiles.
        /// </summary>
        public int GridHeight => _gridHeight;

        /// <summary>
        /// Calculates the source rectangle for a specific tile index in the tileset.
        /// </summary>
        /// <param name="tileIndex">The tile index.</param>
        /// <returns>A <see cref="Rectangle"/> representing the source area in the tileset texture.</returns>
        public Rectangle GetTileSourceRectangle(int tileIndex)
        {
            if (_tileset == null || tileIndex < 0)
                return Rectangle.Empty;

            // Calculate the number of tiles per row, accounting for margins and spacing
            int availableWidth = _tileset.Width - (2 * _tileset.XMargin);
            int tilesPerRow = (availableWidth + _tileset.XSpacing) / (_tileset.TileWidth + _tileset.XSpacing);
            
            // Calculate row and column from tile index
            int row = tileIndex / tilesPerRow;
            int col = tileIndex % tilesPerRow;

            // Calculate the pixel position of the tile in the texture
            int x = _tileset.XMargin + col * (_tileset.TileWidth + _tileset.XSpacing);
            int y = _tileset.YMargin + row * (_tileset.TileHeight + _tileset.YSpacing);

            return new Rectangle(x, y, _tileset.TileWidth, _tileset.TileHeight);
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    int tileIndex = GetTile(x, y);
                    if (tileIndex < 0)
                        continue;

                    Rectangle sourceRect = GetTileSourceRectangle(tileIndex);
                    Vector2 position = new(
                        _x + x * sourceRect.Width,
                        _y + y * sourceRect.Height
                    );

                    spriteBatch.Draw(
                        _tileset.Texture.NativeTexture,
                        position,
                        sourceRect,
                        _color,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        _layerDepth
                    );
                }
            }
        }

        #region Public Methods - Position

        /// <summary>
        /// Sets the X-coordinate of the tilemap's position.
        /// </summary>
        /// <param name="x">The new X-coordinate.</param>
        public void SetX(int x) => _x = x;

        /// <summary>
        /// Sets the Y-coordinate of the tilemap's position.
        /// </summary>
        /// <param name="y">The new Y-coordinate.</param>
        public void SetY(int y) => _y = y;

        /// <summary>
        /// Sets the position of the tilemap.
        /// </summary>
        /// <param name="x">The new X-coordinate.</param>
        /// <param name="y">The new Y-coordinate.</param>
        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets the X-coordinate of the tilemap's position.
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Gets the Y-coordinate of the tilemap's position.
        /// </summary>
        public int Y => _y;

        #endregion

        #region Public Methods - Rendering

        /// <summary>
        /// Sets the layer depth for rendering depth sorting.
        /// </summary>
        /// <param name="layerDepth">The new layer depth (0.0 = front, 1.0 = back).</param>
        public void SetLayerDepth(float layerDepth) => _layerDepth = layerDepth;

        /// <summary>
        /// Sets the color tint applied to the tilemap when rendering.
        /// </summary>
        /// <param name="color">The color tint to apply.</param>
        public void SetColor(Color color)
        {
            _color = color;
        }

        /// <summary>
        /// Sets the visibility of the tilemap.
        /// </summary>
        /// <param name="visible">True to make the tilemap visible, false to hide it.</param>
        public void SetVisible(bool visible)
        {
            _visible = visible;
        }

        /// <summary>
        /// Gets the layer depth used for rendering depth sorting.
        /// </summary>
        public float LayerDepth => _layerDepth;

        /// <summary>
        /// Gets the color tint applied to the tilemap.
        /// </summary>
        public Color Color => _color;

        /// <summary>
        /// Gets the visibility state of the tilemap.
        /// </summary>
        public bool Visible => _visible;

        #endregion

        #region Public Methods - Identity

        /// <summary>
        /// Gets the unique identifier of this tilemap.
        /// </summary>
        public uint ID => _id;

        #endregion
    }
}