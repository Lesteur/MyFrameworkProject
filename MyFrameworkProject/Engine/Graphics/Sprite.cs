using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D sprite with frame-based rendering.
    /// </summary>
    public class Sprite
    {
        protected Texture _texture;

        protected int _xOrigin;
        protected int _yOrigin;

        protected int _width;
        protected int _height;

        protected int _frameCount;
        protected int _frameWidth;
        protected int _frameHeight;

        public Sprite()
        {
            _frameCount = 1;
        }

        public Sprite(Texture texture, int frameCount = 1)
        {
            _texture = texture;
            _frameCount = frameCount;

            _width = texture.GetWidth();
            _height = texture.GetHeight();

            _frameWidth = _width / frameCount;
            _frameHeight = _height;

            _xOrigin = 0;
            _yOrigin = 0;
        }

        public Sprite(Texture texture, int xOrigin, int yOrigin, int frameCount = 1)
            : this(texture, frameCount)
        {
            _xOrigin = xOrigin;
            _yOrigin = yOrigin;
        }

        public Texture GetTexture()
        {
            return _texture;
        }

        public int GetXOrigin()
        {
            return _xOrigin;
        }

        public int GetYOrigin()
        {
            return _yOrigin;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public int GetFrameCount()
        {
            return _frameCount;
        }

        public int GetFrameWidth()
        {
            return _frameWidth;
        }

        public int GetFrameHeight()
        {
            return _frameHeight;
        }

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

        public Vector2 GetOrigin()
        {
            return new Vector2(_xOrigin, _yOrigin);
        }
    }
}