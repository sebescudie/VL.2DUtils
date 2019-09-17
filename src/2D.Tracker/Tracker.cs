using Xenko.Core.Mathematics;
using static _2D.Tracker.Utils;

namespace _2D.Tracker
{
    public class Tracker
    {
        internal RectangleF rectangle;
        internal RectangleF target;
        internal float searchRadius;
        internal bool isAvailable;
        internal bool isDelete;

        internal int timer;
        internal int maxLifetime;
        internal int id;

        internal Tracker(float x, float y, float width, float height, int _id, int _timer, int _maxLifetime)
        {
            rectangle = new RectangleF(x, y, width, height);
            target = rectangle;
            isAvailable = true;
            isDelete = false;
            id = _id;
            timer = _timer;
            maxLifetime = _maxLifetime;
        }

        internal Tracker(RectangleF _rectangle, int _id, int _timer, int _maxLifetime)
        {
            rectangle = _rectangle;
            target = _rectangle;
            isAvailable = true;
            isDelete = false;
            id = _id;
            timer = _timer;
            maxLifetime = _maxLifetime;
        }

        // Public

        public RectangleF Rectangle()
        {
            return this.rectangle;
        }

        public void Split(out RectangleF rectangle, out float searchRadius, out int timer, out int maxLifetime, out int id)
        {
            rectangle = this.rectangle;
            searchRadius = this.searchRadius;
            timer = this.timer;
            maxLifetime = this.maxLifetime;
            id = this.id;
        }

        // Internals

        internal void Update(RectangleF newTarget)
        {
            target = newTarget;
            if (timer < maxLifetime) timer++;
        }

        internal void CountDown()
        {
            timer--;
        }

        internal bool isDead()
        {
            if (timer < 0) return true;
            return false;
        }

        /// <summary>
        /// Runs the tracking algorithm with standard smoothing behavior
        /// </summary>
        /// <param name="smoothing"></param>
        /// <param name="_searchRadius"></param>
        /// <param name="searchPercent"></param>
        internal void Run(float smoothing, float _searchRadius, bool searchPercent)
        {
            // Set searchRadius
            if (searchPercent) { searchRadius = _searchRadius * GetRectMax(rectangle); }
            else { searchRadius = _searchRadius; }

            // Smooth movement to target
            rectangle.X = smoothing * rectangle.X + (1 - smoothing) * target.X;
            rectangle.Y = smoothing * rectangle.Y + (1 - smoothing) * target.Y;
            rectangle.Width = smoothing * rectangle.Width + (1 - smoothing) * target.Width;
            rectangle.Height = smoothing * rectangle.Height + (1 - smoothing) * target.Height;
        }

        /// <summary>
        /// Runs the tracking algorithm with a prediction behavior
        /// </summary>
        internal void RunPrediction()
        {

        }
    }
}
