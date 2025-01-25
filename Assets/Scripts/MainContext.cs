using Physalia;
using Physalia.AssetManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bubble
{
    [DefaultExecutionOrder(-32767)]
    public class MainContext : MonoBehaviour
    {
        [SerializeField]
        private AudioManager _audioManager;
        [SerializeField]
        private ScreenFader _screenFaderPrefab;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var assetManager = new AddressableAssetManager();
            ServiceLocator.Register<IAssetManager>(assetManager);
            ServiceLocator.Register<IAudioManager>(_audioManager);
            ServiceLocator.Register<IVfxManager>(new VfxManager(assetManager));

            ScreenFader screenFader = Instantiate(_screenFaderPrefab);
            screenFader.name = nameof(ScreenFader);
            DontDestroyOnLoad(screenFader);
        }

        private void Start()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
