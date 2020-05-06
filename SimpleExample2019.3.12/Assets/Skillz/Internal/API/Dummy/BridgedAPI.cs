using System;
using System.Collections;
using System.Collections.Generic;

namespace SkillzSDK.Internal.API.Dummy
{
	internal sealed class BridgedAPI : IBridgedAPI, ISyncDelegateInitializer
	{
		private const float DefaultVolume = 0.5f;

		public IRandom Random
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IAsyncAPI.IsMatchInProgress
		{
			get
			{
				return false;
			}
		}

		public float SkillzMusicVolume
		{
			get
			{
				return skillzMusicVolume;
			}
			set
			{
				skillzMusicVolume = SanitizeVolume(value);
			}
		}

		public float SoundEffectsVolume
		{
			get
			{
				return soundEffectsVolume;
			}
			set
			{
				soundEffectsVolume = SanitizeVolume(value);
			}
		}

		bool ISyncAPI.IsMatchCompleted
		{
			get
			{
				return false;
			}
		}

		private float skillzMusicVolume;
		private float soundEffectsVolume;

		private readonly Random random;

		public BridgedAPI()
		{
			random = new Random();

			skillzMusicVolume = DefaultVolume;
			soundEffectsVolume = DefaultVolume;
		}

		public void Initialize(int gameID, Environment environment, Orientation orientation)
		{
		}

		public void LaunchSkillz()
		{
		}

		public Hashtable GetMatchRules()
		{
			return new Hashtable();
		}

		public Match GetMatchInfo()
		{
			return new Match(new Dictionary<string, object>());
		}

		public void AbortMatch()
		{
		}

		public void UpdatePlayersCurrentScore(string score)
		{
		}

		public void UpdatePlayersCurrentScore(int score)
		{
		}

		public void UpdatePlayersCurrentScore(float score)
		{
		}

		public void ReportFinalScore(string score)
		{
		}

		public void ReportFinalScore(int score)
		{
		}

		public void ReportFinalScore(float score)
		{
		}

		public string SDKVersionShort()
		{
			return string.Empty;
		}

		public Player GetPlayer()
		{
			return new Player(new Dictionary<string, object>());
		}

		public void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress)
		{
		}

		public void SetSkillzBackgroundMusic(string fileName)
		{
		}

		int ISyncAPI.GetConnectedPlayerCount()
		{
			return 1;
		}

		ulong ISyncAPI.GetCurrentOpponentPlayerId()
		{
			return uint.MaxValue;
		}

		ulong ISyncAPI.GetCurrentPlayerId()
		{
			return uint.MinValue;
		}

		double ISyncAPI.GetServerTime()
		{
			return double.MaxValue;
		}

		long ISyncAPI.GetTimeLeftForReconnection(ulong playerId)
		{
			return 1000;
		}

		void ISyncAPI.SendData(byte[] data)
		{
		}

		void ISyncDelegateInitializer.Initialize(SkillzSyncDelegate syncDelegate)
		{
		}

		private static float SanitizeVolume(float volume)
		{
			if (volume > 1f)
			{
				return 1f;
			}

			if (volume < 0f)
			{
				return 0f;
			}

			return volume;
		}
	}
}
