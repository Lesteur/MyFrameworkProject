using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D sprite with frame-based rendering capabilities.
    /// Supports sprite sheets with multiple frames arranged horizontally and customizable origin points.
    /// </summary>
    public class Sprite
    {
        #region Fields - Texture

        /// <summary>
        /// The texture containing the sprite image data.
        /// </summary>
        protected Texture _texture;

        #endregion

        #region Fields - Dimensions

        /// <summary>
        /// The total width of the sprite texture in pixels.
        /// </summary>
        protected int _width;

        /// <summary>
        /// The total height of the sprite texture in pixels.
        /// </summary>
        protected int _height;

        #endregion

        #region Fields - Frame

        /// <summary>
        /// The number of frames in the sprite sheet.
        /// </summary>
        protected int _frameCount;

        /// <summary>
        /// The width of a single frame in pixels.
        /// </summary>
        protected int _frameWidth;

        /// <summary>
        /// The height of a single frame in pixels.
        /// </summary>
        protected int _frameHeight;

        #endregion

        #region Fields - Origin

        /// <summary>
        /// The X-coordinate of the sprite's origin point (pivot point for rotation and positioning).
        /// </summary>
        protected int _xOrigin;

        /// <summary>
        /// The Y-coordinate of the sprite's origin point (pivot point for rotation and positioning).
        /// </summary>
        protected int _yOrigin;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class with default values.
        /// Sets the frame count to 1.
        /// </summary>
        public Sprite()
        {
            _frameCount = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class with the specified texture and frame count.
        /// Frames are assumed to be arranged horizontally in the texture.
        /// </summary>
        /// <param name="texture">The texture containing the sprite image data.</param>
        /// <param name="frameCount">The number of frames in the sprite sheet. Default is 1.</param>
        public Sprite(Texture texture, int frameCount = 1)
        {
            _texture = texture;
            _frameCount = frameCount;

            _width = texture.Width;
            _height = texture.Height;

            _frameWidth = _width / frameCount;
            _frameHeight = _height;

            _xOrigin = 0;
            _yOrigin = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class with the specified texture, origin point, and frame count.
        /// Frames are assumed to be arranged horizontally in the texture.
        /// </summary>
        /// <param name="texture">The texture containing the sprite image data.</param>
        /// <param name="xOrigin">The X-coordinate of the sprite's origin point.</param>
        /// <param name="yOrigin">The Y-coordinate of the sprite's origin point.</param>
        /// <param name="frameCount">The number of frames in the sprite sheet. Default is 1.</param>
        public Sprite(Texture texture, int xOrigin, int yOrigin, int frameCount = 1)
            : this(texture, frameCount)
        {
            _xOrigin = xOrigin;
            _yOrigin = yOrigin;
        }

        #endregion

        #region Public Methods - Texture

        /// <summary>
        /// Gets the texture containing the sprite image data.
        /// </summary>
        public Texture Texture => _texture;

        #endregion

        #region Public Methods - Dimensions

        /// <summary>
        /// Gets the total width of the sprite texture in pixels.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the total height of the sprite texture in pixels.
        /// </summary>
        public int Height => _height;

        #endregion

        #region Public Methods - Frame

        /// <summary>
        /// Gets the number of frames in the sprite sheet.
        /// </summary>
        public int FrameCount => _frameCount;

        /// <summary>
        /// Gets the width of a single frame in pixels.
        /// </summary>
        public int FrameWidth => _frameWidth;

        /// <summary>
        /// Gets the height of a single frame in pixels.
        /// </summary>
        public int FrameHeight => _frameHeight;

        /// <summary>
        /// Calculates and returns the source rectangle for the specified frame number.
        /// The frame number wraps around using modulo to prevent out-of-bounds access.
        /// </summary>
        /// <param name="frameNumber">The zero-based index of the frame to retrieve.</param>
        /// <returns>A <see cref="Rectangle"/> representing the source area in the texture for the specified frame.</returns>
        public Rectangle GetSourceRectangle(int frameNumber)
        {
            frameNumber %= _frameCount;

            return new Rectangle(
                frameNumber * _frameWidth,
                0,
                _frameWidth,
                _frameHeight
            );
        }

        #endregion

        #region Public Methods - Origin

        /// <summary>
        /// Gets the X-coordinate of the sprite's origin point.
        /// The origin point is used as the pivot for rotation and positioning.
        /// </summary>
        public int XOrigin => _xOrigin;

        /// <summary>
        /// Gets the Y-coordinate of the sprite's origin point.
        /// The origin point is used as the pivot for rotation and positioning.
        /// </summary>
        public int YOrigin => _yOrigin;

        /// <summary>
        /// Gets the sprite's origin point as a <see cref="Vector2"/>.
        /// The origin point is used as the pivot for rotation and positioning.
        /// </summary>
        public Vector2 Origin => new(_xOrigin, _yOrigin);

        #endregion
    }
}