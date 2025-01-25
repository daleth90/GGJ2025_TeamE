#if !UNITY_EDITOR && UNITY_ANDROID
#define ANDROID_BUILD
#endif

using UnityEngine;

namespace Physalia
{
    // TODO: Not fully tested.
    public static class ApplicationUtility
    {
        public static void OpenBrowser(string url)
        {
            Application.OpenURL(url);
        }

        public static void OpenFacebook(string uriScheme, string fallbackUrl)
        {
#if ANDROID_BUILD
            OpenUriSchemeFromAndroid(uriScheme, fallbackUrl, "com.facebook.katana");
#else
            Application.OpenURL(fallbackUrl);
#endif
        }

        public static void OpenDiscord(string uriScheme, string fallbackUrl)
        {
#if ANDROID_BUILD
            OpenUriSchemeFromAndroid(uriScheme, fallbackUrl, "com.discord");
#else
            Application.OpenURL(fallbackUrl);
#endif
        }

        public static void OpenX(string uriScheme, string fallbackUrl)
        {
#if ANDROID_BUILD
            OpenUriSchemeFromAndroid(uriScheme, fallbackUrl, "com.twitter.android");
#else
            Application.OpenURL(fallbackUrl);
#endif
        }

#if ANDROID_BUILD
        private static void OpenUriSchemeFromAndroid(string uriScheme, string fallbackUrl, string packageName)
        {
            AndroidJavaClass javaClassPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject javaObjectActivity = javaClassPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject javaObjectPackageManager = javaObjectActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject javaObjectIntent = javaObjectPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
            if (javaObjectIntent == null)
            {
                Application.OpenURL(fallbackUrl);
                return;
            }

            // uriObject = Uri.parse(uriScheme)
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", uriScheme);

            // var intent = new Intent()
            // intent.setAction(Intent.ACTION_VIEW)
            // intent.setData(uriObject)
            // intent.setPackage(packageName)
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
            intentObject.Call<AndroidJavaObject>("setData", uriObject);
            // Do setPackage so it knows where to start activity, or it will open the url from the default browser.
            intentObject.Call<AndroidJavaObject>("setPackage", packageName);

            // activity.startActivity(intent)
            javaObjectActivity.Call("startActivity", intentObject);
        }
#endif
    }
}
