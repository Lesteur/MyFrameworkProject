using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Écrit les données TiledMapContent dans le format XNB.
    /// </summary>
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MyFrameworkProject.Engine.Serialization.TiledMapReader, MyFrameworkProject";
        }

        protected override void Write(ContentWriter output, TiledMapContent value)
        {
            // Écrire les dimensions de la carte
            output.Write(value.Width);
            output.Write(value.Height);
            output.Write(value.TileWidth);
            output.Write(value.TileHeight);
            output.Write(value.Orientation ?? "orthogonal");
            output.Write(value.RenderOrder ?? "right-down");

            // Écrire les layers
            output.Write(value.Layers.Count);
            foreach (var layer in value.Layers)
            {
                output.Write(layer.Id);
                output.Write(layer.Name ?? "");
                output.Write(layer.Type ?? "");
                output.Write(layer.Visible);
                output.Write(layer.Opacity);
                output.Write(layer.Width);
                output.Write(layer.Height);

                // Écrire les données de tiles
                if (layer.Data != null)
                {
                    output.Write(layer.Data.Count);
                    foreach (var tile in layer.Data)
                    {
                        output.Write(tile);
                    }
                }
                else
                {
                    output.Write(0);
                }

                // Écrire les objets
                if (layer.Objects != null)
                {
                    output.Write(layer.Objects.Count);
                    foreach (var obj in layer.Objects)
                    {
                        output.Write(obj.Id);
                        output.Write(obj.Name ?? "");
                        output.Write(obj.Type ?? "");
                        output.Write(obj.X);
                        output.Write(obj.Y);
                        output.Write(obj.Width);
                        output.Write(obj.Height);
                        output.Write(obj.Rotation);
                        output.Write(obj.Visible);

                        // Écrire les propriétés
                        if (obj.Properties != null)
                        {
                            output.Write(obj.Properties.Count);
                            foreach (var prop in obj.Properties)
                            {
                                output.Write(prop.Name ?? "");
                                output.Write(prop.Type ?? "");
                                output.Write(prop.Value ?? "");
                            }
                        }
                        else
                        {
                            output.Write(0);
                        }
                    }
                }
                else
                {
                    output.Write(0);
                }
            }

            // Écrire les tilesets
            output.Write(value.Tilesets.Count);
            foreach (var tileset in value.Tilesets)
            {
                output.Write(tileset.FirstGid);
                output.Write(tileset.Source ?? "");
            }
        }
    }
}