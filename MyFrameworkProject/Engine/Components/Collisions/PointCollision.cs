using System;

namespace MyFrameworkProject.Engine.Components.Collisions
{
    /// <summary>
    /// Represents a point collision mask for precise collision detection.
    /// The simplest collision primitive, representing a single point in 2D space.
    /// Useful for projectiles, pickup detection, or precise interaction points.
    /// <param name="offsetX">The X-coordinate offset from the parent entity. Defaults to 0.</param>
    /// <param name="offsetY">The Y-coordinate offset from the parent entity. Defaults to 0.</param>
    public sealed class PointCollision(float offsetX = 0f, float offsetY = 0f) : CollisionMask(offsetX, offsetY)
    {
        #region Internal Methods - Collision Detection

        /// <summary>
        /// Checks if this point intersects with another collision mask.
        /// Delegates to the appropriate collision detection method based on the mask type.
        /// </summary>
        /// <param name="other">The other collision mask to check against.</param>
        /// <param name="thisX">The world X-coordinate of this point.</param>
        /// <param name="thisY">The world Y-coordinate of this point.</param>
        /// <param name="otherX">The world X-coordinate of the other collision mask.</param>
        /// <param name="otherY">The world Y-coordinate of the other collision mask.</param>
        /// <returns>True if the point intersects the other mask; otherwise, false.</returns>
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

        #region Protected Methods - Specific Collision Detection

        /// <summary>
        /// Checks if this point coincides with another point.
        /// </summary>
        internal override bool IntersectsPoint(PointCollision point, float thisX, float thisY, float pointX, float pointY)
        {
            float x1 = thisX + _offsetX;
            float y1 = thisY + _offsetY;
            float x2 = pointX + point._offsetX;
            float y2 = pointY + point._offsetY;

            return Math.Abs(x1 - x2) < float.Epsilon && Math.Abs(y1 - y2) < float.Epsilon;
        }

        /// <summary>
        /// Checks if this point lies on a line segment.
        /// </summary>
        internal override bool IntersectsLine(LineCollision line, float thisX, float thisY, float lineX, float lineY)
        {
            // Delegate to line's point collision detection
            return line.IntersectsPoint(this, lineX, lineY, thisX, thisY);
        }

        /// <summary>
        /// Checks if this point is inside a rectangle.
        /// </summary>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, float thisX, float thisY, float rectX, float rectY)
        {
            // Delegate to rectangle's point collision detection
            return rectangle.IntersectsPoint(this, rectX, rectY, thisX, thisY);
        }

        /// <summary>
        /// Checks if this point is inside a circle.
        /// </summary>
        internal override bool IntersectsCircle(CircleCollision circle, float thisX, float thisY, float circleX, float circleY)
        {
            // Delegate to circle's point collision detection
            return circle.IntersectsPoint(this, circleX, circleY, thisX, thisY);
        }

        #endregion
    }
}