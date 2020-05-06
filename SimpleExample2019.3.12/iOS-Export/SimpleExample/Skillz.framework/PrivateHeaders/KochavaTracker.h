//
//  KochavaTracker.h
//  KochavaTracker
//
//  Created by John Bushnell on 9/26/16.
//  Copyright (c) 2013 - 2017 Kochava, Inc. All rights reserved.
//



#if WHTLBL_REVEAL_TARGET == 1

#warning KochavaTracker.h: libKochavaTrackeriOS

#endif



#pragma mark - IMPORT



#import <Foundation/Foundation.h>

#import "KochavaEvent.h"



#pragma mark - DEFINE



#if WHTLBL_REVEAL_TARGET == 1

#define KOCHAVA_DEPRECATED(MSG) /*__attribute__((deprecated(MSG)))*/

#else

#define KOCHAVA_DEPRECATED(MSG) __attribute__((deprecated(MSG)))

#endif



#pragma mark - CLASS



@class KochavaTracker;

@class UIApplication;



#pragma mark - PROTOCOL



@protocol KochavaTrackerDelegate <NSObject>

@optional



/*!
 @method - tracker:didRetrieveAttributionDictionary:
 
 @brief A method which is called when attribution has been requested and returned to the app.
 */
- (void)tracker:(nonnull KochavaTracker *)tracker didRetrieveAttributionDictionary:(nonnull NSDictionary *)attributionDictionary;



/*!
 @method - Kochava_attributionResult:
 
 @brief A method which is called when attribution has been requested and returned to the app.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead implement tracker:didRetrieveAttributionDictionary:.
 */
- (void)Kochava_attributionResult:(nullable NSDictionary *)attributionResult KOCHAVA_DEPRECATED("Please instead use tracker:didRetrieveAttributionDictionary:");



@end



#pragma mark - CONST



/*!
 @constant kKVAParamAppGUIDStringKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the tracker app id.
 
 @discussion The corresponding value should be a String.
 */
extern NSString * _Nonnull const kKVAParamAppGUIDStringKey;



/*!
 @constant kKVAParamCustomIdStringKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the custom id string.
 
 @discussion The corresponding value should be a String.
 */
extern NSString * _Nonnull const kKVAParamCustomIdStringKey KOCHAVA_DEPRECATED("Please instead use the constant string 'custom_id'");



/*!
 @constant kKVAParamIdentityLinkDictionaryKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the identity link dictionary.
 
 @discussion The corresponding value should be a dictionary.
 */
extern NSString * _Nonnull const kKVAParamIdentityLinkDictionaryKey;



/*!
 @constant kKVAParamAppLimitAdTrackingBoolKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the limit ad tracking boolean.
 
 @discussion The corresponding value should be a boolean wrapped in an NSNumber.
 */
extern NSString * _Nonnull const kKVAParamAppLimitAdTrackingBoolKey;



/*!
 @constant kKVAParamLogLevelEnumKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the log level enum.
 
 @discussion The corresponding value should be an NSString matching one of the defined constants for log levels.
 */
extern NSString * _Nonnull const kKVAParamLogLevelEnumKey;



/*!
 @constant kKVAParamLogMultiLineBoolKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the log multi-line boolean.
 
 @discussion The corresponding value should be a boolean wrapped in an NSNumber.
 */
extern NSString * _Nonnull const kKVAParamLogMultiLineBoolKey;



/*!
 @constant kKVAParamRetrieveAttributionBoolKey
 
 @brief A constant to use for the key when passing the parameter to the tracker to set the retrieve attribution boolean.
 
 @discussion The corresponding value should be a boolean wrapped in an NSNumber.
 
    Important Note:  This should only be done if your app makes use of this information, otherwise it causes needless network communication.  Attribution will performed server-side regardless of the application requesting the results.
 */
extern NSString * _Nonnull const kKVAParamRetrieveAttributionBoolKey;



/*!
 @constant kKVALogLevelEnumNone
 
 @brief A LogLevelEnum of None.
 
 @discussion The None log level generally logs nothing.  It is intended to be an off switch.
 */
extern NSString * _Nonnull const kKVALogLevelEnumNone;



/*!
 @constant kKVALogLevelEnumError
 
 @brief A LogLevelEnum of Error.
 
 @discussion The Error log level generally logs the most severe of exceptions, where the attention of the developer is considered to be required.
 */
extern NSString * _Nonnull const kKVALogLevelEnumError;



/*!
 @constant kKVALogLevelEnumWarn
 
 @brief A LogLevelEnum of Warn (Warning).
 
 @discussion The Warn (Warning) log level generally only logs exceptions that occur outside of normal operation.  With the Warning log level there will be no log entries unless there is something unusual to report, such as an invalid parameter.
 */
extern NSString * _Nonnull const kKVALogLevelEnumWarn;



/*!
 @constant kKVALogLevelEnumInfo
 
 @brief A LogLevelEnum of Info (default).
 
 @discussion The Info log level generally only logs key highlights.  Primarily this includes exceptions that occur outside of normal operation, but it also includes a few key moments such as when a tracker is initialized or deallocated.  Beyond that, with the Info log level there will generally be no log entries unless there is something unusual to report, such as an invalid parameter.
 */
extern NSString * _Nonnull const kKVALogLevelEnumInfo;



/*!
 @constant kKVALogLevelEnumDebug
 
 @brief A LogLevelEnum of Debug.
 
 @discussion The Debug log level expands the logging of the tracker to include details about the internal tasks and network transactions occurring within the tracker.  It is useful for diagnostic purposes.  The Debug log level is higher than is recommended for a released app.
 */
extern NSString * _Nonnull const kKVALogLevelEnumDebug;



/*!
 @constant kKVALogLevelEnumTrace
 
 @brief A LogLevelEnum of Trace.
 
 @discussion The Trace log level expands the logging of the tracker to include finite details useful to trace down where something is occurring in the tracker.  It is useful for diagnostic purposes and is generally used by the SDK Developer.  The Trace log level is higher than is recommended for a released app.
 */
extern NSString * _Nonnull const kKVALogLevelEnumTrace;



#pragma mark - INTERFACE



/*!
 @class KochavaTracker
 
 @brief Allows the app developer to collect unique advertising information from the user for the purpose of measuring ad campaign and user performance.
 
 @discussion Customers use information collected from end users on The Kochava Platform to view reports and data to optimize their advertising campaigns. Customers may also use  the information they collect on The Kochava Platform for the following purposes, which include, but are not limited to:
 
 • Measuring the performance of ad campaigns (for example, by comparing the lifetime value of a user against the expenses required to obtain that user);
 
 • Measuring the value of particular end users;
 
 • Measuring the performance of various Partners, including ad networks and publishers;
 
 • Creating customized performance reports (for example, how well a particular advertisement is performing in a particular geographic region);
 
 • Determining when end users respond to ads; and
 
 • Device attribution.
 
 (lib Kochava)
 
 @author Kochava, Inc.
 
 @copyright 2013 - 2017 Kochava, Inc.
 */
@interface KochavaTracker : NSObject



#pragma mark - SINGLETON



/*!
 @property shared
 
 @brief A singleton shared instance, for convenience.
 
 @discussion This is the preferred way of using a tracker.  To complete the integration you must call configureWithParametersDictionary:delegate:.  You may alternatively use the designated initializer to create your own tracker.  The shared instance provides a few benefits.  First, it simplifies your implementation as you do not need to store an instance to the tracker somewhere in a public location in your own code.  Second, it ensures that if your code unintentionally tries to make use of the shared instance prior to configuration that you can receive a warning in the log from the tracker.  If you use your own property to store the tracker, and it is nil, then this provision would not be automatically available to you.
 */
@property (class, readonly, strong, nonnull) KochavaTracker *shared;



#pragma mark - INSTANCE METHODS (LIFECYCLE)



/*!
 @method - configureWithParametersDictionary:delegate:
 
 @brief The main configuration method for use with the shared instance.  This method configures (or reconfigures) a tracker with a parametersDictionary.  When using the shared this method must be called prior to using the tracker.
 
 @discussion This method configures the tracker with parameters passed in a parametersDictionary.  It is intended for use with the shared instance only.  By calling the KochavaTracker configuration method, you have completed the basic integration with the KochavaTracker SDK.  The call to the configuration method should be located in the logic of your application where things first start up, such as your App Delegate's application:didFinishLaunchingWithOptions: method.

 @param parametersDictionary a dictionary containing any number of parameters with which to configure the tracker.
 
 @param delegate A delegate which can be used to return attribution information along with other information (optional).
 
 @code
 NSMutableDictionary *parametersDictionary = NSMutableDictionary.dictionary;
 parametersDictionary[kKVAParamAppGUIDStringKey] = @"_YOUR_KOCHAVA_APP_GUID_";
 parametersDictionary[kKVAParamLogLevelEnumKey] = kKVALogLevelEnumInfo;
 [KochavaTracker.shared configureWithParametersDictionary:parametersDictionary delegate:self];
 @endcode
*/
- (void)configureWithParametersDictionary:(nonnull NSDictionary *)parametersDictionary delegate:(nullable id<KochavaTrackerDelegate>)delegate;



/*!
 @method - initWithParametersDictionary:delegate:
 
 @brief The designated initializer for a Kochava Tracker.
 
 @discussion This method initializes a tracker with parameters passed in a parametersDictionary.  By calling the KochavaTracker initializer, you have completed the basic integration with the KochavaTracker SDK.  The call to the initializer should be located in the logic of your application where things first start up, such as your App Delegate's application:didFinishLaunchingWithOptions: method.
 
 @param parametersDictionary A dictionary containing the tracker's parameters.
 
 @param delegate A delegate which can be used to return attribution information along with other information (optional).
 
 @return A tracker, or possibly nil if the dictionary did not contain valid properties to form one.
 
 @code
 NSMutableDictionary *parametersDictionary = NSMutableDictionary.dictionary;
 parametersDictionary[kKVAParamAppGUIDStringKey] = @"_YOUR_KOCHAVA_APP_GUID_";
 parametersDictionary[kKVAParamLogLevelEnumKey] = kKVALogLevelEnumInfo;
 KochavaTracker *kochavaTracker = [[KochavaTracker alloc] initWithParametersDictionary:parametersDictionary delegate:self];
 @endcode
 */
- (nullable id)initWithParametersDictionary:(nonnull NSDictionary *)parametersDictionary delegate:(nullable id<KochavaTrackerDelegate>)delegate;



#pragma mark - INSTANCE PROPERTIES



/*!
 @property sleepBool
 
 @brief A boolean which when true (YES) causes the tracker to sleep.
 
 @discussion The default is false (NO).  When set to true (YES), this causes tasks to effectively be suspended until this condition is lifted.  While this is set to true, tasks are not lost per-say;  however, if a task may have otherwise occurred multiple times, it may be represented only once once the condition is lifted.
 */
@property BOOL sleepBool;



#pragma mark - INSTANCE METHODS (GENERAL)



/*!
 @method - sendEvent:
 
 @brief A method to queue a post-install event with standardized parameters to be sent to the server.
 
 @param event A KochavaEvent configured with the values you want to associate with the event.
 */
- (void)sendEvent:(nonnull KochavaEvent *)event;



/*!
 @method - sendEventWithNameString:infoDictionary:
 
 @brief A method to queue a post-install event with custom parameters to be sent to server.
 
 @param nameString String containing event title or key of key/value pair.
 
 @param infoDictionary Dictionary (single dimensional) containing any number of values with keys.
 */
- (void)sendEventWithNameString:(nonnull NSString *)nameString infoDictionary:(nullable NSDictionary *)infoDictionary;



/*!
 @method - sendEventWithNameString:infoString:
 
 @brief A method to queue a post-install event with custom parameters to be sent to server.
 
 @param nameString String containing event title or key of key/value pair.
 
 @param infoString String containing event value or value of key/value pair.  It may be an unnested (single dimensional) dictionary converted to a JSON formatted string.
 */
- (void)sendEventWithNameString:(nonnull NSString *)nameString infoString:(nullable NSString *)infoString;



/*!
 @method - sendIdentityLinkWithDictionary:
 
 @brief A method to queue an Identity Link event to be sent to server.
 
 @param dictionary A dictionary containing key/value pairs to be associated with the app install.  The keys may be any string value.  The values may be any string or numeric value.
 
    Important Note:  When possible, Identity Link information should be provided using the kKVAParamIdentityLinkDictionaryKey when the tracker is being configured, as opposed to using instance method sendIdentityLinkWithDictionary:, to ensure that your identity values are always associated with your install.
 */
- (void)sendIdentityLinkWithDictionary:(nonnull NSDictionary *)dictionary;



/*!
 @method - setAppLimitAdTrackingBool:
 
 @brief A method to limit ad tracking at the application level.
 
 @discussion This feature is related to the Limit Ad Tracking feature which is typically found on an Apple device under Settings, Privacy, Advertising.  In the same way that you can limit ad tracking through that setting, this feature provides a second and independent means for the host app to limit ad tracking by asking the user directly.  A value of NO (false) from either feature (this or Apple's) will result in the limiting of ad tracking.
 
 @param appLimitAdTrackingBool A boolean toggling app level limit ad tracking on (YES) or off (NO).
 */
- (void)setAppLimitAdTrackingBool:(BOOL)appLimitAdTrackingBool;



/*!
 @method - attributionDictionary
 
 @brief A method to return the attribution information previously retrieved from the server (if any).
 
 @discussion The use of this method assumes that the tracker was previously requested to retrieve attribution during its initial initialization.  It is intended that this information be passed automatically back to the parent through delegation.  This method can be used to re-retrieve the same information, but if it is called before attribution information has been retrieved then the result will be nil.
 
 @return a dictionary containing attribution information (or nil).
 */
- (nullable NSDictionary *)attributionDictionary;



/*!
 @method - sendDeepLinkWithOpenURL:sourceApplicationString:
 
 @brief A method to queue a deep-link and its associated data to be sent to server.
 
 @param openURL The url received by the openURL application delegate method.
 
 @param sourceApplicationString The sourceApplication string received by the openURL application delegate method.
 */
- (void)sendDeepLinkWithOpenURL:(nullable NSURL *)openURL sourceApplicationString:(nullable NSString *)sourceApplicationString;



/*!
 @method - deviceIdString
 
 @brief A method to return the unique device ID that was generated when the tracker was first initialized on the current device.
 */
- (nullable NSString *)deviceIdString;



/*!
 @method - handleWatchEvents
 
 @brief A method to tell the server that an Apple Watch has been used with this app.
 
 @discussion If you have a unique identifier to associate with the Apple Watch, you should instead call handleWatchEventsWithWatchIdString:.
 */
- (void)handleWatchEvents;



/*!
 @method - handleWatchEventsWithWatchIdString:
 
 @brief A method to tell the server that a specific, identifiable Apple Watch has been used with this app.
 
 @param watchIdString The name or identifier of watch used with the app.
 */
- (void)handleWatchEventsWithWatchIdString:(nullable NSString *)watchIdString;



/*!
 @method - sendWatchEventWithNameString:infoString:
 
 @brief A method to queue a post-install Apple Watch event to be sent to server.
 
 @param nameString String containing event title or key of key/value pair.
 
 @param infoString String containing event value or value of key/value pair.  Value may be an unnested (single dimensional) dictionary converted to a JSON formatted string.
 */
- (void)sendWatchEventWithNameString:(nonnull NSString *)nameString infoString:(nullable NSString *)infoString;



/*!
 @method - sdkVersionString
 
 @brief A method to return the sdk version string.
 
 @discussion The returned value includes the name of the SDK, followed by its semantic version.
 */
- (nullable NSString *)sdkVersionString;



/*!
 @method - addRemoteNotificationsDeviceToken:
 
 @brief A method which adds a remote notifications device token.
 
 @param deviceToken The device token as provided in NSData.
 */
- (void)addRemoteNotificationsDeviceToken:(nonnull NSData *)deviceToken;



/*!
 @method - removeRemoteNotificationsDeviceToken:
 
 @brief A method which removes any assocation for this device with any previously registered remote notifications device token.
 
 @param deviceToken The device token as provided in NSData.
 */
- (void)removeRemoteNotificationsDeviceToken:(nonnull NSData *)deviceToken;



#pragma mark - DEPRECATED



/*!
 @brief An initializer for a Kochava Tracker (Constructor).
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please initialize a tracker either using the shared, or else using initWithParametersDictionary:delegate:.  In Swift: init(parametersDictionary:delegate:)
 */
- (nullable id)initKochavaWithParams:(nonnull NSDictionary *)parametersDictionary KOCHAVA_DEPRECATED("Please initialize a tracker either using the shared, or else using initWithParametersDictionary:delegate:.  In Swift: init(parametersDictionary:delegate:)");



/*!
 @method - identityLinkEvent:
 
 @brief A method to queue an Identity Link event to be sent to server.
 
 @param dictionary A dictionary containing key/value pairs to be associated with the app install.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendIdentityLinkWithDictionary:.  In Swift: sendIdentityLink(with:)
 */
- (void)identityLinkEvent:(nonnull NSDictionary *)dictionary KOCHAVA_DEPRECATED("Please instead use sendIdentityLinkWithDictionary:.  In Swift: sendIdentityLink(with:)");



/*!
 @method - trackEvent:value:
 
 @brief A method to queue a post-install event with custom parameters to be sent to server.
 
 @param titleString String containing event title or key of key/value pair.
 
 @param valueString String containing event value or value of key/value pair.  Value may be an unnested (single dimensional) dictionary converted to a JSON formatted string.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendEventWithNameString:infoString:.  In Swift: sendEvent(withNameString:infoString:)
 */
- (void)trackEvent:(nonnull NSString *)titleString value:(nullable NSString *)valueString KOCHAVA_DEPRECATED("Please instead use sendEventWithNameString:infoString:.  In Swift: sendEvent(withNameString:infoString:)");



/*!
 @method - trackEvent:withValue:andReceipt:
 
 @brief A method to queue a post-install event with a receipt to be sent to server.
 
 @param titleString String containing event title or key of key/value pair.
 
 @param valueString String containing event value or value of key/value pair.  Value may be an unnested (single dimensional) dictionary converted to a JSON formatted string.
 
 @param appStoreReceiptBase64EncodedString String containing an App Store base64 encoded receipt.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendEventWithNameString:infoString:appStoreReceiptBase64EncodedString.  In Swift: sendEvent(withNameString:infoString:appStoreReceiptBase64EncodedString:)
 */
- (void)trackEvent:(nonnull NSString *)titleString withValue:(nullable NSString *)valueString andReceipt:(nonnull NSString *)appStoreReceiptBase64EncodedString KOCHAVA_DEPRECATED("Please instead use sendEventWithNameString:infoString:appStoreReceiptBase64EncodedString.  In Swift: sendEvent(withNameString:infoString:appStoreReceiptBase64EncodedString:)");



/*!
 @method - setLimitAdTracking:
 
 @brief A method to limit ad tracking at the application level.
 
 @discussion This feature is related to the Limit Ad Tracking feature which is typically found on an Apple device under Settings, Privacy, Advertising.  In the same way that you can limit ad tracking through that setting, this feature provides a second and independent means for the host app to limit ad tracking by asking the user directly.  A value of NO (false) from either feature (this or Apple's) will result in the limiting of ad tracking.
 
 @param limitAdTrackingBool A boolean toggling app level limit ad tracking on (YES) or off (NO).
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use setAppLimitAdTrackingBool:
 */
- (void)setLimitAdTracking:(BOOL)limitAdTrackingBool KOCHAVA_DEPRECATED("Please instead use setAppLimitAdTrackingBool:");



/*!
 @method - sendDeepLink:sourceApplication:
 
 @brief A method to queue a deep-link and its associated data to be sent to server.
 
 @param url The url received by the openURL application delegate method.
 
 @param sourceApplication The sourceApplication string received by the openURL application delegate method.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendDeepLinkWithOpenURL:sourceApplicationString:.  In Swift: sendDeepLink(withOpen:sourceApplicationString:)
 */
- (void)sendDeepLink:(nullable NSURL *)url sourceApplication:(nullable NSString *)sourceApplication KOCHAVA_DEPRECATED("Please instead use sendDeepLinkWithOpenURL:sourceApplicationString:.  In Swift: sendDeepLink(withOpen:sourceApplicationString:)");



/*!
 @method - getKochavaDeviceId
 
 @brief A method to return the device ID generated when the tracker was initialized.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use deviceIdString.
 */
- (nullable NSString *)getKochavaDeviceId KOCHAVA_DEPRECATED("Please instead use deviceIdString");



/*!
 @method - retrieveAttribution
 
 @brief A method to return the attribution information previously retrieved from the server (if any).
 
 @discussion The use of this method assumes that the tracker was previously requested to retrieve attribution during its initial initialization.  It is intended that this information be passed automatically back to the parent through delegation.  This method can be used to re-retrieve the same information, but if it is called before attribution information has been retrieved then the result will be nil.
 
 This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use attributionDictionary.
 
 @return a dictionary containing attribution information (or nil).
 */
- (nullable id)retrieveAttribution KOCHAVA_DEPRECATED("Please instead use attributionDictionary");



/*!
 @method - handleWatchEvents:
 
 @brief A method to tell the server that a specific, identifiable Apple Watch has been used with this app.
 
 @param watchIdString The name or identifier of watch used with the app.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use handleWatchEventsWithWatchIdString:
 */
- (void)handleWatchEvents:(nullable NSString *)watchIdString KOCHAVA_DEPRECATED("Please instead use handleWatchEventsWithWatchIdString:");



/*!
 @method - trackWatchEvent:value:
 
 @brief A method to queue a post-install Apple Watch event to be sent to server.
 
 @param titleString String containing event title or key of key/value pair.
 
 @param valueString String containing event value or value of key/value pair.  Value may be an unnested (single dimensional) dictionary converted to a JSON formatted string.
 
 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendWatchEventWithNameString:infoString:.  In Swift: sendWatchEvent(withNameString:infoString:)
 */
- (void)trackWatchEvent:(nonnull NSString *)titleString valueString:(nullable NSString *)valueString KOCHAVA_DEPRECATED("Please instead use sendWatchEventWithNameString:infoString:.  In Swift: sendWatchEvent(withNameString:infoString:)");



- (void)trackEvent:(nonnull NSString *)titleString :(nullable NSString *)valueString
KOCHAVA_DEPRECATED("Please instead use sendEventWithNameString:infoString:.  In Swift: sendEvent(withNameString:infoString:)");

- (void)sendDeepLink:(nullable NSURL *)url :(nullable NSString *)sourceApplication
KOCHAVA_DEPRECATED("Please instead use sendDeepLinkWithOpenURL:sourceApplicationString:.  In Swift: sendDeepLink(withOpen:sourceApplicationString:)");

- (void)trackWatchEvent:(nonnull NSString *)titleString :(nullable NSString *)valueString
KOCHAVA_DEPRECATED("Please instead use sendWatchEventWithNameString:infoString:.  In Swift: sendWatchEvent(withNameString:infoString:)");



/*!
 @method - sendEventWithEventStandardParameters:
 
 @brief A method to queue a post-install event with standardized parameters to be sent to the server.
 
 @param eventStandardParameters EventStandardParameters configured with the values you want to associate with the event.

 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendEvent:.  In Swift: send(_ event:)
 */
- (void)sendEventWithEventStandardParameters:(nonnull KochavaEvent *)eventStandardParameters KOCHAVA_DEPRECATED("The class EventStandardParameters has been renamed to KochavaEvent.  Please instead use sendEvent: , and also rename the class EventStandardParameters to KochavaEvent wherever you have used it.");



/*!
 @method - sendEventWithNameString:infoString:appStoreReceiptBase64EncodedString:
 
 @brief A method to queue a post-install event with a receipt to be sent to server.
 
 @param nameString String containing event title or key of key/value pair.
 
 @param infoString String containing event value or value of key/value pair.  It may be an unnested (single dimensional) dictionary converted to a JSON formatted string.
 
 @param appStoreReceiptBase64EncodedString String containing an App Store base64 encoded receipt.

 @discussion This method has been deprecated and is scheduled to be permanently removed in v4.0 of this SDK.  Please instead use sendEvent:.  In Swift: send(_ event:)
 */
- (void)sendEventWithNameString:(nonnull NSString *)nameString infoString:(nullable NSString *)infoString appStoreReceiptBase64EncodedString:(nonnull NSString *)appStoreReceiptBase64EncodedString KOCHAVA_DEPRECATED("Please instead use sendEvent:.  In Swift: send(_ event:).  Create a KochavaEvent and pass the appStoreReceiptBase64EncodedString using the standard parameter.  You may use KochavaEventTypeEnumPurchase, and set any of the other applicable standard parameters.");



@end



