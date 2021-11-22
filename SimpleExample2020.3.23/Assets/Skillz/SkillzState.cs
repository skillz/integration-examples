using System;
using System.Collections.Generic;
using SkillzSDK.Internal.API;
using SkillzSDK.Settings;
using UnityEngine;

namespace SkillzSDK
{
	public static class SkillzState
	{
		private const string LogFormat = "[Skillz] {0}";
		private const string SkillzDelegateName = "SkillzDelegate";

		private static SkillzMatchDelegate asyncDelegate;
		private static SkillzSyncDelegate syncDelegate;
		private static ISyncDelegateInitializer syncDelegateInitializer;

		private static ISyncDelegateInitializer SyncDelegateInitializer
		{
			get
			{
				if (syncDelegateInitializer == null)
				{
#if UNITY_ANDROID
					syncDelegateInitializer = new SkillzSDK.Internal.API.Android.BridgedAPI();
#elif UNITY_IOS
					syncDelegateInitializer = new SkillzSDK.Internal.API.iOS.BridgedAPI();
#else
					syncDelegateInitializer = new SkillzSDK.Internal.API.Dummy.BridgedAPI();
#endif
				}

				return syncDelegateInitializer;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#pragma warning disable IDE0051 // Remove unused private members
		private static void OnAfterFirstSceneLoaded()
#pragma warning restore IDE0051 // Remove unused private members
		{
			// This method will be invoked only once, which is after
			// the initial scene of the game has been loaded.

			InitializeSkillz();
			InitializeSkillzDelegate();
		}

		public static void SetAsyncDelegate(SkillzMatchDelegate skillzDelegate)
		{
			asyncDelegate = skillzDelegate;
		}

		public static void SetSyncDelegate(SkillzSyncDelegate skillzSyncDelegate)
		{
			syncDelegate = skillzSyncDelegate;

			SyncDelegateInitializer.Initialize(skillzSyncDelegate);
		}

		public static void NotifyMatchWillBegin(string matchInfoJson)
		{
			Dictionary<string, object> matchInfoDict = DeserializeJSONToDictionary(matchInfoJson);
			Debug.Log(string.Format(LogFormat, $"Match info: {matchInfoJson}"));

			try
			{
				Match matchInfo = new Match(matchInfoDict);

				if (asyncDelegate != null)
				{
					asyncDelegate.OnMatchWillBegin(matchInfo);
				}
				else if (syncDelegate != null)
				{
					// We must re-initialize the sync delegate on Android for each match.
#if UNITY_ANDROID
					SyncDelegateInitializer.Initialize(syncDelegate);
#endif
					syncDelegate.OnMatchWillBegin(matchInfo);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format(LogFormat, $"Error retrieving the match info! Error={e}"));
				throw;
			}
		}

		public static void NotifySkillzWillExit()
		{
			if (asyncDelegate != null)
			{
				asyncDelegate.OnSkillzWillExit();
			}
			else if (syncDelegate != null)
			{
				syncDelegate.OnSkillzWillExit();
			}
		}

		public static void NotifyProgressionRoomEnter()
		{
			if (asyncDelegate != null)
			{
				asyncDelegate.OnProgressionRoomEnter();
			}
			else if (syncDelegate != null)
			{
				syncDelegate.OnProgressionRoomEnter();
			}
		}

		private static void InitializeSkillz()
		{
			Debug.Log(string.Format(LogFormat, $"Initializing Skillz:"));
			Debug.Log(string.Format(LogFormat, $"Game ID={SkillzSettings.Instance.GameID}"));
			Debug.Log(string.Format(LogFormat, $"Environment={SkillzSettings.Instance.Environment}"));
			Debug.Log(string.Format(LogFormat, $"Orientation={SkillzSettings.Instance.Orientation}"));

			SkillzCrossPlatform.Initialize(
				SkillzSettings.Instance.GameID,
				SkillzSettings.Instance.Environment,
				SkillzSettings.Instance.Orientation
			);
		}

		private static void InitializeSkillzDelegate()
		{
			// We are adding a SkillzDelegate game object to the start
			// scene for historical reasons.
			// In previous releases, devs would add an instance via a
			// Skillz -> Generate Delegate menu item.
			//
			// Although most of SkillzDelegate's functionality has been
			// moved to here, the iOS and Android SDKs call the
			// UnitySendMessage to invoke it as a callback when a match
			// begins and ends.
			//
			// Unfortunately, UnitySendMessage can only call a method on a
			// game object that is present in a scene, so we must live
			// with loading an instance in the startup scene.

			Debug.Log(string.Format(LogFormat, "Initializing SkillzDelegate"));

			var gameObject = new GameObject();
			gameObject.name = SkillzDelegateName;

			gameObject.AddComponent<SkillzDelegate>();

#if UNITY_IOS || UNITY_ANDROID
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
#endif
		}

		private static Dictionary<string, object> DeserializeJSONToDictionary(string jsonString)
		{
			return MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
		}
	}
}