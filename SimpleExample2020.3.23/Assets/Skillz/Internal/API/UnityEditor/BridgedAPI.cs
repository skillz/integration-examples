using System;
using System.Collections;
using System.Globalization;
using SkillzSDK.Settings;
using SkillzSDK.MiniJSON;
using UnityEngine;
using System.Collections.Generic;

namespace SkillzSDK.Internal.API.UnityEditor
{
    internal sealed class BridgedAPI : IBridgedAPI, IRandom
    {
        private const string SyncMatchesNotSupported = "Simulated sync matches are currently not supported.";
        private const string LogFormat = "[Skillz] {0}";
        private const string DefaultScoreString = "0";

        private const float DefaultVolume = 0.5f;

        private bool matchInProgress;

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
                return matchInProgress;
            }
        }

        public float SkillzMusicVolume
        {
            get
            {
                return backgroundMusicVolume;
            }
            set
            {
                backgroundMusicVolume = value > 1f ? 1f : value;
                backgroundMusicVolume = backgroundMusicVolume < 0f ? 0f : value;
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
                soundEffectsVolume = value > 1f ? 1f : value;
                soundEffectsVolume = soundEffectsVolume < 0f ? 0f : soundEffectsVolume;
            }
        }

        IRandom ISyncAPI.Random
        {
            get
            {
                throw new NotSupportedException(SyncMatchesNotSupported);
            }
        }

        bool ISyncAPI.IsMatchCompleted
        {
            get
            {
                throw new NotSupportedException(SyncMatchesNotSupported);
            }
        }

        private Match matchInfo;
        private float soundEffectsVolume;
        private float backgroundMusicVolume;
#pragma warning disable IDE0052 // Remove unread private members
        private string currentScore;
#pragma warning restore IDE0052 // Remove unread private members

        public BridgedAPI()
        {
            if (!Application.isEditor)
            {
                throw new InvalidOperationException("This can only be instantiated while the Unity editor is playing");
            }

            soundEffectsVolume = DefaultVolume;
            backgroundMusicVolume = DefaultVolume;
        }

        public void Initialize(int gameID, Environment environment, Orientation orientation)
        {
            currentScore = DefaultScoreString;

            matchInfo = new Match((Dictionary<string, object>)Json.Deserialize(MatchInfoJson.Build(gameID)));
        }

        public void LaunchSkillz()
        {
            SDKScenesLoader.Load(SDKScenesLoader.TournamentSelectionScene);
        }

        public Hashtable GetMatchRules()
        {
            if (!matchInProgress)
            {
                return new Hashtable();
            }

            return new Hashtable(matchInfo.GameParams);
        }

        public Match GetMatchInfo()
        {
            if (!matchInProgress)
            {
                // This behavior is not consistent across platforms. iOS will return null,
                // while Android will throw an NRE because it assumes there is a match to
                // get match info from.
                throw new InvalidOperationException("No simulated match in progress");
            }

            return matchInfo;
        }

        public void AbortMatch()
        {
            FinishSimulatedMatch();
            SDKScenesLoader.Load(SDKScenesLoader.MatchAbortedScene);
        }

        public void AbortBotMatch(string botScore)
        {
            FinishSimulatedMatch();
            SDKScenesLoader.Load(SDKScenesLoader.MatchAbortedScene);
        }

        public void AbortBotMatch(int botScore)
        {
            FinishSimulatedMatch();
            SDKScenesLoader.Load(SDKScenesLoader.MatchAbortedScene);
        }

        public void AbortBotMatch(float botScore)
        {
            FinishSimulatedMatch();
            SDKScenesLoader.Load(SDKScenesLoader.MatchAbortedScene);
        }

        public void UpdatePlayersCurrentScore(string score)
        {
            currentScore = score;
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
            FinishSimulatedMatch();

            if (SkillzSettings.Instance.SimulateMatchWins)
            {
                SDKScenesLoader.Load(SDKScenesLoader.MatchWonScene);
            }
            else
            {
                SDKScenesLoader.Load(SDKScenesLoader.MatchLostScene);
            }
        }

        public void DisplayTournamentResultsWithScore(int score)
        {
            DisplayTournamentResultsWithScore(score.ToString());
        }

        public void DisplayTournamentResultsWithScore(float score)
        {
            DisplayTournamentResultsWithScore(score.ToString(CultureInfo.InvariantCulture));
        }

        public void ReportFinalScoreForBotMatch(string playerScore, string botScore)
        {
            ReportFinalScoreForBotMatch(float.Parse(playerScore), float.Parse(botScore));
        }

        public void ReportFinalScoreForBotMatch(int playerScore, int botScore)
        {
            ReportFinalScoreForBotMatch((float)playerScore, (float)botScore);
        }

        public void ReportFinalScoreForBotMatch(float playerScore, float botScore)
        {
            FinishSimulatedMatch();

            if (playerScore > botScore)
            {
                SDKScenesLoader.Load(SDKScenesLoader.MatchWonScene);
            }
            else
            {
                SDKScenesLoader.Load(SDKScenesLoader.MatchLostScene);
            }
        }

        public void SubmitScore(string score, Action successCallback, Action<string> failureCallback)
        {
            FinishSimulatedMatch();
            if (successCallback != null)
            {
                successCallback();
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
            return true;
        }

        public bool ReturnToSkillz()
        {
            bool hasSubmittedScore = !matchInProgress;
            if (hasSubmittedScore)
            {
                if (SkillzSettings.Instance.SimulateMatchWins)
                {
                    SDKScenesLoader.Load(SDKScenesLoader.MatchWonScene);
                }
                else
                {
                    SDKScenesLoader.Load(SDKScenesLoader.MatchLostScene);
                }
            }
            return hasSubmittedScore;
        }

        public string SDKVersionShort()
        {
            Debug.LogWarning("SDKVersionShort() was called for the In-Unity API.");
            return string.Empty;
        }

        public Player GetPlayer()
        {
            Debug.Log(string.Format(LogFormat, "Returning null for GetPlayer()"));
            return null;
        }

        public void AddMetadataForMatchInProgress(string metadataJson, bool forMatchInProgress)
        {
            // Do nothing. It appears this is an old API that doesn't have a corresponding getter.
        }

        public void SetSkillzBackgroundMusic(string fileName)
        {
        }

        public void GetProgressionUserData(string progressionNamespace, List<string> userDataKeys, Action<Dictionary<string, ProgressionValue>> successCallback, Action<string> failureCallback)
        {
            Debug.Log("Called GetProgressionUserData from inside the Unity Editor");
            if (successCallback != null)
            {
                successCallback(new Dictionary<string, ProgressionValue>());
            }
        }

        public void UpdateProgressionUserData(string progressionNamespace, Dictionary<string, object> userDataUpdates, Action successCallback, Action<string> failureCallback)
        {
            Debug.Log("Called UpdateProgressionUserData from inside the Unity Editor");
            if (successCallback != null)
            {
                successCallback();
            }
        }

        internal void InitializeSimulatedMatch(string matchInfoJson)
        {
            matchInProgress = true;
            matchInfo = new Match((Dictionary<string, object>)Json.Deserialize(matchInfoJson));
        }

        private void FinishSimulatedMatch()
        {
            matchInProgress = false;
            matchInfo = null;
        }

        int ISyncAPI.GetConnectedPlayerCount()
        {
            throw new NotSupportedException(SyncMatchesNotSupported);
        }

        ulong ISyncAPI.GetCurrentOpponentPlayerId()
        {
            throw new NotSupportedException(SyncMatchesNotSupported);
        }

        ulong ISyncAPI.GetCurrentPlayerId()
        {
            throw new NotSupportedException(SyncMatchesNotSupported);
        }

        double ISyncAPI.GetServerTime()
        {
            throw new NotSupportedException(SyncMatchesNotSupported);
        }

        long ISyncAPI.GetTimeLeftForReconnection(ulong playerId)
        {
            throw new NotSupportedException(SyncMatchesNotSupported);
        }

        void ISyncAPI.SendData(byte[] data)
        {
            throw new NotSupportedException(SyncMatchesNotSupported);
        }

        float IRandom.Value()
        {
            return UnityEngine.Random.value;
        }
    }
}