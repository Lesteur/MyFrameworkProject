using Microsoft.Xna.Framework.Content.Pipeline;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Traite les données de tileset Tiled importées pour validation et optimisation.
    /// </summary>
    [ContentProcessor(DisplayName = "Tiled Tileset Processor")]
    public class TiledTilesetProcessor : ContentProcessor<TiledTilesetContent, TiledTilesetContent>
    {
        public override TiledTilesetContent Process(TiledTilesetContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"Processing Tiled tileset: {input.Name}");

            // Validation des données
            if (input.TileWidth <= 0 || input.TileHeight <= 0)
            {
                throw new InvalidContentException("Tile dimensions must be greater than zero");
            }

            if (string.IsNullOrEmpty(input.Image))
            {
                throw new InvalidContentException("Tileset must reference an image");
            }

            if (input.TileCount <= 0)
            {
                context.Logger.LogWarning(null, null, "Tileset has no tiles");
            }

            if (input.Columns <= 0)
            {
                throw new InvalidContentException("Tileset must have at least one column");
            }

            // Validation de la cohérence
            int expectedRows = (input.TileCount + input.Columns - 1) / input.Columns;
            int calculatedImageWidth = input.Margin * 2 + input.Columns * input.TileWidth + (input.Columns - 1) * input.Spacing;
            int calculatedImageHeight = input.Margin * 2 + expectedRows * input.TileHeight + (expectedRows - 1) * input.Spacing;

            if (input.ImageWidth > 0 && input.ImageWidth < calculatedImageWidth)
            {
                context.Logger.LogWarning(null, null, 
                    $"Image width ({input.ImageWidth}) is smaller than calculated width ({calculatedImageWidth})");
            }

            if (input.ImageHeight > 0 && input.ImageHeight < calculatedImageHeight)
            {
                context.Logger.LogWarning(null, null, 
                    $"Image height ({input.ImageHeight}) is smaller than calculated height ({calculatedImageHeight})");
            }

            context.Logger.LogMessage($"Tileset processed successfully: {input.TileCount} tiles, {input.Columns} columns");
            return input;
        }
    }
}