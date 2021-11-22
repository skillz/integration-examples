#if UNITY_EDITOR && UNITY_ANDROID && !UNITY_2019_3_OR_NEWER
using System.IO;
using UnityEditor.Android;
using UnityEngine;
using SkillzSDK.Settings;

namespace SkillzSDK.Internal.Build.Android
{
	/// <summary>
	/// For Unity 2019.2 or earlier. Receives a callback after a Gradle based,
	/// Android Studio project has been exported, but before it is compiled.
	/// This class will make necessary modifications to the project that Skillz
	/// requires for a successful integration.
	/// </summary>
	internal sealed class PreUnity2019Dot3PostGenerateGradleProject : PostGenerateGradleAndroidProject, IPostGenerateGradleAndroidProject
	{
		protected override string SourceResourcesFolder
		{
			get
			{
				return Path.Combine(Application.dataPath, "Skillz", "Resources", "2019.2");
			}
		}

		public void OnPostGenerateGradleAndroidProject(string basePath)
		{
			ModifyGradleProject(basePath);
		}

		protected override void ModifyAndroidManifests(string basePath)
		{
			var appManifestPath = Path.Combine(basePath, "src", "main", "AndroidManifest.xml");

			Debug.Log(string.Format(Constants.LogFormat, $"Writing Skillz specific data to '{appManifestPath}'"));

			var manifest = new AndroidManifest(appManifestPath);

			manifest.SetSupportsScreens(small: true, normal: true, large: true, xLarge: true, anyDensity: true);
			manifest.SetApplicationTheme("@android:style/Theme.NoTitleBar.Fullscreen");
			manifest.SetStartingActivityName(SkillzActivityName);
			manifest.SetLaunchMode("singleTop");
			manifest.SetClearTaskOnLaunch("false");
			manifest.SetAlwaysRetainTaskState("true");

			manifest.AddMetadataElement("skillz_allow_exit", SkillzSettings.Instance.AllowSkillzExit.ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_game_has_sync_bot", SkillzSettings.Instance.HasSyncBot.ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_game_id", SkillzSettings.Instance.GameID.ToString());
			manifest.AddMetadataElement("skillz_production", (SkillzSettings.Instance.Environment == Environment.Production).ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_game_activity", SkillzActivityName);
			manifest.AddMetadataElement("skillz_orientation", SkillzSettings.Instance.Orientation.ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_is_unity", "true");

			manifest.AddUsesFeature("glEsVersion", "0x00020000");

			// See: https://developer.android.com/about/versions/pie/android-9.0-changes-28#apache-p
			manifest.AddUsesLibrary("org.apache.http.legacy", false);

			manifest.Save();
		}

		protected override string GetProguardRulesProPath(string basePath)
		{
			return Path.Combine(basePath, "proguard-rules.pro");
		}

		protected override string GetMultidexKeepPath(string basePath)
		{
			return Path.Combine(basePath, "multidex-keep.txt");
		}

		protected override string GetGradlePath(string basePath)
		{
			return Path.Combine(basePath, BuildDotGradle);
		}

		protected override string GetFirebaseManifestPath(string basePath)
		{
			return Path.Combine(basePath, Firebase, AndroidManifestDotXml);
		}

		protected override string GetFirebaseGradlePath(string basePath)
		{
			return Path.Combine(basePath, Firebase, BuildDotGradle);
		}

		protected override void PerformMiscWork(string basePath)
		{
		}
	}
}
#endif // UNITY_EDITOR && UNITY_ANDROID && !UNITY_2019_3_OR_NEWER