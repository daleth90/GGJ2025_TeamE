using Cinemachine;
using UnityEngine;

namespace Bubble
{
    public class Level : MonoBehaviour
    {
        [field: SerializeField] public Transform startPosition { get; private set; }
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        private PlayerStatus playerStatus;
        

        public void Init(PlayerStatus playerStatus)
        {
            Debug.Log(gameObject.name + " Init");

            cinemachineVirtualCamera.Follow = playerStatus.transform;
            cinemachineVirtualCamera.LookAt = playerStatus.transform;
        }
    }
}
