using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Physalia
{
    public static class RectTransformExtensions
    {
        public static void SetSizeWithScreenArea(this RectTransform rectTransform, Rect targetArea, Rect screen)
        {
            float anchorMinX = targetArea.x / screen.width;
            float anchorMinY = targetArea.y / screen.height;
            float anchorMaxX = (targetArea.x + targetArea.width) / screen.width;
            float anchorMaxY = (targetArea.y + targetArea.height) / screen.height;
            var anchorMin = new Vector2(anchorMinX, anchorMinY);
            var anchorMax = new Vector2(anchorMaxX, anchorMaxY);

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSize(this RectTransform rectTransform)
        {
            return rectTransform.rect.size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSize(this RectTransform rectTransform, float width, float height)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetWidth(this RectTransform rectTransform)
        {
            return rectTransform.rect.width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetHeight(this RectTransform rectTransform)
        {
            return rectTransform.rect.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public static void SetNativeSizeAndAnchor(this Image image)
        {
            Sprite sprite = image.sprite;
            if (sprite == null)
            {
                return;
            }

            image.SetNativeSize();

            Vector2 size = sprite.rect.size;
            Vector2 pivot = sprite.pivot;
            float pivotX = pivot.x / size.x;
            float pivotY = pivot.y / size.y;

            RectTransform rectTransform = image.rectTransform;
            rectTransform.pivot = new Vector2(pivotX, pivotY);
        }
    }
}
