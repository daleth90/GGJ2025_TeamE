using UnityEngine;

namespace Physalia
{
    public static class GUIUtility
    {
        private const float DefaultVirtualWidth = 640f;
        private const float DefaultVirtualHeight = 360f;

        private static float _currentVirtualWidth = DefaultVirtualWidth;
        private static float _currentVirtualHeight = DefaultVirtualHeight;

        public static float CurrentVirtualWidth => _currentVirtualWidth;
        public static float CurrentVirtualHeight => _currentVirtualHeight;

        public static void SetVirtualSize(float width, float height)
        {
            _currentVirtualWidth = width;
            _currentVirtualHeight = height;
        }

        public static void RefreshMatrix()
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(Screen.width / _currentVirtualWidth, Screen.height / _currentVirtualHeight, 1f));
            GUI.matrix = matrix;
        }
    }
}
