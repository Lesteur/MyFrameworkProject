using System;
using System.IO;
using System.Text.Json;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MyFrameworkProject.Engine.Core;
using MyFrameworkProject.Engine.Graphics;

namespace MyFrameworkProject.Engine.Serialization
{
    /// <summary>
    /// Loads and processes Tiled map files (TMJ/JSON format).
    /// </summary>
    public class TiledLoader
    {
        private readonly ContentManager _content;
        private readonly string _contentRootPath;

        public TiledLoader(ContentManager content)
        {
            _content = content;
            // Get the actual root directory path (usually "Content")
            _contentRootPath = Path.Combine(Directory.GetCurrentDirectory(), content.RootDirectory);
        }

        /// <summary>
        /// Loads a Tiled map from a JSON file.
        /// </summary>
        /// <param name="jsonPath">Relative path to the JSON file from Content root (e.g., "JSON/Level").</param>
        /// <returns>The parsed Tiled map.</returns>
        public TiledMap LoadMap(string jsonPath)
        {
            // Normalize path separators
            jsonPath = jsonPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            
            // Try different extensions
            string fullPath = Path.Combine(_contentRootPath, jsonPath + ".tmj");
            
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(_contentRootPath, jsonPath + ".json");
            }

            if (!File.Exists(fullPath))
            {
                Logger.Error($"Tiled map file not found: {fullPath}");
                Logger.Error($"Searched in: {_contentRootPath}");
                Logger.Error($"Current directory: {Directory.GetCurrentDirectory()}");
                return null;
            }

            try
            {
                string jsonContent = File.ReadAllText(fullPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var map = JsonSerializer.Deserialize<TiledMap>(jsonContent, options);
                Logger.Info($"Tiled map loaded successfully: {fullPath}");
                return map;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load Tiled map: {ex.Message}");
                Logger.Error($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Loads a Tiled tileset from a TSX/JSON file.
        /// </summary>
        /// <param name="tsxPath">Relative path to the tileset file (e.g., "TilesetMario.tsx" or "TilesetMario").</param>
        /// <returns>The parsed Tiled tileset.</returns>
        public TiledTileset LoadTileset(string tsxPath)
        {
            // Normalize path separators
            tsxPath = tsxPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            
            // Remove the extension if present
            tsxPath = Path.ChangeExtension(tsxPath, null);
            
            // Get the directory where the tileset should be (same as the map)
            string baseDir = Path.GetDirectoryName(tsxPath);
            string fileName = Path.GetFileName(tsxPath);
            
            // Try in JSON subfolder first (relative to Content root)
            string fullPath = Path.Combine(_contentRootPath, "JSON", fileName + ".tsx");
            
            if (!File.Exists(fullPath))
            {
                // Try with .json extension
                fullPath = Path.Combine(_contentRootPath, "JSON", fileName + ".json");
            }
            
            if (!File.Exists(fullPath))
            {
                // Try in the base directory provided
                fullPath = Path.Combine(_contentRootPath, tsxPath + ".tsx");
            }
            
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(_contentRootPath, tsxPath + ".json");
            }

            if (!File.Exists(fullPath))
            {
                Logger.Error($"Tiled tileset file not found: {fileName}");
                Logger.Error($"Searched in: {Path.Combine(_contentRootPath, "JSON")}")
;
                return null;
            }

            try
            {
                string jsonContent = File.ReadAllText(fullPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var tileset = JsonSerializer.Deserialize<TiledTileset>(jsonContent, options);
                Logger.Info($"Tiled tileset loaded successfully: {fullPath}");
                return tileset;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load Tiled tileset: {ex.Message}");
                Logger.Error($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Creates a Tileset object from a Tiled tileset definition.
        /// </summary>
        /// <param name="tiledTileset">The Tiled tileset data.</param>
        /// <returns>A Tileset ready for rendering.</returns>
        public Tileset CreateTileset(TiledTileset tiledTileset)
        {
            if (tiledTileset == null)
                return null;

            try
            {
                // Extract texture name from image path (remove extension and path)
                string imagePath = tiledTileset.Image.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                string textureName = Path.GetFileNameWithoutExtension(imagePath);
                
                Logger.Info($"Loading texture: {textureName}");
                
                Texture2D nativeTexture = _content.Load<Texture2D>("Textures/" + textureName);
                var texture = new Graphics.Texture(nativeTexture);
                
                var tileset = new Tileset(texture, tiledTileset.TileWidth, tiledTileset.TileHeight,
                                          tiledTileset.Margin, tiledTileset.Margin,
                                          tiledTileset.Spacing, tiledTileset.Spacing);

                Logger.Info($"Tileset created: {tiledTileset.Name} ({tiledTileset.TileWidth}x{tiledTileset.TileHeight})");
                return tileset;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create tileset: {ex.Message}");
                Logger.Error($"Image path was: {tiledTileset?.Image}");
                return null;
            }
        }

        /// <summary>
        /// Creates a Tilemap from a Tiled tile layer.
        /// </summary>
        /// <param name="layer">The Tiled tile layer.</param>
        /// <param name="tileset">The tileset to use for rendering.</param>
        /// <returns>A Tilemap ready for rendering.</returns>
        public Tilemap CreateTilemap(TiledLayer layer, Tileset tileset)
        {
            if (layer == null || layer.Type != "tilelayer" || tileset == null)
                return null;

            var tilemap = new Tilemap(tileset, layer.Width, layer.Height);

            // Fill the tilemap with data from Tiled
            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int index = y * layer.Width + x;
                    if (index < layer.Data.Count)
                    {
                        int tileId = layer.Data[index];
                        // Tiled uses 0 for empty tiles, but we need to subtract 1 for the tileset index
                        if (tileId > 0)
                        {
                            tilemap.SetTile(x, y, tileId - 1);
                        }
                    }
                }
            }

            Logger.Info($"Tilemap created: {layer.Name} ({layer.Width}x{layer.Height})");
            return tilemap;
        }
    }
}