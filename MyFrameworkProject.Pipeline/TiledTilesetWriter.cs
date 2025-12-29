using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Écrit les données TiledTilesetContent dans le format XNB.
    /// </summary>
    [ContentTypeWriter]
    public class TiledTilesetWriter : ContentTypeWriter<TiledTilesetContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // Format: "Namespace.ClassName, AssemblyName"
            return "MyFrameworkProject.Engine.Serialization.TiledTilesetReader, MyFrameworkProject";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            // Format: "Namespace.ClassName, AssemblyName"
            return "MyFrameworkProject.Engine.Serialization.TiledTileset, MyFrameworkProject";
        }

        protected override void Write(ContentWriter output, TiledTilesetContent value)
        {
            // Écrire les métadonnées du tileset
            output.Write(value.Name ?? "");
            output.Write(value.TileWidth);
            output.Write(value.TileHeight);
            output.Write(value.Margin);
            output.Write(value.Spacing);
            output.Write(value.TileCount);
            output.Write(value.Columns);
            output.Write(value.Image ?? "");
            output.Write(value.ImageWidth);
            output.Write(value.ImageHeight);
        }
    }
}