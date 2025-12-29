using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Lit les données TiledMap depuis un fichier XNB.
    /// </summary>
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
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
                Layers = new List<TiledLayer>(),
                Tilesets = new List<TiledTilesetReference>()
            };

            // Lire les layers
            int layerCount = input.ReadInt32();
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
                    Height = input.ReadInt32()
                };

                // Lire les données de tiles
                int dataCount = input.ReadInt32();
                if (dataCount > 0)
                {
                    layer.Data = new List<int>(dataCount);
                    for (int j = 0; j < dataCount; j++)
                    {
                        layer.Data.Add(input.ReadInt32());
                    }
                }

                // Lire les objets
                int objectCount = input.ReadInt32();
                if (objectCount > 0)
                {
                    layer.Objects = new List<TiledObject>(objectCount);
                    for (int j = 0; j < objectCount; j++)
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
                            Visible = input.ReadBoolean()
                        };

                        // Lire les propriétés
                        int propCount = input.ReadInt32();
                        if (propCount > 0)
                        {
                            obj.Properties = new List<TiledProperty>(propCount);
                            for (int k = 0; k < propCount; k++)
                            {
                                obj.Properties.Add(new TiledProperty
                                {
                                    Name = input.ReadString(),
                                    Type = input.ReadString(),
                                    Value = input.ReadString()
                                });
                            }
                        }

                        layer.Objects.Add(obj);
                    }
                }

                map.Layers.Add(layer);
            }

            // Lire les tilesets
            int tilesetCount = input.ReadInt32();
            for (int i = 0; i < tilesetCount; i++)
            {
                map.Tilesets.Add(new TiledTilesetReference
                {
                    FirstGid = input.ReadInt32(),
                    Source = input.ReadString()
                });
            }

            return map;
        }
    }
}