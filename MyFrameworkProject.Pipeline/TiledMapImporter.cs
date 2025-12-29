using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Content importer for Tiled JSON map files (.json, .tmj).
    /// Loads and converts Tiled map data into the Content Pipeline format.
    /// </summary>
    [ContentImporter(".json", ".tmj", DisplayName = "Tiled Map Importer", DefaultProcessor = "TiledMapProcessor")]
    public class TiledMapImporter : ContentImporter<TiledMapContent>
    {
        /// <summary>
        /// Shared JSON serializer options for deserializing Tiled files.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        /// <summary>
        /// Imports a Tiled map file into the Content Pipeline.
        /// </summary>
        /// <param name="filename">The path to the Tiled map file.</param>
        /// <param name="context">The importer context for logging and dependency tracking.</param>
        /// <returns>A TiledMapContent instance containing the imported map data.</returns>
        public override TiledMapContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"Importing Tiled map: {filename}");

            string jsonText = File.ReadAllText(filename);
            var mapData = JsonSerializer.Deserialize<TiledMapContent>(jsonText, JsonOptions) ?? throw new InvalidContentException("Failed to deserialize Tiled map data.");

            // Convert property values from objects to strings
            NormalizePropertyValues(mapData);

            context.Logger.LogMessage($"Successfully imported Tiled map with {mapData.Layers.Count} layers and {mapData.Tilesets.Count} tilesets");
            return mapData;
        }

        /// <summary>
        /// Normalizes property values by converting object values to strings.
        /// This ensures consistent serialization in the XNB format.
        /// </summary>
        /// <param name="mapData">The map data to normalize.</param>
        private static void NormalizePropertyValues(TiledMapContent mapData)
        {
            if (mapData.Layers == null)
            {
                return;
            }

            foreach (var layer in mapData.Layers)
            {
                if (layer.Objects == null)
                {
                    continue;
                }

                foreach (var obj in layer.Objects)
                {
                    if (obj.Properties == null)
                    {
                        continue;
                    }

                    foreach (var prop in obj.Properties)
                    {
                        if (prop.Value != null && prop.Value is not string)
                        {
                            prop.Value = prop.Value.ToString();
                        }
                    }
                }
            }
        }
    }
}