using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Graphics.Shaders;

namespace MyFrameworkProject.Engine.Components
{
    /// <summary>
    /// Represents a base game entity with sprite rendering and animation capabilities.
    /// Provides position, rotation, scale, and color manipulation along with frame-based animation support.
    /// This abstract class serves as the foundation for all renderable game objects.
    /// </summary>
    public abstract class Entity
    {
        #region Static Fields - Identity

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
        /// The sprite associated with this entity for rendering.
        /// </summary>
        private Sprite _sprite;

        #endregion

        #region Fields - Transform

        /// <summary>
        /// The X-coordinate of the entity's position in world space.
        /// </summary>
        protected int _x = 0;

        /// <summary>
        /// The Y-coordinate of the entity's position in world space.
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

        #region Properties - Identity

        /// <summary>
        /// Gets the unique identifier of this entity.
        /// </summary>
        public uint ID => _id;

        #endregion

        #region Properties - Sprite

        /// <summary>
        /// Gets the sprite associated with this entity.
        /// </summary>
        public Sprite Sprite => _sprite;

        #endregion

        #region Properties - Shaders

        /// <summary>
        /// Gets the entity-specific shader effect, if any.
        /// </summary>
        public ShaderEffect Shader => _shader;

        #endregion

        #region Properties - Transform

        /// <summary>
        /// Gets the X-coordinate of the entity's position in world space.
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Gets the Y-coordinate of the entity's position in world space.
        /// </summary>
        public int Y => _y;

        /// <summary>
        /// Gets the position of the entity as a Vector2 in world space.
        /// </summary>
        public Vector2 Position => new(_x, _y);

        /// <summary>
        /// Gets the rotation angle of the entity in degrees.
        /// </summary>
        public int Rotation => _rotation;

        /// <summary>
        /// Gets the horizontal scale factor of the entity.
        /// </summary>
        public float ScaleX => _scaleX;

        /// <summary>
        /// Gets the vertical scale factor of the entity.
        /// </summary>
        public float ScaleY => _scaleY;

        #endregion

        #region Properties - Rendering

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

        #region Properties - Animation

        /// <summary>
        /// Gets the current frame number being displayed in the animation sequence.
        /// </summary>
        public int FrameNumber => _frameNumber;

        /// <summary>
        /// Gets whether the animation is currently playing.
        /// </summary>
        public bool IsAnimationPlaying => _isPlaying;

        /// <summary>
        /// Gets whether the animation is enabled.
        /// </summary>
        public bool IsAnimationEnabled => _animationEnabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class with a unique identifier.
        /// </summary>
        protected Entity()
        {
            _id = (uint)Interlocked.Increment(ref _counter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class with the specified sprite.
        /// </summary>
        /// <param name="sprite">The sprite to assign to this entity.</param>
        protected Entity(Sprite sprite) : this()
        {
            _sprite = sprite;
        }

        #endregion

        #region Public Methods - Sprite Management

        /// <summary>
        /// Sets the sprite for this entity.
        /// </summary>
        /// <param name="sprite">The new sprite to assign.</param>
        public void SetSprite(Sprite sprite)
        {
            _sprite = sprite;
        }

        #endregion

        #region Public Methods - Shader Management

        /// <summary>
        /// Sets an entity-specific shader effect that will be applied when rendering this entity.
        /// This shader overrides any global shaders for this entity.
        /// </summary>
        /// <param name="shader">The shader effect to apply, or null to use global shaders.</param>
        public void SetShader(ShaderEffect shader)
        {
            _shader = shader;
        }

        #endregion

        #region Public Methods - Transform

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
        /// Sets the position of the entity using a Vector2.
        /// </summary>
        /// <param name="position">The new position vector.</param>
        public void SetPosition(Vector2 position)
        {
            _x = (int)position.X;
            _y = (int)position.Y;
        }

        /// <summary>
        /// Moves the entity by the specified offset.
        /// </summary>
        /// <param name="dx">The horizontal offset.</param>
        /// <param name="dy">The vertical offset.</param>
        public void Move(int dx, int dy)
        {
            _x += dx;
            _y += dy;
        }

        /// <summary>
        /// Moves the entity by the specified offset vector.
        /// </summary>
        /// <param name="offset">The offset vector.</param>
        public void Move(Vector2 offset)
        {
            _x += (int)offset.X;
            _y += (int)offset.Y;
        }

        /// <summary>
        /// Sets the rotation angle of the entity in degrees.
        /// </summary>
        /// <param name="rotation">The new rotation angle in degrees.</param>
        public void SetRotation(int rotation)
        {
            _rotation = rotation;
        }

        /// <summary>
        /// Rotates the entity by the specified angle in degrees.
        /// </summary>
        /// <param name="angle">The angle to rotate by in degrees.</param>
        public void Rotate(int angle)
        {
            _rotation += angle;
        }

        /// <summary>
        /// Sets the horizontal scale factor of the entity.
        /// Negative values will flip the sprite horizontally.
        /// </summary>
        /// <param name="scaleX">The new horizontal scale factor.</param>
        public void SetScaleX(float scaleX)
        {
            if (scaleX < 0f)
            {
                _scaleX = -scaleX;
                _spriteEffects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _scaleX = scaleX;
                _spriteEffects = SpriteEffects.None;
            }
        }

        /// <summary>
        /// Sets the vertical scale factor of the entity.
        /// </summary>
        /// <param name="scaleY">The new vertical scale factor.</param>
        public void SetScaleY(float scaleY)
        {
            _scaleY = scaleY;
        }

        /// <summary>
        /// Sets the scale factors of the entity.
        /// Negative scaleX values will flip the sprite horizontally.
        /// </summary>
        /// <param name="scaleX">The new horizontal scale factor.</param>
        /// <param name="scaleY">The new vertical scale factor.</param>
        public void SetScale(float scaleX, float scaleY)
        {
            SetScaleX(scaleX);
            SetScaleY(scaleY);
        }

        /// <summary>
        /// Sets uniform scale for both X and Y axes.
        /// </summary>
        /// <param name="scale">The uniform scale factor.</param>
        public void SetScale(float scale)
        {
            SetScale(scale, scale);
        }

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
        public void SetLayerDepth(float layerDepth)
        {
            _layerDepth = layerDepth;
        }

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
        /// Draws the entity to the screen using the provided sprite batch.
        /// Renders the current frame of the sprite with all transform and rendering properties applied.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for rendering.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_sprite == null)
                return;

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

        #endregion

        #region Public Methods - Animation

        /// <summary>
        /// Updates the entity's animation state based on the elapsed time.
        /// Advances to the next frame when the frame duration has elapsed.
        /// This method should be called once per frame by the game loop.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void UpdateAnimation(float deltaTime)
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
        /// Disables animation and stops playback.
        /// </summary>
        public void DisableAnimation()
        {
            _animationEnabled = false;
            _isPlaying = false;
        }

        /// <summary>
        /// Starts or resumes the animation playback.
        /// </summary>
        public void PlayAnimation()
        {
            if (_animationEnabled)
                _isPlaying = true;
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
            if (_sprite != null && frameNumber >= 0 && frameNumber < _sprite.FrameCount)
            {
                _frameNumber = frameNumber;
            }
        }

        #endregion
    }
}