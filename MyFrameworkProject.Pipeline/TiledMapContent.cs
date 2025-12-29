using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Represents Tiled map data for the Content Pipeline.
    /// This class is serializable and will be converted to XNB format.
    /// </summary>
    public class TiledMapContent
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
        /// Gets or sets the map orientation (e.g., "orthogonal", "isometric").
        /// </summary>
        [JsonPropertyName("orientation")]
        public string Orientation { get; set; }

        /// <summary>
        /// Gets or sets the render order (e.g., "right-down", "right-up").
        /// </summary>
        [JsonPropertyName("renderorder")]
        public string RenderOrder { get; set; }

        /// <summary>
        /// Gets or sets the collection of layers in the map.
        /// </summary>
        [JsonPropertyName("layers")]
        public List<TiledLayerContent> Layers { get; set; } = new List<TiledLayerContent>();

        /// <summary>
        /// Gets or sets the collection of tileset references.
        /// </summary>
        [JsonPropertyName("tilesets")]
        public List<TiledTilesetRefContent> Tilesets { get; set; } = new List<TiledTilesetRefContent>();
    }

    /// <summary>
    /// Represents a layer in a Tiled map for the Content Pipeline.
    /// Layers can contain tile data or objects.
    /// </summary>
    public class TiledLayerContent
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
        public List<TiledObjectContent> Objects { get; set; }
    }

    /// <summary>
    /// Represents an object in a Tiled object layer for the Content Pipeline.
    /// </summary>
    public class TiledObjectContent
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
        public List<TiledPropertyContent> Properties { get; set; }
    }

    /// <summary>
    /// Represents a custom property in Tiled for the Content Pipeline.
    /// </summary>
    public class TiledPropertyContent
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
        /// Gets or sets the value of the property as a string.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    /// <summary>
    /// Represents a tileset reference in a Tiled map for the Content Pipeline.
    /// </summary>
    public class TiledTilesetRefContent
    {
        /// <summary>
        /// Gets or sets the first global tile ID (GID) for this tileset.
        /// </summary>
        [JsonPropertyName("firstgid")]
        public int FirstGid { get; set; }

        /// <summary>
        /// Gets or sets the source path to the external tileset file.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
}