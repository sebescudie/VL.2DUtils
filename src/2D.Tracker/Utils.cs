using System;
using Stride.Core.Mathematics;

namespace _2D.Tracker
{
    internal static class Utils
    {
        /// <summary>
        /// Gets the length of the longest side of the rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        internal static float GetRectMax(RectangleF rectangle)
        {
            return Math.Max(rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Gets the area of the Rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        internal static float GetArea(RectangleF rectangle)
        {
            return rectangle.Width * rectangle.Height;
        }
    }
}
