using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Content type writer for serializing TiledTilesetContent to XNB format.
    /// Writes tileset data in a format compatible with TiledTilesetReader at runtime.
    /// </summary>
    [ContentTypeWriter]
    public class TiledTilesetWriter : ContentTypeWriter<TiledTilesetContent>
    {
        /// <summary>
        /// Gets the runtime reader type for deserializing the XNB file.
        /// </summary>
        /// <param name="targetPlatform">The target platform.</param>
        /// <returns>The fully qualified type name of the runtime reader.</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MyFrameworkProject.Engine.Serialization.TiledTilesetReader, MyFrameworkProject";
        }

        /// <summary>
        /// Gets the runtime type that will be created by the reader.
        /// </summary>
        /// <param name="targetPlatform">The target platform.</param>
        /// <returns>The fully qualified type name of the runtime type.</returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MyFrameworkProject.Engine.Serialization.TiledTileset, MyFrameworkProject";
        }

        /// <summary>
        /// Writes the tileset content to the XNB file.
        /// Data must be written in the same order as read by TiledTilesetReader.
        /// </summary>
        /// <param name="output">The content writer.</param>
        /// <param name="value">The tileset data to write.</param>
        protected override void Write(ContentWriter output, TiledTilesetContent value)
        {
            // Write tileset metadata in a specific order
            // This order MUST match the read order in TiledTilesetReader
            output.Write(value.Name ?? string.Empty);
            output.Write(value.TileWidth);
            output.Write(value.TileHeight);
            output.Write(value.Margin);
            output.Write(value.Spacing);
            output.Write(value.TileCount);
            output.Write(value.Columns);
            output.Write(value.Image ?? string.Empty);
            output.Write(value.ImageWidth);
            output.Write(value.ImageHeight);
        }
    }
}