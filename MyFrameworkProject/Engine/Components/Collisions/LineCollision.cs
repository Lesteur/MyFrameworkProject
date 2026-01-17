using System;

namespace MyFrameworkProject.Engine.Components.Collisions
{
    /// <summary>
    /// Represents a line segment collision mask for linear collision detection.
    /// Defined by a starting point and an ending point, forming a line segment in 2D space.
    /// Useful for raycasting, wall collision, or sweep detection.
    /// </summary>
    public sealed class LineCollision : CollisionMask
    {
        #region Fields - Line Definition

        /// <summary>
        /// The X-coordinate of the line's end point relative to the offset.
        /// </summary>
        private float _x2;

        /// <summary>
        /// The Y-coordinate of the line's end point relative to the offset.
        /// </summary>
        private float _y2;

        #endregion

        #region Properties - Line Definition

        /// <summary>
        /// Gets or sets the X-coordinate of the line's end point.
        /// </summary>
        public float X2
        {
            get => _x2;
            set => _x2 = value;
        }

        /// <summary>
        /// Gets or sets the Y-coordinate of the line's end point.
        /// </summary>
        public float Y2
        {
            get => _y2;
            set => _y2 = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineCollision"/> class.
        /// The line starts at (offsetX, offsetY) and ends at (x2, y2).
        /// </summary>
        /// <param name="offsetX">The X-coordinate of the line's start point. Defaults to 0.</param>
        /// <param name="offsetY">The Y-coordinate of the line's start point. Defaults to 0.</param>
        /// <param name="x2">The X-coordinate of the line's end point. Defaults to 0.</param>
        /// <param name="y2">The Y-coordinate of the line's end point. Defaults to 0.</param>
        public LineCollision(float offsetX = 0f, float offsetY = 0f, float x2 = 0f, float y2 = 0f) : base(offsetX, offsetY)
        {
            _x2 = x2;
            _y2 = y2;
        }

        #endregion

        #region Public Methods - Collision Detection

        /// <summary>
        /// Checks if this line intersects with another collision mask.
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
        /// Checks if a point lies on this line segment.
        /// Uses distance formula to check if point is on the line within epsilon tolerance.
        /// </summary>
        internal override bool IntersectsPoint(PointCollision point, float thisX, float thisY, float pointX, float pointY)
        {
            float x1 = thisX + _offsetX;
            float y1 = thisY + _offsetY;
            float x2 = thisX + _x2;
            float y2 = thisY + _y2;
            float px = pointX + point.OffsetX;
            float py = pointY + point.OffsetY;

            // Calculate distances
            float lineLength = MathF.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            float d1 = MathF.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));
            float d2 = MathF.Sqrt((px - x2) * (px - x2) + (py - y2) * (py - y2));

            // Check if sum of distances equals line length (within epsilon)
            return Math.Abs((d1 + d2) - lineLength) < 0.01f;
        }

        /// <summary>
        /// Checks if this line intersects with another line segment.
        /// Uses parametric line intersection formula.
        /// </summary>
        internal override bool IntersectsLine(LineCollision line, float thisX, float thisY, float lineX, float lineY)
        {
            float x1 = thisX + _offsetX;
            float y1 = thisY + _offsetY;
            float x2 = thisX + _x2;
            float y2 = thisY + _y2;
            
            float x3 = lineX + line._offsetX;
            float y3 = lineY + line._offsetY;
            float x4 = lineX + line._x2;
            float y4 = lineY + line._y2;

            // Calculate the denominator for the intersection formula
            float denominator = ((x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4));

            // Lines are parallel if denominator is zero
            if (Math.Abs(denominator) < float.Epsilon)
                return false;

            // Calculate intersection parameters
            float t = (((x1 - x3) * (y3 - y4)) - ((y1 - y3) * (x3 - x4))) / denominator;
            float u = -(((x1 - x2) * (y1 - y3)) - ((y1 - y2) * (x1 - x3))) / denominator;

            // Check if intersection point is within both line segments
            return t >= 0f && t <= 1f && u >= 0f && u <= 1f;
        }

        /// <summary>
        /// Checks if this line intersects with a rectangle.
        /// Tests intersection against all four edges of the rectangle.
        /// </summary>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, float thisX, float thisY, float rectX, float rectY)
        {
            // Delegate to rectangle's line collision detection
            return rectangle.IntersectsLine(this, rectX, rectY, thisX, thisY);
        }

        /// <summary>
        /// Checks if this line intersects with a circle.
        /// Uses distance from line to circle center formula.
        /// </summary>
        internal override bool IntersectsCircle(CircleCollision circle, float thisX, float thisY, float circleX, float circleY)
        {
            // Delegate to circle's line collision detection
            return circle.IntersectsLine(this, circleX, circleY, thisX, thisY);
        }

        #endregion

        #region Public Methods - Configuration

        /// <summary>
        /// Sets the endpoints of the line segment.
        /// </summary>
        /// <param name="x1">The X-coordinate of the start point.</param>
        /// <param name="y1">The Y-coordinate of the start point.</param>
        /// <param name="x2">The X-coordinate of the end point.</param>
        /// <param name="y2">The Y-coordinate of the end point.</param>
        public void SetLine(float x1, float y1, float x2, float y2)
        {
            _offsetX = x1;
            _offsetY = y1;
            _x2 = x2;
            _y2 = y2;
        }

        #endregion
    }
}