using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Physalia
{
    [Serializable]
    public class DeviceData
    {
        public string name;
        public string modelName;
        public List<OrientationData> orientations = new();
    }

    [Serializable]
    public class OrientationData
    {
        public ScreenOrientation orientation;
        public int marginLeft;
        public int marginRight;
        public int marginTop;
        public int marginBottom;
    }

    [CreateAssetMenu(fileName = "ScreenDeviceSettings", menuName = "ScreenDeviceSettings")]
    public class ScreenDeviceSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<DeviceData> _deviceDatas = new();

        private readonly Dictionary<string, DeviceData> _deviceTable = new();

        public IReadOnlyList<DeviceData> DeviceDatas => _deviceDatas;

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            _deviceTable.Clear();
            foreach (DeviceData deviceData in _deviceDatas)
            {
                _deviceTable.Add(deviceData.modelName, deviceData);
            }
        }
#if UNITY_EDITOR
        public void RemoveDeviceData(string modelName)
        {
            bool success = _deviceTable.TryGetValue(modelName, out DeviceData deviceData);
            if (success)
            {
                _deviceDatas.Remove(deviceData);
                _deviceTable.Remove(modelName);

                EditorUtility.SetDirty(this);
            }
        }

        public void AddOrientationData(string modelName, OrientationData orientationData)
        {
            bool success = _deviceTable.TryGetValue(modelName, out DeviceData deviceData);
            if (!success)
            {
                deviceData = new DeviceData
                {
                    modelName = modelName,
                };

                _deviceDatas.Add(deviceData);
                _deviceTable.Add(modelName, deviceData);
            }

            deviceData.orientations.Add(orientationData);
            deviceData.orientations.Sort((a, b) => a.orientation.CompareTo(b.orientation));

            EditorUtility.SetDirty(this);
        }
#endif
        public bool TryGetOrientationData(string modelName, ScreenOrientation orientation, out OrientationData orientationData)
        {
            bool success = _deviceTable.TryGetValue(modelName, out DeviceData deviceData);
            if (success)
            {
                for (var i = 0; i < deviceData.orientations.Count; i++)
                {
                    OrientationData data = deviceData.orientations[i];
                    if (data.orientation == orientation)
                    {
                        orientationData = data;
                        return true;
                    }
                }
            }

            // When device or orientation not found
            orientationData = null;
            return false;
        }
    }
}
