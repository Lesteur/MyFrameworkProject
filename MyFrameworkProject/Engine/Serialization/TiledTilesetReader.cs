using Microsoft.Xna.Framework.Content;

using MyFrameworkProject.Engine.Core;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Content type reader for loading TiledTileset data from XNB files at runtime.
    /// Deserializes tileset metadata and image information.
    /// </summary>
    public class TiledTilesetReader : ContentTypeReader<TiledTileset>
    {
        /// <summary>
        /// Reads a TiledTileset instance from the content reader.
        /// Data must be read in the same order as written by TiledTilesetWriter.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <param name="existingInstance">An existing instance to populate, or null to create a new one.</param>
        /// <returns>A fully populated TiledTileset instance.</returns>
        protected override TiledTileset Read(ContentReader input, TiledTileset existingInstance)
        {
            var tileset = existingInstance ?? new TiledTileset
            {
                Name = input.ReadString(),
                TileWidth = input.ReadInt32(),
                TileHeight = input.ReadInt32(),
                Margin = input.ReadInt32(),
                Spacing = input.ReadInt32(),
                TileCount = input.ReadInt32(),
                Columns = input.ReadInt32(),
                Image = input.ReadString(),
                ImageWidth = input.ReadInt32(),
                ImageHeight = input.ReadInt32()
            };

            // Read animated tile data
            bool hasTiles = input.ReadBoolean();

            Logger.Info($"Reading tiles for tileset '{tileset.Name}': HasTiles = {hasTiles}");

            if (hasTiles)
            {
                int tileCount = input.ReadInt32();
                tileset.Tiles = new TiledTileData[tileCount];

                Logger.Info($"Reading {tileCount} tiles from tileset '{tileset.Name}'.");

                for (int i = 0; i < tileCount; i++)
                {
                    var tile = new TiledTileData
                    {
                        Id = input.ReadInt32()
                    };

                    bool hasAnimation = input.ReadBoolean();

                    if (hasAnimation)
                    {
                        int frameCount = input.ReadInt32();
                        tile.Animation = new TiledAnimationFrame[frameCount];

                        for (int j = 0; j < frameCount; j++)
                        {
                            tile.Animation[j] = new TiledAnimationFrame
                            {
                                Duration = input.ReadInt32(),
                                TileId = input.ReadInt32()
                            };
                        }
                    }

                    tileset.Tiles[i] = tile;
                }
            }

            return tileset;
        }
    }
}