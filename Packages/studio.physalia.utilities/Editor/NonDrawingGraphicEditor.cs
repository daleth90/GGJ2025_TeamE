using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <remarks>
/// Reference: http://answers.unity.com/answers/1157876/view.html
/// </remarks>
[CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
public class NonDrawingGraphicEditor : GraphicEditor
{
    private SerializedProperty _preferredWidth;
    private SerializedProperty _preferredHeight;

    protected override void OnEnable()
    {
        base.OnEnable();
        _preferredWidth = serializedObject.FindProperty("_preferredWidth");
        _preferredHeight = serializedObject.FindProperty("_preferredHeight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Script);

        // Skip AppearanceControlsGUI
        RaycastControlsGUI();

        // Prefered size
        EditorGUILayout.PropertyField(_preferredWidth);
        EditorGUILayout.PropertyField(_preferredHeight);

        serializedObject.ApplyModifiedProperties();
    }
}
