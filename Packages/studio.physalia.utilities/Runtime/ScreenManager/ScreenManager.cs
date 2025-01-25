using System;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Physalia
{
    [DefaultExecutionOrder(-32000)]
    public sealed class ScreenManager : MonoBehaviour, IScreenManager
    {
        public event Action<ScreenData> ScreenChanged;

        private const float UpdatePeriod = 0.1f;

        [SerializeField]
        private ScreenDeviceSettings _deviceSettings;

        private OrientationData _orientationDataCurrent;
        private ScreenData _screenDataCurrent;
        private ScreenData _screenDataUpdating;
        private float _timer = 0f;

        public ScreenData ScreenData => _screenDataCurrent;
        public OrientationData OrientationData => _orientationDataCurrent;

        private void Awake()
        {
            _screenDataUpdating = GetScreenData();
            _screenDataCurrent = _screenDataUpdating;
        }

        private void Start()
        {
            CheckIfScreenChanged();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < UpdatePeriod)
            {
                return;
            }

            _timer = 0f;
            CheckIfScreenChanged();
        }

        private void CheckIfScreenChanged()
        {
            Rect safeArea = Screen.safeArea;
            _screenDataUpdating = GetScreenData();

            if (_screenDataCurrent != _screenDataUpdating)
            {
                _screenDataCurrent = _screenDataUpdating;
                try
                {
                    ScreenChanged?.Invoke(_screenDataCurrent);
                }
                catch (Exception e)
                {
                    Logger.Fatal(e);
                }
            }
        }

        private ScreenData GetScreenData()
        {
            ScreenOrientation orientation = Screen.orientation;
            _orientationDataCurrent = GetOrientationData(SystemInfo.deviceModel, orientation);
            if (_orientationDataCurrent != null)
            {
                int screenWidth = Screen.width;
                int screenHeight = Screen.height;
                return new ScreenData
                {
                    width = screenWidth,
                    height = screenHeight,
                    safeAreaX = _orientationDataCurrent.marginLeft,
                    safeAreaY = _orientationDataCurrent.marginBottom,
                    safeAreaWidth = screenWidth - _orientationDataCurrent.marginRight - _orientationDataCurrent.marginLeft,
                    safeAreaHeight = screenHeight - _orientationDataCurrent.marginTop - _orientationDataCurrent.marginBottom,
                };
            }
            else
            {
                return new ScreenData
                {
                    width = Screen.width,
                    height = Screen.height,
                    safeAreaX = (int)Screen.safeArea.x,
                    safeAreaY = (int)Screen.safeArea.y,
                    safeAreaWidth = (int)Screen.safeArea.width,
                    safeAreaHeight = (int)Screen.safeArea.height,
                };
            }
        }

        private OrientationData GetOrientationData(string deviceModel, ScreenOrientation orientation)
        {
            if (_deviceSettings != null)
            {
                if (deviceModel != SystemInfo.unsupportedIdentifier)
                {
                    bool success = _deviceSettings.TryGetOrientationData(deviceModel, orientation, out OrientationData orientationData);
                    if (success)
                    {
                        return orientationData;
                    }
                }
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// This method does not overwrite the existing data.
        /// </summary>
        public void RegisterOrientationData()
        {
            if (_orientationDataCurrent != null)
            {
                Logger.Warn($"OrientationData already exists for {SystemInfo.deviceModel} {_orientationDataCurrent.orientation}");
                return;
            }

            _orientationDataCurrent = new OrientationData
            {
                orientation = Screen.orientation,
                marginLeft = (int)Screen.safeArea.x,
                marginRight = Screen.width - (int)Screen.safeArea.x - (int)Screen.safeArea.width,
                marginTop = Screen.height - (int)Screen.safeArea.y - (int)Screen.safeArea.height,
                marginBottom = (int)Screen.safeArea.y,
            };
            _deviceSettings.AddOrientationData(SystemInfo.deviceModel, _orientationDataCurrent);
        }

        public void ResetOrientationData()
        {
            if (_orientationDataCurrent != null)
            {
                _orientationDataCurrent.marginLeft = (int)Screen.safeArea.x;
                _orientationDataCurrent.marginRight = Screen.width - (int)Screen.safeArea.x - (int)Screen.safeArea.width;
                _orientationDataCurrent.marginTop = Screen.height - (int)Screen.safeArea.y - (int)Screen.safeArea.height;
                _orientationDataCurrent.marginBottom = (int)Screen.safeArea.y;
            }
        }

        public void RemoveDeviceData()
        {
            _orientationDataCurrent = null;
            _deviceSettings.RemoveDeviceData(SystemInfo.deviceModel);
        }

        public void SetOrientationDataDirty()
        {
            UnityEditor.EditorUtility.SetDirty(_deviceSettings);
        }
#endif
    }
}
