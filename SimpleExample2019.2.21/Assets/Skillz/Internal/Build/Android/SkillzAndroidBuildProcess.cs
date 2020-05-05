#if UNITY_EDITOR && UNITY_ANDROID
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace SkillzSDK.Internal.Build.Android
{
	internal sealed class SkillzAndroidPreProcessBuild : IPreprocessBuild
	{
		public int callbackOrder
		{
			get
			{
				return 0;
			}
		}

		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
			{
				return;
			}

			const string warningMessage = "The Skillz SDK does not support building an APK directly from Unity. Export your project to Android Studio when you need to build your APK.";

			EditorUtility.DisplayDialog(
				"Please Export To Android Studio",
				warningMessage,
				"OK"
			);

			Debug.LogWarning(warningMessage);
		}
	}
}
#endif