using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Bubble
{
    public class AirZone : MonoBehaviour, IInteractable
    {
        [SerializeField] private float RestoreOxygenSpeed = 1f;

        private bool isStart;
        private CancellationToken cancellationToken;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public async void Interact(PlayerStatus playerStatus)
        {
            if (cancellationToken == null) cancellationToken = this.GetCancellationTokenOnDestroy();

            if (isStart) return;

            isStart = true;

            while(isStart)
            {
                await UniTask.NextFrame(cancellationToken: cancellationToken);
                playerStatus.oxygen += RestoreOxygenSpeed * Time.deltaTime;
                animator.SetTrigger("AirBump");
            }
        }

        public void CancelInteract()
        {
            isStart = false;
        }
    }
}
