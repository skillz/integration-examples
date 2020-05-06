//
//  Skillz+Analytics.h
//  SkillzSDK-iOS
//
//  Created by TJ Fallon on 7/11/16.
//  Copyright © 2016 Skillz. All rights reserved.
//

#import "SkillzInstance.h"

@interface Skillz (Analytics)

/**
 * You may use this method to track player actions, level types, or other information pertinent to your Skillz integration.
 * There should be no more than 10 Key Value pairs in the supplied attributes dictionary, excess values will be trimmed.
 * Collected data will only be available to Skillz and will help Skillz identify fairness in level based games.
 *
 * @param attributes Values to be recorded. Keys and values must be strings. Keys must be less than 50 characters, values must be less than 200 characters.
 * @param forMatchInProgress If this relates to an in progress match additional match info will be tracked with these attributes.
 */

- (void)addMetadata:(nonnull NSDictionary *)attributes forMatchInProgress:(BOOL)forMatchInProgress;

@end
