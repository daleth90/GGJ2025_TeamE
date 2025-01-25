using UnityEngine;

namespace Physalia
{
    public static class MathUtility
    {
        public const float Tolerence = 0.00001f;

        public static bool Approximately(float a, float b)
        {
            float diff = a - b;
            if (diff < 0f)
            {
                diff = -diff;
            }

            return diff <= Tolerence;
        }

        public static bool ApproximatelyZero(float value)
        {
            if (value < 0f)
            {
                value = -value;
            }

            return value <= Tolerence;
        }

        public static bool ApproximatelyZero(Vector2 value)
        {
            return ApproximatelyZero(value.x) &&
                ApproximatelyZero(value.y);
        }

        public static bool ApproximatelyZero(Vector3 value)
        {
            return ApproximatelyZero(value.x) &&
                ApproximatelyZero(value.y) &&
                ApproximatelyZero(value.z);
        }

        public static bool ApproximatelyZero(Vector4 value)
        {
            return ApproximatelyZero(value.x) &&
                ApproximatelyZero(value.y) &&
                ApproximatelyZero(value.z) &&
                ApproximatelyZero(value.w);
        }

        public static float Round(float value)
        {
            return RoundToInt(value);
        }

        public static int RoundToInt(float value)
        {
            int integer = (int)value;
            float fraction = value - integer;
            if (Mathf.Abs(fraction) >= 0.5f - Tolerence)
            {
                if (fraction > 0)
                {
                    return integer + 1;
                }
                else if (fraction < 0)
                {
                    return integer - 1;
                }
            }

            return integer;
        }
    }
}
