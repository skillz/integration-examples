//
//  Skillz+Progression.h
//  Skillz
//
//  Created by Tyler Gier on 2/24/21.
//  Copyright © 2021 Skillz. All rights reserved.
//

#import "SkillzInstance.h"

/**
 * The default progression data namespace. This is READ ONLY and contains information
 * tracked and maintained by Skillz.
 */
static const NSString* _Nonnull ProgressionNamespaceDefaultPlayerData = @"DefaultPlayerData";
/**
 * The namespace for all Player Data keys configured and tracked by the
 * game developer.
 */
static const NSString* _Nonnull ProgressionNamespacePlayerData = @"PlayerData";
/**
 * The namespace for all In Game Items keys configured and tracked by the
 * game developer.
 */
static const NSString* _Nonnull ProgressionNamespaceInGameItems = @"InGameItems";

@interface Skillz (Progression)

/**
 * Call this function to fetch user data for the given keys from the Skillz servers.
 *
 * @param progressionNamespace The name of the progression data set to fetch
 * @param keys A list of the keys to fetch the player progression data for
 * @param successCompletion A completion function that is called when the data has been successfully retrieved. It is called
 *          with a dictionary containing the key-value pairs for the requested data.
 * @param failureCompletion A completion function that is called when the data retrieval has failed.
 */
- (void)getUserDataForNamespace:(NSString *)progressionNamespace
                       withKeys:(NSMutableArray *)userDataKeys
                    withSuccess:(void (^_Nonnull)(NSDictionary * userData))successCompletion
                    withFailure:(void (^_Nonnull)(NSString * error))failureCompletion;

/**
 * Call this function to update user data with the given data. This function can only update 25 elements at a time.
 *
 * @param progressionNamespace The name of the progression data set to fetch
 * @param userData A list of the keys to fetch the player progression data for
 * @param successCompletion A completion function that is called when the data has been successfully retrieved. It is called
 *          with a dictionary containing the key-value pairs for the requested data.
 * @param failureCompletion A completion function that is called when the data retrieval has failed.
 */
- (void)updateUserDataForNamespace:(NSString *)progressionNamespace
                      withUserData:(NSDictionary *)userDataUpdates
                       withSuccess:(void (^_Nonnull)(NSDictionary * userData))successCompletion
                       withFailure:(void (^_Nonnull)(NSString * error))failureCompletion;

- (void)launchProgressionRoom;

@end
