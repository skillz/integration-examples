#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SkillzSDK.Internal.Build.Android
{
	internal abstract class PostGenerateGradleAndroidProject
	{
		protected const string SkillzActivityName = "com.skillz.activity.UnityGameActivity";
		protected const string Firebase = "Firebase";
		protected const string AndroidManifestDotXml = "AndroidManifest.xml";
		protected const string BuildDotGradle = "build.gradle";

		public int callbackOrder
		{
			get
			{
				return int.MaxValue;
			}
		}

		protected void ModifyGradleProject(string basePath)
		{
			AppendLocallySetSdkVersionToGradleProperties(GetProjectGradlePropertiesPath(basePath));
			EnableAndroidxInGradleProperties(GetProjectGradlePropertiesPath(basePath));
			ModifyAndroidManifests(basePath);
			ModifyGradleFile(GetGradlePath(basePath));
			ModifyFirebaseManifest(GetFirebaseManifestPath(basePath));
			ModifyFirebaseGradle(GetFirebaseGradlePath(basePath));
			CopyMultidexKeep(GetMultidexKeepPath(basePath));
		}

		protected abstract void ModifyAndroidManifests(string basePath);
		protected abstract string GetMultidexKeepPath(string basePath);
		protected abstract string GetGradlePath(string basePath);
		protected abstract string GetFirebaseManifestPath(string basePath);
		protected abstract string GetFirebaseGradlePath(string basePath);

		private void AppendLocallySetSdkVersionToGradleProperties(string projectGradlePropertiesPath)
		{
			if (!UseLocallyProvidedSkillzVersion(out string localSkillzVersion))
			{
				return;
			}

			Debug.Log(string.Format(Constants.LogFormat, $"Appending {Constants.SkillzVersionVariable}={localSkillzVersion} to '{projectGradlePropertiesPath}'"));
			using (var projectGradleProperties = new GradleProperties(projectGradlePropertiesPath))
			{
				projectGradleProperties.Append($"{Constants.SkillzVersionVariable}={localSkillzVersion}");
			}
		}

		private void EnableAndroidxInGradleProperties(string projectGradlePropertiesPath)
		{
			Debug.Log($"Adding two properties to '{projectGradlePropertiesPath}' to enable Androidx");

			// https://stackoverflow.com/questions/52033810/can-i-use-library-that-used-android-support-with-androidx-projects
			const string useAndroidX = "android.useAndroidX=true";
			const string enableJetifier = "android.enableJetifier=true";

			using (var projectGradleProperties = new GradleProperties(projectGradlePropertiesPath))
			{
				// The developer should have checked the "Use Jetifier" option in the
				// Google Play Services Resolver to swap out the old App Compat libraries
				// for their Androidx counterparts. However, enable it in the exported
				// project as a precaution in case they did not.

				if (!projectGradleProperties.FileContents.Contains(useAndroidX))
				{
					projectGradleProperties.Append(useAndroidX);
				}

				if (!projectGradleProperties.FileContents.Contains(enableJetifier))
				{
					projectGradleProperties.Append(enableJetifier);
				}
			}
		}

		private void ModifyGradleFile(string gradlePath)
		{
			if (!File.Exists(gradlePath))
			{
				return;
			}

			Debug.Log(string.Format(Constants.LogFormat, $"Modifying '{gradlePath}'"));

			using (var buildDotGradle = new BuildDotGradle(gradlePath))
			{
				// HACK: Manually excluding some transitive dependencies because
				// the Google Play Resolver currently does not have exclusion semantics!!!
				//
				// The GPR resolves dependencies either by patching the gradle template
				// or downloading AARs, so sweep the gradle file for both.

				ExcludeCardIoFromPaypal(buildDotGradle);
				ExcludeCardIoAAR(buildDotGradle);

				ExcludeMobileAppKitFromMopub(buildDotGradle);
				ExcludeMobileAppKitAAR(buildDotGradle);

				ExcludeReactFromLinearGradient(buildDotGradle);
				ExcludeReactFromImagePicker(buildDotGradle);
				ExcludeReactFromScrollView(buildDotGradle);
				ExcludeReactAAR(buildDotGradle);
			}
		}

		private void ExcludeCardIoFromPaypal(BuildDotGradle buildDotGradle)
		{
			const string paypalPackage = "com.paypal.sdk:paypal-android-sdk";
			const string cardIoGroup = "io.card";

			buildDotGradle.ExcludeItemFromDependency(cardIoGroup, paypalPackage);
		}

		private void ExcludeCardIoAAR(BuildDotGradle buildDotGradle)
		{
			const string cardIoName = "io.card.android-sdk-5.4.0";

			buildDotGradle.ExcludeAARDependency(cardIoName);
		}

		private void ExcludeMobileAppKitFromMopub(BuildDotGradle buildDotGradle)
		{
			const string mopubPackage = "com.skillz-mopub:mopub-sdk-base";
			const string mobileAppKitModule = "moat-mobile-app-kit";

			buildDotGradle.ExcludeItemFromDependency(mobileAppKitModule, mopubPackage, ExcludeType.Module);
		}

		private void ExcludeMobileAppKitAAR(BuildDotGradle buildDotGradle)
		{
			const string mobileAppKitPackage = "com.moat.analytics.mobile.mpub.moat-mobile-app-kit-2.4.5";

			buildDotGradle.ExcludeAARDependency(mobileAppKitPackage);
		}

		private void ExcludeReactFromLinearGradient(BuildDotGradle buildDotGradle)
		{
			const string linearGradientPackage = "com.BV:react-native-linear-gradient";

			ExcludeReactFromPackage(buildDotGradle, linearGradientPackage);
		}

		private void ExcludeReactFromImagePicker(BuildDotGradle buildDotGradle)
		{
			const string imagePickerPackage = "com.react-native-image-picker:rn-image-picker-android";

			ExcludeReactFromPackage(buildDotGradle, imagePickerPackage);
		}

		private void ExcludeReactFromScrollView(BuildDotGradle buildDotGradle)
		{
			const string scrollViewPackage = "com.react-native-spring-scrollview:react-native-spring-scrollview-android";

			ExcludeReactFromPackage(buildDotGradle, scrollViewPackage);
		}

		private void ExcludeReactFromPackage(BuildDotGradle buildDotGradle, string packageName)
		{
			const string reactGroup = "com.facebook.react";

			buildDotGradle.ExcludeItemFromDependency(reactGroup, packageName);
		}

		private void ExcludeReactAAR(BuildDotGradle buildDotGradle)
		{
			// Ensure the stock react-native package doesn't conflict with the V8 counterpart
			buildDotGradle.ExcludeAARDependency("com.facebook.react.react-native-0.59.10");
		}

		private bool UseLocallyProvidedSkillzVersion(out string localSkillzVersion)
		{
			localSkillzVersion = GetLocallyProvidedSkillzVersion();

			return !string.IsNullOrWhiteSpace(localSkillzVersion);
		}

		private string GetLocallyProvidedSkillzVersion()
		{
			var localGradlePropertiesPath = GetLocalGradlePropertiesPath();

			if (!File.Exists(localGradlePropertiesPath))
			{
				Debug.Log(string.Format(Constants.LogFormat, $"'{localGradlePropertiesPath}' does not exist."));
				return string.Empty;
			}

			var gradleProperties = new GradleProperties(localGradlePropertiesPath);
			if (string.IsNullOrWhiteSpace(gradleProperties.SkillzMajorVersionValue))
			{
				Debug.Log(string.Format(Constants.LogFormat, $"The {Constants.SkillzVersionVariable} variable was not set."));
			}

			return gradleProperties.SkillzMajorVersionValue;
		}

		private void ModifyFirebaseManifest(string firebaseManifestPath)
		{
			if (!File.Exists(firebaseManifestPath))
			{
				return;
			}

			Debug.Log(string.Format(Constants.LogFormat, "Making adjustments to Firebase's AndroidManifest.xml"));

			var manifest = new AndroidManifest(firebaseManifestPath);

			manifest.RemoveUsesSdkElement();

			manifest.Save();
		}

		private void ModifyFirebaseGradle(string firebaseGradlePath)
		{
			if (!File.Exists(firebaseGradlePath))
			{
				return;
			}

			Debug.Log(string.Format(Constants.LogFormat, "Making adjustments to Firebase's build.gradle"));

			const uint androidSdkLevel = 28;
			const string androidToolsPluginVersion = "3.5.2";

			using (var buildDotGradle = new BuildDotGradle(firebaseGradlePath))
			{
				buildDotGradle.ChangeCompileSdkVersion(androidSdkLevel);
				buildDotGradle.ChangeTargetSdkVersion(androidSdkLevel);
				buildDotGradle.ChangeAndroidToolsPluginVersion(androidToolsPluginVersion);
			}
		}

		private void CopyMultidexKeep(string destPath)
		{
			// Unity isn't copying the multidex-keep.txt that we emit
			// to /Assets/Plugins/Android/
			Debug.Log($"Making '{destPath}'");
			FileUtil.ReplaceFile(Path.Combine(Application.dataPath, "Skillz", "Resources", "multidex-keep.txt"), destPath);
		}

		private static string GetProjectGradlePropertiesPath(string basePath)
		{
			return Path.Combine(basePath, "gradle.properties");
		}

		private static string GetLocalGradlePropertiesPath()
		{
			return Path.Combine(Application.dataPath, "Skillz", "Resources", "gradle.properties");
		}
	}
}
#endif