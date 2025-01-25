using System;
using System.Collections;
using UnityEngine.Networking;

namespace Physalia
{
    public static class NetworkUtility
    {
        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClass();

        public static IEnumerator CheckInternetConnection(Action<bool> action)
        {
            const string TestUrl = "http://google.com";

            var request = new UnityWebRequest(TestUrl);
            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            try
            {
                action?.Invoke(success);
            }
            catch (Exception e)
            {
                Logger.Error(Label, e);
            }
        }
    }
}
