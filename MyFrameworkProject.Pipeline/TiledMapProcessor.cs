using Microsoft.Xna.Framework.Content.Pipeline;
using System.Linq;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Content processor for Tiled map data.
    /// Validates and optimizes imported map data before serialization.
    /// </summary>
    [ContentProcessor(DisplayName = "Tiled Map Processor")]
    public class TiledMapProcessor : ContentProcessor<TiledMapContent, TiledMapContent>
    {
        /// <summary>
        /// Processes the imported Tiled map data.
        /// Performs validation and optional optimizations.
        /// </summary>
        /// <param name="input">The imported map data.</param>
        /// <param name="context">The processor context for logging.</param>
        /// <returns>The processed map data ready for serialization.</returns>
        public override TiledMapContent Process(TiledMapContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"Processing Tiled map: {input.Width}x{input.Height} tiles ({input.TileWidth}x{input.TileHeight} px)");

            ValidateMapData(input, context);
            OptimizeMapData(input, context);

            context.Logger.LogMessage("Tiled map processed successfully");
            return input;
        }

        /// <summary>
        /// Validates the map data for common issues.
        /// </summary>
        /// <param name="input">The map data to validate.</param>
        /// <param name="context">The processor context for logging.</param>
        private static void ValidateMapData(TiledMapContent input, ContentProcessorContext context)
        {
            if (input.Layers == null || input.Layers.Count == 0)
            {
                context.Logger.LogWarning(null, null, "Tiled map has no layers");
            }

            if (input.Tilesets == null || input.Tilesets.Count == 0)
            {
                context.Logger.LogWarning(null, null, "Tiled map has no tilesets");
            }

            // Validate layer dimensions match map dimensions
            foreach (var layer in input.Layers.Where(l => l.Type == "tilelayer"))
            {
                if (layer.Width != input.Width || layer.Height != input.Height)
                {
                    context.Logger.LogWarning(
                        null,
                        null,
                        $"Layer '{layer.Name}' dimensions ({layer.Width}x{layer.Height}) differ from map dimensions ({input.Width}x{input.Height})"
                    );
                }
            }
        }

        /// <summary>
        /// Performs optional optimizations on the map data.
        /// </summary>
        /// <param name="input">The map data to optimize.</param>
        /// <param name="context">The processor context for logging.</param>
        private static void OptimizeMapData(TiledMapContent input, ContentProcessorContext context)
        {
            // Set default values to reduce serialization overhead
            input.Orientation ??= "orthogonal";
            input.RenderOrder ??= "right-down";

            int emptyLayersRemoved = 0;

            // Remove invisible layers with no data (optional optimization)
            // Uncomment if you want to remove invisible empty layers
            /*
            input.Layers.RemoveAll(layer =>
            {
                bool isEmpty = !layer.Visible &&
                              (layer.Data == null || layer.Data.Count == 0) &&
                              (layer.Objects == null || layer.Objects.Count == 0);
                
                if (isEmpty)
                {
                    emptyLayersRemoved++;
                }
                
                return isEmpty;
            });
            */

            if (emptyLayersRemoved > 0)
            {
                context.Logger.LogMessage($"Removed {emptyLayersRemoved} empty invisible layer(s)");
            }
        }
    }
}