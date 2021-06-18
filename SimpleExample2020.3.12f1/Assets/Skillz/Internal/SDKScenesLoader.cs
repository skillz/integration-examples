using System;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

// NOTE: The wonky pragmas here are for getting generation of the Asset Bundles
// and compilation to work without forcing each consumer of this class to use
// the pragma.

namespace SkillzSDK.Internal
{
	internal static class SDKScenesLoader
	{
		public const string TournamentSelectionScene = "TournamentSelection";
		public const string MatchWonScene = "MatchWon";
		public const string MatchLostScene = "MatchLost";
		public const string MatchAbortedScene = "MatchAborted";
		public const string EnteringMatchScene = "EnteringMatch";

		private const string LogFormat = "[Skillz] {0}";

		private static AssetBundle scenesAssetBundle;
		private static AssetBundle ancillaryAssetBundle;

		static SDKScenesLoader()
		{
#if UNITY_EDITOR
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
#endif
		}

		public static void Load(string sdkSceneName)
		{
			try
			{
				// This bundle contains assets that the scenes in 'skillz-scenes' depend on,
				// so this must be loaded first for the all the dependencies to be available.
				if (ancillaryAssetBundle == null)
				{
					ancillaryAssetBundle = AssetBundle.LoadFromFile(Path.Combine("Assets", "Skillz", "Internal", "AssetBundles", "skillz-assets"));
				}

				if (scenesAssetBundle == null)
				{
					scenesAssetBundle = AssetBundle.LoadFromFile(Path.Combine("Assets", "Skillz", "Internal", "AssetBundles", "skillz-scenes"));
				}

				var sdkScenePath = scenesAssetBundle.GetAllScenePaths().FirstOrDefault(scenePath => string.Equals(Path.GetFileNameWithoutExtension(scenePath), sdkSceneName, StringComparison.InvariantCulture));
				if (string.IsNullOrEmpty(sdkScenePath))
				{
					Debug.LogWarning(string.Format(LogFormat, sdkSceneName));
				}

				SceneManager.LoadScene(sdkScenePath);
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat(string.Format(LogFormat, e));
			}
		}

#if UNITY_EDITOR
		private static void PlayModeStateChanged(PlayModeStateChange playmodeState)
		{
			if (playmodeState != PlayModeStateChange.ExitingPlayMode)
			{
				return;
			}

			EditorApplication.playModeStateChanged -= PlayModeStateChanged;

			Debug.Log(string.Format(LogFormat, "Unloading the SDK's AssetBundle"));

			scenesAssetBundle.Unload(true);
			ancillaryAssetBundle.Unload(true);
		}
#endif
	}
}
