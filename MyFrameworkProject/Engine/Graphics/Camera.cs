using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// 2D orthographic camera for MonoGame with translation, rotation and zoom support.
    /// Handles virtual resolution scaling automatically.
    /// </summary>
    public sealed class Camera(int x, int y, int viewportWidth, int viewportHeight)
    {
        private Vector2 _position = new(x, y);
        private float _rotation = 0f;
        private float _zoom = 1f;
        private readonly int _viewportWidth = viewportWidth;
        private readonly int _viewportHeight = viewportHeight;

        private Matrix _transformMatrix = Matrix.Identity;
        private bool _dirty = true;

        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public float Zoom => _zoom;
        public int ViewportWidth => _viewportWidth;
        public int ViewportHeight => _viewportHeight;

        public void SetPosition(float x, float y)
        {
            if (_position.X == x && _position.Y == y)
                return;

            _position.X = x;
            _position.Y = y;
            _dirty = true;
        }

        public void Move(float dx, float dy)
        {
            if (dx == 0f && dy == 0f)
                return;

            _position.X += dx;
            _position.Y += dy;
            _dirty = true;
        }

        public void SetRotation(float rotation)
        {
            if (_rotation == rotation)
                return;

            _rotation = rotation;
            _dirty = true;
        }

        public void Rotate(float angle)
        {
            if (angle == 0f)
                return;

            _rotation += angle;
            _dirty = true;
        }

        public void SetZoom(float zoom)
        {
            if (_zoom == zoom)
                return;

            _zoom = MathHelper.Max(zoom, 0.1f);
            _dirty = true;
        }

        public Matrix GetTransformMatrix()
        {
            if (_dirty)
                Recalculate();

            return _transformMatrix;
        }

        private void Recalculate()
        {
            // 1. Translate world so camera position is at origin
            // 2. Apply rotation around camera center
            // 3. Apply zoom
            // 4. Translate back to center of viewport
            _transformMatrix =
                Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0f)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_zoom, _zoom, 1f) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0f));

            _dirty = false;
        }
    }
}