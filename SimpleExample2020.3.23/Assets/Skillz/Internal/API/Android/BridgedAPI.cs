using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
namespace SkillzSDK.Internal.API.Android
{
	internal sealed class BridgedAPI : IBridgedAPI, IRandom, ISyncDelegateInitializer
	{
		public IRandom Random
		{
			get
			{
				return this;
			}
		}

		bool IAsyncAPI.IsMatchInProgress
		{
			get
			{
				using (var skillz = GetSkillz())
				{
					return skillz.CallStatic<bool>("isMatchInProgress");
				}
			}
		}

		public float SkillzMusicVolume
		{
			get
			{
				using (var skillzAudio = GetSkillzAudio())
				{
					return skillzAudio.CallStatic<float>("getSkillzMusicVolume");
				}
			}
			set
			{
				using (var skillzAudio = GetSkillzAudio())
				{
					skillzAudio.CallStatic("setSkillzMusicVolume", value);
				}
			}
		}

		public float SoundEffectsVolume
		{
			get
			{
				using (var skillzAudio = GetSkillzAudio())
				{
					return skillzAudio.CallStatic<float>("getSFXVolume");
				}
			}
			set
			{
				using (var skillzAudio = GetSkillzAudio())
				{
					skillzAudio.CallStatic("setSFXVolume", value);
				}
			}
		}

		bool ISyncAPI.IsMatchCompleted
		{
			get
			{
				using (var skillzSync = GetSkillzSync())
				{
					return skillzSync.CallStatic<bool>("isMatchCompleted");
				}
			}
		}

		public void Initialize(int gameID, Environment environment, Orientation orientation)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				Debug.LogWarning("Trying to initialize Skillz on a platform other than Android");
				return;
			}

			string environmentString = environment == Environment.Sandbox
				? "false"
				: "true";

			using (var preferences = GetSkillzPreferences())
			{
				preferences.CallStatic("setCrossplatformGameOrientation", GetCurrentActivity(), orientation.ToString().ToLower());
				preferences.CallStatic("setCrossplatformGameId", GetCurrentActivity(), gameID.ToString());
				preferences.CallStatic("setCrossplatformGameEnvironment", GetCurrentActivity(), environmentString);
			}
		}

		public void LaunchSkillz()
		{
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("launch", GetCurrentActivity());
			}
		}

		public Hashtable GetMatchRules()
		{
			var matchRules = new Hashtable();

			using (var skillz = GetSkillz())
			using (var matchRulesJO = skillz.CallStatic<AndroidJavaObject>("getMatchRules"))
			using (var matchRulesEntrySet = matchRulesJO.Call<AndroidJavaObject>("entrySet"))
			using (var matchRulesIterator = matchRulesEntrySet.Call<AndroidJavaObject>("iterator"))
			{
				while (matchRulesIterator.Call<bool>("hasNext"))
				{
					AndroidJavaObject next = matchRulesIterator.Call<AndroidJavaObject>("next");
					matchRules.Add(next.Call<string>("getKey"), next.Call<string>("getValue"));
				}
			}

			return matchRules;
		}

		public Match GetMatchInfo()
		{
			using (var skillz = GetSkillz())
			{
				var matchInfoJsonString = skillz.CallStatic<string>("getMatchInfoJsonString", GetCurrentActivity());
				Dictionary<string, object> matchInfoDict = DeserializeJSONToDictionary(matchInfoJsonString);
				return new Match(matchInfoDict);
			}
		}

		public void AbortMatch()
		{
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("abortMatch", GetCurrentActivity());
			}
		}
		
		public void AbortBotMatch(string botScore)
		{
			string matchId = GetMatchInfo().ID.ToString();
			using (var bigDecBotScore = new AndroidJavaObject("java.math.BigDecimal", botScore))
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("abortBotMatch", GetCurrentActivity(), bigDecBotScore, matchId);
			}
		}

		public void AbortBotMatch(int botScore)
		{
			AbortBotMatch(botScore.ToString());
		}

		public void AbortBotMatch(float botScore)
		{
			AbortBotMatch(botScore.ToString());
		}

		public void UpdatePlayersCurrentScore(string score)
		{
			using (var bigDecScore = new AndroidJavaObject("java.math.BigDecimal", score))
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("updatePlayersCurrentScore", GetCurrentActivity(), bigDecScore);
			}
		}

		public void UpdatePlayersCurrentScore(int score)
		{
			UpdatePlayersCurrentScore(score.ToString());
		}

		public void UpdatePlayersCurrentScore(float score)
		{
			UpdatePlayersCurrentScore(score.ToString());
		}

		public void DisplayTournamentResultsWithScore(string score)
		{
			string matchId = GetMatchInfo().ID.ToString();
			using (var bigDecScore = new AndroidJavaObject("java.math.BigDecimal", score))
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("displayTournamentResultsWithScore", GetCurrentActivity(), bigDecScore, matchId);
			}
		}

		public void DisplayTournamentResultsWithScore(int score)
		{
			DisplayTournamentResultsWithScore(score.ToString());
		}

		public void DisplayTournamentResultsWithScore(float score)
		{
			DisplayTournamentResultsWithScore(score.ToString());
		}

		public void ReportFinalScoreForBotMatch(string playerScore, string botScore)
		{
			string matchId = GetMatchInfo().ID.ToString();
			using (var bigDecPlayerScore = new AndroidJavaObject("java.math.BigDecimal", playerScore))
			using (var bigDecBotScore = new AndroidJavaObject("java.math.BigDecimal", botScore))
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("reportScoreForBotMatch", GetCurrentActivity(), bigDecPlayerScore, bigDecBotScore, matchId);
			}
		}

		public void ReportFinalScoreForBotMatch(int playerScore, int botScore)
		{
			ReportFinalScoreForBotMatch(playerScore.ToString(), botScore.ToString());
		}

		public void ReportFinalScoreForBotMatch(float playerScore, float botScore)
		{
			ReportFinalScoreForBotMatch(playerScore.ToString(), botScore.ToString());
		}
			
		public void SubmitScore(string score, Action successCallback, Action<string> failureCallback)
		{
			string matchId = GetMatchInfo().ID.ToString();
			using (var bigDecScore = new AndroidJavaObject("java.math.BigDecimal", score))
			using (var skillz = GetSkillz())
			{
				SkillzScoreCallback scoreCallback = new SkillzScoreCallback(successCallback, failureCallback);
				skillz.CallStatic("submitScore", GetCurrentActivity(), bigDecScore, matchId, scoreCallback);
			}
		}

		public void SubmitScore(int score, Action successCallback, Action<string> failureCallback)
		{
			SubmitScore(score.ToString(), successCallback, failureCallback);
		}

		public void SubmitScore(float score, Action successCallback, Action<string> failureCallback)
		{
			SubmitScore(score.ToString(), successCallback, failureCallback);
		}

		public bool EndReplay()
		{
			string matchId = null;
			if (((IAsyncAPI)this).IsMatchInProgress)
			{
				matchId = GetMatchInfo().ID.ToString();
			}
			using (var skillz = GetSkillz())
			{
				return skillz.CallStatic<bool>("endReplayRecording", matchId);
			}
		}

		public bool ReturnToSkillz()
		{
			string matchId = null;
			if (((IAsyncAPI)this).IsMatchInProgress)
			{
				matchId = GetMatchInfo().ID.ToString();
			}
			using (var skillz = GetSkillz())
			{
				return skillz.CallStatic<bool>("returnToSkillz", GetCurrentActivity(), matchId);
			}
		}

		public string SDKVersionShort()
		{
			using (var skillz = GetSkillz())
			{
				return skillz.CallStatic<string>("SDKVersionShort");
			}
		}

		public Player GetPlayer()
		{
			using (var skillz = GetSkillz())
			using (var playerNative = skillz.CallStatic<AndroidJavaObject>("getPlayerDetails", GetCurrentActivity()))
			{
				var map = new Dictionary<string, object>
				{
					{ "userName", playerNative.Call<string>("getUserName") },
					{ "userId", Convert.ToUInt32(playerNative.Call<string>("getUserId"), 10) },
					{ "avatarUrl", playerNative.Call<string>("getAvatarUrl") },
					{ "flagUrl", playerNative.Call<string>("getFlagUrl") },
					{ "playerMatchId", playerNative.Call<string>("getPlayerMatchId") },
					{ "isCurrentPlayer", playerNative.Call<bool>("isCurrentPlayer").ToString() }
				};

				return new Player(map);
			}
		}

		public void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress)
		{
			using (var skillz = GetSkillz())
			{
				skillz.CallStatic("addMetadataForUnityMatchInProgress", metadataJson, forMatchInProgress);
			}
		}

		public void SetSkillzBackgroundMusic(string fileName)
		{
			Debug.Log("SkillzAudio Skillz.cs setSkillzBackgroundMusic with file name: " + fileName);
			using (var skillzAudio = GetSkillzAudio())
			{
				skillzAudio.CallStatic("setSkillzBackgroundMusic", fileName);
			}
		}

		public void GetProgressionUserData(string progressionNamespace, List<string> userDataKeys, Action<Dictionary<string, ProgressionValue>> successCallback, Action<string> failureCallback)
		{
			using (var skillzProgression = GetSkillzProgression())
			{
				ProgressionCallback callback = new ProgressionCallback(successCallback, failureCallback);

				// Have to convert to an ArrayList to get across the JNI
				AndroidJavaObject keysList = new AndroidJavaObject("java.util.ArrayList");
				foreach (string key in userDataKeys)
				{
					keysList.Call<bool>("add", key);
				}
				skillzProgression.CallStatic("getUserData", progressionNamespace, keysList, callback);
			}
		}

		public void UpdateProgressionUserData(string progressionNamespace, Dictionary<string, object> userDataUpdates, Action successCallback, Action<string> failureCallback)
		{
			using (var skillzProgression = GetSkillzProgression())
			{
				ProgressionCallback callback = new ProgressionCallback(data => {
					if (successCallback != null)
					{
						successCallback();
					}
				}, failureCallback);

				// Have to convert to an ArrayList to get across the JNI
				AndroidJavaObject dataMap = new AndroidJavaObject("java.util.HashMap");
				foreach (string key in userDataUpdates.Keys)
				{
					object value = userDataUpdates[key];
					if (value != null)
					{
						dataMap.Call<AndroidJavaObject>("put", key, value.ToString());
					}
				}
				skillzProgression.CallStatic("updateUserData", progressionNamespace, dataMap, callback);
			}
		}

		int ISyncAPI.GetConnectedPlayerCount()
		{
			using (var skillzSync = GetSkillzSync())
			{
				return skillzSync.CallStatic<int>("getConnectedPlayerCount");
			}
		}

		ulong ISyncAPI.GetCurrentOpponentPlayerId()
		{
			using (var skillz = GetSkillzSync())
			{
				return (ulong)skillz.CallStatic<long>("getOpponentPlayerId");
			}
		}

		ulong ISyncAPI.GetCurrentPlayerId()
		{
			using (var skillz = GetSkillzSync())
			{
				return (ulong)skillz.CallStatic<long>("getCurrentPlayerId");
			}
		}

		double ISyncAPI.GetServerTime()
		{
			using (var skillzSync = GetSkillzSync())
			{
				return skillzSync.CallStatic<double>("getServerTime");
			}
		}

		long ISyncAPI.GetTimeLeftForReconnection(ulong playerId)
		{
			var javaClass = AndroidJNI.FindClass("com/skillz/SkillzSync");
			var javaMethod = AndroidJNI.GetStaticMethodID(javaClass, "getTimeLeftForReconnection", "(J)J");
			jvalue[] jArgs = AndroidJNIHelper.CreateJNIArgArray(new object[] { (long)playerId });
			return AndroidJNI.CallStaticLongMethod(javaClass, javaMethod, jArgs);
		}

		void ISyncAPI.SendData(byte[] data)
		{
			var javaClass = AndroidJNI.FindClass("com/skillz/SkillzSync");
			var javaMethod = AndroidJNI.GetStaticMethodID(javaClass, "sendData", "([B)V");
			jvalue[] jArgs = AndroidJNIHelper.CreateJNIArgArray(new object[] { data });
			AndroidJNI.CallStaticVoidMethod(javaClass, javaMethod, jArgs);
		}

		float IRandom.Value()
		{
			if (!((IAsyncAPI)this).IsMatchInProgress)
			{
				return UnityEngine.Random.value;
			}

			using (var skillz = GetSkillz())
			using (var skillzRandom = skillz.CallStatic<AndroidJavaObject>("getRandom"))
			{
				return skillzRandom.Call<float>("nextFloat");
			}
		}

		void ISyncDelegateInitializer.Initialize(SkillzSyncDelegate syncDelegate)
		{
			using (var skillzSync = GetSkillzSync())
			{
				skillzSync.CallStatic("registerSyncDelegate", new SkillzSyncProxy(syncDelegate), GetCurrentActivity());
			}
		}

		private static AndroidJavaClass GetSkillz()
		{
			return new AndroidJavaClass("com.skillz.Skillz");
		}

		private static AndroidJavaClass GetSkillzAudio()
		{
			return new AndroidJavaClass("com.skillz.util.music.SkillzAudio");
		}

		private static AndroidJavaClass GetSkillzSync()
		{
			return new AndroidJavaClass("com.skillz.SkillzSync");
		}

		private static AndroidJavaClass GetSkillzPreferences()
		{
			return new AndroidJavaClass("com.skillz.Skillz");
		}

		private static AndroidJavaObject GetCurrentActivity()
		{
			AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			return activity;
		}

		private static AndroidJavaObject GetSkillzProgression()
		{
			return new AndroidJavaClass("com.skillz.SkillzProgression");
		}

		private static Dictionary<string, object> DeserializeJSONToDictionary(string jsonString)
		{
			return MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
		}

		/// <summary>
		/// Proxy for the sync delegate in the Skillz Android SDK that acts a
		/// bridge to Unity.
		/// </summary>
		private sealed class SkillzSyncProxy : AndroidJavaProxy
		{
			private readonly SkillzSyncDelegate _syncDelegate;

			public SkillzSyncProxy(SkillzSyncDelegate syncDelegate)
				: base("com.skillz.SkillzSync$UnitySyncDelegate")
			{
				_syncDelegate = syncDelegate;
			}

			public void onMatchCompleted()
			{
				_syncDelegate.OnMatchCompleted();
			}

			public void onDidReceiveData(AndroidJavaObject dataReceived)
			{
				var dataFromAndroid = dataReceived.Get<AndroidJavaObject>("ReceivedData");
				var byteData = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(dataFromAndroid.GetRawObject());
				_syncDelegate.OnDidReceiveData(byteData);
			}

			public void onOpponentHasLostConnection(long playerId)
			{
				var opponentPlayerId = Convert.ToUInt64(playerId);
				_syncDelegate.OnOpponentHasLostConnection(opponentPlayerId);
			}

			public void onOpponentHasReconnected(long playerId)
			{
				var opponentPlayerId = Convert.ToUInt64(playerId);
				_syncDelegate.OnOpponentHasReconnected(opponentPlayerId);
			}

			public void onOpponentHasLeftMatch(long playerId)
			{
				var opponentPlayerId = Convert.ToUInt64(playerId);
				_syncDelegate.OnOpponentHasLeftMatch(opponentPlayerId);
			}

			public void onCurrentPlayerHasLostConnection()
			{
				_syncDelegate.OnCurrentPlayerHasLostConnection();
			}

			public void onCurrentPlayerHasReconnected()
			{
				_syncDelegate.OnCurrentPlayerHasReconnected();
			}

			public void onCurrentPlayerHasLeftMatch()
			{
				_syncDelegate.OnCurrentPlayerHasLeftMatch();
			}
		}
	}
}
#endif