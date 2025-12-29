using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using System.Text.Json;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Importe les fichiers de tileset Tiled (TSX/JSON) dans le Content Pipeline.
    /// </summary>
    [ContentImporter(".tsx", ".json", DisplayName = "Tiled Tileset Importer", DefaultProcessor = "TiledTilesetProcessor")]
    public class TiledTilesetImporter : ContentImporter<TiledTilesetContent>
    {
        public override TiledTilesetContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"Importing Tiled tileset: {filename}");

            // Lire le fichier JSON
            string jsonText = File.ReadAllText(filename);

            // Options pour la désérialisation
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var tiledData = JsonSerializer.Deserialize<TiledTilesetData>(jsonText, options);

            if (tiledData == null)
            {
                context.Logger.LogWarning(null, null, "Failed to deserialize Tiled tileset");
                return null;
            }

            // Convertir en TiledTilesetContent
            var content = new TiledTilesetContent
            {
                Name = tiledData.Name,
                TileWidth = tiledData.TileWidth,
                TileHeight = tiledData.TileHeight,
                Margin = tiledData.Margin,
                Spacing = tiledData.Spacing,
                TileCount = tiledData.TileCount,
                Columns = tiledData.Columns,
                Image = tiledData.Image,
                ImageWidth = tiledData.ImageWidth,
                ImageHeight = tiledData.ImageHeight
            };

            context.Logger.LogMessage($"Successfully imported Tiled tileset: {tiledData.Name} ({tiledData.TileWidth}x{tiledData.TileHeight})");
            return content;
        }
    }
}