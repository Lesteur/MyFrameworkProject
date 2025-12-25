using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D orthographic camera for MonoGame with translation, rotation, and zoom capabilities.
    /// Provides automatic transform matrix calculation and dirty-checking optimization for performance.
    /// The camera centers the view around its position and applies transformations relative to the viewport center.
    /// </summary>
    public sealed class Camera
    {
        #region Fields - Transform

        /// <summary>
        /// The position of the camera in world space.
        /// The camera centers the view around this position.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// The rotation angle of the camera in radians.
        /// Positive values rotate clockwise.
        /// </summary>
        private float _rotation = 0f;

        /// <summary>
        /// The zoom level of the camera.
        /// Values greater than 1.0 zoom in, values less than 1.0 zoom out.
        /// Minimum value is clamped to 0.1 to prevent invalid transformations.
        /// </summary>
        private float _zoom = 1f;

        #endregion

        #region Fields - Viewport

        /// <summary>
        /// The width of the viewport in pixels.
        /// </summary>
        private readonly int _viewportWidth;

        /// <summary>
        /// The height of the viewport in pixels.
        /// </summary>
        private readonly int _viewportHeight;

        #endregion

        #region Fields - Caching

        /// <summary>
        /// The cached transform matrix used for rendering.
        /// Recalculated only when the camera's transform properties change.
        /// </summary>
        private Matrix _transformMatrix = Matrix.Identity;

        /// <summary>
        /// Indicates whether the transform matrix needs to be recalculated.
        /// Set to true whenever position, rotation, or zoom changes.
        /// </summary>
        private bool _dirty = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current position of the camera in world space.
        /// </summary>
        public Vector2 Position => _position;

        /// <summary>
        /// Gets the current rotation angle of the camera in radians.
        /// </summary>
        public float Rotation => _rotation;

        /// <summary>
        /// Gets the current zoom level of the camera.
        /// </summary>
        public float Zoom => _zoom;

        /// <summary>
        /// Gets the width of the viewport in pixels.
        /// </summary>
        public int ViewportWidth => _viewportWidth;

        /// <summary>
        /// Gets the height of the viewport in pixels.
        /// </summary>
        public int ViewportHeight => _viewportHeight;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class with the specified position and viewport dimensions.
        /// </summary>
        /// <param name="x">The initial X-coordinate of the camera position in world space.</param>
        /// <param name="y">The initial Y-coordinate of the camera position in world space.</param>
        /// <param name="viewportWidth">The width of the viewport in pixels.</param>
        /// <param name="viewportHeight">The height of the viewport in pixels.</param>
        public Camera(int x, int y, int viewportWidth, int viewportHeight)
        {
            _position = new Vector2(x, y);
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }

        #endregion

        #region Public Methods - Position

        /// <summary>
        /// Sets the camera position to the specified coordinates in world space.
        /// Only updates and marks the transform as dirty if the position actually changes.
        /// </summary>
        /// <param name="x">The new X-coordinate of the camera position.</param>
        /// <param name="y">The new Y-coordinate of the camera position.</param>
        public void SetPosition(float x, float y)
        {
            if (_position.X == x && _position.Y == y)
                return;

            _position.X = x;
            _position.Y = y;
            _dirty = true;
        }

        /// <summary>
        /// Moves the camera by the specified offset in world space.
        /// Only updates and marks the transform as dirty if the offset is non-zero.
        /// </summary>
        /// <param name="dx">The horizontal offset to move the camera.</param>
        /// <param name="dy">The vertical offset to move the camera.</param>
        public void Move(float dx, float dy)
        {
            if (dx == 0f && dy == 0f)
                return;

            _position.X += dx;
            _position.Y += dy;
            _dirty = true;
        }

        #endregion

        #region Public Methods - Rotation

        /// <summary>
        /// Sets the camera rotation to the specified angle in radians.
        /// Only updates and marks the transform as dirty if the rotation actually changes.
        /// </summary>
        /// <param name="rotation">The new rotation angle in radians.</param>
        public void SetRotation(float rotation)
        {
            if (_rotation == rotation)
                return;

            _rotation = rotation;
            _dirty = true;
        }

        /// <summary>
        /// Rotates the camera by the specified angle in radians.
        /// Only updates and marks the transform as dirty if the angle is non-zero.
        /// </summary>
        /// <param name="angle">The angle to rotate the camera by, in radians.</param>
        public void Rotate(float angle)
        {
            if (angle == 0f)
                return;

            _rotation += angle;
            _dirty = true;
        }

        #endregion

        #region Public Methods - Zoom

        /// <summary>
        /// Sets the camera zoom level to the specified value.
        /// The zoom is clamped to a minimum of 0.1 to prevent invalid transformations.
        /// Only updates and marks the transform as dirty if the zoom actually changes.
        /// </summary>
        /// <param name="zoom">The new zoom level. Values greater than 1.0 zoom in, less than 1.0 zoom out.</param>
        public void SetZoom(float zoom)
        {
            if (_zoom == zoom)
                return;

            _zoom = MathHelper.Max(zoom, 0.1f);
            _dirty = true;
        }

        #endregion

        #region Public Methods - Transform

        /// <summary>
        /// Gets the transform matrix for this camera.
        /// The matrix is recalculated only if the camera's properties have changed since the last call.
        /// This matrix can be applied to a SpriteBatch to render sprites from the camera's perspective.
        /// </summary>
        /// <returns>The camera's transform matrix.</returns>
        public Matrix GetTransformMatrix()
        {
            if (_dirty)
                Recalculate();

            return _transformMatrix;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recalculates the transform matrix based on the current camera properties.
        /// The transformation is applied in the following order:
        /// 1. Translate the world so the camera position is at the origin
        /// 2. Apply rotation around the camera center
        /// 3. Apply zoom scaling
        /// 4. Translate back to the center of the viewport
        /// </summary>
        private void Recalculate()
        {
            _transformMatrix =
                Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0f)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_zoom, _zoom, 1f) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0f));

            _dirty = false;
        }

        #endregion
    }
}