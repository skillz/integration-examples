namespace SkillzSDK.Settings
{
	internal abstract class SettingsLoader
	{
		protected const string SkillzFolder = "Skillz";
		protected const string SkillzAssetName = "skillz-settings";

		protected abstract bool AssetExists
		{
			get;
		}

		public SkillzSettings Load()
		{
			return AssetExists ? LoadSettings() : CreateSettings();
		}

		protected abstract SkillzSettings LoadSettings();

		protected abstract SkillzSettings CreateSettings();
	}
}