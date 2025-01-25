using System.Collections.Generic;
using UnityEngine;

namespace Physalia
{
    public class ObjectPool<T> where T : class
    {
        private readonly ObjectInstanceFactory<T> _factory;

        private readonly List<T> _allInstances = new();
        private readonly HashSet<T> _usingInstances = new();
        private int _size;
        private int _maxUsage;
        private int _rotationIndex;

        public int Size => _size;
        public int RemainedCount => _size - _usingInstances.Count;
        public int UsingCount => _usingInstances.Count;
        public int MaxUsage => _maxUsage;

        public ObjectPool(ObjectInstanceFactory<T> factory, int startSize)
        {
            _factory = factory;
            for (int i = 0; i < startSize; i++)
            {
                CreateInstance();
            }
        }

        protected virtual void OnInstanceCreated(T instance) { }

        protected virtual void OnInstanceGet(T instance) { }

        protected virtual void OnInstanceReleased(T instance) { }

        protected virtual void OnDestroyed() { }

        protected void CreateInstance()
        {
            T instance = _factory.Create();
            _allInstances.Add(instance);
            _size++;

            OnInstanceCreated(instance);
        }

        public T Get()
        {
            if (_usingInstances.Count == _size)
            {
                // Directly point to the future first new instance.
                _rotationIndex = _size;

                int expandCount = GetExpandCount(_factory, _size);
                for (int i = 0; i < expandCount; i++)
                {
                    CreateInstance();
                }
            }

            // Find an instance that is not being used.
            T instance = _allInstances[_rotationIndex];
            while (_usingInstances.Contains(instance))
            {
                _rotationIndex = (_rotationIndex + 1) % _size;
                instance = _allInstances[_rotationIndex];
            }

            _usingInstances.Add(instance);
            _rotationIndex = (_rotationIndex + 1) % _size;

            // Record max usage
            if (_maxUsage < _usingInstances.Count)
            {
                _maxUsage = _usingInstances.Count;
            }

            OnInstanceGet(instance);
            return instance;
        }

        private static int GetExpandCount(ObjectInstanceFactory<T> factory, int currentSize)
        {
            int expandCount = factory.GetExpandCount(currentSize);
            if (expandCount < 1)
            {
                expandCount = 1;
            }

            return expandCount;
        }

        public void Release(T instance)
        {
            if (instance == null)
            {
                Logger.Error($"[{nameof(ObjectPool<T>)}] Release null into the object pool. Pool Name: '{_factory.Name}'");
                return;
            }

            bool success = _usingInstances.Remove(instance);
            if (!success)
            {
                Logger.Error($"[{nameof(ObjectPool<T>)}] The released object is not belong to this pool. Pool Name: '{_factory.Name}'");
                return;
            }

            // Handle destroyed instances.
            bool isDestroyed = false;
            if (instance is GameObject gameObject && gameObject == null)
            {
                Logger.Warn($"[{nameof(ObjectPool<T>)}] The released gameObject has already been destroyed. Pool Name: '{_factory.Name}'");
                isDestroyed = true;
            }

            if (instance is Component component && component.gameObject == null)
            {
                Logger.Warn($"[{nameof(ObjectPool<T>)}] The released component has already been destroyed. Pool Name: '{_factory.Name}'");
                isDestroyed = true;
            }

            if (!isDestroyed)
            {
                _factory.Reset(instance);
                OnInstanceReleased(instance);
            }
        }

        public void ReleaseAll()
        {
            foreach (T instance in _usingInstances)
            {
                // Handle destroyed instances.
                bool isDestroyed = false;
                if (instance is GameObject gameObject && gameObject == null)
                {
                    Logger.Warn($"[{nameof(ObjectPool<T>)}] The released gameObject has already been destroyed. Pool Name: '{_factory.Name}'");
                    isDestroyed = true;
                }

                if (instance is Component component && component.gameObject == null)
                {
                    Logger.Warn($"[{nameof(ObjectPool<T>)}] The released component has already been destroyed. Pool Name: '{_factory.Name}'");
                    isDestroyed = true;
                }

                if (!isDestroyed)
                {
                    _factory.Reset(instance);
                    OnInstanceReleased(instance);
                }
            }

            _usingInstances.Clear();
        }

        /// <remarks>
        /// This method will release all using instances first, making sure their resources are released, and then destroy all instances.
        /// </remarks>
        public void Destroy()
        {
            ReleaseAll();

            for (var i = 0; i < _allInstances.Count; i++)
            {
                _factory.Destroy(_allInstances[i]);
            }

            _allInstances.Clear();
            _usingInstances.Clear();
            _size = 0;

            OnDestroyed();
        }
    }
}
