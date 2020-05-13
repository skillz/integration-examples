using UnityEngine;

namespace Servers
{
    [CreateAssetMenu(fileName = "ConnectArgs", menuName = "Connection Args (Custom Server Sync)")]
    public sealed class ConnectArgs : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        private string developerDescription;
#endif

        [SerializeField]
        private string url;

        [SerializeField]
        private uint port;

        [SerializeField]
        private string matchId;

        [SerializeField]
        private string matchToken;

        public string Url
        {
            get
            {
                return url;
            }
        }

        public uint Port
        {
            get
            {
                return port;
            }
        }

        public long UserId
        {
            get;
            set;
        }

        public string MatchId
        {
            get
            {
                return matchId;
            }
        }

        public string MatchToken
        {
            get
            {
                return matchToken;
            }
        }
    }
}
