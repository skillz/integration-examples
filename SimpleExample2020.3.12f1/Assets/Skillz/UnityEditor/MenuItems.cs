#if UNITY_EDITOR
using SkillzSDK.Settings;
using UnityEditor;

namespace SkillzSDK.UnityEditor
{
	public static class MenuItems
	{
		[MenuItem("Skillz/Regenerate Android Files")]
		public static void RegenerateAndroidFiles()
		{
			AndroidFilesGenerator.GenerateFiles();
		}
	}
}
#endif
