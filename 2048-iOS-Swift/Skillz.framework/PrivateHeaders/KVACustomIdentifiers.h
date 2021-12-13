//
//  KVACustomIdentifiers.h
//  KochavaTracker
//
//  Created by John Bushnell on 9/16/20.
//  Copyright © 2016 - 2021 Kochava, Inc.  All rights reserved.
//



#ifndef KVACustomIdentifiers_h
#define KVACustomIdentifiers_h



#pragma mark - IMPORT



#ifdef KOCHAVA_FRAMEWORK
#import <KochavaCore/KochavaCore.h>
#else
#import "KVAAsForContextObjectProtocol.h"
#import "KVAConfigureWithObjectProtocol.h"
#import "KVAFromObjectProtocol.h"
#endif



#pragma mark - INTERFACE



/*!
 @class KVACustomIdentifiers
 
 @brief A controller for working with customIdentifiers.
 
 @author John Bushnell
 
 @copyright 2016 - 2021 Kochava, Inc.
 */
@interface KVACustomIdentifiers : NSObject <KVAAsForContextObjectProtocol, KVAConfigureWithObjectProtocol, KVAFromObjectProtocol>



/*!
 @method - registerWithNameString:
 
 @brief Registers a custom identifier.
 
 @param nameString The name of the identifier.
 
 @param identifierString The identifier.
 
 @discussion In order to send a custom identifier it must be whitelisted on your account.
 */
- (void)registerWithNameString:(nonnull NSString *)nameString identifierString:(nonnull NSString *)identifierString NS_SWIFT_NAME(register(withNameString:identifierString:));



@end



#endif



