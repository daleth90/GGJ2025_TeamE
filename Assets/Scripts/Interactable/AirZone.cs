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

        public async void Interact(PlayerStatus playerStatus)
        {
            if (cancellationToken == null) cancellationToken = this.GetCancellationTokenOnDestroy();

            if (isStart) return;

            isStart = true;

            while(isStart)
            {
                await UniTask.NextFrame(cancellationToken: cancellationToken);
                playerStatus.oxygen += RestoreOxygenSpeed * Time.deltaTime;
            }
        }

        public void CancelInteract()
        {
            isStart = false;
        }
    }
}
