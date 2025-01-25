using UnityEngine;

namespace Physalia
{
    internal class GameObjectInstanceFactory : ObjectInstanceFactory<GameObject>
    {
        private readonly GameObject _original;

        public override string Name => _original.name;

        public GameObjectInstanceFactory(GameObject original)
        {
            _original = original;
        }

        public override GameObject Create()
        {
            GameObject instance = Object.Instantiate(_original);
            instance.SetActive(false);
            return instance;
        }

        public override void Reset(GameObject instance)
        {
            instance.SetActive(false);
        }

        public override void Destroy(GameObject instance)
        {
            if (instance != null)
            {
                Object.Destroy(instance);
            }
        }
    }
}
