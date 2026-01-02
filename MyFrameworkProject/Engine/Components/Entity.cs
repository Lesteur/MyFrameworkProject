using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Graphics.Shaders;
using System.Threading;

namespace MyFrameworkProject.Engine.Components
{
    /// <summary>
    /// Represents a game entity with sprite rendering and animation capabilities.
    /// Provides position, rotation, scale, and color manipulation along with frame-based animation support.
    /// </summary>
    public abstract class Entity
    {
        #region Static Fields

        /// <summary>
        /// Global counter used to generate unique identifiers for each entity instance.
        /// </summary>
        private static int _counter = 0;

        #endregion

        #region Fields - Identity

        /// <summary>
        /// Unique identifier for this entity instance.
        /// </summary>
        private readonly uint _id;

        #endregion

        #region Fields - Sprite

        /// <summary>
        /// The sprite associated with this entity.
        /// </summary>
        private Sprite _sprite;

        #endregion

        #region Fields - Transform

        /// <summary>
        /// The X-coordinate of the entity's position.
        /// </summary>
        protected int _x = 0;

        /// <summary>
        /// The Y-coordinate of the entity's position.
        /// </summary>
        protected int _y = 0;

        /// <summary>
        /// The rotation angle of the entity in degrees.
        /// </summary>
        private int _rotation = 0;

        /// <summary>
        /// The horizontal scale factor applied to the entity's sprite.
        /// </summary>
        private float _scaleX = 1f;

        /// <summary>
        /// The vertical scale factor applied to the entity's sprite.
        /// </summary>
        private float _scaleY = 1f;

        #endregion

        #region Fields - Rendering

        /// <summary>
        /// Sprite effects applied when rendering (e.g., horizontal/vertical flip).
        /// </summary>
        private SpriteEffects _spriteEffects = SpriteEffects.None;

        /// <summary>
        /// The layer depth used for sorting during rendering (0.0 = front, 1.0 = back).
        /// </summary>
        private float _layerDepth = 0f;

        /// <summary>
        /// The color tint applied to the sprite when rendering.
        /// </summary>
        private Color _color = Color.White;

        /// <summary>
        /// Indicates whether the entity is visible and should be rendered.
        /// </summary>
        private bool _visible = true;

        #endregion

        #region Fields - Shaders

        /// <summary>
        /// Entity-specific shader effect that overrides global shaders for this entity.
        /// If null, global shaders will be applied instead.
        /// </summary>
        private ShaderEffect _shader;

        #endregion

        #region Fields - Animation

        /// <summary>
        /// Indicates whether animation is enabled for this entity.
        /// </summary>
        private bool _animationEnabled = false;

        /// <summary>
        /// Indicates whether the animation is currently playing.
        /// </summary>
        private bool _isPlaying = false;

        /// <summary>
        /// Indicates whether the animation should loop when reaching the last frame.
        /// </summary>
        private bool _looping = false;

        /// <summary>
        /// The current frame number being displayed in the animation sequence.
        /// </summary>
        private int _frameNumber = 0;

        /// <summary>
        /// The duration in seconds that each frame should be displayed.
        /// </summary>
        private float _frameDuration = 0.1f;

        /// <summary>
        /// The elapsed time since the last frame change.
        /// </summary>
        private float _elapsedTime = 0f;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class with a unique identifier.
        /// </summary>
        public Entity()
        {
            _id = (uint)Interlocked.Increment(ref _counter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class with the specified sprite.
        /// </summary>
        /// <param name="sprite">The sprite to assign to this entity.</param>
        public Entity(Sprite sprite) : this()
        {
            _sprite = sprite;
        }

        #endregion

        #region Public Methods - Update

        /// <summary>
        /// Updates the entity's animation state based on the elapsed time.
        /// Advances to the next frame when the frame duration has elapsed.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public void UpdateAnimation(float deltaTime)
        {
            if (!_animationEnabled || !_isPlaying || _sprite == null)
                return;

            _elapsedTime += deltaTime;

            if (_elapsedTime >= _frameDuration)
            {
                _elapsedTime -= _frameDuration; // Preserve overflow for smoother animation
                _frameNumber++;

                int frameCount = _sprite.FrameCount;
                if (_frameNumber >= frameCount)
                {
                    if (_looping)
                    {
                        _frameNumber = 0;
                    }
                    else
                    {
                        _frameNumber = frameCount - 1;
                        _isPlaying = false;
                    }
                }
            }
        }

        #endregion

        #region Public Methods - Sprite

        /// <summary>
        /// Sets the sprite for this entity.
        /// </summary>
        /// <param name="sprite">The new sprite to assign.</param>
        public void SetSprite(Sprite sprite)
        {
            _sprite = sprite;
        }

        /// <summary>
        /// Gets the sprite associated with this entity.
        /// </summary>
        public Sprite Sprite => _sprite;

        #endregion

        #region Public Methods - Shaders

        /// <summary>
        /// Sets an entity-specific shader effect that will be applied when rendering this entity.
        /// This shader overrides any global shaders for this entity.
        /// </summary>
        /// <param name="shader">The shader effect to apply, or null to use global shaders.</param>
        public void SetShader(ShaderEffect shader)
        {
            _shader = shader;
        }

        /// <summary>
        /// Gets the entity-specific shader effect, if any.
        /// </summary>
        public ShaderEffect Shader => _shader;

        #endregion

        #region Public Methods - Position

        /// <summary>
        /// Sets the X-coordinate of the entity's position.
        /// </summary>
        /// <param name="x">The new X-coordinate.</param>
        public void SetX(int x) => _x = x;

        /// <summary>
        /// Sets the Y-coordinate of the entity's position.
        /// </summary>
        /// <param name="y">The new Y-coordinate.</param>
        public void SetY(int y) => _y = y;

        /// <summary>
        /// Sets the position of the entity.
        /// </summary>
        /// <param name="x">The new X-coordinate.</param>
        /// <param name="y">The new Y-coordinate.</param>
        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets the X-coordinate of the entity's position.
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Gets the Y-coordinate of the entity's position.
        /// </summary>
        public int Y => _y;

        public Vector2 Position => new(_x, _y);

        #endregion

        #region Public Methods - Rotation

        /// <summary>
        /// Sets the rotation angle of the entity.
        /// </summary>
        /// <param name="rotation">The new rotation angle in degrees.</param>
        public void SetRotation(int rotation)
        {
            _rotation = rotation;
        }

        /// <summary>
        /// Gets the rotation angle of the entity.
        /// </summary>
        public int Rotation => _rotation;

        #endregion

        #region Public Methods - Scale

        /// <summary>
        /// Sets the horizontal scale factor of the entity.
        /// </summary>
        /// <param name="scaleX">The new horizontal scale factor.</param>
        public void SetScaleX(float scaleX)
        {
            if (scaleX > 0f)
                _scaleX = scaleX;
        }

        /// <summary>
        /// Sets the vertical scale factor of the entity.
        /// </summary>
        /// <param name="scaleY">The new vertical scale factor.</param>
        public void SetScaleY(float scaleY)
        {
            if (scaleY > 0f)
                _scaleY = scaleY;
        }

        /// <summary>
        /// Sets the scale factors of the entity.
        /// Optimized to set both values directly without extra method calls.
        /// </summary>
        /// <param name="scaleX">The new horizontal scale factor.</param>
        /// <param name="scaleY">The new vertical scale factor.</param>
        public void SetScale(float scaleX, float scaleY)
        {
            if (scaleX > 0f)
                _scaleX = scaleX;
            if (scaleY > 0f)
                _scaleY = scaleY;
        }

        /// <summary>
        /// Gets the horizontal scale factor of the entity.
        /// </summary>
        public float ScaleX => _scaleX;

        /// <summary>
        /// Gets the vertical scale factor of the entity.
        /// </summary>
        public float ScaleY => _scaleY;

        #endregion

        #region Public Methods - Rendering

        /// <summary>
        /// Sets the sprite effects applied when rendering (e.g., flipping).
        /// </summary>
        /// <param name="effects">The sprite effects to apply.</param>
        public void SetSpriteEffects(SpriteEffects effects)
        {
            _spriteEffects = effects;
        }

        /// <summary>
        /// Sets the layer depth for rendering depth sorting.
        /// </summary>
        /// <param name="layerDepth">The new layer depth (0.0 = front, 1.0 = back).</param>
        public void SetLayerDepth(float layerDepth) => _layerDepth = layerDepth;

        /// <summary>
        /// Sets the color tint applied to the sprite when rendering.
        /// </summary>
        /// <param name="color">The color tint to apply.</param>
        public void SetColor(Color color)
        {
            _color = color;
        }

        /// <summary>
        /// Sets the visibility of the entity.
        /// </summary>
        /// <param name="visible">True to make the entity visible, false to hide it.</param>
        public void SetVisible(bool visible)
        {
            _visible = visible;
        }

        /// <summary>
        /// Gets the sprite effects applied when rendering.
        /// </summary>
        public SpriteEffects SpriteEffects => _spriteEffects;

        /// <summary>
        /// Gets the layer depth used for rendering depth sorting.
        /// </summary>
        public float LayerDepth => _layerDepth;

        /// <summary>
        /// Gets the color tint applied to the sprite.
        /// </summary>
        public Color Color => _color;

        /// <summary>
        /// Gets whether the entity is visible and should be rendered.
        /// </summary>
        public bool Visible => _visible;

        #endregion

        #region Public Methods - Animation

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _sprite.Texture.NativeTexture,
                new Vector2(_x, _y),
                _sprite.GetSourceRectangle(_frameNumber),
                _color,
                MathHelper.ToRadians(_rotation),
                new Vector2(_sprite.XOrigin, _sprite.YOrigin),
                new Vector2(_scaleX, _scaleY),
                _spriteEffects,
                _layerDepth
            );
        }

        /// <summary>
        /// Enables animation with the specified frame duration and looping behavior.
        /// Starts playing the animation immediately.
        /// </summary>
        /// <param name="frameDuration">The duration in seconds that each frame should be displayed.</param>
        /// <param name="loop">Whether the animation should loop when reaching the last frame.</param>
        public void EnableAnimation(float frameDuration, bool loop = true)
        {
            _animationEnabled = true;
            _isPlaying = true;
            _looping = loop;
            _frameDuration = frameDuration;
            _elapsedTime = 0f;
        }

        /// <summary>
        /// Pauses the currently playing animation without resetting the frame number.
        /// </summary>
        public void PauseAnimation()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Resumes a paused animation from the current frame.
        /// </summary>
        public void ResumeAnimation()
        {
            if (_animationEnabled)
                _isPlaying = true;
        }

        /// <summary>
        /// Stops the animation and resets it to the first frame.
        /// </summary>
        public void StopAnimation()
        {
            _isPlaying = false;
            _frameNumber = 0;
            _elapsedTime = 0f;
        }

        /// <summary>
        /// Sets whether the animation should loop when reaching the last frame.
        /// </summary>
        /// <param name="looping">True to enable looping, false otherwise.</param>
        public void SetLooping(bool looping)
        {
            _looping = looping;
        }

        /// <summary>
        /// Sets the duration in seconds that each frame should be displayed.
        /// </summary>
        /// <param name="duration">The frame duration in seconds.</param>
        public void SetFrameDuration(float duration)
        {
            _frameDuration = duration;
        }

        /// <summary>
        /// Sets the current frame number in the animation sequence.
        /// </summary>
        /// <param name="frameNumber">The new frame number.</param>
        public void SetFrameNumber(int frameNumber)
        {
            _frameNumber = frameNumber;
        }

        /// <summary>
        /// Gets the current frame number being displayed in the animation sequence.
        /// </summary>
        public int FrameNumber => _frameNumber;

        #endregion

        #region Public Methods - Identity

        /// <summary>
        /// Gets the unique identifier of this entity.
        /// </summary>
        public uint ID => _id;

        #endregion
    }
}