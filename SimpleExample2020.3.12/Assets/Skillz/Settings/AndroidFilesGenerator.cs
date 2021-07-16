#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SkillzSDK.Settings
{
	internal abstract class AndroidFilesGenerator
	{
		private const string LogFormat = "[Skillz] {0}";

		private const string RootAssetsFolder = "Assets";

		private const string MainTemplateGradle = "mainTemplate.gradle";

		private const string ConsumerRules = "consumer-rules.pro";

		private static readonly string AndroidPluginDirectory = Path.Combine(RootAssetsFolder, "Plugins", "Android");
		private static readonly string BuildGradlePath = Path.Combine(AndroidPluginDirectory, "mainTemplate.gradle");

		private static readonly string SkillzPluginResourcesFolder = Path.Combine(RootAssetsFolder, "Skillz", "Resources");

		protected abstract string OverwriteFilesMessage
		{
			get;
		}

		protected abstract string SourceResourcesFolder
		{
			get;
		}

		public static void GenerateFiles()
		{
			var filesGenerator = CreateInstance();

			Directory.CreateDirectory(AndroidPluginDirectory);

			if (!EditorUtility.DisplayDialog("Overwrite Android Files?", filesGenerator.OverwriteFilesMessage, "Yes", "No"))
			{
				return;
			}

			filesGenerator.CreateDefaultGradleTemplates();
			filesGenerator.CreateDefaultProGuard();
			filesGenerator.CreateMultidexKeep();

			// Refresh the editor so these files are immediately visible in the editor.
			AssetDatabase.Refresh();
		}

		private static AndroidFilesGenerator CreateInstance()
		{
#if UNITY_2019_3_OR_NEWER
			return new Unity2019Dot3Implementation();
#else
			return new PreUnity2019Dot3Implementation();
#endif // UNITY_2019_OR_NEWER
		}

		protected abstract void CreateDefaultGradleTemplatesCore();

		private void CreateDefaultGradleTemplates()
		{
			Debug.Log(string.Format(LogFormat, $"Generating {BuildGradlePath}"));
			CreateDefaultGradleTemplatesCore();
		}

		private void CreateDefaultProGuard()
		{
			var proguardPath = Path.Combine(AndroidPluginDirectory, "proguard-rules.pro");

			Debug.Log(string.Format(LogFormat, $"Generating {proguardPath}"));
			FileUtil.ReplaceFile(Path.Combine(SourceResourcesFolder, "proguard-rules.pro"), proguardPath);
		}

		private void CreateMultidexKeep()
		{
			var multidexKeepPath = Path.Combine(AndroidPluginDirectory, "multidex-keep.txt");

			Debug.Log(string.Format(LogFormat, $"Generating {multidexKeepPath}"));
			FileUtil.ReplaceFile(Path.Combine(SkillzPluginResourcesFolder, "multidex-keep.txt"), multidexKeepPath);
		}

#if UNITY_2019_3_OR_NEWER
		/// <summary>
		/// Unity 2019.3 changed the Android Studio project structure because of
		/// the new "Unity as a Library" feature. This Android files generator
		/// will generate the appropriate gradle templates, etc. for 2019.3 and above.
		/// </summary>
		private sealed class Unity2019Dot3Implementation : AndroidFilesGenerator
		{
			private const string BaseProjectTemplateGradle = "baseProjectTemplate.gradle";
			private const string LauncherTemplateGradle = "launcherTemplate.gradle";

			protected override string OverwriteFilesMessage
			{
				get
				{
					return "Allow Skillz to overwrite mainTemplate.gradle, baseProjectTemplate.gradle, launcherTemplate.gradle, multidex-keep.txt, proguard-rules.pro, and consumer-rules.pro?";
				}
			}

			protected override string SourceResourcesFolder
			{
				get
				{
					return Path.Combine(SkillzPluginResourcesFolder, "2019.3");
				}
			}

			protected override void CreateDefaultGradleTemplatesCore()
			{
				FileUtil.ReplaceFile(Path.Combine(SourceResourcesFolder, MainTemplateGradle), Path.Combine(AndroidPluginDirectory, MainTemplateGradle));
				FileUtil.ReplaceFile(Path.Combine(SourceResourcesFolder, BaseProjectTemplateGradle), Path.Combine(AndroidPluginDirectory, BaseProjectTemplateGradle));
				FileUtil.ReplaceFile(Path.Combine(SourceResourcesFolder, LauncherTemplateGradle), Path.Combine(AndroidPluginDirectory, LauncherTemplateGradle));
				FileUtil.ReplaceFile(Path.Combine(SourceResourcesFolder, ConsumerRules), Path.Combine(AndroidPluginDirectory, ConsumerRules));
			}
		}
#endif // UNITY_2019_OR_NEWER

		/// <summary>
		/// This Android files generator will generate the gradle template, etc.
		/// for Unity 2019.2 and earlier.
		/// </summary>
		private sealed class PreUnity2019Dot3Implementation : AndroidFilesGenerator
		{
			protected override string OverwriteFilesMessage
			{
				get
				{
					return "Allow Skillz to overwrite mainTemplate.gradle, multidex-keep.txt, and proguard-rules.pro?";
				}
			}

			protected override string SourceResourcesFolder
			{
				get
				{
					return Path.Combine(SkillzPluginResourcesFolder, "2019.2");
				}
			}


			protected override void CreateDefaultGradleTemplatesCore()
			{
				FileUtil.ReplaceFile(Path.Combine(SourceResourcesFolder, MainTemplateGradle), Path.Combine(AndroidPluginDirectory, MainTemplateGradle));
			}
		}
	}
}
#endif //UNITY_EDITOR