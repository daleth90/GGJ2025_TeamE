using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A concrete subclass of the Unity UI `Graphic` class that just skips drawing.
/// Useful for providing a raycast target without actually drawing anything.
/// </summary>
/// <remarks>
/// Reference: http://answers.unity.com/answers/1157876/view.html
/// </remarks>
[RequireComponent(typeof(CanvasRenderer))]
public sealed class NonDrawingGraphic : Graphic, ILayoutElement
{
    [SerializeField]
    private float _preferredWidth = 0f;
    [SerializeField]
    private float _preferredHeight = 0f;

    public float minWidth => 0f;
    public float minHeight => 0f;
    public float flexibleWidth => 0f;
    public float flexibleHeight => 0f;
    public float preferredWidth => _preferredWidth;
    public float preferredHeight => _preferredHeight;
    public int layoutPriority => 0;

    public void CalculateLayoutInputHorizontal()
    {

    }

    public void CalculateLayoutInputVertical()
    {

    }

    public override void SetMaterialDirty()
    {
        return;
    }

    public override void SetVerticesDirty()
    {
        return;
    }

    /// <remarks>
    /// Probably not necessary since the chain of calls `Rebuild()`->`UpdateGeometry()`->`DoMeshGeneration()`->`OnPopulateMesh()` won't happen; so here really just as a fail-safe.
    /// </remarks>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        return;
    }
}
