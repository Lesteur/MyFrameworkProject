using Microsoft.Xna.Framework.Content.Pipeline;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Traite les données Tiled importées pour optimisation et validation.
    /// </summary>
    [ContentProcessor(DisplayName = "Tiled Map Processor")]
    public class TiledMapProcessor : ContentProcessor<TiledMapContent, TiledMapContent>
    {
        public override TiledMapContent Process(TiledMapContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"Processing Tiled map: {input.Width}x{input.Height} tiles");

            // Validation des données
            if (input.Layers == null || input.Layers.Count == 0)
            {
                context.Logger.LogWarning(null, null, "Tiled map has no layers");
            }

            // Validation des tilesets
            if (input.Tilesets == null || input.Tilesets.Count == 0)
            {
                context.Logger.LogWarning(null, null, "Tiled map has no tilesets");
            }

            // Vous pouvez ajouter ici des optimisations ou transformations
            // Par exemple : compression des données de tiles, pré-calcul, etc.

            context.Logger.LogMessage("Tiled map processed successfully");
            return input;
        }
    }
}