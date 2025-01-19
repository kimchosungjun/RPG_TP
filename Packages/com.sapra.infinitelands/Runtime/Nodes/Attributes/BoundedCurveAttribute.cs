using UnityEngine;

namespace sapra.InfiniteLands
{
    public class BoundedCurveAttribute : PropertyAttribute
    {
        public readonly Rect bounds;
        public readonly int guiHeight;

        public BoundedCurveAttribute(float xMin, float yMin, float xMax, float yMax, int height = 1)
        {
            this.bounds = new Rect(xMin, yMin, xMax, yMax);
            this.guiHeight = height;
        }

        public BoundedCurveAttribute(float xMax, float yMax, int height = 1)
        {
            this.bounds = new Rect(0, 0, xMax, yMax);
            this.guiHeight = height;
        }

        public BoundedCurveAttribute(int height = 1)
        {
            this.bounds = new Rect(0, 0, 1, 1);
            this.guiHeight = height;
        }
    }
}