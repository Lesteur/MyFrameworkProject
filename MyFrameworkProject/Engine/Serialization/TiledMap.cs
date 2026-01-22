using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Represents a Tiled map loaded from TMJ format.
    /// Contains all map data including dimensions, layers, and tileset references.
    /// </summary>
    public class TiledMap
    {
        /// <summary>
        /// Gets or sets the width of the map in tiles.
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the map in tiles.
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

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
        /// Gets or sets the collection of layers in the map.
        /// </summary>
        [JsonPropertyName("layers")]
        public List<TiledLayer> Layers { get; set; }

        /// <summary>
        /// Gets or sets the collection of tileset references used by this map.
        /// </summary>
        [JsonPropertyName("tilesets")]
        public List<TiledTilesetReference> Tilesets { get; set; }

        /// <summary>
        /// Gets or sets the map orientation (e.g., "orthogonal", "isometric").
        /// </summary>
        [JsonPropertyName("orientation")]
        public string Orientation { get; set; }

        /// <summary>
        /// Gets or sets the render order (e.g., "right-down", "right-up").
        /// </summary>
        [JsonPropertyName("renderorder")]
        public string RenderOrder { get; set; }
    }

    /// <summary>
    /// Represents a layer in a Tiled map.
    /// Layers can contain tile data, objects, or other elements.
    /// </summary>
    public class TiledLayer
    {
        /// <summary>
        /// Gets or sets the unique identifier for this layer.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the layer (e.g., "tilelayer", "objectgroup").
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the layer is visible.
        /// </summary>
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the opacity of the layer (0.0 to 1.0).
        /// </summary>
        [JsonPropertyName("opacity")]
        public float Opacity { get; set; }

        /// <summary>
        /// Gets or sets the width of the layer in tiles.
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the layer in tiles.
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the tile data array for tile layers.
        /// Each value represents a global tile ID (GID).
        /// </summary>
        [JsonPropertyName("data")]
        public List<int> Data { get; set; }

        /// <summary>
        /// Gets or sets the collection of objects for object layers.
        /// </summary>
        [JsonPropertyName("objects")]
        public List<TiledObject> Objects { get; set; }
    }

    /// <summary>
    /// Represents an object in a Tiled object layer.
    /// Objects can represent entities, collision areas, spawn points, etc.
    /// </summary>
    public class TiledObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for this object.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the object (user-defined).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate of the object in pixels.
        /// </summary>
        [JsonPropertyName("x")]
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the object in pixels.
        /// </summary>
        [JsonPropertyName("y")]
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the object in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the object in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the object in degrees (clockwise).
        /// </summary>
        [JsonPropertyName("rotation")]
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is visible.
        /// </summary>
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the collection of custom properties associated with this object.
        /// </summary>
        [JsonPropertyName("properties")]
        public List<TiledProperty> Properties { get; set; }
    }

    /// <summary>
    /// Represents a custom property in Tiled.
    /// Properties provide additional metadata for maps, layers, objects, and tiles.
    /// </summary>
    public class TiledProperty
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data type of the property (e.g., "string", "int", "bool").
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    /// <summary>
    /// Represents a tileset reference in a Tiled map.
    /// References an external tileset file (TSX format).
    /// </summary>
    public class TiledTilesetReference
    {
        /// <summary>
        /// Gets or sets the first global tile ID (GID) for this tileset.
        /// This is the starting index for tiles from this tileset.
        /// </summary>
        [JsonPropertyName("firstgid")]
        public int FirstGid { get; set; }

        /// <summary>
        /// Gets or sets the source path to the external tileset file.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

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
    /// Represents a Tiled tileset loaded from TSX format.
    /// Contains tileset metadata and image information.
    /// </summary>
    public class TiledTileset
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