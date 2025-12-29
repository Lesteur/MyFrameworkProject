using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using System.Text.Json;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Importe les fichiers JSON Tiled dans le Content Pipeline.
    /// </summary>
    [ContentImporter(".json", ".tmj", DisplayName = "Tiled Map Importer", DefaultProcessor = "TiledMapProcessor")]
    public class TiledMapImporter : ContentImporter<TiledMapContent>
    {
        public override TiledMapContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage($"Importing Tiled map: {filename}");

            // Lire le fichier JSON
            string jsonText = File.ReadAllText(filename);
            
            // Options pour la désérialisation
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            var tiledData = JsonSerializer.Deserialize<TiledMapData>(jsonText, options);

            // Convertir en TiledMapContent
            var content = new TiledMapContent
            {
                Width = tiledData.Width,
                Height = tiledData.Height,
                TileWidth = tiledData.TileWidth,
                TileHeight = tiledData.TileHeight,
                Orientation = tiledData.Orientation,
                RenderOrder = tiledData.RenderOrder
            };

            // Convertir les layers
            if (tiledData.Layers != null)
            {
                foreach (var layer in tiledData.Layers)
                {
                    var layerContent = new TiledLayerContent
                    {
                        Id = layer.Id,
                        Name = layer.Name,
                        Type = layer.Type,
                        Visible = layer.Visible,
                        Opacity = layer.Opacity,
                        Width = layer.Width,
                        Height = layer.Height,
                        Data = layer.Data
                    };

                    // Convertir les objets si présents
                    if (layer.Objects != null)
                    {
                        layerContent.Objects = new System.Collections.Generic.List<TiledObjectContent>();
                        foreach (var obj in layer.Objects)
                        {
                            var objContent = new TiledObjectContent
                            {
                                Id = obj.Id,
                                Name = obj.Name,
                                Type = obj.Type,
                                X = obj.X,
                                Y = obj.Y,
                                Width = obj.Width,
                                Height = obj.Height,
                                Rotation = obj.Rotation,
                                Visible = obj.Visible
                            };

                            // Convertir les propriétés
                            if (obj.Properties != null)
                            {
                                objContent.Properties = new System.Collections.Generic.List<TiledPropertyContent>();
                                foreach (var prop in obj.Properties)
                                {
                                    objContent.Properties.Add(new TiledPropertyContent
                                    {
                                        Name = prop.Name,
                                        Type = prop.Type,
                                        Value = prop.Value?.ToString()
                                    });
                                }
                            }

                            layerContent.Objects.Add(objContent);
                        }
                    }

                    content.Layers.Add(layerContent);
                }
            }

            // Convertir les tilesets
            if (tiledData.Tilesets != null)
            {
                foreach (var tileset in tiledData.Tilesets)
                {
                    content.Tilesets.Add(new TiledTilesetRefContent
                    {
                        FirstGid = tileset.FirstGid,
                        Source = tileset.Source
                    });
                }
            }

            context.Logger.LogMessage($"Successfully imported Tiled map with {content.Layers.Count} layers");
            return content;
        }
    }
}