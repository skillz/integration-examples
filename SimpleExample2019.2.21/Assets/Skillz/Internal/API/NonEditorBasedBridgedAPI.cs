using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SkillzSDK.Internal.API
{
	/// <summary>
	/// Thin <see cref="IBridgedAPI"/> wrapper. This checks if
	/// the wrapped bridged API is being initialized when the
	/// game is running from the Unity editor. If so, a dialog
	/// is displayed to tell the developer to switch the platform to
	/// PC, Mac, and Linux in order to test their game's workflow with
	/// Skillz from the Unity editor.
	/// </summary>
	internal sealed class NonEditorBasedBridgedAPI : IBridgedAPI
	{
		public IRandom Random
		{
			get
			{
				return ((IAsyncAPI)actualAPI).Random;
			}
		}

		public bool IsMatchInProgress
		{
			get
			{
				return actualAPI.IsMatchInProgress;
			}
		}

		public float SkillzMusicVolume
		{
			get
			{
				return actualAPI.SkillzMusicVolume;
			}
			set
			{
				actualAPI.SkillzMusicVolume = value;
			}
		}

		public float SoundEffectsVolume
		{
			get
			{
				return actualAPI.SoundEffectsVolume;
			}
			set
			{
				actualAPI.SoundEffectsVolume = value;
			}
		}

		public bool IsMatchCompleted
		{
			get
			{
				return actualAPI.IsMatchCompleted;
			}
		}

		private readonly IBridgedAPI actualAPI;

		public NonEditorBasedBridgedAPI(IBridgedAPI actualAPI)
		{
			this.actualAPI = actualAPI;
		}

		public void AbortMatch()
		{
			actualAPI.AbortMatch();
		}

		public void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress)
		{
			actualAPI.AddMetadataForMatchInProgress(metadataJson, forMatchInProgress);
		}

		public int GetConnectedPlayerCount()
		{
			return actualAPI.GetConnectedPlayerCount();
		}

		public ulong GetCurrentOpponentPlayerId()
		{

			return actualAPI.GetCurrentPlayerId();
		}

		public ulong GetCurrentPlayerId()
		{
			return actualAPI.GetCurrentPlayerId();
		}

		public Match GetMatchInfo()
		{
			return actualAPI.GetMatchInfo();
		}

		public Hashtable GetMatchRules()
		{
			return actualAPI.GetMatchRules();
		}

		public Player GetPlayer()
		{
			return actualAPI.GetPlayer();
		}

		public double GetServerTime()
		{
			return actualAPI.GetServerTime();
		}

		public long GetTimeLeftForReconnection(ulong playerId)
		{
			return actualAPI.GetTimeLeftForReconnection(playerId);
		}

		public void Initialize(int gameID, Environment environment, Orientation orientation)
		{
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				EditorUtility.DisplayDialog(
					"Skillz Workflow Cannot Be Tested",
					"Skillz cannot be tested from the Unity editor with the currently selected platform.\r\n\r\nTo test Skillz in the Unity editor, please change your platform to \"PC, Mac, and Linux Standalone\".",
					"OK"
				);
			}
#endif

			actualAPI.Initialize(gameID, environment, orientation);
		}

		public void LaunchSkillz()
		{
			actualAPI.LaunchSkillz();
		}

		public void ReportFinalScore(string score)
		{
			actualAPI.ReportFinalScore(score);
		}

		public void ReportFinalScore(int score)
		{
			actualAPI.ReportFinalScore(score);
		}

		public void ReportFinalScore(float score)
		{
			actualAPI.ReportFinalScore(score);
		}

		public string SDKVersionShort()
		{
			return actualAPI.SDKVersionShort();
		}

		public void SendData(byte[] data)
		{
			actualAPI.SendData(data);
		}

		public void SetSkillzBackgroundMusic(string fileName)
		{
			actualAPI.SetSkillzBackgroundMusic(fileName);
		}

		public void UpdatePlayersCurrentScore(string score)
		{
			actualAPI.UpdatePlayersCurrentScore(score);
		}

		public void UpdatePlayersCurrentScore(int score)
		{
			actualAPI.UpdatePlayersCurrentScore(score);
		}

		public void UpdatePlayersCurrentScore(float score)
		{
			actualAPI.UpdatePlayersCurrentScore(score);
		}
	}
}