//
//  SKZMatchInfo.h
//  SkillzSDK-iOS
//
//  Created by James McMahon on 6/10/15.
//  Copyright (c) 2015 Skillz. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "SKZPlayer.h"
#import "SKZSyncConnectionInfo.h"

__attribute__ ((visibility("default")))

/**
 *  This object contains various Skillz specific pieces of data that will allow you to customize your UI further.
 */
@interface SKZMatchInfo : NSObject

/**
 * Unique match id
 */
@property (readonly) NSInteger id;

/**
 * Match description as configured in the Skillz Developer Portal
 */
@property (readonly, nullable) NSString *matchDescription;

/**
 * Cash entry fee, nil if there is none
 */
@property (readonly, nullable) NSNumber *entryCash;

/**
 * Z points entry fee, nil if there is none
 */
@property (readonly, nullable) NSNumber *entryPoints;

/**
 * Signifies a cash match
 */
@property (readonly) BOOL isCash;

/**
 * Signifies that the match is a tiebreaker. Entry fees described in entryCash or entryPoints refer to the
 * entry fee for the original match that was tied, and will not be deducted from the player's balance for tiebreaker matches.
 */
@property (readonly) BOOL isTieBreaker;

/**
 * Signifies that the match to be played is a synchronous match, and should use your synchronous game flow.
 */
@property (readonly) BOOL isSynchronous;

/**
 *  Match name as configured in the Skillz Developer Portal
 */
@property (readonly, nonnull) NSString *name;

/**
 * An array of the players in the match
 */
@property (readonly, nonnull) NSArray *players;

/**
 * Template id for the template that the match is based on. These templates are
 * configured in the Skillz Developer Portal
 */
@property (readonly, nonnull) NSNumber *templateId;

/**
 * Signifies that the match is a part of a bracketed tournament.
 */
@property (readonly) BOOL isBracket;

/**
 * The 1-indexed round number for bracketed tournaments. If the tournament is not a bracketed tournament, this will be nil.
 *
 * Entry fees are only deducted from the user's balance when playing the first round in a bracketed tournament. 
 */
@property (readonly, nullable) NSNumber *bracketRound;

/**
 * Signifies that the match was entered by waching a video ad instead of paying the entry fee.
 *
 * If this is true, the entry fee described by entryCash or entryPoints were not deducted from the user's balance.
 */
@property (readonly) BOOL isVideoAdEntry;

@property (readonly, nullable) SKZSyncConnectionInfo *connectionInfo;

@end
