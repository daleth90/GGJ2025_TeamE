using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bubble
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private float _fadeTime = 1f;

        private static ScreenFader _instance;

        public static ScreenFader Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogWarning("ScreenFader already exists. Destroying new instance.");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _image.color = new Color(0f, 0f, 0f, 0f);
            _image.gameObject.SetActive(false);
        }

        public IEnumerator FadeInOutProcess(Action action)
        {
            yield return FadeInCoroutine();
            action?.Invoke();
            yield return FadeOutCoroutine();
        }

        public void FadeIn()
        {
            StartCoroutine(FadeInCoroutine());
        }

        public void FadeOut()
        {
            StartCoroutine(FadeOutCoroutine());
        }

        private IEnumerator FadeInCoroutine()
        {
            _image.gameObject.SetActive(true);
            for (var i = 0f; i <= _fadeTime; i += Time.deltaTime)
            {
                _image.color = new Color(0f, 0f, 0f, i / _fadeTime);
                yield return null;
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            _image.gameObject.SetActive(true);
            for (var i = _fadeTime; i >= 0f; i -= Time.deltaTime)
            {
                _image.color = new Color(0f, 0f, 0f, i / _fadeTime);
                yield return null;
            }
            _image.gameObject.SetActive(false);
        }
    }
}
