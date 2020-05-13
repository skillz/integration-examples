using System;
using SyncServer;

namespace Servers
{
    public sealed class MatchReadyArgs : EventArgs
    {
        public string MatchId
        {
            get
            {
                return matchSuccessPacket.RegisteredMatchId;
            }
        }

        private readonly MatchSuccessPacket matchSuccessPacket;

        public MatchReadyArgs(MatchSuccessPacket matchSuccessPacket)
        {
            this.matchSuccessPacket = matchSuccessPacket;
        }
    }
}