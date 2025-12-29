using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Classes pour désérialiser le JSON Tiled.
    /// Ces classes correspondent exactement au format JSON de Tiled.
    /// </summary>
    internal class TiledMapData
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("tilewidth")]
        public int TileWidth { get; set; }

        [JsonPropertyName("tileheight")]
        public int TileHeight { get; set; }

        [JsonPropertyName("orientation")]
        public string Orientation { get; set; }

        [JsonPropertyName("renderorder")]
        public string RenderOrder { get; set; }

        [JsonPropertyName("layers")]
        public List<TiledLayerData> Layers { get; set; }

        [JsonPropertyName("tilesets")]
        public List<TiledTilesetRefData> Tilesets { get; set; }
    }

    internal class TiledLayerData
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
        public List<TiledObjectData> Objects { get; set; }
    }

    internal class TiledObjectData
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
        public List<TiledPropertyData> Properties { get; set; }
    }

    internal class TiledPropertyData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    internal class TiledTilesetRefData
    {
        [JsonPropertyName("firstgid")]
        public int FirstGid { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
}