using UnityEngine;

namespace Meditation.Core.Utils
{
    public static class AppUtils
    {
        public static int GetVersionCode()
        {
#if UNITY_EDITOR
            return UnityEditor.PlayerSettings.Android.bundleVersionCode;
#elif UNITY_ANDROID
            return GetAndroidVersionCode();
#else
            return -1;
#endif
        }
        
        private static int GetAndroidVersionCode()
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var packageManager = context.Call<AndroidJavaObject>("getPackageManager");
            var packageName = context.Call<string>("getPackageName");
            var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
            return packageInfo.Get<int>("versionCode");
        }
    }
}