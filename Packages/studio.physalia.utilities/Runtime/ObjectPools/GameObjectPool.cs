using System.Diagnostics;
using UnityEngine;

namespace Physalia
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        private readonly GameObject _original;
        private readonly Transform _root;
        private readonly string _rootName;

        public bool IsRootDestroyed => _root == null;

        public GameObjectPool(GameObject original, int startSize) : this(original, startSize, null)
        {

        }

        public GameObjectPool(GameObject original, int startSize, Transform poolParent) : base(new GameObjectInstanceFactory(original), 0)
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

        protected override void OnInstanceCreated(GameObject instance)
        {
            RefreshRootName();

            instance.name = $"{_original.name}_{Size - 1}";
            instance.transform.SetParent(_root, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }

        protected override void OnInstanceGet(GameObject instance)
        {
            RefreshRootName();
        }

        protected override void OnInstanceReleased(GameObject instance)
        {
            RefreshRootName();

            instance.transform.SetParent(_root, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
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
