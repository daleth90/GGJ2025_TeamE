using System.Threading;
using UnityEngine;

namespace Physalia
{
    public class AppCancellationTokenSource : MonoBehaviour
    {
        private static AppCancellationTokenSource _instance;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        public static CancellationToken Token
        {
            get
            {
                if (_instance == null)
                {
                    Logger.Error($"{nameof(AppCancellationTokenSource)} is not initialized. Returns CancellationToken.None.");
                    return CancellationToken.None;
                }

                return _instance._token;
            }
        }

        public static void Create()
        {
            _ = new GameObject($"[{nameof(AppCancellationTokenSource)}]").AddComponent<AppCancellationTokenSource>();
        }

        public static void Cancel()
        {
            if (_instance != null)
            {
                _instance._tokenSource.Cancel();
            }
        }

        public static void Destroy()
        {
            if (_instance != null)
            {
                DestroyImmediate(_instance.gameObject);
                _instance = null;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Logger.Warn($"{nameof(AppCancellationTokenSource)} is already initialized. Destroying this instance.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
            }
        }

        private void OnApplicationQuit()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
            }
        }
    }
}
