using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Components.Collisions
{
    /// <summary>
    /// Abstract base class for all collision masks.
    /// Provides common functionality for collision detection with position and origin point support.
    /// Collision masks can be attached to game objects to enable 2D physics and collision detection.
    /// </summary>
    public abstract class CollisionMask
    {
        #region Fields - Transform

        /// <summary>
        /// The X-coordinate offset from the parent entity's position.
        /// </summary>
        protected float _offsetX;

        /// <summary>
        /// The Y-coordinate offset from the parent entity's position.
        /// </summary>
        protected float _offsetY;

        #endregion

        #region Properties - Transform

        /// <summary>
        /// Gets or sets the X-coordinate offset from the parent entity's position.
        /// </summary>
        public float OffsetX
        {
            get => _offsetX;
            set => _offsetX = value;
        }

        /// <summary>
        /// Gets or sets the Y-coordinate offset from the parent entity's position.
        /// </summary>
        public float OffsetY
        {
            get => _offsetY;
            set => _offsetY = value;
        }

        /// <summary>
        /// Gets or sets the offset as a Vector2.
        /// </summary>
        public Vector2 Offset
        {
            get => new(_offsetX, _offsetY);
            set
            {
                _offsetX = value.X;
                _offsetY = value.Y;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionMask"/> class with default offset (0, 0).
        /// </summary>
        protected CollisionMask()
        {
            _offsetX = 0f;
            _offsetY = 0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionMask"/> class with the specified offset.
        /// </summary>
        /// <param name="offsetX">The X-coordinate offset.</param>
        /// <param name="offsetY">The Y-coordinate offset.</param>
        protected CollisionMask(float offsetX, float offsetY)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        #endregion

        #region Public Methods - Collision Detection

        /// <summary>
        /// Checks if this collision mask intersects with another collision mask.
        /// Uses double dispatch pattern to delegate to specific collision detection methods.
        /// </summary>
        /// <param name="other">The other collision mask to check against.</param>
        /// <param name="thisX">The world X-coordinate of this collision mask.</param>
        /// <param name="thisY">The world Y-coordinate of this collision mask.</param>
        /// <param name="otherX">The world X-coordinate of the other collision mask.</param>
        /// <param name="otherY">The world Y-coordinate of the other collision mask.</param>
        /// <returns>True if the collision masks intersect; otherwise, false.</returns>
        public abstract bool Intersects(CollisionMask other, float thisX, float thisY, float otherX, float otherY);

        /// <summary>
        /// Checks collision with a point collision mask.
        /// </summary>
        /// <param name="point">The point collision mask.</param>
        /// <param name="thisX">The world X-coordinate of this collision mask.</param>
        /// <param name="thisY">The world Y-coordinate of this collision mask.</param>
        /// <param name="pointX">The world X-coordinate of the point.</param>
        /// <param name="pointY">The world Y-coordinate of the point.</param>
        /// <returns>True if this mask contains the point; otherwise, false.</returns>
        internal abstract bool IntersectsPoint(PointCollision point, float thisX, float thisY, float pointX, float pointY);

        /// <summary>
        /// Checks collision with a line collision mask.
        /// </summary>
        /// <param name="line">The line collision mask.</param>
        /// <param name="thisX">The world X-coordinate of this collision mask.</param>
        /// <param name="thisY">The world Y-coordinate of this collision mask.</param>
        /// <param name="lineX">The world X-coordinate of the line.</param>
        /// <param name="lineY">The world Y-coordinate of the line.</param>
        /// <returns>True if this mask intersects the line; otherwise, false.</returns>
        internal abstract bool IntersectsLine(LineCollision line, float thisX, float thisY, float lineX, float lineY);

        /// <summary>
        /// Checks collision with a rectangle collision mask.
        /// </summary>
        /// <param name="rectangle">The rectangle collision mask.</param>
        /// <param name="thisX">The world X-coordinate of this collision mask.</param>
        /// <param name="thisY">The world Y-coordinate of this collision mask.</param>
        /// <param name="rectX">The world X-coordinate of the rectangle.</param>
        /// <param name="rectY">The world Y-coordinate of the rectangle.</param>
        /// <returns>True if this mask intersects the rectangle; otherwise, false.</returns>
        internal abstract bool IntersectsRectangle(RectangleCollision rectangle, float thisX, float thisY, float rectX, float rectY);

        /// <summary>
        /// Checks collision with a circle collision mask.
        /// </summary>
        /// <param name="circle">The circle collision mask.</param>
        /// <param name="thisX">The world X-coordinate of this collision mask.</param>
        /// <param name="thisY">The world Y-coordinate of this collision mask.</param>
        /// <param name="circleX">The world X-coordinate of the circle.</param>
        /// <param name="circleY">The world Y-coordinate of the circle.</param>
        /// <returns>True if this mask intersects the circle; otherwise, false.</returns>
        internal abstract bool IntersectsCircle(CircleCollision circle, float thisX, float thisY, float circleX, float circleY);

        #endregion

        #region Public Methods - Offset Management

        /// <summary>
        /// Sets the offset of this collision mask.
        /// </summary>
        /// <param name="offsetX">The X-coordinate offset.</param>
        /// <param name="offsetY">The Y-coordinate offset.</param>
        public void SetOffset(float offsetX, float offsetY)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        /// <summary>
        /// Sets the offset of this collision mask using a Vector2.
        /// </summary>
        /// <param name="offset">The offset vector.</param>
        public void SetOffset(Vector2 offset)
        {
            _offsetX = offset.X;
            _offsetY = offset.Y;
        }

        #endregion
    }
}