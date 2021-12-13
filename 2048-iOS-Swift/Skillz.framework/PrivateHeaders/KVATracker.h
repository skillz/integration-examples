//
//  KVATracker.h
//  KochavaTracker
//
//  Created by John Bushnell on 9/26/16.
//  Copyright © 2013 - 2021 Kochava, Inc.  All rights reserved.
//



#ifndef KVATracker_h
#define KVATracker_h



#pragma mark - IMPORT



#if TARGET_OS_TV
#import <JavaScriptCore/JavaScriptCore.h>
#endif

#ifdef KOCHAVA_FRAMEWORK
#import <KochavaCore/KochavaCore.h>
#else
#import <Foundation/Foundation.h>  // for #if conditionals.  TARGET_OS_*
#import "KVAAsForContextObjectProtocol.h"
#import "KVAConfigureWithObjectProtocol.h"
#import "KVADeeplink.h"  // for KVADeeplinksProcessorProvider
#import "KVAFromObjectProtocol.h"
#import "KVAEventSender.h"
#import "KVAPrivacyProfile.h"  // for KVAPrivacyProfileRegistrarProvider.
#import "KVAPushNotificationsToken.h"  // for KVAPushNotificationsTokenAdderRemoverProvider.
#import "KVASharedPropertyProvider.h"
#endif



#pragma mark - DEFINE



#if KVA_REVEAL_TARGET == 1
#define KOCHAVA_DEPRECATED(MSG) /*__attribute__((deprecated(MSG)))*/
#else
#define KOCHAVA_DEPRECATED(MSG) __attribute__((deprecated(MSG)))
#endif



#pragma mark - CLASS



#pragma mark - public class KVATracker



/*!
 @class KVATracker
 
 @brief The class KVATracker provides an interface between a host application and Kochava’s Attribution and Analytics Servers.

 @discussion The class KVATracker is the main interface for the KochavaTracker SDK.  A tracker manages the exchange of data between these two entities, along with the associated tasks and network transactions.  If you have not already integrated a KochavaTracker SDK into your project, refer to our KochavaTracker iOS SDK support documentation.
 
 You rarely create instances of the KVATracker class.  Instead, you start the provided shared instance using one of the start instance methods.
 
 From there, the tracker proceeds to initialize immediately and perform its various tasks.  This is typically done during the earliest phases of the host’s life-cycle, so that installation attribution can be quickly established and post-install events may immediately begin to be queued.
 
 You may alternately create an instance of the KVATracker class.  If you do, it is your responsibility to maintain a strong reference.  And if you create multiple instances, it is your responsibility to configure each with a unique storageIdentifierString.
 
 @author Kochava, Inc.
 
 @copyright 2013 - 2021 Kochava, Inc.
 */
@interface KVATracker : NSObject



@end



#pragma mark - feature General



#if TARGET_OS_TV
@protocol KVATrackerGeneralJSExport <JSExport>
@property (class, readonly, strong, nonnull) KVATracker *shared;
+ (nonnull instancetype)tracker NS_SWIFT_NAME(tracker());
+ (nonnull instancetype)trackerWithStorageIdString:(nullable NSString *)storageIdString NS_SWIFT_NAME(init(storageIdString:));
- (void)invalidate NS_SWIFT_NAME(invalidate());
- (void)startWithAppGUIDString:(nonnull NSString *)appGUIDString NS_SWIFT_NAME(start(withAppGUIDString:));
- (void)startWithPartnerNameString:(nonnull NSString *)partnerNameString NS_SWIFT_NAME(start(withPartnerNameString:));
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (General_Public) <KVAAsForContextObjectProtocol, KVAConfigureWithObjectProtocol, KVAFromObjectProtocol, KVASharedPropertyProvider, KVATrackerGeneralJSExport>
#else
@interface KVATracker (General_Public) <KVAAsForContextObjectProtocol, KVAConfigureWithObjectProtocol, KVAFromObjectProtocol, KVASharedPropertyProvider>
#endif



/*!
 @property shared
 
 @brief A shared instance, for convenience.
 
 @discussion This is the preferred way of using a tracker.  To complete the integration you must call func start(withAppGUIDString:) or func start(withPartnerNameString:).  You may alternatively use a constructor to create your own tracker.  The shared instance provides a few benefits.  First, it simplifies your implementation as you do not need to store an instance to the tracker somewhere in a public location in your own code.  Second, it ensures that if your code unintentionally tries to make use of the shared instance prior to configuration that you can receive a warning in the log from the tracker.  If you use your own property to store the tracker, and it is nil, then this provision would not be automatically available to you.
 */
@property (class, readonly, strong, nonnull) KVATracker *shared;



/*!
 @property sharedStorageIdString
 
 @brief A string used as a  storage identifier for the shared instance.
 
 @discussion This is used to further qualify where in persistent storage the information for this instance is stored.  This property should not be used except in very specific circumstances.  Please contact your client success representative if you are interested in using this.  You would set this value to a short unique string consisting of regular alphanumeric characters.  Following deployment with a given storage identifer this should never be changed except to represent an entirely new integration.  If used, it is absolutely imperative that this value be consistently set prior to accessing the shared instance for the first time.
 
 INTERNAL NOTE:  This property exists at the present time for testing and to retain this functionality should it ever be needed in the future.  It is located in an internal header which prevents clients from using it.  If ever this functionality is desired, this property can be moved to the public header, and this note removed.
 */
@property (class, strong, nonatomic, nullable) NSString *sharedStorageIdString;



/*!
 @method + tracker
 
 @brief Constructs and returns an instance of class KVATtracker.
 
  @discussion This convenience constructor exists for Objective-C.  In Swift you should use default constructor KVATracker().
 */
+ (nonnull instancetype)tracker NS_SWIFT_NAME(tracker());



/*!
 @method init(storageIdString:)
 
 @brief Constructs and returns an instance of class KVATtracker.
 
 @param storageIdString An optional storage identifier.
 
 @discussion The storage identifier should be left unset unless you have a reason to not use the default storage space.  See default constructor KVATracker(), or in Objective-C see convenience constructor tracker.
 */
+ (nonnull instancetype)trackerWithStorageIdString:(nullable NSString *)storageIdString NS_SWIFT_NAME(init(storageIdString:));
    


/*!
 @method func invalidate()
 
 @brief Invalidates an instance of class KVATracker.
 
 @discussion This is similar to allowing an instance of the tracker deallocate, but it can also be used on the shared instance.  It will additionally signal certain sub-systems to invalidate themselves, which can result in a more assured and immediate halt.  The scope of this invalidation is not absolute.  Certain sub-systems will continue to run for a period of time until they may gracefully complete.  When using this method with the shared instance, you are guaranteed to be re-defaulted a new instance the next time it is referenced, and you may immediately move forward to re-configure it.
 
 When you are not using Intelligent Consent Management, this method can be used to signal that the tracker may no longer run following consent having been denied.  When used this way, you may re-configure a tracker if/when consent is granted.
 */
- (void)invalidate NS_SWIFT_NAME(invalidate());



/*!
 @property - startedBool
 
 @brief A boolean indicating whether or not the tracker has been started.
 */
@property (readonly) BOOL startedBool;



/*!
 @method func start(withAppGUIDString:)
 
 @brief Starts the tracker.
 
 @param appGUIDString A Kochava app GUID.
 
 @discussion See also func start(withPartnerNameString:).  You may start a tracker with either an appGUIDString or a partnerNameString.
 
 @code
 KVATracker.shared.start(withAppGUIDString: "_YOUR_KOCHAVA_APP_GUID_")
 @endcode
 */
- (void)startWithAppGUIDString:(nonnull NSString *)appGUIDString NS_SWIFT_NAME(start(withAppGUIDString:));



/*!
 @method func start(withPartnerNameString:)

 @brief Starts the tracker.

 @param partnerNameString A Kochava partner name.
 
 @discussion See also func start(withAppGUIDString:).  You may start a tracker with either an appGUIDString or a partnerNameString.

@code
KVATracker.shared.start(withPartnerNameString: "_YOUR_KOCHAVA_PARTNER_NAME_")
@endcode
*/
- (void)startWithPartnerNameString:(nonnull NSString *)partnerNameString NS_SWIFT_NAME(start(withPartnerNameString:));



/*!
 @method - executeAdvancedInstructionWithIdentifierString:
 
 @brief A method to execute an advanced instruction.
 
 @param identifierString An identifier for the advanced instruction.
 
 @param valueObject A value object for the advanced instruction.
 */
- (void)executeAdvancedInstructionWithIdentifierString:(nonnull NSString *)identifierString valueObject:(nullable id)valueObject NS_SWIFT_NAME(executeAdvancedInstruction(withIdentifierString:valueObject:));



@end



#pragma mark - feature Ad Network



@protocol KVAAdNetworkProtocol;




@interface KVATracker (AdNetwork_Public)


/*!
 @property adNetwork
 
 @brief An instance of class KVAAdNetwork.
 
 @discussion A controller for working with location services.
 */
@property (strong, nonatomic, nullable, readonly) id<KVAAdNetworkProtocol> adNetwork;



@end



#pragma mark - feature App Limit Ad Tracking



#if TARGET_OS_TV
@protocol KVATrackerAppLimitAdTrackingJSExport <JSExport>
@property (readwrite) BOOL appLimitAdTrackingBool NS_SWIFT_NAME(appLimitAdTrackingBool);
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (AppLimitAdTracking_Public) <KVATrackerAppLimitAdTrackingJSExport>
#else
@interface KVATracker (AppLimitAdTracking_Public)
#endif



/*!
 @property appLimitAdTrackingBool
 
 @brief A property which indicates if you want to limit ad tracking at the application level.
 
 @discussion This feature is related to the Limit Ad Tracking feature which is typically found on an Apple device under Settings, Privacy, Advertising.  In the same way that you can limit ad tracking through that setting, this feature provides a second and independent means for the host app to limit ad tracking by asking the user directly.  A value of false (NO) from either this feature or Apple's will result in the limiting of ad tracking.
 */
@property (readwrite) BOOL appLimitAdTrackingBool NS_SWIFT_NAME(appLimitAdTrackingBool);



@end



#pragma mark - feature App Tracking Transparency



@class KVAAppTrackingTransparency;



#if TARGET_OS_TV
@protocol KVATrackerAppTrackingTransparencyJSExport <JSExport>
@property (strong, nonatomic, nonnull, readonly) KVAAppTrackingTransparency *appTrackingTransparency;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (AppTrackingTransparency_Public) <KVATrackerAppTrackingTransparencyJSExport>
#else
@interface KVATracker (AppTrackingTransparency_Public)
#endif



/*!
 @property appTrackingTransparency
 
 @brief An instance of class KVAAppTrackingTransparency.
 */
@property (strong, nonatomic, nonnull, readonly) KVAAppTrackingTransparency *appTrackingTransparency;



@end



#pragma mark - feature Attribution



@class KVAAttribution;



#if TARGET_OS_TV
@protocol KVATrackerAttributionJSExport <JSExport>
@property (strong, nonatomic, nonnull, readonly) KVAAttribution *attribution;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (Attribution_Public) <KVATrackerAttributionJSExport>
#else
@interface KVATracker (Attribution_Public)
#endif



/*!
 @property attribution
 
 @brief An instance of class KVAAttribution.
 */
@property (strong, nonatomic, nonnull, readonly) KVAAttribution *attribution;



@end



#pragma mark - feature Consent



@class KVAConsent;



#if TARGET_OS_TV
@protocol KVATrackerConsentJSExport <JSExport>
@property (strong, nonatomic, nonnull, readonly) KVAConsent *consent;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (Consent_Public) <KVATrackerConsentJSExport>
#else
@interface KVATracker (Consent_Public)
#endif



/*!
 @property consent
 
 @brief An instance of class KVAConsent.
 
 @discussion Data sharing privacy laws such as GDPR require consent to be obtained before certain kinds of personal data may be collected or calculated, kept in memory, persisted or retained in persistent storage, and/or shared with partners.  During the natural lifecycle, there are times where partners may be added and cause the consent status to fall back to an unknown state.  Later the user may again be prompted and the consent status may (or may not) again come to be known.  All of this is predicated upon whether or not consent is required, which is governed by a variety of factors such as location.
 */
@property (strong, nonatomic, nonnull, readonly) KVAConsent *consent;



@end



#pragma mark - feature Custom Identifiers



@class KVACustomIdentifiers;



#if TARGET_OS_TV
@protocol KVATrackerCustomIdentifiersJSExport <JSExport>
@property (strong, nonatomic, nonnull, readonly) KVACustomIdentifiers *customIdentifiers;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (CustomIdentifiers_Public) <KVATrackerCustomIdentifiersJSExport>
#else
@interface KVATracker (CustomIdentifiers_Public)
#endif



/*!
 @property customIdentifiers
 
 @brief An instance of class KVACustomIdentifiers.
 */
@property (strong, nonatomic, nonnull, readonly) KVACustomIdentifiers *customIdentifiers;



@end



#pragma mark - feature Deeplinks



@class KVADeeplinks;



@protocol KVADeeplinksProcessorProvider;



#if TARGET_OS_TV
@interface KVATracker (Deeplinks_Public) <KVADeeplinksProcessorProvider>
#else
@interface KVATracker (Deeplinks_Public) <KVADeeplinksProcessorProvider>
#endif



/*!
 @property deeplinks
 
 @brief An instance of class KVADeeplinks.
 */
@property (strong, nonatomic, nonnull, readonly) KVADeeplinks<KVADeeplinksProcessor> *deeplinks;



@end



#pragma mark - feature Device Id



#if TARGET_OS_TV
@protocol KVATrackerDeviceIdJSExport <JSExport>
@property (strong, nonatomic, nullable, readonly) NSString *deviceIdString;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (DeviceId_Public) <KVATrackerDeviceIdJSExport>
#else
@interface KVATracker (DeviceId_Public)
#endif



/*!
 @property - deviceIdString
 
 @brief A property containing the unique device ID that was generated when the tracker was first initialized on the current install.
 */
@property (strong, nonatomic, nullable, readonly) NSString *deviceIdString;



@end



#pragma mark - feature Events



@class KVAEvent;
@class KVAEvents;



@protocol KVAEventSender;
@protocol KVAEventSenderProvider;



#if TARGET_OS_TV
@protocol KVATrackerEventsJSExport <JSExport, KVAEventSenderProvider>
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (Events_Public) <KVAEventSenderProvider, KVATrackerEventsJSExport>
#else
@interface KVATracker (Events_Public) <KVAEventSenderProvider>
#endif



/*!
@property events

@brief A property which conforms to protocol KVAEventSender.
*/
@property (strong, nonatomic, nonnull, readonly) KVAEvents<KVAEventSender> *events;



@end



#pragma mark - feature Identity Link



@class KVAIdentityLink;



#if TARGET_OS_TV
@interface KVATracker (IdentityLink_Public)
#else
@interface KVATracker (IdentityLink_Public)
#endif



/*!
 @property identityLink
 
 @brief An instance of class KVAIdentityLink.
 */
@property (strong, nonatomic, nonnull, readonly) KVAIdentityLink *identityLink;



@end
#if TARGET_OS_IOS



#pragma mark - feature Location Services



@protocol KVALocationServicesProtocol;




@interface KVATracker (LocationServices_Public)


/*!
 @property locationServices
 
 @brief An instance of class KVALocationServices.
 
 @discussion A controller for working with location services.
 */
@property (strong, nonatomic, nullable, readonly) id<KVALocationServicesProtocol> locationServices;



@end
#endif



#pragma mark - feature Privacy



@class KVAPrivacy;



@protocol KVAPrivacyProfileRegistrar;
@protocol KVAPrivacyProfileRegistrarProvider;



#if TARGET_OS_TV
@protocol KVATrackerPrivacyJSExport <JSExport>
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (Privacy_Public) <KVAPrivacyProfileRegistrarProvider, KVATrackerPrivacyJSExport>
#else
@interface KVATracker (Privacy_Public) <KVAPrivacyProfileRegistrarProvider>
#endif



/*!
 @property privacy
 
 @brief An instance of class KVAPrivacy.
 */
@property (strong, nonatomic, nonnull, readonly) KVAPrivacy<KVAPrivacyProfileRegistrar> *privacy;



@end



#pragma mark - feature Push Notifications



@class KVAPushNotifications;



@protocol KVAPushNotificationsTokenAdder;
@protocol KVAPushNotificationsTokenRemover;
@protocol KVAPushNotificationsTokenAdderRemoverProvider;



#if TARGET_OS_TV
@protocol KVATrackerPushNotificationsJSExport <JSExport>
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (PushNotifications_Public) <KVAPushNotificationsTokenAdderRemoverProvider, KVATrackerPushNotificationsJSExport>
#else
@interface KVATracker (PushNotifications_Public) <KVAPushNotificationsTokenAdderRemoverProvider>
#endif



/*!
 @property pushNotifications
 
 @brief An instance of class KVAPushNotifications.
 */
@property (strong, nonatomic, nonnull, readonly) KVAPushNotifications<KVAPushNotificationsTokenAdder, KVAPushNotificationsTokenRemover> *pushNotifications;



@end



#pragma mark - feature SDK Version



#if TARGET_OS_TV
@protocol KVATrackerSDKVersionJSExport <JSExport>
- (nullable NSString *)sdkVersionString;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (SDKVersion_Public) <KVATrackerSDKVersionJSExport>
#else
@interface KVATracker (SDKVersion_Public)
#endif



/*!
 @method - sdkVersionString
 
 @brief A method to return the sdk version string.
 
 @discussion The returned value includes the name of the SDK, followed by its semantic version.  When applicable it will be followed by a wrapper SDK version in parentheses.
 */
- (nullable NSString *)sdkVersionString;



@end



#pragma mark - feature Sleep



#if TARGET_OS_TV
@protocol KVATrackerSleepJSExport <JSExport>
@property BOOL sleepBool;
@end
#endif



#if TARGET_OS_TV
@interface KVATracker (Sleep_Public) <KVATrackerSleepJSExport>
#else
@interface KVATracker (Sleep_Public)
#endif



/*!
 @property sleepBool
 
 @brief A boolean which when true causes the tracker to sleep.
 
 @discussion The default is false.  When set to true, this causes tasks to effectively be suspended until this condition is lifted.  While this is set to true, tasks are not lost per-say;  however, if a task may have otherwise occurred multiple times, it may be represented only once once the condition is lifted.
 */
@property BOOL sleepBool;



@end



#endif



