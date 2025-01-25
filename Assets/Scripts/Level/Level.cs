using UnityEngine;

namespace Bubble
{
    public class Level : MonoBehaviour
    {
        [field: SerializeField] public Transform startPosition { get; private set; }

        public void Init()
        {
            Debug.Log(gameObject.name + " Init");
        }
    }
}
