using System.Diagnostics;
using UnityEngine;

namespace Physalia
{
    public class ComponentPool<T> : ObjectPool<T> where T : Component
    {
        private readonly T _original;
        private readonly Transform _root;
        private readonly string _rootName;

        public ComponentPool(T original, int startSize) : this(original, startSize, null)
        {

        }

        public ComponentPool(T original, int startSize, Transform poolParent) : base(new ComponentInstanceFactory<T>(original), 0)
        {
            _original = original;
            _rootName = $"Pool ({original.name})";
            _root = new GameObject(_rootName).transform;

            if (_original.transform is RectTransform)
            {
                RectTransform rootRectTransform = _root.gameObject.AddComponent<RectTransform>();
                _root = rootRectTransform;

                rootRectTransform.SetSize(0f, 0f);
                if (poolParent != null)
                {
                    rootRectTransform.SetParentAndResetLocalPRS(poolParent);
                }
            }
            else
            {
                if (poolParent != null)
                {
                    _root.SetParentAndResetLocalPRS(poolParent);
                }
            }

            for (var i = 0; i < startSize; i++)
            {
                CreateInstance();
            }
        }

        protected override void OnInstanceCreated(T instance)
        {
            RefreshRootName();
            instance.name = $"{_original.name}_{Size - 1}";
            instance.transform.SetParentAndResetLocalPRS(_root);
        }

        protected override void OnInstanceGet(T instance)
        {
            RefreshRootName();
        }

        protected override void OnInstanceReleased(T instance)
        {
            RefreshRootName();
            instance.transform.SetParentAndResetLocalPRS(_root);
        }

        [Conditional("UNITY_EDITOR")]
        private void RefreshRootName()
        {
            _root.name = $"{_rootName} {RemainedCount}/{Size}<{MaxUsage}";
        }

        protected override void OnDestroyed()
        {
            if (_root != null)
            {
                Object.Destroy(_root.gameObject);
            }
        }
    }
}
