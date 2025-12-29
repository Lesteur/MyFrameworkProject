using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Content type writer for serializing TiledMapContent to XNB format.
    /// Writes map data in a format compatible with TiledMapReader at runtime.
    /// </summary>
    [ContentTypeWriter]
    public class TiledMapWriter : ContentTypeWriter<TiledMapContent>
    {
        /// <summary>
        /// Gets the runtime reader type for deserializing the XNB file.
        /// </summary>
        /// <param name="targetPlatform">The target platform.</param>
        /// <returns>The fully qualified type name of the runtime reader.</returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MyFrameworkProject.Engine.Serialization.TiledMapReader, MyFrameworkProject";
        }

        /// <summary>
        /// Gets the runtime type that will be created by the reader.
        /// </summary>
        /// <param name="targetPlatform">The target platform.</param>
        /// <returns>The fully qualified type name of the runtime type.</returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MyFrameworkProject.Engine.Serialization.TiledMap, MyFrameworkProject";
        }

        /// <summary>
        /// Writes the map content to the XNB file.
        /// Data must be written in the same order as read by TiledMapReader.
        /// </summary>
        /// <param name="output">The content writer.</param>
        /// <param name="value">The map data to write.</param>
        protected override void Write(ContentWriter output, TiledMapContent value)
        {
            WriteMapProperties(output, value);
            WriteLayers(output, value);
            WriteTilesets(output, value);
        }

        /// <summary>
        /// Writes the basic map properties.
        /// </summary>
        private static void WriteMapProperties(ContentWriter output, TiledMapContent value)
        {
            output.Write(value.Width);
            output.Write(value.Height);
            output.Write(value.TileWidth);
            output.Write(value.TileHeight);
            output.Write(value.Orientation ?? "orthogonal");
            output.Write(value.RenderOrder ?? "right-down");
        }

        /// <summary>
        /// Writes all layers to the output.
        /// </summary>
        private static void WriteLayers(ContentWriter output, TiledMapContent value)
        {
            output.Write(value.Layers.Count);

            foreach (var layer in value.Layers)
            {
                WriteLayerProperties(output, layer);
                WriteTileData(output, layer);
                WriteObjects(output, layer);
            }
        }

        /// <summary>
        /// Writes the basic layer properties.
        /// </summary>
        private static void WriteLayerProperties(ContentWriter output, TiledLayerContent layer)
        {
            output.Write(layer.Id);
            output.Write(layer.Name ?? string.Empty);
            output.Write(layer.Type ?? string.Empty);
            output.Write(layer.Visible);
            output.Write(layer.Opacity);
            output.Write(layer.Width);
            output.Write(layer.Height);
        }

        /// <summary>
        /// Writes the tile data for a layer.
        /// </summary>
        private static void WriteTileData(ContentWriter output, TiledLayerContent layer)
        {
            if (layer.Data == null)
            {
                output.Write(0);
                return;
            }

            output.Write(layer.Data.Count);
            foreach (var tile in layer.Data)
            {
                output.Write(tile);
            }
        }

        /// <summary>
        /// Writes the objects for a layer.
        /// </summary>
        private static void WriteObjects(ContentWriter output, TiledLayerContent layer)
        {
            if (layer.Objects == null)
            {
                output.Write(0);
                return;
            }

            output.Write(layer.Objects.Count);
            foreach (var obj in layer.Objects)
            {
                WriteObjectProperties(output, obj);
                WriteObjectCustomProperties(output, obj);
            }
        }

        /// <summary>
        /// Writes the basic object properties.
        /// </summary>
        private static void WriteObjectProperties(ContentWriter output, TiledObjectContent obj)
        {
            output.Write(obj.Id);
            output.Write(obj.Name ?? string.Empty);
            output.Write(obj.Type ?? string.Empty);
            output.Write(obj.X);
            output.Write(obj.Y);
            output.Write(obj.Width);
            output.Write(obj.Height);
            output.Write(obj.Rotation);
            output.Write(obj.Visible);
        }

        /// <summary>
        /// Writes the custom properties for an object.
        /// </summary>
        private static void WriteObjectCustomProperties(ContentWriter output, TiledObjectContent obj)
        {
            if (obj.Properties == null)
            {
                output.Write(0);
                return;
            }

            output.Write(obj.Properties.Count);
            foreach (var prop in obj.Properties)
            {
                output.Write(prop.Name ?? string.Empty);
                output.Write(prop.Type ?? string.Empty);
                output.Write(prop.Value?.ToString() ?? string.Empty);
            }
        }

        /// <summary>
        /// Writes all tileset references to the output.
        /// </summary>
        private static void WriteTilesets(ContentWriter output, TiledMapContent value)
        {
            output.Write(value.Tilesets.Count);
            foreach (var tileset in value.Tilesets)
            {
                output.Write(tileset.FirstGid);
                output.Write(tileset.Source ?? string.Empty);
            }
        }
    }
}