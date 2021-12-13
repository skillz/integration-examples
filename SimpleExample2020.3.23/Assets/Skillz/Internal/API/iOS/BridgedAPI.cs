using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_IOS
namespace SkillzSDK.Internal.API.iOS
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
				return (Application.platform == RuntimePlatform.IPhonePlayer) && (InteropMethods._tournamentIsInProgress() != 0);
			}
		}

		public float SkillzMusicVolume
		{
			get
			{
				return InteropMethods._getSkillzMusicVolume();
			}
			set
			{
				InteropMethods._setSkillzMusicVolume(value);
			}
		}

		public float SoundEffectsVolume
		{
			get
			{
				return InteropMethods._getSFXVolume();
			}
			set
			{
				InteropMethods._setSFXVolume(value);
			}
		}

		bool ISyncAPI.IsMatchCompleted
		{
			get
			{
				return InteropMethods._isMatchCompleted();
			}
		}

		private Match _matchInfo;

		public void Initialize(int gameID, Environment environment, Orientation orientation)
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer && !SystemInfo.deviceModel.ToLower().Contains("ipad"))
			{
				Debug.LogWarning("Trying to initialize Skillz on a platform other than iPhone");
				return;
			}

			var environmentString = environment == Environment.Sandbox
				? "SkillzSandbox"
				: "SkillzProduction";

			InteropMethods._skillzInitForGameIdAndEnvironment(gameID.ToString(), environmentString);
			SkillzProgressionProxy.Initialize();
			SkillzSDKProxy.Initialize();
		}

		public void LaunchSkillz()
		{
			InteropMethods._launchSkillz();
		}

		public Hashtable GetMatchRules()
		{
			var matchRules = Marshal.PtrToStringAnsi(InteropMethods._getMatchRules());
			Dictionary<string, object> matchInfoDict = DeserializeJSONToDictionary(matchRules);
			return new Hashtable(matchInfoDict);
		}

		public Match GetMatchInfo()
		{
			if (_matchInfo == null)
			{
				string matchInfo = Marshal.PtrToStringAnsi(InteropMethods._getMatchInfo());
				Dictionary<string, object> matchInfoDict = DeserializeJSONToDictionary(matchInfo);
				_matchInfo = new Match(matchInfoDict);
			}

			return _matchInfo;
		}

		public void AbortMatch()
		{
			InteropMethods._notifyPlayerAbortWithCompletion();
			_matchInfo = null;
		}

		public void AbortBotMatch(string botScore)
		{
			InteropMethods._notifyPlayerAbortWithStringBotScore(botScore);
			_matchInfo = null;
		}

		public void AbortBotMatch(int botScore)
		{
			InteropMethods._notifyPlayerAbortWithBotScore(botScore);
			_matchInfo = null;
		}

		public void AbortBotMatch(float botScore)
		{
			InteropMethods._notifyPlayerAbortWithFloatBotScore(botScore);
			_matchInfo = null;
		}

		public void UpdatePlayersCurrentScore(string score)
		{
			InteropMethods._updatePlayersCurrentStringScore(score);
		}

		public void UpdatePlayersCurrentScore(int score)
		{
			InteropMethods._updatePlayersCurrentIntScore(score);
		}

		public void UpdatePlayersCurrentScore(float score)
		{
			InteropMethods._updatePlayersCurrentScore(score);
		}

		public void DisplayTournamentResultsWithScore(string score)
		{
			InteropMethods._displayTournamentResultsWithStringScore(score);
			_matchInfo = null;
		}

		public void DisplayTournamentResultsWithScore(int score)
		{
			InteropMethods._displayTournamentResultsWithScore(score);
			_matchInfo = null;
		}

		public void DisplayTournamentResultsWithScore(float score)
		{
			InteropMethods._displayTournamentResultsWithFloatScore(score);
			_matchInfo = null;
		}

		public void ReportFinalScoreForBotMatch(string playerScore, string botScore)
		{
			InteropMethods._displayBotTournamentResultsWithStringScores(playerScore, botScore);
			_matchInfo = null;
		}

		public void ReportFinalScoreForBotMatch(int playerScore, int botScore)
		{
			InteropMethods._displayBotTournamentResultsWithScores(playerScore, botScore);
			_matchInfo = null;
		}

		public void ReportFinalScoreForBotMatch(float playerScore, float botScore)
		{
			InteropMethods._displayBotTournamentResultsWithFloatScores(playerScore, botScore);
			_matchInfo = null;
		}
				
		public void SubmitScore(string score, Action successCallback, Action<string> failureCallback)
		{
			// Set the callbacks in the SkillzSDKProxy to be called on success/failure
			SkillzSDKProxy.submitSuccessCallback = successCallback;
			SkillzSDKProxy.submitFailureCallback = failureCallback;
			InteropMethods._submitStringScore(score);
		}

		public void SubmitScore(int score, Action successCallback, Action<string> failureCallback)
		{
			// Set the callbacks in the SkillzSDKProxy to be called on success/failure
			SkillzSDKProxy.submitSuccessCallback = successCallback;
			SkillzSDKProxy.submitFailureCallback = failureCallback;
			InteropMethods._submitScore(score);
		}

		public void SubmitScore(float score, Action successCallback, Action<string> failureCallback)
		{
			// Set the callbacks in the SkillzSDKProxy to be called on success/failure
			SkillzSDKProxy.submitSuccessCallback = successCallback;
			SkillzSDKProxy.submitFailureCallback = failureCallback;
			InteropMethods._submitFloatScore(score);
		}

		public bool EndReplay()
		{
			return InteropMethods._endReplay();
		}

		public bool ReturnToSkillz()
		{
			return InteropMethods._returnToSkillz();
		}

		public string SDKVersionShort()
		{
			return Marshal.PtrToStringAnsi(InteropMethods._SDKShortVersion());
		}

		public Player GetPlayer()
		{
			var playerJson = Marshal.PtrToStringAnsi(InteropMethods._player());
			Dictionary<string, object> playerDict = DeserializeJSONToDictionary(playerJson);
			return new Player(playerDict);
		}

		public void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress)
		{
			InteropMethods._addMetadataForMatchInProgress(metadataJson, forMatchInProgress);
		}

		public void SetSkillzBackgroundMusic(string fileName)
		{
			Debug.Log("SkillzAudio Api.cs setSkillzBackgroundMusic with file name: " + fileName);
			InteropMethods._setSkillzBackgroundMusic(fileName);
		}

		public void GetProgressionUserData(string progressionNamespace, List<string> userDataKeys, 
			Action<Dictionary<string, ProgressionValue>> successCallback, Action<string> failureCallback)
		{
			string keysJSON = MiniJSON.Json.Serialize(userDataKeys);
			// set success/failure Actions that will be called in the progression proxy
			int requestId = SkillzProgressionProxy.RegisterGetCallbacks(successCallback, failureCallback);
			InteropMethods._getProgressionUserData(requestId, progressionNamespace, keysJSON);
		}

		public void UpdateProgressionUserData(string progressionNamespace, Dictionary<string, object> userDataUpdates,
			Action successCallback, Action<string> failureCallback)
		{
			string updatesJSON = MiniJSON.Json.Serialize(userDataUpdates);

			// set success/failure Actions that will be called in the progression proxy
			int requestId = SkillzProgressionProxy.RegisterUpdateCallbacks(successCallback, failureCallback);
			InteropMethods._updateProgressionUserData(requestId, progressionNamespace, updatesJSON);
		}

		int ISyncAPI.GetConnectedPlayerCount()
		{
			return InteropMethods._connectedPlayerCount();
		}

		ulong ISyncAPI.GetCurrentOpponentPlayerId()
		{
			return InteropMethods._currentOpponentPlayerId();
		}

		ulong ISyncAPI.GetCurrentPlayerId()
		{
			return InteropMethods._currentPlayerId();
		}

		double ISyncAPI.GetServerTime()
		{
			return InteropMethods._getServerTime();
		}

		long ISyncAPI.GetTimeLeftForReconnection(ulong playerId)
		{
			return InteropMethods._reconnectTimeLeftForPlayer(playerId);
		}

		void ISyncAPI.SendData(byte[] data)
		{
			using (UnmanagedArray ua = new UnmanagedArray(data))
			{
				InteropMethods._sendData(ua.IntPtr, ua.Length);
			}
		}

		float IRandom.Value()
		{
			if (((IAsyncAPI)this).IsMatchInProgress)
			{
				return InteropMethods._getRandomFloat();
			}

			return UnityEngine.Random.value;
		}

		void ISyncDelegateInitializer.Initialize(SkillzSyncDelegate syncDelegate)
		{
			SkillzSyncProxy.Initialize(syncDelegate);
		}

		private static Dictionary<string, object> DeserializeJSONToDictionary(string jsonString)
		{
			return MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
		}

		/// <summary>
		/// Proxy for marshalling callbacks for use with the general Skillz SDK methods.
		/// </summary>
		private static class SkillzSDKProxy
		{
			public static Action submitSuccessCallback;
			public static Action<string> submitFailureCallback;

			private delegate void SubmitScoreSuccessCallback();
			private delegate void SubmitScoreFailureCallback(string errorString);
			
			[MonoPInvokeCallback(typeof(SubmitScoreSuccessCallback))]
			public static void submitScoreSuccessCallback()
			{
				if (submitSuccessCallback != null)
				{
					submitSuccessCallback();
					submitSuccessCallback = null;
				}
			}

			[MonoPInvokeCallback(typeof(SubmitScoreSuccessCallback))]
			public static void submitScoreFailureCallback(string errorString)
			{
				if (submitFailureCallback != null)
				{
					submitFailureCallback(errorString);
					submitFailureCallback = null;
				}
			}

			public static void Initialize()
			{
				var onSubmitScoreSuccessFP = new SubmitScoreSuccessCallback(submitScoreSuccessCallback);
				IntPtr onSubmitScoreSuccessIP = Marshal.GetFunctionPointerForDelegate(onSubmitScoreSuccessFP);
				InteropMethods._assignOnSubmitScoreSuccessCallback(onSubmitScoreSuccessIP);

				var onSubmitScoreFailureFP = new SubmitScoreFailureCallback(submitScoreFailureCallback);
				IntPtr onSubmitScoreFailureIP = Marshal.GetFunctionPointerForDelegate(onSubmitScoreFailureFP);
				InteropMethods._assignOnSubmitScoreFailureCallback(onSubmitScoreFailureIP);
			}
		}

		/// <summary>
		/// Proxy for marshalling callbacks for use with the Skillz SDK progression data.
		/// </summary>
		private static class SkillzProgressionProxy
		{
			private static int nextRequestId = 0;
			public static Dictionary<int, Action<Dictionary<string, ProgressionValue>>> getSuccessCallbackDict;
			public static Dictionary<int, Action<string>> getFailureCallbackDict;
			public static Dictionary<int, Action> updateSuccessCallbackDict;
			public static Dictionary<int, Action<string>> updateFailureCallbackDict;

			private delegate void ProgressionGetSuccess(int requestId, string dataJSON);
			private delegate void ProgressionGetFailure(int requestId, string errorString);
			private delegate void ProgressionUpdateSuccess(int requestId);
			private delegate void ProgressionUpdateFailure(int requestId, string errorString);

			private static int GetNextRequestID()
			{
				int requestId = SkillzProgressionProxy.nextRequestId += 1;
				return requestId;
			}


			public static int RegisterGetCallbacks(Action<Dictionary<string, ProgressionValue>> successCallback, 
				Action<string> failureCallback)
			{
				int requestId = GetNextRequestID();
				if (getSuccessCallbackDict != null && getFailureCallbackDict != null) 
				{
					getSuccessCallbackDict.Add(requestId, successCallback);
					getFailureCallbackDict.Add(requestId, failureCallback);
				}
				return requestId;
			}

			public static void ClearGetCallbacks(int requestId)
			{
				if (getSuccessCallbackDict != null && getFailureCallbackDict != null) 
				{
					getSuccessCallbackDict.Remove(requestId);
					getFailureCallbackDict.Remove(requestId);
				}
			}

			public static int RegisterUpdateCallbacks(Action successCallback, 
				Action<string> failureCallback)
			{
				int requestId = GetNextRequestID();
				if (updateSuccessCallbackDict != null && updateFailureCallbackDict != null) 
				{
					updateSuccessCallbackDict.Add(requestId, successCallback);
					updateFailureCallbackDict.Add(requestId, failureCallback);
				}
				return requestId;
			}

			public static void ClearUpdateCallbacks(int requestId)
			{
				if (updateSuccessCallbackDict != null && updateFailureCallbackDict != null) 
				{
					updateSuccessCallbackDict.Remove(requestId);
					updateFailureCallbackDict.Remove(requestId);
				}
			}

			[MonoPInvokeCallback(typeof(ProgressionGetSuccess))]
			public static void progressionGetSuccessCallback(int requestId, string dataJSON)
			{
				if (getSuccessCallbackDict != null && getSuccessCallbackDict[requestId] != null)
				{
					Dictionary<string, object> jsonDict  = DeserializeJSONToDictionary(dataJSON);
					Dictionary<string, ProgressionValue> formattedData = ProgressionValue.GetProgressionValuesFromJSON(jsonDict);

					Action<Dictionary<string, ProgressionValue>> callback = getSuccessCallbackDict[requestId];
					ClearGetCallbacks(requestId);					
					callback(formattedData);
				}
			}

			[MonoPInvokeCallback(typeof(ProgressionGetFailure))]
			public static void progressionGetFailureCallback(int requestId, string errorString)
			{
				if (getFailureCallbackDict != null && getFailureCallbackDict[requestId] != null)
				{
					Action<string> callback = getFailureCallbackDict[requestId];
					ClearGetCallbacks(requestId);
					callback(errorString);
				}
			}

			[MonoPInvokeCallback(typeof(ProgressionUpdateSuccess))]
			public static void progressionUpdateSuccessCallback(int requestId)
			{
				if (updateSuccessCallbackDict != null && updateSuccessCallbackDict[requestId] != null)
				{
					Action callback = updateSuccessCallbackDict[requestId];
					ClearUpdateCallbacks(requestId);
					callback();
				}
			}

			[MonoPInvokeCallback(typeof(ProgressionUpdateFailure))]
			public static void progressionUpdateFailureCallback(int requestId, string errorString)
			{
				if (updateFailureCallbackDict != null && updateFailureCallbackDict[requestId] != null) 
				{
					Action<string> callback = updateFailureCallbackDict[requestId];
					ClearUpdateCallbacks(requestId);
					callback(errorString);
				}
			}

			public static void Initialize()
			{
				getSuccessCallbackDict = new Dictionary<int, Action<Dictionary<string, ProgressionValue>>>();
				getFailureCallbackDict = new Dictionary<int, Action<string>>();
				updateSuccessCallbackDict = new Dictionary<int, Action>();
				updateFailureCallbackDict = new Dictionary<int, Action<string>>();

				var onProgressionGetSuccessFP = new ProgressionGetSuccess(progressionGetSuccessCallback);
				IntPtr onProgressionGetSuccessIP = Marshal.GetFunctionPointerForDelegate(onProgressionGetSuccessFP);
				InteropMethods._assignOnProgressionGetSuccess(onProgressionGetSuccessIP);

				var onProgressionGetFailureFP = new ProgressionGetFailure(progressionGetFailureCallback);
				IntPtr onProgressionGetFailureIP = Marshal.GetFunctionPointerForDelegate(onProgressionGetFailureFP);
				InteropMethods._assignOnProgressionGetFailure(onProgressionGetFailureIP);

				var onProgressionUpdateSuccessFP = new ProgressionUpdateSuccess(progressionUpdateSuccessCallback);
				IntPtr onProgressionUpdateSuccessIP = Marshal.GetFunctionPointerForDelegate(onProgressionUpdateSuccessFP);
				InteropMethods._assignOnProgressionUpdateSuccess(onProgressionUpdateSuccessIP);

				var onProgressionUpdateFailureFP = new ProgressionUpdateFailure(progressionUpdateFailureCallback);
				IntPtr onProgressionUpdateFailureIP = Marshal.GetFunctionPointerForDelegate(onProgressionUpdateFailureFP);
				InteropMethods._assignOnProgressionUpdateFailure(onProgressionUpdateFailureIP);
			}
		}

		/// <summary>
		/// Proxy for marshaling callbacks from the sync delegate instance
		/// in the Skillz iOS SDK.
		/// </summary>
		private static class SkillzSyncProxy
		{
			private static SkillzSyncDelegate _syncDelegate;

			public static void Initialize(SkillzSyncDelegate syncDelegate)
			{
				_syncDelegate = syncDelegate;
				AssignSyncDelegateFunctions();
			}

			[MonoPInvokeCallback(typeof(IntFP))]
			public static void onOpponentHasLostConnection(ulong playerId)
			{
				_syncDelegate.OnOpponentHasLostConnection(playerId);
			}

			[MonoPInvokeCallback(typeof(IntFP))]
			public static void onOpponentHasReconnected(ulong playerId)
			{
				_syncDelegate.OnOpponentHasReconnected(playerId);
			}

			[MonoPInvokeCallback(typeof(IntFP))]
			public static void onOpponentHasLeftMatch(ulong playerId)
			{
				_syncDelegate.OnOpponentHasLeftMatch(playerId);
			}

			[MonoPInvokeCallback(typeof(VoidFP))]
			public static void onCurrentPlayerHasLostConnection()
			{
				_syncDelegate.OnCurrentPlayerHasLostConnection();
			}

			[MonoPInvokeCallback(typeof(VoidFP))]
			public static void onCurrentPlayerHasReconnected()
			{
				_syncDelegate.OnCurrentPlayerHasReconnected();
			}

			[MonoPInvokeCallback(typeof(VoidFP))]
			public static void onCurrentPlayerHasLeftMatch()
			{
				_syncDelegate.OnCurrentPlayerHasLeftMatch();
			}

			[MonoPInvokeCallback(typeof(IntPtrIntFP))]
			public static void onDidReceiveData(IntPtr value, ulong length)
			{
				var managedArray = new byte[length];
				Marshal.Copy(value, managedArray, 0, (int)length);

				_syncDelegate.OnDidReceiveData(managedArray);
			}

			[MonoPInvokeCallback(typeof(VoidFP))]
			public static void onMatchCompleted()
			{
				_syncDelegate.OnMatchCompleted();
			}

			private static void AssignSyncDelegateFunctions()
			{
				Debug.Log("Assign Sync Delegate Functions");

				var onMatchCompletedFP = new VoidFP(onMatchCompleted);
				IntPtr onMatchCompletedIP = Marshal.GetFunctionPointerForDelegate(onMatchCompletedFP);
				InteropMethods._assignOnMatchCompletedFunc(onMatchCompletedIP);

				var onDidReceiveDataFP = new IntPtrIntFP(onDidReceiveData);
				IntPtr onDidReceiveDataIP = Marshal.GetFunctionPointerForDelegate(onDidReceiveDataFP);
				InteropMethods._assignOnDidReceiveDataFunc(onDidReceiveDataIP);

				var onOpponentHasLostConnectionFP = new IntFP(onOpponentHasLostConnection);
				IntPtr onOpponentHasLostConnectionIP = Marshal.GetFunctionPointerForDelegate(onOpponentHasLostConnectionFP);
				InteropMethods._assignOnOpponentHasLostConnectionFunc(onOpponentHasLostConnectionIP);

				var OnOpponentHasReconnectedFP = new IntFP(onOpponentHasReconnected);
				IntPtr onOpponentHasReconnectedIP = Marshal.GetFunctionPointerForDelegate(OnOpponentHasReconnectedFP);
				InteropMethods._assignOnOpponentHasReconnectedFunc(onOpponentHasReconnectedIP);

				var onOpponentHasLeftMatchFP = new IntFP(onOpponentHasLeftMatch);
				IntPtr onOpponentHasLeftMatchIP = Marshal.GetFunctionPointerForDelegate(onOpponentHasLeftMatchFP);
				InteropMethods._assignOnOpponentHasLeftMatchFunc(onOpponentHasLeftMatchIP);

				var onCurrentPlayerHasLostConnectionFP = new VoidFP(onCurrentPlayerHasLostConnection);
				IntPtr onCurrentPlayerHasLostConnectionIP = Marshal.GetFunctionPointerForDelegate(onCurrentPlayerHasLostConnectionFP);
				InteropMethods._assignOnCurrentPlayerHasLostConnectionFunc(onCurrentPlayerHasLostConnectionIP);

				var onCurrentPlayerHasReconnectedFP = new VoidFP(onCurrentPlayerHasReconnected);
				IntPtr onCurrentPlayerHasReconnectedIP = Marshal.GetFunctionPointerForDelegate(onCurrentPlayerHasReconnectedFP);
				InteropMethods._assignOnCurrentPlayerHasReconnectedFunc(onCurrentPlayerHasReconnectedIP);

				var onCurrentPlayerHasLeftMatchFP = new VoidFP(onCurrentPlayerHasLeftMatch);
				IntPtr onCurrentPlayerHasLeftMatchIP = Marshal.GetFunctionPointerForDelegate(onCurrentPlayerHasLeftMatchFP);
				InteropMethods._assignOnCurrentPlayerHasLeftMatchFunc(onCurrentPlayerHasLeftMatchIP);
			}
		}
	}
}
#endif
