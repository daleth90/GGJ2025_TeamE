using UnityEngine;
using UnityEngine.Events;

public class TriggerHelper : MonoBehaviour
{
    [SerializeField] private LayerMask target_layerMask;
    private int target_layer;

    public event UnityAction<Collider> TriggerEnterAction;
    public event UnityAction<Collider> TriggerExitAction;

    private void Start()
    {
        target_layer = (int)Mathf.Log(target_layerMask.value, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != target_layer) return;
        TriggerEnterAction?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != target_layer) return;
        TriggerExitAction?.Invoke(other);
    }
}
