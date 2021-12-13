//
//  KVALog.h
//  KochavaCore
//
//  Created by John Bushnell on 8/1/16.
//  Copyright © 2017 - 2021 Kochava, Inc.  All rights reserved.
//



#ifndef KVALog_h
#define KVALog_h



#pragma mark - CLASS



@class KVALogLevel;
@class KVAProduct;



#pragma mark - INTERFACE



/*!
 @class KVALog
 
 @brief A controller for working with the log.
 
 @discussion This class provides controlling mechanisms for the log.  It maintains a log enabled boolean and a log level, providing methods to support logging.
 
 Inherits from: NSObject
 
 @classdesign Singleton
 
 @author John Bushnell
 
 @copyright 2017 - 2021 Kochava, Inc.
 */
@interface KVALog : NSObject



#pragma mark - SHARED INSTANCE (SINGLETON)



/*!
 @property shared
 
 @brief The singleton shared instance.
 */
@property (class, readonly, strong, nonnull) KVALog *shared;



#pragma mark - PROPERTIES



/*!
 @property level
 
 @brief The visible maximum log level for log messages.
 */
 @property (strong, atomic, nonnull, readwrite) KVALogLevel *level;



/*!
 @property prettyPrintBool
 
 @brief A boolean indicating if log messages should be pretty printed.
 
 @discussion Default true.
 */
@property (atomic, readwrite) BOOL prettyPrintBool;



@end



#endif



