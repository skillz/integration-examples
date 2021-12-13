//
//  KVALogLevel.h
//  KochavaCore
//
//  Created by John Bushnell on 12/19/17.
//  Copyright © 2017 - 2021 Kochava, Inc.  All rights reserved.
//



#ifndef KVALogLevel_h
#define KVALogLevel_h



#pragma mark - IMPORT



#import <os/log.h>

#ifdef KOCHAVA_FRAMEWORK
#import <KochavaCore/KVAAsForContextObjectProtocol.h>
#import <KochavaCore/KVAFromObjectProtocol.h>
#else
#import "KVAAsForContextObjectProtocol.h"
#import "KVAFromObjectProtocol.h"
#endif



#pragma mark - INTERFACE



/*!
 @class KVALogLevel
 
 @brief A class which defines a log level enumeration.
 
 Inherits from: NSObject
 
 @author John Bushnell
 
 @copyright 2017 - 2021 Kochava, Inc.
 */
@interface KVALogLevel : NSObject <KVAAsForContextObjectProtocol, KVAFromObjectProtocol>



#pragma mark - ENUMERATED VALUES



/*!
 @property + never
 
 @brief A LogLevel which never prints visibly to the log.
 
 @discussion When LogMessage(s) are not printed visibly to the log, they are still posted as notifications.  This enables all LogMessage(s) to be observed, regardless of their current visibility.
 */
@property (class, strong, nonatomic, nonnull, readonly) KVALogLevel *never;



/*!
 @property + error
 
 @brief A LogLevel for an error.
 
 @discussion The definition of an error adopted by the SDK is anything which is essentially fatal.  This does not mean that there needs to be a crash, but that something failed unrecoverably.
 */
@property (class, strong, nonatomic, nonnull, readonly) KVALogLevel *error;



/*!
 @property + warn
 
 @brief A LogLevel for a warning.
 
 @discussion A warning is generally anything that behaved unexpectedly and should be brought to the developer's attention.
 */
@property (class, strong, nonatomic, nonnull, readonly) KVALogLevel *warn;



/*!
 @property + info
 
 @brief A LogLevel for a piece of general information.
 
 @discussion General information is anything not rising to the level of a warning but also something that does not require you to be debugging a product to see.  This level should be viewed as to be used sparingly, as it is the default log level.
 */
@property (class, strong, nonatomic, nonnull, readonly) KVALogLevel *info;



/*!
 @property + debug
 
 @brief A LogLevel for a piece of debug information.
 
 @discussion Debug information is something helpful to illuminate what is happening, without going into the minutia.
 */
@property (class, strong, nonatomic, nonnull, readonly) KVALogLevel *debug;



/*!
 @property + trace
 
 @brief A LogLevel for a piece of trace information.
 
 @discussion Trace information is the minutia of what is happening.  This information would clutter the logs, even when debugging, and so is intended to be used when trying to trace down an obscure problem.
 */
@property (class,strong, nonatomic, nonnull, readonly) KVALogLevel *trace;



/*!
 @property + always
 
 @brief A LogLevel which always prints visibly to the log.
 
 @discussion When the logLevel is .always and the visibleMaximumLogLevel resolves to .never, the visibleMaximumLogLevel will win, resulting in no visibility.
 */
@property (class,strong, nonatomic, nonnull, readonly) KVALogLevel *always;



#pragma mark - PARAMETERS



/*!
 @property nameString
 
 @brief The name.
 
 @discussion Examples:  "never", "error", "warn", "info", "debug", "trace".
 */
@property (strong, nonatomic, nonnull) NSString *nameString;



/*!
 @property levelInt
 
 @brief The level.
 
 @discussion Examples:  0, 1, 2, 3, 4, 5.  This is used to determine the relative value between log levels, such that everything below or everything above may be determined.
 */
@property NSInteger levelInt;



/*!
 @property os_log_type
 
 @brief The os_log_type.
 
 @discussion When outputting to os_log, this is the os_log_type to use.  Recommendations from Apple keynote https://developer.apple.com/videos/play/wwdc2016/721/
 * Use os_log to log critical details to help debug issues.
 * Use os_log_info for additional info that will be captured during error or fault.
 * Use os_log_debug for high-volume debugging during development.
 * Use os_log_error to cause additional information capture from app.
 * Use os_log_fault to cause additional information capture from system.
 
 Note:  The type here should be os_log_type_t, but it's technically an iOS 10+ type.  So we currently use uint8_t, which is its base type.  Later when the SDK is distributed as iOS 10+ this can  be re-typed.
 */
@property uint8_t os_log_type;



#pragma mark - CLASS GENERAL



/*!
 @method + logLevel:visibleBoolWithVisibleMaximumLogLevel:
 
 @brief A method to return if a given logLevel is visible with the given visibleMaximumLogLevel.
 */
+ (BOOL)logLevel:(nullable KVALogLevel *)logLevel visibleBoolWithVisibleMaximumLogLevel:(nullable KVALogLevel *)visibleMaximumLogLevel;



@end



#endif



