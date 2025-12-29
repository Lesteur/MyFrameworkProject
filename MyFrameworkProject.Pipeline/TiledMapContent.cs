using System.Collections.Generic;

namespace MyFrameworkProject.Pipeline
{
    /// <summary>
    /// Représente les données d'une carte Tiled pour le Content Pipeline.
    /// Cette classe est sérialisable et sera convertie en format XNB.
    /// </summary>
    public class TiledMapContent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public string Orientation { get; set; }
        public string RenderOrder { get; set; }
        
        public List<TiledLayerContent> Layers { get; set; } = new List<TiledLayerContent>();
        public List<TiledTilesetRefContent> Tilesets { get; set; } = new List<TiledTilesetRefContent>();
    }

    /// <summary>
    /// Représente un layer Tiled.
    /// </summary>
    public class TiledLayerContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Visible { get; set; }
        public float Opacity { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        // Pour les tile layers
        public List<int> Data { get; set; }
        
        // Pour les object layers
        public List<TiledObjectContent> Objects { get; set; }
    }

    /// <summary>
    /// Représente un objet Tiled.
    /// </summary>
    public class TiledObjectContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Rotation { get; set; }
        public bool Visible { get; set; }
        public List<TiledPropertyContent> Properties { get; set; }
    }

    /// <summary>
    /// Représente une propriété personnalisée Tiled.
    /// </summary>
    public class TiledPropertyContent
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Représente une référence à un tileset.
    /// </summary>
    public class TiledTilesetRefContent
    {
        public int FirstGid { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// Représente un tileset Tiled.
    /// </summary>
    public class TiledTilesetContent
    {
        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int Margin { get; set; }
        public int Spacing { get; set; }
        public int TileCount { get; set; }
        public int Columns { get; set; }
        public string Image { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }
}