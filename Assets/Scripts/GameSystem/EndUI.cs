using UnityEngine;
using UnityEngine.UI;

namespace Bubble
{
    public class EndUI : MonoBehaviour
    {
        [field: SerializeField] public Button restartButton { get; private set; }
        [field: SerializeField] public Button quitButton { get; private set; }

        [SerializeField] private GameObject ui;

        public void ShowEndUI(bool set)
        {
            ui.SetActive(set);
        }
    }
}
