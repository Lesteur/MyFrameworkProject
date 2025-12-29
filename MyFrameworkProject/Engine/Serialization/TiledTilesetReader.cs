using Microsoft.Xna.Framework.Content;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Reads TiledTileset data from XNB files at runtime.
    /// </summary>
    public class TiledTilesetReader : ContentTypeReader<TiledTileset>
    {
        protected override TiledTileset Read(ContentReader input, TiledTileset existingInstance)
        {
            var tileset = existingInstance ?? new TiledTileset();

            // Read data in the same order as TiledTilesetWriter writes it
            tileset.Name = input.ReadString();
            tileset.TileWidth = input.ReadInt32();
            tileset.TileHeight = input.ReadInt32();
            tileset.Margin = input.ReadInt32();
            tileset.Spacing = input.ReadInt32();
            tileset.TileCount = input.ReadInt32();
            tileset.Columns = input.ReadInt32();
            tileset.Image = input.ReadString();
            tileset.ImageWidth = input.ReadInt32();
            tileset.ImageHeight = input.ReadInt32();

            return tileset;
        }
    }
}