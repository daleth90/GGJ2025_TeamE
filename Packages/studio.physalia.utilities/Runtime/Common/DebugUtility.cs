using UnityEngine;

namespace Physalia
{
    public static class DebugUtility
    {
        public static void DrawBox2D(Vector3 position, Vector2 size, Color color, float duration)
        {
            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;
            Vector3 p1 = position + new Vector3(-halfWidth, -halfHeight, 0f);
            Vector3 p2 = position + new Vector3(halfWidth, -halfHeight, 0f);
            Vector3 p3 = position + new Vector3(halfWidth, halfHeight, 0f);
            Vector3 p4 = position + new Vector3(-halfWidth, halfHeight, 0f);

            Debug.DrawLine(p1, p2, color, duration, false);
            Debug.DrawLine(p2, p3, color, duration, false);
            Debug.DrawLine(p3, p4, color, duration, false);
            Debug.DrawLine(p4, p1, color, duration, false);
        }

        public static void DrawBox2D(Vector2 position, Vector2 size, float angle, Color color, float duration)
        {
            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;
            Vector2 p1 = position + new Vector2(-halfWidth, -halfHeight).Rotate(angle);
            Vector2 p2 = position + new Vector2(halfWidth, -halfHeight).Rotate(angle);
            Vector2 p3 = position + new Vector2(halfWidth, halfHeight).Rotate(angle);
            Vector2 p4 = position + new Vector2(-halfWidth, halfHeight).Rotate(angle);

            Debug.DrawLine(p1, p2, color, duration, false);
            Debug.DrawLine(p2, p3, color, duration, false);
            Debug.DrawLine(p3, p4, color, duration, false);
            Debug.DrawLine(p4, p1, color, duration, false);
        }

        public static void DrawCircle(Vector3 position, float radius, int segments, Color color, float duration)
        {
            // If either radius or number of segments are less or equal to 0, skip drawing
            if (radius <= 0f || segments <= 0)
            {
                return;
            }

            // Single segment of the circle covers (2 * PI / number of segments) degrees
            float radianStep = 2f * Mathf.PI / segments;

            // lineStart and lineEnd variables are declared outside of the following for loop
            Vector3 lineStart = Vector3.zero;
            Vector3 lineEnd = Vector3.zero;

            for (int i = 0; i < segments; i++)
            {
                // Line start is defined as starting angle of the current segment (i)
                lineStart.x = Mathf.Cos(radianStep * i);
                lineStart.y = Mathf.Sin(radianStep * i);

                // Line end is defined by the angle of the next segment (i+1)
                lineEnd.x = Mathf.Cos(radianStep * (i + 1));
                lineEnd.y = Mathf.Sin(radianStep * (i + 1));

                // Results are multiplied so they match the desired radius
                lineStart *= radius;
                lineEnd *= radius;

                // Results are offset by the desired position/origin 
                lineStart += position;
                lineEnd += position;

                // Points are connected using DrawLine method and using the passed color
                Debug.DrawLine(lineStart, lineEnd, color, duration, false);
            }
        }

        public static void DrawBounds(Bounds bounds, Color color, float duration)
        {
            float halfWidth = bounds.size.x * 0.5f;
            float halfHeight = bounds.size.y * 0.5f;
            Vector3 p1 = bounds.center + new Vector3(-halfWidth, -halfHeight, 0f);
            Vector3 p2 = bounds.center + new Vector3(halfWidth, -halfHeight, 0f);
            Vector3 p3 = bounds.center + new Vector3(halfWidth, halfHeight, 0f);
            Vector3 p4 = bounds.center + new Vector3(-halfWidth, halfHeight, 0f);

            Debug.DrawLine(p1, p2, color, duration, false);
            Debug.DrawLine(p2, p3, color, duration, false);
            Debug.DrawLine(p3, p4, color, duration, false);
            Debug.DrawLine(p4, p1, color, duration, false);
        }
    }
}
