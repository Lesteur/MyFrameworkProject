using System.Text.Json.Serialization;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Represents a single animation frame for a tile.
    /// </summary>
    public class TiledAnimationFrame
    {
        /// <summary>
        /// Gets or sets the duration of this frame in milliseconds.
        /// </summary>
        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the tile ID to display for this frame.
        /// </summary>
        [JsonPropertyName("tileid")]
        public int TileId { get; set; }
    }

    /// <summary>
    /// Represents tile-specific data including animation information.
    /// </summary>
    public class TiledTileData
    {
        /// <summary>
        /// Gets or sets the local tile ID.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the animation frames for this tile.
        /// Null or empty if the tile is not animated.
        /// </summary>
        [JsonPropertyName("animation")]
        public TiledAnimationFrame[] Animation { get; set; }
    }

    /// <summary>
    /// Represents Tiled tileset data for the Content Pipeline.
    /// This class is serializable and will be converted to XNB format.
    /// Supports both TSX and JSON tileset formats.
    /// </summary>
    public class TiledTilesetContent
    {
        /// <summary>
        /// Gets or sets the name of the tileset.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the width of a single tile in pixels.
        /// </summary>
        [JsonPropertyName("tilewidth")]
        public int TileWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of a single tile in pixels.
        /// </summary>
        [JsonPropertyName("tileheight")]
        public int TileHeight { get; set; }

        /// <summary>
        /// Gets or sets the margin around the tileset image in pixels.
        /// </summary>
        [JsonPropertyName("margin")]
        public int Margin { get; set; }

        /// <summary>
        /// Gets or sets the spacing between tiles in pixels.
        /// </summary>
        [JsonPropertyName("spacing")]
        public int Spacing { get; set; }

        /// <summary>
        /// Gets or sets the total number of tiles in the tileset.
        /// </summary>
        [JsonPropertyName("tilecount")]
        public int TileCount { get; set; }

        /// <summary>
        /// Gets or sets the number of tile columns in the tileset image.
        /// </summary>
        [JsonPropertyName("columns")]
        public int Columns { get; set; }

        /// <summary>
        /// Gets or sets the source path to the tileset image file.
        /// </summary>
        [JsonPropertyName("image")]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the width of the tileset image in pixels.
        /// </summary>
        [JsonPropertyName("imagewidth")]
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the tileset image in pixels.
        /// </summary>
        [JsonPropertyName("imageheight")]
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the array of tile-specific data including animations.
        /// Null or empty if no tiles have special properties.
        /// </summary>
        [JsonPropertyName("tiles")]
        public TiledTileData[] Tiles { get; set; }
    }
}