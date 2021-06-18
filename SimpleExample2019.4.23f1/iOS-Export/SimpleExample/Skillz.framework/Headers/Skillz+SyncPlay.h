//
//  Skillz+SyncPlay.h
//  SkillzSDK-iOS
//
//  Created by TJ Fallon on 7/15/16.
//  Copyright © 2016 Skillz. All rights reserved.
//

#import "SkillzInstance.h"
#import "SKZPlayer.h"

typedef uint16_t SKZSyncKey;
typedef uint64_t SKZSyncPlayerId;

/**
 *  The object you pass into the delegate argument of initWithGameId:forDelegate:withEnvironment:allowExit: should conform to this protocol.
 */
@protocol SkillzSyncDelegate <NSObject>

#pragma mark Standard Implementation

@required

/** @name User Connection Management */

/**
 *  This method will be called when the opponent player has successfully reconnected within the timeout.
 *  This will allow you to resume the game.
 *
 *  Note: This method will be called from a background thread.
 *
 *  @param playerId     Id of player who has reconnected
 */
- (void)onOpponentHasReconnected:(SKZSyncPlayerId)playerId;

/**
 *  This method will be called when the opponent player's connection has failed and Skillz is attempting to reconnect
 *  This will allow you to pause the game while waiting for the player to reconnect.
 *
 *  Note: This method will be called from a background thread.
 *
 *  @param playerId     Id of player who is attempting to reconnect
 */
- (void)onOpponentHasLostConnection:(SKZSyncPlayerId)playerId;

/**
 *  This method will be called when the opponent player leaves the match (due to connection failure or abort) and is unable to rejoin. At this point, Skillz handles the player as if they have aborted.
 *  At this point your game should gracefully end the match and report the current player's score.
 *
 *  Note: If you receive this message without a corresponding "onOpponentHasLostConnection:" call, you should assume the player has manually aborted.
 *
 *  Note: This method will be called from a background thread.
 *
 *  @param playerId     Id of player who has permanently left the match 
 */
- (void)onOpponentHasLeftMatch:(SKZSyncPlayerId)playerId;

/**
 *  This method will be called when the current player has successfully reconnected within the timeout.
 *  This will allow you to resume the game.
 *
 *  Note: This method will be called from a background thread.
 */
- (void)onCurrentPlayerHasReconnected;

/**
 *  This method will be called when the current player's connection has failed and Skillz is attempting to reconnect
 *  This will allow you to pause the game while waiting for the player to reconnect.
 *
 *  Note: This method will be called from a background thread.
 */
- (void)onCurrentPlayerHasLostConnection;

/**
 *  This method will be called when the current player leaves the match (due to connection failure or abort) and is unable to rejoin. At this point, Skillz handles the player as if they have aborted.
 *
 *  Note: You should follow up by calling "notifyPlayerAbortWithCompletion:" at your convenience.
 *
 *  Note: This method will be called from a background thread.
 */
- (void)onCurrentPlayerHasLeftMatch;

@optional


/** @name Message Passing */
#pragma mark Synchronous Message Passing

/**
 *  When another instance of your client connected to the same match passes a message using the below `sendData:`, this method will be called on all other clients.
 *
 *  Note: This method will be called from a background thread.
 *
 *  @param data      The message sent by your client
 */
- (void)onDidReceiveData:(NSData * _Nonnull)data;

#pragma mark Match Status Helpers

/**
 *  This method will be triggered on all clients when one client has explicitly reported a score or aborted.
 *
 *  Note: This method will be called from a background thread.
 */
- (void)onMatchCompleted;

@end


/**
 * @warning Starting and Ending a Tournament
 *
 * Note: Use existing methods for ending and beginning a match that are defined in SkillzInstance.h
 *
 * You should implement `tournamentWillBegin:withMatchInfo:`, which will be called when a match should begin.
 * You can check the supplied SKZMatchInfo's isSynchronous property to determine whether the match is Sync or Async.
 *
 * For reporting a player's score, you should use `displayTournamentResultsWithScore:withCompletion:`.
 *
 * For aborting a player, you should use `notifyPlayerAbortWithCompletion:`
 *
 */
@interface Skillz (SyncPlay)

/** @name Message Passing */
#pragma mark Message Passing

/**
 *  Call this method in order to send a message to all clients connected to this match.
 *  This will trigger `onDidReceiveData:` on all clients connected to the current match.
 *
 *  Note: NSData passed to this function are limited to a certain size based on the game, and this function will assert if over that size. (2048 bytes currently)
 *  Please reach out to integrations@skillz.com for more information.
 *
 *  @param message  The message to be sent.
 */
- (void)sendData:(NSData * _Nonnull)data;

#pragma mark Match Status Helpers

/**
 *  This will return whether or not another client connected to this match has called 
 *  either `displaySynchronousTournamentResultsWithScore` or `initiateSynchronousAbortWithCompletion:`
 *  to end the match.
 *
 *  @return Whether or not the match has been completed.
 */
- (BOOL)isMatchCompleted;

#pragma mark Fetching State Data
/** @name Fetching State Data */

/**
 *  Use this method to query the number of players currently connected to a match.
 *
 *  @return The numbers of players connected.
 */
- (NSInteger)getConnectedPlayerCount;

/**
 *  This will return the current user's player ID for an in progress match.
 *  You can use this to easily fetch the current SKZSyncPlayer, or for use in variable passing.
 *
 *
 *  @return Integer identifying the current user in the match.
 */
- (SKZSyncPlayerId)getCurrentPlayerId;

/**
 *  This will return the current user's opponent player ID for an in progress match.
 *  You can use this to easily fetch the current SKZSyncPlayer, or for use in variable passing.
 *
 *
 *  @return Integer identifying the current user in the match.
 */
- (SKZSyncPlayerId)getCurrentOpponentPlayerId;


/**
 *  Will return the current server time so that you may synchronize via that rather than local device time.
 *  Note: Do not use this to display a time and date.
 *
 *  @return Current time in seconds
 */
- (double)getServerTime;

/**
 *  A player only has this much time remaining to reconnect once disconnected. This is the actual time they have left when this method is called.
 *  Note: If the value returned is negative, the player should be considered aborted from the match.
 *
 *  @param The player id of the disconnected player.
 *
 *  @return Current time in milliseconds
 */
- (uint64_t)getTimeLeftForReconnection:(SKZSyncPlayerId)playerId;

@end
