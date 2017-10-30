using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DbLogger.Viewer.Extensions
{
    static class RectangleExtensions
    {
        /// <summary>
        /// Gets a new Rectangle using just the right side of this rectangle.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Rectangle RightSide(this Rectangle bounds, int width)
            => width > bounds.Width
            ? bounds
            : new Rectangle(bounds.X + (bounds.Width - width), bounds.Y, width, bounds.Height);
    }
}
