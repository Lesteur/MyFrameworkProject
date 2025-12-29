using System.Text.Json.Serialization;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Class for deserializing Tiled tileset JSON (TSX/JSON format).
    /// Matches exactly the Tiled tileset JSON structure.
    /// </summary>
    internal class TiledTilesetData
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