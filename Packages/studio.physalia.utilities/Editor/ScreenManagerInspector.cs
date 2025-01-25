using UnityEditor;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Physalia
{
    [CustomEditor(typeof(ScreenManager))]
    public class ScreenManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                return;
            }

            EditorGUILayout.Space();
            ScreenManager screenManager = target as ScreenManager;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"DeviceModel: {SystemInfo.deviceModel}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Orientation: {Screen.orientation}");

            EditorGUILayout.LabelField("Screen Data", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Screen: {Screen.width}x{Screen.height}");
            EditorGUILayout.LabelField($"SafeArea: ({(int)Screen.safeArea.x}, {(int)Screen.safeArea.y}) {(int)Screen.safeArea.width}x{(int)Screen.safeArea.height}");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Orientation Data", EditorStyles.boldLabel);

            string os = SystemInfo.operatingSystem;
            if (!os.StartsWith("Android") && !os.StartsWith("iOS"))
            {
                EditorGUILayout.LabelField("Custom orientation data only mobile platforms.");
                return;
            }

            OrientationData currentOrientationData = screenManager.OrientationData;
            if (currentOrientationData == null)
            {
                EditorGUILayout.LabelField("No Orientation Data");
                if (GUILayout.Button("Register Device Screen", GUILayout.Width(150f), GUILayout.Height(50f)))
                {
                    screenManager.RegisterOrientationData();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                currentOrientationData.marginLeft = EditorGUILayout.IntField("Margin Left", currentOrientationData.marginLeft);
                currentOrientationData.marginRight = EditorGUILayout.IntField("Margin Right", currentOrientationData.marginRight);
                currentOrientationData.marginTop = EditorGUILayout.IntField("Margin Top", currentOrientationData.marginTop);
                currentOrientationData.marginBottom = EditorGUILayout.IntField("Margin Bottom", currentOrientationData.marginBottom);

                bool change = EditorGUI.EndChangeCheck();
                if (change)
                {
                    screenManager.SetOrientationDataDirty();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Reset Device Screen", GUILayout.Width(150f), GUILayout.Height(25f)))
                {
                    screenManager.ResetOrientationData();
                }

                if (GUILayout.Button("Remove Device Data", GUILayout.Width(150f), GUILayout.Height(25f)))
                {
                    screenManager.RemoveDeviceData();
                }
            }
        }
    }
}
