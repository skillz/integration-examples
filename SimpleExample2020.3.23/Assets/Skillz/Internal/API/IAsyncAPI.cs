using System;
using System.Collections;
using System.Collections.Generic;

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

        void DisplayTournamentResultsWithScore(string score);

        void DisplayTournamentResultsWithScore(int score);

        void DisplayTournamentResultsWithScore(float score);

        void AbortBotMatch(string botScore);

        void AbortBotMatch(int botScore);

        void AbortBotMatch(float botScore);

        void ReportFinalScoreForBotMatch(string playerScore, string botScore);

        void ReportFinalScoreForBotMatch(int playerScore, int botScore);

        void ReportFinalScoreForBotMatch(float playerScore, float botScore);

        void SubmitScore(string score, Action successCallback, Action<string> failureCallback);
        void SubmitScore(int score, Action successCallback, Action<string> failureCallback);

        void SubmitScore(float score, Action successCallback, Action<string> failureCallback);

        bool EndReplay();

        bool ReturnToSkillz();

        string SDKVersionShort();

        Player GetPlayer();

        void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress);

        void SetSkillzBackgroundMusic(string fileName);

        void GetProgressionUserData(string progressionNamespace, List<string> userDataKeys, Action<Dictionary<string, ProgressionValue>> successCallback, Action<string> failureCallback);

        void UpdateProgressionUserData(string progressionNamespace, Dictionary<string, object> userDataUpdates, Action successCallback, Action<string> failureCallback);
    }
}
