using System;

namespace MyFrameworkProject.Engine.Components.Collisions
{
    /// <summary>
    /// Represents a circular collision mask for radial collision detection.
    /// Defined by a center point (offset) and a radius.
    /// Ideal for circular objects, explosions, or radial area effects.
    /// </summary>
    public sealed class CircleCollision : CollisionMask
    {
        #region Fields - Dimensions

        /// <summary>
        /// The radius of the circle.
        /// </summary>
        private float _radius;

        #endregion

        #region Properties - Dimensions

        /// <summary>
        /// Gets or sets the radius of the circle.
        /// </summary>
        public float Radius
        {
            get => _radius;
            set => _radius = Math.Max(0f, value);
        }

        /// <summary>
        /// Gets the diameter of the circle (twice the radius).
        /// </summary>
        public float Diameter => _radius * 2f;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollision"/> class.
        /// Creates a circle with radius 1 at the origin.
        /// </summary>
        public CircleCollision() : base()
        {
            _radius = 1f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollision"/> class with the specified radius.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        public CircleCollision(float radius) : base()
        {
            _radius = Math.Max(0f, radius);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollision"/> class with the specified center and radius.
        /// </summary>
        /// <param name="offsetX">The X-coordinate of the circle's center.</param>
        /// <param name="offsetY">The Y-coordinate of the circle's center.</param>
        /// <param name="radius">The radius of the circle.</param>
        public CircleCollision(float offsetX, float offsetY, float radius) : base(offsetX, offsetY)
        {
            _radius = Math.Max(0f, radius);
        }

        #endregion

        #region Public Methods - Collision Detection

        /// <summary>
        /// Checks if this circle intersects with another collision mask.
        /// Delegates to the appropriate collision detection method based on the mask type.
        /// </summary>
        public override bool Intersects(CollisionMask other, float thisX, float thisY, float otherX, float otherY)
        {
            return other switch
            {
                PointCollision point => IntersectsPoint(point, thisX, thisY, otherX, otherY),
                LineCollision line => IntersectsLine(line, thisX, thisY, otherX, otherY),
                RectangleCollision rectangle => IntersectsRectangle(rectangle, thisX, thisY, otherX, otherY),
                CircleCollision circle => IntersectsCircle(circle, thisX, thisY, otherX, otherY),
                _ => false
            };
        }

        #endregion

        #region Internal Methods - Specific Collision Detection

        /// <summary>
        /// Checks if a point is inside this circle.
        /// Uses distance comparison for optimal performance.
        /// </summary>
        internal override bool IntersectsPoint(PointCollision point, float thisX, float thisY, float pointX, float pointY)
        {
            float centerX = thisX + _offsetX;
            float centerY = thisY + _offsetY;
            float px = pointX + point.OffsetX;
            float py = pointY + point.OffsetY;

            float dx = px - centerX;
            float dy = py - centerY;
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= _radius * _radius;
        }

        /// <summary>
        /// Checks if a line segment intersects this circle.
        /// Uses perpendicular distance from line to circle center.
        /// </summary>
        internal override bool IntersectsLine(LineCollision line, float thisX, float thisY, float lineX, float lineY)
        {
            float centerX = thisX + _offsetX;
            float centerY = thisY + _offsetY;

            float x1 = lineX + line.OffsetX;
            float y1 = lineY + line.OffsetY;
            float x2 = lineX + line.X2;
            float y2 = lineY + line.Y2;

            // Vector from line start to circle center
            float dx = centerX - x1;
            float dy = centerY - y1;

            // Line direction vector
            float ldx = x2 - x1;
            float ldy = y2 - y1;

            // Line length squared
            float lineLengthSquared = ldx * ldx + ldy * ldy;

            // Find closest point on line to circle center (clamped to segment)
            float t = Math.Clamp((dx * ldx + dy * ldy) / lineLengthSquared, 0f, 1f);

            // Closest point coordinates
            float closestX = x1 + t * ldx;
            float closestY = y1 + t * ldy;

            // Distance from closest point to circle center
            float distX = centerX - closestX;
            float distY = centerY - closestY;
            float distanceSquared = distX * distX + distY * distY;

            return distanceSquared <= _radius * _radius;
        }

        /// <summary>
        /// Checks if this circle intersects with a rectangle.
        /// Finds the closest point on the rectangle to the circle center.
        /// </summary>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, float thisX, float thisY, float rectX, float rectY)
        {
            float centerX = thisX + _offsetX;
            float centerY = thisY + _offsetY;

            float left = rectX + rectangle.OffsetX;
            float top = rectY + rectangle.OffsetY;
            float right = left + rectangle.Width;
            float bottom = top + rectangle.Height;

            // Find the closest point on the rectangle to the circle center
            float closestX = Math.Clamp(centerX, left, right);
            float closestY = Math.Clamp(centerY, top, bottom);

            // Calculate distance from circle center to closest point
            float dx = centerX - closestX;
            float dy = centerY - closestY;
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= _radius * _radius;
        }

        /// <summary>
        /// Checks if this circle intersects with another circle.
        /// Uses distance comparison between centers for optimal performance.
        /// </summary>
        internal override bool IntersectsCircle(CircleCollision circle, float thisX, float thisY, float circleX, float circleY)
        {
            float center1X = thisX + _offsetX;
            float center1Y = thisY + _offsetY;
            float center2X = circleX + circle._offsetX;
            float center2Y = circleY + circle._offsetY;

            float dx = center2X - center1X;
            float dy = center2Y - center1Y;
            float distanceSquared = dx * dx + dy * dy;

            float radiusSum = _radius + circle._radius;
            return distanceSquared <= radiusSum * radiusSum;
        }

        #endregion

        #region Public Methods - Configuration

        /// <summary>
        /// Sets the radius of the circle.
        /// </summary>
        /// <param name="radius">The new radius.</param>
        public void SetRadius(float radius)
        {
            _radius = Math.Max(0f, radius);
        }

        /// <summary>
        /// Sets the center position and radius of the circle.
        /// </summary>
        /// <param name="offsetX">The X-coordinate of the center.</param>
        /// <param name="offsetY">The Y-coordinate of the center.</param>
        /// <param name="radius">The radius.</param>
        public void SetCircle(float offsetX, float offsetY, float radius)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _radius = Math.Max(0f, radius);
        }

        #endregion
    }
}