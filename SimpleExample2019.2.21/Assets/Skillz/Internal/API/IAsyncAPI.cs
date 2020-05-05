using System.Collections;

namespace SkillzSDK.Internal.API
{
	/// <summary>
	/// Represents platform-agnostic Skillz APIs for asynchronous matches.
	/// </summary>
	internal interface IAsyncAPI
	{
		IRandom Random
		{
			get;
		}

		bool IsMatchInProgress
		{
			get;
		}

		float SkillzMusicVolume
		{
			get;
			set;
		}

		float SoundEffectsVolume
		{
			get;
			set;
		}

		void LaunchSkillz();

		Hashtable GetMatchRules();

		Match GetMatchInfo();

		void AbortMatch();

		void UpdatePlayersCurrentScore(string score);

		void UpdatePlayersCurrentScore(int score);

		void UpdatePlayersCurrentScore(float score);

		void ReportFinalScore(string score);

		void ReportFinalScore(int score);

		void ReportFinalScore(float score);

		string SDKVersionShort();

		Player GetPlayer();

		void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress);

		void SetSkillzBackgroundMusic(string fileName);
	}
}
