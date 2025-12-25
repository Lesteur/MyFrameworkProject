using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a game entity with sprite rendering and animation capabilities.
    /// Provides position, rotation, scale, and color manipulation along with frame-based animation support.
    /// </summary>
    public class Entity
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
            _id = (uint)++_counter;
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
        public void Update(float deltaTime)
        {
            if (!_animationEnabled || !_isPlaying || _sprite == null)
                return;

            _elapsedTime += deltaTime;

            if (_elapsedTime >= _frameDuration)
            {
                _elapsedTime = 0f;
                _frameNumber++;

                if (_frameNumber >= _sprite.GetFrameCount())
                {
                    if (_looping)
                        _frameNumber = 0;
                    else
                    {
                        _frameNumber = _sprite.GetFrameCount() - 1;
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
        /// <param name="newSprite">The new sprite to assign.</param>
        public void SetSprite(Sprite newSprite)
        {
            _sprite = newSprite;
        }

        /// <summary>
        /// Gets the sprite associated with this entity.
        /// </summary>
        /// <returns>The current sprite.</returns>
        public Sprite GetSprite() => _sprite;

        #endregion

        #region Public Methods - Position

        /// <summary>
        /// Sets the X-coordinate of the entity's position.
        /// </summary>
        /// <param name="newX">The new X-coordinate.</param>
        public void SetX(int newX) => _x = newX;

        /// <summary>
        /// Sets the Y-coordinate of the entity's position.
        /// </summary>
        /// <param name="newY">The new Y-coordinate.</param>
        public void SetY(int newY) => _y = newY;

        /// <summary>
        /// Sets the position of the entity.
        /// </summary>
        /// <param name="newX">The new X-coordinate.</param>
        /// <param name="newY">The new Y-coordinate.</param>
        public void SetPosition(int newX, int newY)
        {
            _x = newX;
            _y = newY;
        }

        /// <summary>
        /// Gets the X-coordinate of the entity's position.
        /// </summary>
        /// <returns>The current X-coordinate.</returns>
        public int GetX() => _x;

        /// <summary>
        /// Gets the Y-coordinate of the entity's position.
        /// </summary>
        /// <returns>The current Y-coordinate.</returns>
        public int GetY() => _y;

        #endregion

        #region Public Methods - Rotation

        /// <summary>
        /// Sets the rotation angle of the entity.
        /// </summary>
        /// <param name="newRotation">The new rotation angle in degrees.</param>
        public void SetRotation(int newRotation)
        {
            _rotation = newRotation;
        }

        /// <summary>
        /// Gets the rotation angle of the entity.
        /// </summary>
        /// <returns>The current rotation angle in degrees.</returns>
        public int GetRotation() => _rotation;

        #endregion

        #region Public Methods - Scale

        /// <summary>
        /// Sets the horizontal scale factor of the entity.
        /// </summary>
        /// <param name="newScaleX">The new horizontal scale factor.</param>
        public void SetScaleX(float newScaleX)
        {
            _scaleX = newScaleX;
        }

        /// <summary>
        /// Sets the vertical scale factor of the entity.
        /// </summary>
        /// <param name="newScaleY">The new vertical scale factor.</param>
        public void SetScaleY(float newScaleY)
        {
            _scaleY = newScaleY;
        }

        /// <summary>
        /// Sets the scale factors of the entity.
        /// </summary>
        /// <param name="newScaleX">The new horizontal scale factor.</param>
        /// <param name="newScaleY">The new vertical scale factor.</param>
        public void SetScale(float newScaleX, float newScaleY)
        {
            _scaleX = newScaleX;
            _scaleY = newScaleY;
        }

        /// <summary>
        /// Gets the horizontal scale factor of the entity.
        /// </summary>
        /// <returns>The current horizontal scale factor.</returns>
        public float GetScaleX() => _scaleX;

        /// <summary>
        /// Gets the vertical scale factor of the entity.
        /// </summary>
        /// <returns>The current vertical scale factor.</returns>
        public float GetScaleY() => _scaleY;

        #endregion

        #region Public Methods - Rendering

        /// <summary>
        /// Sets the sprite effects applied when rendering (e.g., flipping).
        /// </summary>
        /// <param name="newEffects">The sprite effects to apply.</param>
        public void SetSpriteEffects(SpriteEffects newEffects)
        {
            _spriteEffects = newEffects;
        }

        /// <summary>
        /// Sets the layer depth for rendering depth sorting.
        /// </summary>
        /// <param name="newLayerDepth">The new layer depth (0.0 = front, 1.0 = back).</param>
        public void SetLayerDepth(float newLayerDepth) => _layerDepth = newLayerDepth;

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
        public void SetVisible(bool visible)
        {
            _visible = visible;
        }

        /// <summary>
        /// Gets the sprite effects applied when rendering.
        /// </summary>
        /// <returns>The current sprite effects.</returns>
        public SpriteEffects GetSpriteEffects() => _spriteEffects;

        /// <summary>
        /// Gets the layer depth used for rendering depth sorting.
        /// </summary>
        /// <returns>The current layer depth.</returns>
        public float GetLayerDepth() => _layerDepth;

        /// <summary>
        /// Gets the color tint applied to the sprite.
        /// </summary>
        /// <returns>The current color tint.</returns>
        public Color GetColor() => _color;

        /// <summary>
        /// Gets whether the entity is visible and should be rendered.
        /// </summary>
        public bool IsVisible() => _visible;

        #endregion

        #region Public Methods - Animation

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
        /// <param name="newFrameNumber">The new frame number.</param>
        public void SetFrameNumber(int newFrameNumber)
        {
            _frameNumber = newFrameNumber;
        }

        /// <summary>
        /// Gets the current frame number being displayed in the animation sequence.
        /// </summary>
        /// <returns>The current frame number.</returns>
        public int GetFrameNumber() => _frameNumber;

        #endregion

        #region Public Methods - Identity

        /// <summary>
        /// Gets the unique identifier of this entity.
        /// </summary>
        /// <returns>The entity's unique identifier.</returns>
        public uint GetID() => _id;

        #endregion
    }
}