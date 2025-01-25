using UnityEngine;

namespace Physalia
{
    internal class ComponentInstanceFactory<T> : ObjectInstanceFactory<T> where T : Component
    {
        private readonly T _original;

        public override string Name => _original.name;

        public ComponentInstanceFactory(T original)
        {
            _original = original;
        }

        public override T Create()
        {
            T instance = Object.Instantiate(_original);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public override void Reset(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        public override void Destroy(T instance)
        {
            if (instance != null)
            {
                Object.Destroy(instance.gameObject);
            }
        }
    }
}
