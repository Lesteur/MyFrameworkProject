using Microsoft.Xna.Framework.Content;

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

            return tileset;
        }
    }
}