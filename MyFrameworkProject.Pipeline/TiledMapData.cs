using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Classes pour désérialiser le JSON Tiled.
    /// Ces classes correspondent exactement au format JSON de Tiled.
    /// </summary>
    internal class TiledMapData
    {
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("tilewidth")]
        public int TileWidth { get; set; }

        [JsonProperty("tileheight")]
        public int TileHeight { get; set; }

        [JsonProperty("orientation")]
        public string Orientation { get; set; }

        [JsonProperty("renderorder")]
        public string RenderOrder { get; set; }

        [JsonProperty("layers")]
        public List<TiledLayerData> Layers { get; set; }

        [JsonProperty("tilesets")]
        public List<TiledTilesetRefData> Tilesets { get; set; }
    }

    internal class TiledLayerData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("opacity")]
        public float Opacity { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("data")]
        public List<int> Data { get; set; }

        [JsonProperty("objects")]
        public List<TiledObjectData> Objects { get; set; }
    }

    internal class TiledObjectData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("height")]
        public float Height { get; set; }

        [JsonProperty("rotation")]
        public float Rotation { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("properties")]
        public List<TiledPropertyData> Properties { get; set; }
    }

    internal class TiledPropertyData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }

    internal class TiledTilesetRefData
    {
        [JsonProperty("firstgid")]
        public int FirstGid { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}