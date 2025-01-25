using Physalia;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bubble
{
    public class ButtonEx : Button
    {
        private IAudioManager _audioManager;

        protected override void Awake()
        {
            base.Awake();
            if (ServiceLocator.Has<IAudioManager>())
            {
                _audioManager = ServiceLocator.Resolve<IAudioManager>();
            }
        }

        public new bool IsPressed()
        {
            return base.IsPressed();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            _audioManager?.PlaySound("SFX_UIClick");
        }
    }
}
