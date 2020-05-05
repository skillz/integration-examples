using System;
using System.Runtime.InteropServices;

#if UNITY_IOS
namespace SkillzSDK.Internal.API.iOS
{
	/// <summary>
	/// Interoped native Skillz iOS SDK methods.
	/// </summary>
	internal static class InteropMethods
	{
		[DllImport("__Internal")]
		public static extern void _addMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress);

		[DllImport("__Internal")]
		public static extern int _getRandomNumber();

		[DllImport("__Internal")]
		public static extern int _getRandomNumberWithMinAndMax(int min, int max);

		[DllImport("__Internal")]
		public static extern void _skillzInitForGameIdAndEnvironment(string gameId, string environment);

		[DllImport("__Internal")]
		public static extern int _tournamentIsInProgress();

		// Need to use an IntPtr instead of a string here, for more information see
		// http://www.mono-project.com/docs/advanced/pinvoke/#strings-as-return-values
		[DllImport("__Internal")]
		public static extern IntPtr _player();

		[DllImport("__Internal")]
		public static extern IntPtr _SDKShortVersion();

		[DllImport("__Internal")]
		public static extern void _showSDKVersionInfo();

		[DllImport("__Internal")]
		public static extern IntPtr _getMatchRules();

		[DllImport("__Internal")]
		public static extern IntPtr _getMatchInfo();

		[DllImport("__Internal")]
		public static extern void _launchSkillz();

		[DllImport("__Internal")]
		public static extern void _displayTournamentResultsWithScore(int score);

		[DllImport("__Internal")]
		public static extern void _displayTournamentResultsWithFloatScore(float score);

		[DllImport("__Internal")]
		public static extern void _displayTournamentResultsWithStringScore(string score);

		[DllImport("__Internal")]
		public static extern void _notifyPlayerAbortWithCompletion();

		[DllImport("__Internal")]
		public static extern void _updatePlayersCurrentScore(float score);

		[DllImport("__Internal")]
		public static extern void _updatePlayersCurrentStringScore(string score);

		[DllImport("__Internal")]
		public static extern void _updatePlayersCurrentIntScore(int score);

		[DllImport("__Internal")]
		public static extern float _getRandomFloat();

		[DllImport("__Internal")]
		public static extern void _setSkillzBackgroundMusic(string filePath);

		[DllImport("__Internal")]
		public static extern void _setSFXVolume(float volume);

		[DllImport("__Internal")]
		public static extern void _setSkillzMusicVolume(float volume);

		[DllImport("__Internal")]
		public static extern float _getSFXVolume();

		[DllImport("__Internal")]
		public static extern float _getSkillzMusicVolume();

		[DllImport("__Internal")]
		public static extern void _sendData(IntPtr value, ulong length); //This "UInt64" may need to be an "int"

		[DllImport("__Internal")]
		public static extern bool _isMatchCompleted();

		[DllImport("__Internal")]
		public static extern int _connectedPlayerCount();

		[DllImport("__Internal")]
		public static extern ulong _currentPlayerId();

		[DllImport("__Internal")]
		public static extern ulong _currentOpponentPlayerId();

		[DllImport("__Internal")]
		public static extern ulong _getServerTime();

		[DllImport("__Internal")]
		public static extern int _reconnectTimeLeftForPlayer(ulong playerId); //This "UInt64" may need to be an "int"

		[DllImport("__Internal")]
		public static extern void _assignOnCurrentPlayerHasReconnectedFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnCurrentPlayerHasLostConnectionFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnCurrentPlayerHasLeftMatchFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnOpponentHasReconnectedFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnOpponentHasLostConnectionFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnOpponentHasLeftMatchFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnDidReceiveDataFunc(IntPtr funcPtr);

		[DllImport("__Internal")]
		public static extern void _assignOnMatchCompletedFunc(IntPtr funcPtr);
	}
}
#endif