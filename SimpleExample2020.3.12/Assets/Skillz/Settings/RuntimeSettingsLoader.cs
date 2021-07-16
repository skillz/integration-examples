#if !UNITY_EDITOR
using UnityEngine;

namespace SkillzSDK.Settings
{
	internal sealed class RuntimeSettingsLoader : SettingsLoader
	{
		private const string LogFormat = "[Skillz Runtime Settings] {0}";

		protected override bool AssetExists
		{
			get
			{
				if (skillzSettings == null)
				{
					// This API is wonky. The path will be expanded to Assets/Resources/Skillz/skillz-settings.
					skillzSettings = Resources.Load<SkillzSettings>($"{SkillzFolder}/{SkillzAssetName}");
				}

				return skillzSettings != null;
			}
		}

		private SkillzSettings skillzSettings;

		protected override SkillzSettings CreateSettings()
		{
			Debug.LogError(string.Format(LogFormat, "Skillz Settings were not found. Did you configure your game for Skillz in the Unity editor?"));
			Debug.LogError(string.Format(LogFormat, "Creating temporary Skillz Settings"));

			return ScriptableObject.CreateInstance<SkillzSettings>();
		}

		protected override SkillzSettings LoadSettings()
		{
			Debug.Log(string.Format(LogFormat, $"Loading Skillz Settings"));

			return skillzSettings;
		}
	}
}
#endif