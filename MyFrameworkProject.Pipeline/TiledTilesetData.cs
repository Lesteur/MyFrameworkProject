using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Classe pour désérialiser le JSON d'un tileset Tiled (TSX/JSON).
    /// Correspond exactement au format JSON de Tiled pour les tilesets.
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