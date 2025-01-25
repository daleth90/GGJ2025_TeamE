using System.Runtime.CompilerServices;
using UnityEngine;

namespace Physalia
{
    public static class TransformExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParentAndResetLocalPRS(this Transform transform, Transform parent)
        {
            transform.SetParent(parent, false);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetLocalPRS(this Transform transform)
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParentAndResetLocalPRS(this RectTransform rectTransform, Transform parent)
        {
            rectTransform.SetParent(parent, false);
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSiblingIndexBefore(this Transform transform, Transform sibling)
        {
            int index = sibling.GetSiblingIndex();
            transform.SetSiblingIndex(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSiblingIndexAfter(this Transform transform, Transform sibling)
        {
            int index = sibling.GetSiblingIndex();
            transform.SetSiblingIndex(index + 1);
        }

        public static Transform FindDeep(this Transform transform, string name)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                Transform deepChild = child.FindDeep(name);
                if (deepChild != null)
                {
                    return deepChild;
                }
            }

            return null;
        }
    }
}
