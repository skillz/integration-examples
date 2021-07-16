using System;
using UnityEngine;

namespace SkillzSDK.Settings
{
	[Serializable]
	public sealed class SkillzSettings : ScriptableObject
	{
		public const int MaxMatchParameters = 10;

		private static SkillzSettings instance;
		private static SettingsLoader settingsLoader;

		public static SkillzSettings Instance
		{
			get
			{
				return instance != null ? instance : (instance = SettingsLoader.Load());
			}
		}

		[HideInInspector]
		[SerializeField]
		public int GameID;

		[HideInInspector]
		[SerializeField]
		public Environment Environment;

		[HideInInspector]
		[SerializeField]
		public Orientation Orientation;

		[HideInInspector]
		[SerializeField]
		public StringKeyValue[] MatchParameters;

		[HideInInspector]
		[SerializeField]
		public bool SimulateMatchWins;

		[HideInInspector]
		[SerializeField]
		public bool AllowSkillzExit;

		[HideInInspector]
		[SerializeField]
		public bool HasSyncBot;

		private static SettingsLoader SettingsLoader
		{
			get
			{
				if (settingsLoader == null)
				{
#if UNITY_EDITOR
					settingsLoader = new EditorSettingsLoader();
#else
					settingsLoader = new RuntimeSettingsLoader();
#endif
				}

				return settingsLoader;
			}
		}

		private SkillzSettings()
		{
			MatchParameters = new StringKeyValue[MaxMatchParameters];
			AllowSkillzExit = true;
			HasSyncBot = false;
		}
	}
}