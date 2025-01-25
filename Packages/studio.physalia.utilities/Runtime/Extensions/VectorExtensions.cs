using System.Runtime.CompilerServices;
using UnityEngine;

namespace Physalia
{
    public static class VectorExtensions
    {
        /// <remarks>
        /// Think as the classic Cartesian coordinate system, so positive degrees go counterclockwise
        /// </remarks>
        public static Vector2 Rotate(this Vector2 vector, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float ox = vector.x;
            float oy = vector.y;
            vector.x = cos * ox - sin * oy;
            vector.y = sin * ox + cos * oy;
            return vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this RectInt rectInt)
        {
            return new Rect(rectInt.x, rectInt.y, rectInt.width, rectInt.height);
        }
    }
}
