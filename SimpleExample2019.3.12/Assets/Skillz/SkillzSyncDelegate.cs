using System;


/// <summary>
/// The base class for a script that responds to messages from the Skillz SDK about events related to synchronous tournaments.
/// All of the optional methods will by default not do anything when called.
/// These callbacks can be used to control game flow or aggresively clean up resources you no longer need.
/// </summary>
public interface SkillzSyncDelegate : SkillzMatchDelegate
{
    /// <summary>
    /// This method will be triggered on all clients when one client has explicitly reported a score or aborted.
    /// </summary>
    void OnMatchCompleted();

    /// <summary>
    ///  When another instance of your client connected to the same match sends data using `SendData`,
    ///  this method will be called on all other clients
    /// </summary>
    /// <param name="data">Data that was sent</param>
    void OnDidReceiveData(byte[] value);

    /// <summary>
    /// This method will be called when the current player's connection has failed, and Skillz is attempting to reconnect.
    /// This will allow you to pause the game while waiting for the player to reconnect.
    /// </summary>
    void OnCurrentPlayerHasLostConnection();

    /// <summary>
    /// This method will be called when the current player, who previously disconnected, has successfully reconnected within the timeout.
    /// When receiving this callback, you should sync game data between all clients, and then resume gameplay.
    /// </summary>
    void OnCurrentPlayerHasReconnected();

    /// <summary>
    /// This method will be called when the current user has left the match (due to connection failure or abort) and is unable to rejoin.
    /// At this point, Skillz will consider this player as having forfeited the match.
    ///
    /// Note: You should follow up by calling "AbortMatch" after cleaning up your game state, in order to pass control back to Skillz.
    /// </summary>
    void OnCurrentPlayerHasLeftMatch();

    /// <summary>
    /// This method will be called when an opponent's connection has failed, and Skillz is attempting to reconnect.
    /// This will allow you to pause the game while waiting for the opponent to reconnect.
    /// </summary>
    /// <param name="playerId">ID of Player that disconnected</param>
    void OnOpponentHasLostConnection(UInt64 playerId);

    /// <summary>
    /// This method will be called when an opponent, who previously disconnected, has successfully reconnected within the timeout.
    /// When receiving this callback, you should sync game data between all clients, and then resume gameplay.
    /// </summary>
    /// <param name="playerId">ID of Player that reconnected</param>
    void OnOpponentHasReconnected(UInt64 playerId);

    /// <summary>
    /// This method will be called when an opponent has left the match (due to connection failure or abort) and is unable to rejoin.
    /// At this point, Skillz will consider this player as having forfeited the match.
    ///
    /// Note: You should follow up by gracefully ending the game, and reporting your player's score in order to pass control back to Skillz.
    /// </summary>
    /// <param name="playerId">ID of Player that left</param>
    void OnOpponentHasLeftMatch(UInt64 playerId);
}