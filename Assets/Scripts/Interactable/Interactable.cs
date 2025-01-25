using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Reset()
    {
        gameObject.SetActive(true);
    }
}
