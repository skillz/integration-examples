using System.Text;
using SkillzSDK.Extensions;
using JSONDict = System.Collections.Generic.Dictionary<string, object>;

namespace SkillzSDK
{
    /// <summary>
    /// Represents the connection information to a custom server that
    /// coordinates a real-time, synchronous match.
    /// </summary>
    public sealed class CustomServerConnectionInfo
    {
        /// <summary>
        /// The ID of the real-time match.
        /// </summary>
        public readonly string MatchId;

        /// <summary>
        /// The address of the custom server.
        /// </summary>
        public readonly string ServerIp;

        /// <summary>
        /// The port of the custom server.
        /// </summary>
        public readonly string ServerPort;

        /// <summary>
        /// The token for entering the match. This is encrypted.
        /// </summary>
        public readonly string MatchToken;

        /// <summary>
        /// Whether or not this game should be played against a synchronous gameplay bot.
        /// This should be used as part of the synchronous gameplay on-boarding experience.
        /// </summary>
        public readonly bool IsBotMatch;

        internal CustomServerConnectionInfo(JSONDict connectionInfoJson)
        {
            MatchId    = connectionInfoJson.SafeGetStringValue("matchId");
            ServerIp   = connectionInfoJson.SafeGetStringValue("serverIP");
            ServerPort = connectionInfoJson.SafeGetStringValue("serverPort");
            MatchToken = connectionInfoJson.SafeGetStringValue("matchToken");
            IsBotMatch = connectionInfoJson.SafeGetBoolValue("isBotMatch") ?? false;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{ ");

            stringBuilder.Append(string.Format("MatchId: {0}, ", MatchId));
            stringBuilder.Append(string.Format("ServerIp: {0}, ", ServerIp));
            stringBuilder.Append(string.Format("ServerPort: {0}, ", ServerPort));
            stringBuilder.Append(string.Format("MatchToken: {0} ", MatchToken));
            stringBuilder.Append(string.Format("IsBotMatch: {0} ", IsBotMatch));

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }
    }
}
