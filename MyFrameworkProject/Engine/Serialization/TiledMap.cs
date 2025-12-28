using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Represents a Tiled map (TMJ format).
    /// </summary>
    public class TiledMap
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("tilewidth")]
        public int TileWidth { get; set; }

        [JsonPropertyName("tileheight")]
        public int TileHeight { get; set; }

        [JsonPropertyName("layers")]
        public List<TiledLayer> Layers { get; set; }

        [JsonPropertyName("tilesets")]
        public List<TiledTilesetReference> Tilesets { get; set; }

        [JsonPropertyName("orientation")]
        public string Orientation { get; set; }

        [JsonPropertyName("renderorder")]
        public string RenderOrder { get; set; }
    }

    /// <summary>
    /// Represents a layer in a Tiled map.
    /// </summary>
    public class TiledLayer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("opacity")]
        public float Opacity { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("data")]
        public List<int> Data { get; set; }

        [JsonPropertyName("objects")]
        public List<TiledObject> Objects { get; set; }
    }

    /// <summary>
    /// Represents an object in a Tiled object layer.
    /// </summary>
    public class TiledObject
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("width")]
        public float Width { get; set; }

        [JsonPropertyName("height")]
        public float Height { get; set; }

        [JsonPropertyName("rotation")]
        public float Rotation { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("properties")]
        public List<TiledProperty> Properties { get; set; }
    }

    /// <summary>
    /// Represents a custom property in Tiled.
    /// </summary>
    public class TiledProperty
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    /// <summary>
    /// Represents a tileset reference in a Tiled map.
    /// </summary>
    public class TiledTilesetReference
    {
        [JsonPropertyName("firstgid")]
        public int FirstGid { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    /// <summary>
    /// Represents a Tiled tileset (TSX format).
    /// </summary>
    public class TiledTileset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("tilewidth")]
        public int TileWidth { get; set; }

        [JsonPropertyName("tileheight")]
        public int TileHeight { get; set; }

        [JsonPropertyName("margin")]
        public int Margin { get; set; }

        [JsonPropertyName("spacing")]
        public int Spacing { get; set; }

        [JsonPropertyName("tilecount")]
        public int TileCount { get; set; }

        [JsonPropertyName("columns")]
        public int Columns { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("imagewidth")]
        public int ImageWidth { get; set; }

        [JsonPropertyName("imageheight")]
        public int ImageHeight { get; set; }
    }
}