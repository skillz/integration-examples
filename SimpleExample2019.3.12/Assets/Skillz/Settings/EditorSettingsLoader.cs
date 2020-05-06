#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SkillzSDK.Settings
{
	internal sealed class EditorSettingsLoader : SettingsLoader
	{
		private const string LogFormat = "[Skillz Editor Settings] {0}";

		private const string RootAssetsFolder = "Assets";
		private const string ResourcesFolder = "Resources";
		private const string AssetExt = ".asset";

		private static string BaseResourcesFolder
		{
			get
			{
				return Path.Combine(RootAssetsFolder, ResourcesFolder);
			}
		}

		private static string AssetFolder
		{
			get
			{
				return Path.Combine(BaseResourcesFolder, SkillzFolder);
			}
		}

		private static string AssetPath
		{
			get
			{
				return Path.Combine(AssetFolder, string.Concat(SkillzAssetName, AssetExt));
			}
		}

		protected override bool AssetExists
		{
			get
			{
				return AssetDatabase
					.FindAssets($"t:{typeof(SkillzSettings)}")
					.Any(assetGuid =>
					{
						// HACK: GUIDToAssetPath returns a path with `/` delimiters regardless of platform (re: Windows vs Mac OS)
						var normalizedAssetPath = AssetPath.Replace(@"\", @"/");

						return string.Compare(AssetDatabase.GUIDToAssetPath(assetGuid), normalizedAssetPath, StringComparison.InvariantCulture) == 0;
					});
			}
		}

		protected override SkillzSettings CreateSettings()
		{
			var skillzSettings = ScriptableObject.CreateInstance<SkillzSettings>();

			if (!AssetDatabase.IsValidFolder(BaseResourcesFolder))
			{
				Debug.LogWarning(string.Format(LogFormat, $"'{BaseResourcesFolder}' is missing, Skillz will create it."));

				var baseResourcesFolder = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(RootAssetsFolder, ResourcesFolder));
				Debug.Log(string.Format(LogFormat, $"Created asset folder '{baseResourcesFolder}'"));
			}

			var skillzAssetsFolder = AssetFolder;
			if (!AssetDatabase.IsValidFolder(skillzAssetsFolder))
			{
				skillzAssetsFolder = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(BaseResourcesFolder, SkillzFolder));
				Debug.Log(string.Format(LogFormat, $"Created asset folder '{skillzAssetsFolder}'"));
			}

			Debug.Log(string.Format($"Creating Skillz settings at '{AssetPath}'"));
			AssetDatabase.CreateAsset(skillzSettings, AssetPath);

			return skillzSettings;
		}

		protected override SkillzSettings LoadSettings()
		{
			try
			{
				Debug.Log(string.Format(LogFormat, $"Loading Skillz Settings from '{AssetPath}'"));
				return AssetDatabase.LoadAssetAtPath<SkillzSettings>(AssetPath);
			}
			catch (Exception exception)
			{
				Debug.LogWarning(string.Format(LogFormat, $"Failed to load the Skillz Settings from '{AssetPath}'!"));
				Debug.LogWarning(exception);

				AssetDatabase.DeleteAsset(AssetPath);

				return CreateSettings();
			}
		}
	}
}
#endif