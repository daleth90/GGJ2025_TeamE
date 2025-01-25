using UnityEngine;
using UnityEngine.Assertions;

namespace Physalia
{
    public static class CameraUtility
    {
        public static float FovToOrthographicSize(float fieldOfView, float cameraDistance)
        {
            float halfFrustumHeight = cameraDistance * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);
            return halfFrustumHeight;
        }

        public static float OrthographicSizeToFov(float orthographicSize, float cameraDistance)
        {
            Assert.IsTrue(cameraDistance >= 0, "Camera distance must be positive");
            if (cameraDistance <= MathUtility.Tolerence)
            {
                return 179f;
            }

            return Mathf.Atan(orthographicSize / cameraDistance) * 2f * Mathf.Rad2Deg;
        }
    }
}
