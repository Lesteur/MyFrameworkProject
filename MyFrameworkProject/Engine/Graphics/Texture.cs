using Microsoft.Xna.Framework.Graphics;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D texture stored in GPU memory.
    /// </summary>
    public sealed class Texture(Texture2D texture)
    {
        private readonly Texture2D _texture = texture;
        private readonly int _width = texture.Width;
        private readonly int _height = texture.Height;

        public Texture2D GetNativeTexture()
        {
            return _texture;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }
    }
}