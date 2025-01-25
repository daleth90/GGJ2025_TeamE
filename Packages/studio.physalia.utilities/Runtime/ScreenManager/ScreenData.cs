using System;
using UnityEngine;

namespace Physalia
{
    public struct ScreenData : IEquatable<ScreenData>
    {
        public int width;
        public int height;
        public int safeAreaX;
        public int safeAreaY;
        public int safeAreaWidth;
        public int safeAreaHeight;

        public readonly RectInt Screen => new(0, 0, width, height);
        public readonly RectInt SafeArea => new(safeAreaX, safeAreaY, safeAreaWidth, safeAreaHeight);

        public override readonly bool Equals(object obj)
        {
            return obj is ScreenData other && Equals(other);
        }

        public readonly bool Equals(ScreenData other)
        {
            return width == other.width &&
                height == other.height &&
                safeAreaX == other.safeAreaX &&
                safeAreaY == other.safeAreaY &&
                safeAreaWidth == other.safeAreaWidth &&
                safeAreaHeight == other.safeAreaHeight;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(width, height, safeAreaX, safeAreaY, safeAreaWidth, safeAreaHeight);
        }

        public override readonly string ToString()
        {
            return $"({width}x{height} ({safeAreaX},{safeAreaY} {safeAreaWidth}x{safeAreaHeight}))";
        }

        public static bool operator ==(ScreenData left, ScreenData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScreenData left, ScreenData right)
        {
            return !(left == right);
        }
    }
}
