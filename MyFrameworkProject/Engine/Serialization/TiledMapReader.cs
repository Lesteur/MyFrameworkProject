using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Content type reader for loading TiledMap data from XNB files at runtime.
    /// Deserializes Tiled map data including layers, objects, and tileset references.
    /// </summary>
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        /// <summary>
        /// Reads a TiledMap instance from the content reader.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <param name="existingInstance">An existing instance to populate, or null to create a new one.</param>
        /// <returns>A fully populated TiledMap instance.</returns>
        protected override TiledMap Read(ContentReader input, TiledMap existingInstance)
        {
            var map = new TiledMap
            {
                Width = input.ReadInt32(),
                Height = input.ReadInt32(),
                TileWidth = input.ReadInt32(),
                TileHeight = input.ReadInt32(),
                Orientation = input.ReadString(),
                RenderOrder = input.ReadString(),
                Layers = ReadLayers(input),
                Tilesets = ReadTilesets(input)
            };

            return map;
        }

        /// <summary>
        /// Reads all layers from the content reader.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <returns>A list of TiledLayer instances.</returns>
        private static List<TiledLayer> ReadLayers(ContentReader input)
        {
            int layerCount = input.ReadInt32();
            var layers = new List<TiledLayer>(layerCount);

            for (int i = 0; i < layerCount; i++)
            {
                var layer = new TiledLayer
                {
                    Id = input.ReadInt32(),
                    Name = input.ReadString(),
                    Type = input.ReadString(),
                    Visible = input.ReadBoolean(),
                    Opacity = input.ReadSingle(),
                    Width = input.ReadInt32(),
                    Height = input.ReadInt32(),
                    Data = ReadTileData(input),
                    Objects = ReadObjects(input)
                };

                layers.Add(layer);
            }

            return layers;
        }

        /// <summary>
        /// Reads tile data array from the content reader.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <returns>A list of tile IDs, or null if no data is present.</returns>
        private static List<int> ReadTileData(ContentReader input)
        {
            int dataCount = input.ReadInt32();
            if (dataCount == 0)
            {
                return null;
            }

            var data = new List<int>(dataCount);
            for (int i = 0; i < dataCount; i++)
            {
                data.Add(input.ReadInt32());
            }

            return data;
        }

        /// <summary>
        /// Reads all objects from the content reader.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <returns>A list of TiledObject instances, or null if no objects are present.</returns>
        private static List<TiledObject> ReadObjects(ContentReader input)
        {
            int objectCount = input.ReadInt32();
            if (objectCount == 0)
            {
                return null;
            }

            var objects = new List<TiledObject>(objectCount);
            for (int i = 0; i < objectCount; i++)
            {
                var obj = new TiledObject
                {
                    Id = input.ReadInt32(),
                    Name = input.ReadString(),
                    Type = input.ReadString(),
                    X = input.ReadSingle(),
                    Y = input.ReadSingle(),
                    Width = input.ReadSingle(),
                    Height = input.ReadSingle(),
                    Rotation = input.ReadSingle(),
                    Visible = input.ReadBoolean(),
                    Properties = ReadProperties(input)
                };

                objects.Add(obj);
            }

            return objects;
        }

        /// <summary>
        /// Reads all properties from the content reader.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <returns>A list of TiledProperty instances, or null if no properties are present.</returns>
        private static List<TiledProperty> ReadProperties(ContentReader input)
        {
            int propCount = input.ReadInt32();
            if (propCount == 0)
            {
                return null;
            }

            var properties = new List<TiledProperty>(propCount);
            for (int i = 0; i < propCount; i++)
            {
                properties.Add(new TiledProperty
                {
                    Name = input.ReadString(),
                    Type = input.ReadString(),
                    Value = input.ReadString()
                });
            }

            return properties;
        }

        /// <summary>
        /// Reads all tileset references from the content reader.
        /// </summary>
        /// <param name="input">The content reader to read from.</param>
        /// <returns>A list of TiledTilesetReference instances.</returns>
        private static List<TiledTilesetReference> ReadTilesets(ContentReader input)
        {
            int tilesetCount = input.ReadInt32();
            var tilesets = new List<TiledTilesetReference>(tilesetCount);

            for (int i = 0; i < tilesetCount; i++)
            {
                tilesets.Add(new TiledTilesetReference
                {
                    FirstGid = input.ReadInt32(),
                    Source = input.ReadString()
                });
            }

            return tilesets;
        }
    }
}