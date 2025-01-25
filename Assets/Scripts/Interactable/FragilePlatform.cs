using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Bubble
{
    public class FragilePlatform : MonoBehaviour, IInteractable
    {
        public event UnityAction BreakAction;

        [SerializeField] float holdTime = 1f;

        private bool isStart;
        private CancellationToken cancellationToken;

        public void Init()
        {
            if (cancellationToken == null) cancellationToken = this.GetCancellationTokenOnDestroy();

            isStart = false;
            transform.parent.gameObject.SetActive(true);
        }

        public async void Interact(PlayerStatus playerStatus = null)
        {
            if (isStart) return;

            isStart = true;

            await UniTask.Delay((int)(1000f * holdTime), cancellationToken: cancellationToken);
            transform.parent.gameObject.SetActive(false);
            BreakAction?.Invoke();
        }

        private void Awake()
        {
            GameSystemModel.instance.GameStartAction += Init;
        }

        private void OnDestroy()
        {
            GameSystemModel.instance.GameStartAction -= Init;
        }
    }
}
