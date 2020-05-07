//
//  SKZPlayer.h
//  SkillzSDK-iOS
//
//  Created by James McMahon on 6/10/15.
//  Copyright (c) 2015 Skillz. All rights reserved.
//

#import <Foundation/Foundation.h>

__attribute__ ((visibility("default")))

/**
 * Represents data for the current logged in player
 */
@interface SKZPlayer : NSObject

/**
 * Player's unique id
 */
@property (readonly, nonnull) NSString *id;

/**
 * Player's id for the match
 */
@property (readonly, nullable) NSString *playerMatchId;

/**
 * Flag for if the player is the current player
 */
@property (readonly) bool isCurrentPlayer;

/**
 * Player's profile picture (or avatar) URL
 */
@property (readonly, nullable) NSString *avatarURL;

/**
 *  Player's display name
 */
@property (readonly, nonnull) NSString *displayName;

/**
 *  URL for the flag for the player
 */
@property (readonly, nullable) NSString *flagURL;

@end
