using UnityEngine;

namespace Extensions.Unity
{
    public static class VectorExtensions
    {
        public static Vector2 GetRelativePositionTo(this Vector2 value, Vector2 origin)
        {
            return value - origin;
        }
        public static Vector2 Rotate(this Vector2 v, float angle) {
            var radians = angle * Mathf.Deg2Rad;
            var sin = Mathf.Sin(radians);
            var cos = Mathf.Cos(radians);
            var x = v.x * cos - v.y * sin;
            var y = v.x * sin + v.y * cos;
            return new Vector2(x, y);
        }
    }
}