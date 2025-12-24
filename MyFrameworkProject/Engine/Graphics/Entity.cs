namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a game entity with sprite and animation.
    /// </summary>
    public class Entity
    {
        private static int _counter = 0;

        private readonly uint _id;
        private Sprite _sprite;

        private int _x = 0;
        private int _y = 0;

        private int _frameNumber = 0;
        private int _rotation = 0;

        private float _scaleX = 1f;
        private float _scaleY = 1f;

        private bool _animationEnabled = false;
        private bool _isPlaying = false;
        private bool _looping = false;

        private float _frameDuration = 0.1f;
        private float _elapsedTime = 0f;

        public Entity()
        {
            _id = (uint)++_counter;
        }

        public Entity(Sprite sprite) : this()
        {
            _sprite = sprite;
        }

        public void SetX(int newX) => _x = newX;
        public void SetY(int newY) => _y = newY;

        public void SetPosition(int newX, int newY)
        {
            _x = newX;
            _y = newY;
        }

        public void SetFrameNumber(int newFrameNumber)
        {
            _frameNumber = newFrameNumber;
        }

        public void SetRotation(int newRotation)
        {
            _rotation = newRotation;
        }

        public void SetScaleX(float newScaleX)
        {
            _scaleX = newScaleX;
        }

        public void SetScaleY(float newScaleY)
        {
            _scaleY = newScaleY;
        }

        public void SetScale(float newScaleX, float newScaleY)
        {
            _scaleX = newScaleX;
            _scaleY = newScaleY;
        }

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

        public void EnableAnimation(float frameDuration, bool loop = true)
        {
            _animationEnabled = true;
            _isPlaying = true;
            _looping = loop;
            _frameDuration = frameDuration;
            _elapsedTime = 0f;
        }

        public void PauseAnimation()
        {
            _isPlaying = false;
        }

        public void ResumeAnimation()
        {
            if (_animationEnabled)
                _isPlaying = true;
        }

        public void StopAnimation()
        {
            _isPlaying = false;
            _frameNumber = 0;
            _elapsedTime = 0f;
        }

        public void SetLooping(bool looping)
        {
            _looping = looping;
        }

        public void SetFrameDuration(float duration)
        {
            _frameDuration = duration;
        }

        public uint GetID() => _id;
        public Sprite GetSprite() => _sprite;
        public int GetX() => _x;
        public int GetY() => _y;
        public int GetFrameNumber() => _frameNumber;
        public int GetRotation() => _rotation;
        public float GetScaleX() => _scaleX;
        public float GetScaleY() => _scaleY;
    }
}