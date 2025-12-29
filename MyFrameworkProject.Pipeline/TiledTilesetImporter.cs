using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using System.Text.Json;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Content importer for Tiled tileset files (.tsx, .json).
    /// Loads and converts Tiled tileset data into the Content Pipeline format.
    /// </summary>
    [ContentImporter(".tsx", ".json", DisplayName = "Tiled Tileset Importer", DefaultProcessor = "TiledTilesetProcessor")]
    public class TiledTilesetImporter : ContentImporter<TiledTilesetContent>
    {
        /// <summary>
        /// Shared JSON serializer options for deserializing Tiled tileset files.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        /// <summary>
        /// Imports a Tiled tileset file into the Content Pipeline.
        /// </summary>
        /// <param name="filename">The path to the Tiled tileset file.</param>
        /// <param name="context">The importer context for logging and dependency tracking.</param>
        /// <returns>A TiledTilesetContent instance containing the imported tileset data.</returns>
        public override TiledTilesetContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"Importing Tiled tileset: {filename}");

            string jsonText = File.ReadAllText(filename);
            var tilesetData = JsonSerializer.Deserialize<TiledTilesetContent>(jsonText, JsonOptions) ?? throw new InvalidContentException("Failed to deserialize Tiled tileset data.");
            context.Logger.LogMessage(
                $"Successfully imported Tiled tileset: {tilesetData.Name} " +
                $"({tilesetData.TileWidth}x{tilesetData.TileHeight} px, {tilesetData.TileCount} tiles)"
            );

            return tilesetData;
        }
    }
}