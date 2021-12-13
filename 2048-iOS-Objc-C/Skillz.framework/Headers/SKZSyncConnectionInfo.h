//
//  SKZSyncConnectionInfo.h
//  SkillzSDK-iOS
//
//  Created by Emily Beckers on 5/23/19.
//  Copyright © 2019 Skillz. All rights reserved.
//

@interface SKZSyncConnectionInfo : NSObject

@property (readonly, nonnull) NSString *serverIP;

@property (readonly, nonnull) NSString *serverPort;

@property (readonly, nonnull) NSString *matchId;

// RSA encrypted
@property (readonly, nonnull) NSString *matchToken;

@property (readonly, assign) BOOL isBotMatch;

@end
