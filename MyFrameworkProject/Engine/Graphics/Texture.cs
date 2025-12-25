using Microsoft.Xna.Framework.Graphics;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D texture wrapper that encapsulates a MonoGame Texture2D and provides cached dimension information.
    /// This class provides a simplified interface for accessing texture properties and the underlying native texture.
    /// </summary>
    public sealed class Texture
    {
        #region Fields - Texture

        /// <summary>
        /// The underlying MonoGame Texture2D that contains the actual texture data.
        /// </summary>
        private readonly Texture2D _texture;

        #endregion

        #region Fields - Dimensions

        /// <summary>
        /// The cached width of the texture in pixels.
        /// Stored to avoid repeated property access on the native texture.
        /// </summary>
        private readonly int _width;

        /// <summary>
        /// The cached height of the texture in pixels.
        /// Stored to avoid repeated property access on the native texture.
        /// </summary>
        private readonly int _height;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class with the specified MonoGame texture.
        /// The texture's dimensions are cached during initialization for improved performance.
        /// </summary>
        /// <param name="texture">The MonoGame Texture2D to wrap.</param>
        public Texture(Texture2D texture)
        {
            _texture = texture;
            _width = texture.Width;
            _height = texture.Height;
        }

        #endregion

        #region Public Methods - Texture

        /// <summary>
        /// Gets the underlying MonoGame Texture2D for use with native rendering operations.
        /// This provides access to the raw texture data for SpriteBatch.Draw calls.
        /// </summary>
        /// <returns>The native MonoGame Texture2D instance.</returns>
        public Texture2D GetNativeTexture()
        {
            return _texture;
        }

        #endregion

        #region Public Methods - Dimensions

        /// <summary>
        /// Gets the width of the texture in pixels.
        /// Returns the cached value for improved performance.
        /// </summary>
        /// <returns>The width of the texture in pixels.</returns>
        public int GetWidth()
        {
            return _width;
        }

        /// <summary>
        /// Gets the height of the texture in pixels.
        /// Returns the cached value for improved performance.
        /// </summary>
        /// <returns>The height of the texture in pixels.</returns>
        public int GetHeight()
        {
            return _height;
        }

        #endregion
    }
}