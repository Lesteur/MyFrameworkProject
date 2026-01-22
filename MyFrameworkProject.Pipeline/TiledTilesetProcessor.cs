using Microsoft.Xna.Framework.Content.Pipeline;
using System;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Content processor for Tiled tileset data.
    /// Validates and optimizes imported tileset data before serialization.
    /// </summary>
    [ContentProcessor(DisplayName = "Tiled Tileset Processor")]
    public class TiledTilesetProcessor : ContentProcessor<TiledTilesetContent, TiledTilesetContent>
    {
        /// <summary>
        /// Processes the imported Tiled tileset data.
        /// Performs validation and consistency checks.
        /// </summary>
        /// <param name="input">The imported tileset data.</param>
        /// <param name="context">The processor context for logging.</param>
        /// <returns>The processed tileset data ready for serialization.</returns>
        /// <exception cref="InvalidContentException">Thrown when validation fails.</exception>
        public override TiledTilesetContent Process(TiledTilesetContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage($"Processing Tiled tileset: {input.Name}");

            ValidateBasicProperties(input);
            ValidateImageConsistency(input, context);
            ValidateAnimatedTiles(input, context);

            int animatedTileCount = input.Tiles?.Length ?? 0;
            context.Logger.LogMessage(
                $"Tileset processed successfully: {input.TileCount} tiles, " +
                $"{input.Columns} columns, {CalculateRows(input)} rows, " +
                $"{animatedTileCount} animated tile(s)"
            );

            return input;
        }

        /// <summary>
        /// Validates the basic tileset properties.
        /// </summary>
        /// <param name="input">The tileset data to validate.</param>
        /// <exception cref="InvalidContentException">Thrown when validation fails.</exception>
        private static void ValidateBasicProperties(TiledTilesetContent input)
        {
            if (input.TileWidth <= 0 || input.TileHeight <= 0)
            {
                throw new InvalidContentException(
                    $"Tile dimensions must be greater than zero. " +
                    $"Got: {input.TileWidth}x{input.TileHeight}"
                );
            }

            if (string.IsNullOrWhiteSpace(input.Image))
            {
                throw new InvalidContentException("Tileset must reference an image file.");
            }

            if (input.TileCount <= 0)
            {
                throw new InvalidContentException(
                    $"Tileset must have at least one tile. Got: {input.TileCount}"
                );
            }

            if (input.Columns <= 0)
            {
                throw new InvalidContentException(
                    $"Tileset must have at least one column. Got: {input.Columns}"
                );
            }

            if (input.Margin < 0)
            {
                throw new InvalidContentException(
                    $"Margin cannot be negative. Got: {input.Margin}"
                );
            }

            if (input.Spacing < 0)
            {
                throw new InvalidContentException(
                    $"Spacing cannot be negative. Got: {input.Spacing}"
                );
            }
        }

        /// <summary>
        /// Validates the consistency between tileset properties and image dimensions.
        /// </summary>
        /// <param name="input">The tileset data to validate.</param>
        /// <param name="context">The processor context for logging warnings.</param>
        private static void ValidateImageConsistency(TiledTilesetContent input, ContentProcessorContext context)
        {
            int expectedRows = CalculateRows(input);
            int calculatedImageWidth = CalculateRequiredImageWidth(input);
            int calculatedImageHeight = CalculateRequiredImageHeight(input, expectedRows);

            // Only validate if image dimensions are specified
            if (input.ImageWidth > 0)
            {
                if (input.ImageWidth < calculatedImageWidth)
                {
                    context.Logger.LogWarning(
                        null,
                        null,
                        $"Image width ({input.ImageWidth} px) is smaller than the calculated minimum " +
                        $"required width ({calculatedImageWidth} px) for {input.Columns} columns."
                    );
                }
            }

            if (input.ImageHeight > 0)
            {
                if (input.ImageHeight < calculatedImageHeight)
                {
                    context.Logger.LogWarning(
                        null,
                        null,
                        $"Image height ({input.ImageHeight} px) is smaller than the calculated minimum " +
                        $"required height ({calculatedImageHeight} px) for {expectedRows} rows."
                    );
                }
            }
        }

        /// <summary>
        /// Validates animated tile data if present.
        /// </summary>
        /// <param name="input">The tileset data to validate.</param>
        /// <param name="context">The processor context for logging warnings.</param>
        private static void ValidateAnimatedTiles(TiledTilesetContent input, ContentProcessorContext context)
        {
            if (input.Tiles == null || input.Tiles.Length == 0)
            {
                return;
            }

            foreach (var tile in input.Tiles)
            {
                // Validate tile ID is within bounds
                if (tile.Id < 0 || tile.Id >= input.TileCount)
                {
                    context.Logger.LogWarning(
                        null,
                        null,
                        $"Tile ID {tile.Id} is out of bounds (0-{input.TileCount - 1})."
                    );
                }

                // Validate animation if present
                if (tile.Animation != null && tile.Animation.Length > 0)
                {
                    for (int i = 0; i < tile.Animation.Length; i++)
                    {
                        var frame = tile.Animation[i];

                        if (frame.Duration <= 0)
                        {
                            context.Logger.LogWarning(
                                null,
                                null,
                                $"Animation frame {i} of tile {tile.Id} has invalid duration: {frame.Duration}ms. " +
                                $"Duration must be greater than zero."
                            );
                        }

                        if (frame.TileId < 0 || frame.TileId >= input.TileCount)
                        {
                            context.Logger.LogWarning(
                                null,
                                null,
                                $"Animation frame {i} of tile {tile.Id} references invalid tile ID: {frame.TileId}. " +
                                $"Valid range is 0-{input.TileCount - 1}."
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the number of rows required for the tileset.
        /// </summary>
        /// <param name="input">The tileset data.</param>
        /// <returns>The number of rows.</returns>
        private static int CalculateRows(TiledTilesetContent input)
        {
            return (input.TileCount + input.Columns - 1) / input.Columns;
        }

        /// <summary>
        /// Calculates the minimum required image width for the tileset.
        /// Formula: margin * 2 + columns * tileWidth + (columns - 1) * spacing
        /// </summary>
        /// <param name="input">The tileset data.</param>
        /// <returns>The minimum required width in pixels.</returns>
        private static int CalculateRequiredImageWidth(TiledTilesetContent input)
        {
            return input.Margin * 2 +
                   input.Columns * input.TileWidth +
                   Math.Max(0, input.Columns - 1) * input.Spacing;
        }

        /// <summary>
        /// Calculates the minimum required image height for the tileset.
        /// Formula: margin * 2 + rows * tileHeight + (rows - 1) * spacing
        /// </summary>
        /// <param name="input">The tileset data.</param>
        /// <param name="rows">The number of rows in the tileset.</param>
        /// <returns>The minimum required height in pixels.</returns>
        private static int CalculateRequiredImageHeight(TiledTilesetContent input, int rows)
        {
            return input.Margin * 2 +
                   rows * input.TileHeight +
                   Math.Max(0, rows - 1) * input.Spacing;
        }
    }
}