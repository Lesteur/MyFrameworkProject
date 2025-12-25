using Microsoft.Xna.Framework.Graphics;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D texture wrapper that encapsulates a MonoGame Texture2D and provides cached dimension information.
    /// This class provides a simplified interface for accessing texture properties and the underlying native texture.
    /// </summary>
    /// <param name="texture">The MonoGame Texture2D to wrap.</param>
    public sealed class Texture(Texture2D texture)
    {
        #region Fields - Texture

        /// <summary>
        /// The underlying MonoGame Texture2D that contains the actual texture data.
        /// </summary>
        private readonly Texture2D _texture = texture;

        #endregion

        #region Fields - Dimensions

        /// <summary>
        /// The cached width of the texture in pixels.
        /// Stored to avoid repeated property access on the native texture.
        /// </summary>
        private readonly int _width = texture.Width;

        /// <summary>
        /// The cached height of the texture in pixels.
        /// Stored to avoid repeated property access on the native texture.
        /// </summary>
        private readonly int _height = texture.Height;

        #endregion

        #region Public Methods - Texture

        /// <summary>
        /// Gets the underlying MonoGame Texture2D for use with native rendering operations.
        /// This provides access to the raw texture data for SpriteBatch.Draw calls.
        /// </summary>
        public Texture2D NativeTexture => _texture;

        #endregion

        #region Public Methods - Dimensions

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// </summary>
        public int Height => _height;

        #endregion
    }
}