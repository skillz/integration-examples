#if UNITY_EDITOR && UNITY_ANDROID && UNITY_2019_3_OR_NEWER
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using SkillzSDK.Settings;

namespace SkillzSDK.Internal.Build.Android
{
	/// <summary>
	/// For Unity 2019.3 and later. Receives a callback after a Gradle based,
	/// Android Studio project has been exported, but before it is compiled.
	/// This class will make necessary modifications to the project that Skillz
	/// requires for a successful integration.
	/// </summary>
	internal sealed class Unity2019Dot3PostGenerateGradleProject : PostGenerateGradleAndroidProject, IPostGenerateGradleAndroidProject
	{
		private const string Src = "src";
		private const string Main = "main";
		private const string Launcher = "launcher";
		private const string UnityLibrary = "unityLibrary";

		protected override string SourceResourcesFolder
		{
			get
			{
				return Path.Combine(Application.dataPath, "Skillz", "Resources", "2019.3");
			}
		}

		public void OnPostGenerateGradleAndroidProject(string basePath)
		{
			ModifyGradleProject(CoerceBasePath(basePath));
		}

		protected override void ModifyAndroidManifests(string basePath)
		{
			ModifyLauncherManifest(Path.Combine(basePath, Launcher, Src, Main, AndroidManifestDotXml));
			ModifyUnityLibraryManifest(Path.Combine(basePath, UnityLibrary, Src, Main, AndroidManifestDotXml));
		}

		protected override string GetProguardRulesProPath(string basePath)
		{
			return Path.Combine(basePath, Launcher, "proguard-rules.pro");
		}

		protected override string GetMultidexKeepPath(string basePath)
		{
			return Path.Combine(basePath, Launcher, "multidex-keep.txt");
		}

		protected override void PerformMiscWork(string basePath)
		{
			FileUtil.ReplaceFile(Path.Combine(PluginsFolder, "consumer-rules.pro"), Path.Combine(basePath, UnityLibrary, "consumer-rules.pro"));
		}

		private void ModifyLauncherManifest(string manifestPath)
		{
			Debug.Log(string.Format(Constants.LogFormat, $"Writing Skillz specific data to '{manifestPath}'"));

			var manifest = new AndroidManifest(manifestPath);

			manifest.SetSupportsScreens(small: true, normal: true, large: true, xLarge: true, anyDensity: true);
			manifest.SetApplicationTheme("@android:style/Theme.NoTitleBar.Fullscreen");

			// See: https://developer.android.com/about/versions/pie/android-9.0-changes-28#apache-p
			manifest.AddUsesLibrary("org.apache.http.legacy", false);

			manifest.Save();
		}

		private void ModifyUnityLibraryManifest(string manifestPath)
		{
			Debug.Log(string.Format(Constants.LogFormat, $"Writing Skillz specific data to '{manifestPath}'"));

			const string skillzActivityName = "com.skillz.activity.UnityGameActivity";

			var manifest = new AndroidManifest(manifestPath);

			manifest.SetStartingActivityName(skillzActivityName);
			manifest.SetLaunchMode("singleTop");
			manifest.SetClearTaskOnLaunch("false");
			manifest.SetAlwaysRetainTaskState("true");

			manifest.AddMetadataElement("skillz_allow_exit", SkillzSettings.Instance.AllowSkillzExit.ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_game_has_sync_bot", SkillzSettings.Instance.HasSyncBot.ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_game_id", SkillzSettings.Instance.GameID.ToString());
			manifest.AddMetadataElement("skillz_production", (SkillzSettings.Instance.Environment == Environment.Production).ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_game_activity", skillzActivityName);
			manifest.AddMetadataElement("skillz_orientation", SkillzSettings.Instance.Orientation.ToString().ToLowerInvariant());
			manifest.AddMetadataElement("skillz_is_unity", "true");

			manifest.AddUsesFeature("glEsVersion", "0x00020000");

			manifest.Save();
		}

		protected override string GetGradlePath(string basePath)
		{
			return Path.Combine(basePath, UnityLibrary, BuildDotGradle);
		}

		protected override string GetFirebaseManifestPath(string basePath)
		{
			return Path.Combine(basePath, UnityLibrary, Firebase, AndroidManifestDotXml);
		}

		protected override string GetFirebaseGradlePath(string basePath)
		{
			return Path.Combine(basePath, UnityLibrary, Firebase, BuildDotGradle);
		}

		private static string CoerceBasePath(string basePath)
		{
			var pathComponents = basePath.Split(Path.DirectorySeparatorChar);
			if (string.CompareOrdinal(pathComponents[pathComponents.Length - 1], UnityLibrary) != 0)
			{
				Debug.Log(string.Format(Constants.LogFormat, $"No need to coerce base path '{basePath}'"));
				return basePath;
			}

			pathComponents =  pathComponents.Take(pathComponents.Length - 1).ToArray();
			var coercedPath = string.Join(new string(Path.DirectorySeparatorChar, 1), pathComponents);
			Debug.Log(string.Format(Constants.LogFormat, $"Coerced base path to '{coercedPath}'"));

			return coercedPath;
		}
	}
}
#endif // UNITY_EDITOR && UNITY_ANDROID && UNITY_2019_3_OR_NEWER