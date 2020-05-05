using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using SkillzSDK.Settings;
using SkillzSDK.Extensions;

using JSONDict = System.Collections.Generic.Dictionary<string, object>;
using System.Linq;

namespace SkillzSDK
{
    /// <summary>
    /// A Skillz user.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The user's display name.
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// An ID unique to this user.
        /// </summary>
        public readonly UInt64? ID;

        /// <summary>
        /// A Tournament Player ID unique to this user.
        /// </summary>
        public readonly UInt64? TournamentPlayerID;

        /// <summary>
        /// A link to the user's avatar image.
        /// </summary>
        public readonly string AvatarURL;

        /// <summary>
        /// A link to the user's country's flag image.
        /// </summary>
        public readonly string FlagURL;

        /// <summary>
        /// This Player represents the current user if this is true.
        /// </summary>
        public readonly bool IsCurrentPlayer;


    public Player(JSONDict playerJSON) {
        #if UNITY_IOS
            ID = playerJSON.SafeGetUintValue("id");
            DisplayName = playerJSON.SafeGetStringValue("displayName");
            AvatarURL = playerJSON.SafeGetStringValue("avatarURL");
            FlagURL = playerJSON.SafeGetStringValue("flagURL");
            IsCurrentPlayer = (bool)playerJSON.SafeGetBoolValue("isCurrentPlayer");
            TournamentPlayerID = playerJSON.SafeGetUintValue("playerMatchId");
        #elif UNITY_ANDROID
            ID = playerJSON.SafeGetUintValue("userId");
            DisplayName = playerJSON.SafeGetStringValue("userName");
            AvatarURL = playerJSON.SafeGetStringValue("avatarUrl");
            FlagURL = playerJSON.SafeGetStringValue("flagUrl");
            IsCurrentPlayer = (bool)playerJSON.SafeGetBoolValue("isCurrentPlayer");
            TournamentPlayerID = playerJSON.SafeGetUintValue("playerMatchId");
        #endif
    }


        public override string ToString ()
        {
            return "Player: " +
            " ID: [" + ID + "]" +
            " DisplayName: [" + DisplayName + "]" +
            " AvatarURL: [" + AvatarURL + "]" +
            " FlagURL: [" + FlagURL + "]";
        }
    }


    /// <summary>
    /// A Skillz match.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:SkillzSDK.Match"/> represents a custom synchronous match.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is a custom synchronous match; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomSynchronousMatch
        {
            get
            {
                return CustomServerConnectionInfo != null;
            }
        }

        /// <summary>
        /// The name of this tournament type.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The description of this tournament type.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The unique ID for this match.
        /// </summary>
        public readonly ulong? ID;

        /// <summary>
        /// The unique ID for the tournament template this match is based on.
        /// </summary>
        public readonly int? TemplateID;

        /// <summary>
        /// If this game supports "Automatic Difficulty" (specified in the Developer Portal --
        /// https://www.developers.skillz.com/developer), this value represents the difficulty this game
        /// should have, from 1 to 10 (inclusive).
        /// Note that this value will only exist in Production, not Sandbox.
        /// </summary>
        public readonly uint? SkillzDifficulty;

        /// <summary>
        /// Is this match being played for real cash or for Z?
        /// </summary>
        public readonly bool? IsCash;

        /// <summary>
        /// If this tournament is being played for Z,
        /// this is the amount of Z required to enter.
        /// </summary>
        public readonly int? EntryPoints;
        /// <summary>
        /// If this tournament is being played for real cash,
        /// this is the amount of cash required to enter.
        /// </summary>
        public readonly float? EntryCash;

        /// <summary>
        /// If this tournament is Synchronous or Asynchronous?
        /// </summary>
        public readonly bool IsSynchronous;

        /// <summary>
        /// The user playing this match.
        /// </summary>
        public readonly List<Player> Players;

        /// <summary>
        /// The connection info to a custom server that coordinates a real-time match.
        /// This will be <c>null</c> if the match is either asynchronous,
        /// or synchronous and coordinated by Skillz.
        /// </summary>
        public readonly CustomServerConnectionInfo CustomServerConnectionInfo;

        /// <summary>
        /// The custom parameters for this tournament type.
        /// Specified by the developer on the Skillz Developer Portal.
        /// </summary>
        public readonly Dictionary<string, string> GameParams;

        public Match (JSONDict jsonData)
        {
            Description = jsonData.SafeGetStringValue ("matchDescription");
            EntryCash = (float)jsonData.SafeGetDoubleValue ("entryCash");
            EntryPoints = jsonData.SafeGetIntValue ("entryPoints");
            ID = jsonData.SafeGetUlongValue ("id");
            TemplateID = jsonData.SafeGetIntValue ("templateId");
            Name = jsonData.SafeGetStringValue ("name");
            IsCash = jsonData.SafeGetBoolValue ("isCash");
            IsSynchronous = (bool)jsonData.SafeGetBoolValue ("isSynchronous");

            object players = jsonData.SafeGetValue ("players");
            Players = new List<Player>();

            List<object> playerArray = (List<object>)players;
            foreach (object player in playerArray) {
                Players.Add(new Player((Dictionary<string, object>)player));
            }

            var connectionInfoJson = jsonData.SafeGetValue("connectionInfo") as JSONDict;
            CustomServerConnectionInfo = connectionInfoJson != null
                ? new CustomServerConnectionInfo(connectionInfoJson)
                : null;

			if (Application.isEditor)
			{
				GameParams = LoadSimulatedMatchParameters();
			}

#if UNITY_IOS
            GameParams = new Dictionary<string, string>();
            object parameters = jsonData.SafeGetValue("gameParameters");
            if (parameters != null && parameters.GetType() == typeof(JSONDict)) {
                foreach (KeyValuePair<string, object> kvp in (JSONDict)parameters) {
                    if (kvp.Value == null) {
                        continue;
                    }

                    string val = kvp.Value.ToString();
                    if (kvp.Key == "skillz_difficulty") {
                        SkillzDifficulty = Helpers.SafeUintParse(val);
                    } else {
                        GameParams.Add(kvp.Key, val);
                    }
                }
            }
#elif UNITY_ANDROID
            GameParams = HashtableToDictionary(SkillzCrossPlatform.GetMatchRules());
            SkillzDifficulty = jsonData.SafeGetUintValue("skillzDifficulty");
#endif
        }

        public override string ToString ()
        {
            string paramStr = "";

            foreach (KeyValuePair<string, string> entry in GameParams) {
                paramStr += " " + entry.Key + ": " + entry.Value;
            }

            var stringValue = "Match: " +
            " ID: [" + ID + "]" +
            " Name: [" + Name + "]" +
            " Description: [" + Description + "]" +
            " TemplateID: [" + TemplateID + "]" +
            " SkillzDifficulty: [" + SkillzDifficulty + "]" +
            " IsCash: [" + IsCash + "]" +
            " IsSynchronous: [" + IsSynchronous + "]" +
            " IsCustomSynchronousMatch: [" + IsCustomSynchronousMatch + "]" +
            " EntryPoints: [" + EntryPoints + "]" +
            " EntryCash: [" + EntryCash + "]" +
            " GameParams: [" + paramStr + "]" +
                " Player: [" + Players + "]";

            if (CustomServerConnectionInfo != null)
            {
                stringValue = string.Concat(stringValue, string.Format(" ConnectionInfo: [{0}]", CustomServerConnectionInfo));
            }

            return stringValue;
        }

		private Dictionary<string, string> LoadSimulatedMatchParameters()
		{
			Debug.Log("Loading simulated match parameters:");

			var simulatedMatchParameters = new Dictionary<string, string>();

			foreach (var stringKeyValue in SkillzSettings.Instance.MatchParameters)
			{
				var trimmedKey = !string.IsNullOrWhiteSpace(stringKeyValue.Key) ? stringKeyValue.Key.Trim() : string.Empty;
				var trimmedValue = !string.IsNullOrWhiteSpace(stringKeyValue.Value) ? stringKeyValue.Value.Trim() : stringKeyValue.Value;

				if (string.IsNullOrWhiteSpace(trimmedKey))
				{
					continue;
				}

				Debug.Log($"\t'{trimmedKey}' : '{trimmedValue}'");
				simulatedMatchParameters.Add(trimmedKey, trimmedValue);
			}

			return simulatedMatchParameters;
		}

		private static Dictionary<string, string> HashtableToDictionary (Hashtable gameParamsHashTable)
        {
            Dictionary<string,string> gameParamsdict = new Dictionary<string,string> ();
            foreach (DictionaryEntry entry in gameParamsHashTable) {
                gameParamsdict.Add ((string)entry.Key, (string)entry.Value);
            }

            return gameParamsdict;
        }
    }
}
