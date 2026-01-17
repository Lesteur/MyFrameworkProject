using System;

namespace MyFrameworkProject.Engine.Components.Collisions
{
    /// <summary>
    /// Represents an axis-aligned rectangle collision mask for bounding box collision detection.
    /// Defined by width and height, with the offset representing the top-left corner.
    /// Highly efficient for most game collision scenarios.
    /// </summary>
    public sealed class RectangleCollision : CollisionMask
    {
        #region Fields - Dimensions

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        private float _width;

        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        private float _height;

        #endregion

        #region Properties - Dimensions

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public float Width
        {
            get => _width;
            set => _width = Math.Max(0f, value);
        }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public float Height
        {
            get => _height;
            set => _height = Math.Max(0f, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleCollision"/> class.
        /// </summary>
        /// <param name="offsetX">The X-coordinate offset (left edge). Defaults to 0.</param>
        /// <param name="offsetY">The Y-coordinate offset (top edge). Defaults to 0.</param>
        /// <param name="width">The width of the rectangle. Defaults to 1.</param>
        /// <param name="height">The height of the rectangle. Defaults to 1.</param>
        public RectangleCollision(float offsetX = 0f, float offsetY = 0f, float width = 1f, float height = 1f) : base(offsetX, offsetY)
        {
            _width = Math.Max(0f, width);
            _height = Math.Max(0f, height);
        }

        #endregion

        #region Public Methods - Collision Detection

        /// <summary>
        /// Checks if this rectangle intersects with another collision mask.
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
        /// Checks if a point is inside this rectangle.
        /// Uses simple bounds checking for optimal performance.
        /// </summary>
        internal override bool IntersectsPoint(PointCollision point, float thisX, float thisY, float pointX, float pointY)
        {
            float left = thisX + _offsetX;
            float top = thisY + _offsetY;
            float right = left + _width;
            float bottom = top + _height;

            float px = pointX + point.OffsetX;
            float py = pointY + point.OffsetY;

            return px >= left && px <= right && py >= top && py <= bottom;
        }

        /// <summary>
        /// Checks if a line segment intersects this rectangle.
        /// Tests if either endpoint is inside, or if the line crosses any edge.
        /// </summary>
        internal override bool IntersectsLine(LineCollision line, float thisX, float thisY, float lineX, float lineY)
        {
            float left = thisX + _offsetX;
            float top = thisY + _offsetY;
            float right = left + _width;
            float bottom = top + _height;

            float x1 = lineX + line.OffsetX;
            float y1 = lineY + line.OffsetY;
            float x2 = lineX + line.X2;
            float y2 = lineY + line.Y2;

            // Check if either endpoint is inside the rectangle
            if ((x1 >= left && x1 <= right && y1 >= top && y1 <= bottom) ||
                (x2 >= left && x2 <= right && y2 >= top && y2 <= bottom))
                return true;

            // Check intersection with each edge of the rectangle
            LineCollision topEdge = new(left, top, right, top);
            LineCollision bottomEdge = new(left, bottom, right, bottom);
            LineCollision leftEdge = new(left, top, left, bottom);
            LineCollision rightEdge = new(right, top, right, bottom);

            return line.IntersectsLine(topEdge, lineX, lineY, 0, 0) ||
                   line.IntersectsLine(bottomEdge, lineX, lineY, 0, 0) ||
                   line.IntersectsLine(leftEdge, lineX, lineY, 0, 0) ||
                   line.IntersectsLine(rightEdge, lineX, lineY, 0, 0);
        }

        /// <summary>
        /// Checks if this rectangle intersects with another rectangle.
        /// Uses axis-aligned bounding box (AABB) collision detection for optimal performance.
        /// </summary>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, float thisX, float thisY, float rectX, float rectY)
        {
            float left1 = thisX + _offsetX;
            float top1 = thisY + _offsetY;
            float right1 = left1 + _width;
            float bottom1 = top1 + _height;

            float left2 = rectX + rectangle._offsetX;
            float top2 = rectY + rectangle._offsetY;
            float right2 = left2 + rectangle._width;
            float bottom2 = top2 + rectangle._height;

            // AABB collision detection
            return !(right1 < left2 || left1 > right2 || bottom1 < top2 || top1 > bottom2);
        }

        /// <summary>
        /// Checks if this rectangle intersects with a circle.
        /// Uses clamping to find the closest point on the rectangle to the circle center.
        /// </summary>
        internal override bool IntersectsCircle(CircleCollision circle, float thisX, float thisY, float circleX, float circleY)
        {
            // Delegate to circle's rectangle collision detection
            return circle.IntersectsRectangle(this, circleX, circleY, thisX, thisY);
        }

        #endregion

        #region Public Methods - Configuration

        /// <summary>
        /// Sets the dimensions of the rectangle.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public void SetSize(float width, float height)
        {
            _width = Math.Max(0f, width);
            _height = Math.Max(0f, height);
        }

        /// <summary>
        /// Sets the position and dimensions of the rectangle.
        /// </summary>
        /// <param name="offsetX">The X-coordinate offset.</param>
        /// <param name="offsetY">The Y-coordinate offset.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetBounds(float offsetX, float offsetY, float width, float height)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _width = Math.Max(0f, width);
            _height = Math.Max(0f, height);
        }

        #endregion
    }
}