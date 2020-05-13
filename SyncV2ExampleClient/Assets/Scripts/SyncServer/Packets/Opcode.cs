namespace Servers
{
    /// <summary>
    /// Opcodes for the example game server protocol.
    /// </summary>
    /// <remarks>
    /// There is currently a bug in Flatbuffers where setting a default value in
    /// the schema does not work. As a result, you have to manually add the
    /// opcode when building one of the Packet instances because Flatbuffer
    /// will default to a value of 0. This value is used as a sentinel to represent
    /// an invalid opcode.
    /// </remarks>
    public enum Opcode : short
    {
        /// <summary>
        /// Invalid/Unrecognized packet.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Sent to connect to a game server.
        /// </summary>
        Connect = 1,

        /// <summary>
        /// A player has connected to the game server.
        /// </summary>
        Success = 2,

        /// <summary>
        /// Sent to the game server to abort from the match.
        /// </summary>
        Abort = 3,

        /// <summary>
        /// Sent to the game server by a player to update their data.
        /// </summary>
        PlayerUpdate = 4,

        /// <summary>
        /// Sent to clients when the game state has updated.
        /// </summary>
        GameStateUpdate = 5,

        /// <summary>
        /// Sent to clients when both players have joined the match.
        /// </summary>
        MatchSuccess = 6,

        /// <summary>
        /// Sent to clients when the game has ended.
        /// </summary>
        GameOver = 7,

        /// <summary>
        /// Sent to clients regularly to keep connection alive.
        /// </summary>
        KeepAlive = 8
    }
}