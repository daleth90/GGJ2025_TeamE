using UnityEngine;

namespace Bubble
{
    public class Consumable : MonoBehaviour
    {
        public virtual void Reset()
        {
            gameObject.SetActive(true);
        }
    }
}