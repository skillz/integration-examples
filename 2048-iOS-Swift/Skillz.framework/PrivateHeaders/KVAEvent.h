//
//  KVAEvent.h
//  KochavaCore
//
//  Created by John Bushnell on 9/13/16.
//  Copyright © 2017 - 2021 Kochava, Inc.  All rights reserved.
//



#ifndef KVAEvent_h
#define KVAEvent_h



#pragma mark - IMPORT



#if TARGET_OS_TV
#import <JavaScriptCore/JavaScriptCore.h>
#endif

#ifdef KOCHAVA_FRAMEWORK
#import <KochavaCore/KVAAsForContextObjectProtocol.h>
#import <KochavaCore/KVAFromObjectProtocol.h>
#else
#import "KVAAsForContextObjectProtocol.h"
#import "KVAFromObjectProtocol.h"
#endif



#pragma mark - CLASS



@class KVAEvent;
@class KVAConsent;
@class KVAEventType;



#pragma mark - PROTOCOL



@protocol KVAEventSenderProvider;
@protocol KVADispatchQueueDelegate;



#if TARGET_OS_TV
@protocol KVAEventJSExport <JSExport>
@property (strong, nonatomic, nullable) NSString *actionString;
@property (strong, nonatomic, nullable) NSString *adCampaignIdString;
@property (strong, nonatomic, nullable) NSString *adCampaignNameString;
@property (strong, nonatomic, nullable) NSString *adDeviceTypeString;
@property (strong, nonatomic, nullable) NSString *adGroupIdString;
@property (strong, nonatomic, nullable) NSString *adGroupNameString;
@property (strong, nonatomic, nullable) NSString *adMediationNameString;
@property (strong, nonatomic, nullable) NSString *adNetworkNameString;
@property (strong, nonatomic, nullable) NSString *adPlacementString;
@property (strong, nonatomic, nullable) NSString *adSizeString;
@property (strong, nonatomic, nullable) NSString *adTypeString;
@property BOOL appleWatchBool;
@property (strong, nonatomic, nullable) NSString *appleWatchIdString;
@property (strong, nonatomic, nullable) NSString *appStoreReceiptBase64EncodedString;
@property (strong, nonatomic, nullable) NSNumber *backgroundBoolNumber;
@property (strong, nonatomic, nullable) NSString *checkoutAsGuestString;
@property (strong, nonatomic, nullable) NSNumber *completedBoolNumber;
@property (copy, nonatomic, nullable) KVAConsent *consent;
@property (strong, nonatomic, nullable) NSString *contentIdString;
@property (strong, nonatomic, nullable) NSString *contentTypeString;
@property (strong, nonatomic, nullable) NSString *currencyString;
@property (strong, nonatomic, nullable) NSString *customEventNameString;
@property (strong, nonatomic, nullable) NSDate *date;
@property (strong, nonatomic, nullable) NSString *dateString;
@property (strong, nonatomic, nullable) NSString *descriptionString;
@property (strong, nonatomic, nullable) NSString *destinationString;
@property (strong, nonatomic, nullable) NSNumber *durationTimeIntervalNumber;
@property (strong, nonatomic, nullable) NSDate *endDate;
@property (strong, nonatomic, nullable) NSString *endDateString;
@property (strong, nonatomic, nullable, readonly) KVAEventType *eventType;
@property (strong, nonatomic, nullable) NSDictionary *infoDictionary;
@property (strong, nonatomic, nullable) NSString *infoString;
@property (strong, nonatomic, nullable) NSString *itemAddedFromString;
@property (strong, nonatomic, nullable) NSString *levelString;
@property (strong, nonatomic, nullable) NSNumber *maxRatingValueDoubleNumber;
@property (strong, nonatomic, nullable) NSString *nameString;
@property (strong, nonatomic, nullable) NSString *orderIdString;
@property (strong, nonatomic, nullable) NSString *originString;
@property (strong, nonatomic, nullable) NSDictionary *payloadDictionary;
@property (strong, nonatomic, nullable) NSDecimalNumber *priceDecimalNumber;
@property (strong, nonatomic, nullable) NSNumber *priceDoubleNumber;
@property (strong, nonatomic, nullable) NSNumber *quantityDoubleNumber;
@property (strong, nonatomic, nullable) NSNumber *ratingValueDoubleNumber;
@property (strong, nonatomic, nullable) NSString *receiptIdString;
@property (strong, nonatomic, nullable) NSString *referralFromString;
@property (strong, nonatomic, nullable) NSString *registrationMethodString;
@property (strong, nonatomic, nullable) NSString *resultsString;
@property (strong, nonatomic, nullable) NSString *scoreString;
@property (strong, nonatomic, nullable) NSString *searchTermString;
@property (strong, nonatomic, nullable) NSString *sourceString;
@property (strong, nonatomic, nullable) NSNumber *spatialXDoubleNumber;
@property (strong, nonatomic, nullable) NSNumber *spatialYDoubleNumber;
@property (strong, nonatomic, nullable) NSNumber *spatialZDoubleNumber;
@property (strong, nonatomic, nullable) NSDate *startDate;
@property (strong, nonatomic, nullable) NSString *startDateString;
@property (strong, nonatomic, nullable) NSString *successString;
@property (strong, nonatomic, nullable) NSString *uriString;
@property (strong, nonatomic, nullable) NSString *userIdString;
@property (strong, nonatomic, nullable) NSString *userNameString;
@property (strong, nonatomic, nullable) NSString *validatedString;
+ (nullable instancetype)eventWithTypeNameString:(nonnull NSString *)eventTypeNameString NS_SWIFT_NAME(init(typeNameString:));
@end
#endif



#pragma mark - INTERFACE



/*!
 @class KVAEvent
 
 @brief The class KVAEvent provides a means of defining a post-install event, providing standardized parameters.
 
 @discussion Sending post-install events is not a requirement.  To track installation information, you do not need to do anything more than call the constructor for the tracker.  Still, many advertisers want to understand and correlate the relationship between conversion and attribution source information with post-install behaviors.  This can only be done by tracking post-install events.
 
 Once the tracker has been configured, Kochava event tracking methods can be called from anywhere within the application.  Events will be coupled with the information sent by the tracker to report events, based on user device and application information.  Events are not sent immediately to Kochava servers but queued, should the device not have connectivity.
 
 The KVAEvent class defines an individual post-install event, providing a variety of standardized event names and parameters.  Standard event names may represent a purchase, the completion of a level, an achievement, etc.  Standard parameters may represent a currency, price, level, etc.
 
 The use of standard event names and parameters maximizes parity with external analytics partners and ensures optimal reporting and analytical output via the Kochava platform.  If the data you are sending through post-install events can be sent using standard event names and parameters we recommend you use an instance of class KVAEvent to do so.
 
 Inherits from: NSObject
 
 @author John Bushnell
 
 @copyright 2017 - 2021 Kochava, Inc.
 */
@interface KVAEvent : NSObject
<
#if TARGET_OS_TV
KVAEventJSExport,
#endif
KVAAsForContextObjectProtocol,
KVAFromObjectProtocol
>



#pragma mark - CONSTRUCTION



/*!
 @method + eventWithType:
 
 @brief Creates an instance of class KVAEvent.
 
 @param eventType An event type.
 
 @discussion The designated initializer.
 */
+ (nonnull instancetype)eventWithType:(nonnull KVAEventType *)eventType NS_SWIFT_NAME(init(type:));



#pragma mark - PARAMETERS



/*!
 @property actionString
 
 @brief A property containing an action string.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *actionString;



/*!
 @property adCampaignIdString
 
 @brief A property containing an ad campaign identifier string.
 
 @discussion This is expected to contain a string corresponding to an ad campaign identifier.
 */
@property (strong, nonatomic, nullable) NSString *adCampaignIdString;



/*!
 @property adCampaignNameString
 
 @brief A property containing an ad campaign name string.
 
 @discussion This is expected to contain a string corresponding to an ad campaign name.
 */
@property (strong, nonatomic, nullable) NSString *adCampaignNameString;



/*!
 @property adDeviceTypeString
 
 @brief A property containing an ad device type string.
 
 @discussion This is expected to contain a string corresponding to a device type.
 */
@property (strong, nonatomic, nullable) NSString *adDeviceTypeString;



/*!
 @property adGroupIdString
 
 @brief A property containing an ad group identifier string.
 
 @discussion This is expected to contain a string corresponding to an ad group identifier.
 */
@property (strong, nonatomic, nullable) NSString *adGroupIdString;



/*!
 @property adGroupNameString
 
 @brief A property containing an ad group name string.
 
 @discussion This is expected to contain a string corresponding to an ad group name.
 */
@property (strong, nonatomic, nullable) NSString *adGroupNameString;



/*!
 @property adMediationNameString
 
 @brief A property containing an ad mediation network name string.
 
 @discussion This is expected to contain a standardized string corresponding to an advertising network.  It is the name of the network that was used during mediation.
 */
@property (strong, nonatomic, nullable) NSString *adMediationNameString;



/*!
 @property adNetworkNameString
 
 @brief A property containing an ad network name string.
 
 @discussion This is expected to contain a standardized string corresponding to an advertising network.
 */
@property (strong, nonatomic, nullable) NSString *adNetworkNameString;



/*!
 @property adPlacementString
 
 @brief A property containing an ad placement string.
 
 @discussion This is expected to contain a string corresponding to an ad placement, ad unit, etc.
 */
@property (strong, nonatomic, nullable) NSString *adPlacementString;



/*!
 @property adSizeString
 
 @brief A property containing an ad size string.
 
 @discussion This is expected to contain a string corresponding to an ad size.
 */
@property (strong, nonatomic, nullable) NSString *adSizeString;



/*!
 @property adTypeString
 
 @brief A property containing an ad type string.
 
 @discussion This is expected to contain a string corresponding to an ad type.
 */
@property (strong, nonatomic, nullable) NSString *adTypeString;



/*!
 @property appleWatchBool
 
 @brief A boolean indicating that this event originated from an Apple Watch.
 */
@property BOOL appleWatchBool;



/*!
 @property appleWatchIdString
 
 @brief A string containing a unique identifier associated with the Apple Watch from which this event originated.
 
 @discussion Optional.  You may set this property in addition to appleWatchBool if you have a unique identifier associated with the watch.
 */
@property (strong, nonatomic, nullable) NSString *appleWatchIdString;



/*!
 @property appStoreReceiptBase64EncodedString
 
 @brief A property containing an App Store receipt which has been converted into a base64 encoded string.
 
 @discussion The format of the information is expected to be that which is provided by the main bundle's appStoreReceiptURL method, the data of which being loaded and base-64-string-encoded.
 */
@property (strong, nonatomic, nullable) NSString *appStoreReceiptBase64EncodedString;



/*!
 @property backgroundBoolNumber
 
 @brief A property containing a boolean wrapped in an NSNumber which indicates that something is background.
 
 @discussion This is expected to contain a boolean which indicates if something is background, or occurred while in the background.  This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSNumber *backgroundBoolNumber;



/*!
 @property checkoutAsGuestString
 
 @brief A property indicating whether a checkout took place as a guest.
 
 @discussion This is generally taken to be a boolean, but it is passed as a string so that you can provide more than two states.  Suggested values are "true" and "false", but can also be values such as "yes", "no", or "partial".
 */
@property (strong, nonatomic, nullable) NSString *checkoutAsGuestString;



/*!
 @property completedBoolNumber
 
 @brief A property containing a boolean wrapped in an NSNumber which indicates that something is completed.
 
 @discussion This is expected to contain a boolean which indicates if something is completed.  This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSNumber *completedBoolNumber;



/*!
 @property consent
 
 @brief An instance of KVAConsent.
 */
@property (copy, nonatomic, nullable) KVAConsent *consent;



/*!
 @property contentIdString
 
 @brief A property containing a content identifier string.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *contentIdString;



/*!
 @property contentTypeString
 
 @brief A property containing a content type string.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *contentTypeString;



/*!
 @property currencyString
 
 @brief A property containing a currency type string.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *currencyString;



/*!
 @property customEventNameString
 
 @brief A property containing a custom event name string.
 
 @discussion Standardized event names are automatically determined from the event type enum.  If an appropriate event type is not present, you may set the event type to KVAEventType.custom and the customEventNameString property to your custom event name string.  Event names do not need to be pre-registered.  They can be any alphanumeric string value which does not begin with an underscore.  NOTE: Prepending an event name with an underscore is a convention reserved for Kochava system events. (i.e. _INSTALL)
 */
@property (strong, nonatomic, nullable) NSString *customEventNameString;



/*!
 @property date
 
 @brief A property containing a date.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSDate *date;



/*!
 @property dateString
 
 @brief A property containing a date string.  "dateString" can be used as an alternate to "date" when a specific date format is desired.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *dateString;



/*!
 @property descriptionString
 
 @brief A property containing a description.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *descriptionString;



/*!
 @property destinationString
 
 @brief A property containing a destination.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *destinationString;



/*!
 @property durationTimeIntervalNumber
 
 @brief A property that contains a duration.  It is a time interval that is wrapped in an NSNumber.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSNumber *durationTimeIntervalNumber;



/*!
 @property endDate
 
 @brief A property that contains an end date.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSDate *endDate;



/*!
 @property endDateString
 
 @brief A property that contains an end date.  "endDateString" can be used as an alternate to "endDate" when a specific date format is desired.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *endDateString;



/*!
 @property eventType
 
 @brief An event type.
 
 @discussion Readonly.  This value may be set when an event is constucted.
 */
@property (strong, nonatomic, nullable, readonly) KVAEventType *eventType;



/*!
 @property infoDictionary
 
 @brief A property containing an informational dictionary of key/value pairs.
 
 @discussion A information dictionary.  The keys and values can be any alphanumeric string value.  This field has an entirely generic quality, in that it can contain whatever you consider to be fitting value.  The dictionary should not contain sub-dictionaries.
 */
@property (strong, nonatomic, nullable) NSDictionary *infoDictionary;



/*!
 @property infoString
 
 @brief A property containing an informational string.
 
 @discussion A informational string.  This can be any alphanumeric string value.  This field has an entirely generic quality, in that it can contain whatever you consider to be fitting value.
 
 If the string passed is all numeric (and may include a decimal point), Kochava will automatically sum the amounts passed for the same nameString.  For example, if you sent the purchase amount of in-app purchases in infoString and named nameString “IAP – Purchase Price”, Kochava would add up all the purchase amounts for you and present them as a total for “IAP – Purchase Price”.  Regardless of what is passed in infoString, Kochava’s user dashboard will let you access all the data passed in infoString for any nameString, and present a count of all times an event was sent passing any given nameString.
 
 NOTE: If you pass a string of JSON represented data, only the root level (no nested chunks) is stored.  Also, a limit of 75 characters is applied for a non-JSON-decodable string passed as an event infoString.
 */
@property (strong, nonatomic, nullable) NSString *infoString;



/*!
 @property itemAddedFromString
 
 @brief A property that contains an indication of where an item as added from.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *itemAddedFromString;



/*!
 @property levelString
 
 @brief A property that contains a level.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *levelString;



/*!
 @property maxRatingValueDoubleNumber
 
 @brief A property that contains a maximum rating value.  It is a double that is wrapped in an NSNumber.  This property is used in conjunction with ratingValueDoubleNumber.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSNumber *maxRatingValueDoubleNumber;



/*!
 @property nameString
 
 @brief A property that contains a name.  It can be a human name or the name of any other item type.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.  It can be a person's name or the name of any other object.
 */
@property (strong, nonatomic, nullable) NSString *nameString;



/*!
 @property orderIdString
 
 @brief A property that contains an order id.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *orderIdString;



/*!
 @property originString
 
 @brief A property that contains an origin.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *originString;



/*!
 @property payloadDictionary
 
 @brief A property that contains a payload in the form of a dictionary.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSDictionary *payloadDictionary;



/*!
 @property priceDecimalNumber
 
 @brief A property that contains a price.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.  Because it uses an NSDecimalNumber, it is better suited for preserving decimal precision than priceDoubleNumber.  priceDecimalNumber and priceDoubleNumber share the same key when sent to the server.  If both are set, the value within priceDecimalNumber will win.
 */
@property (strong, nonatomic, nullable) NSDecimalNumber *priceDecimalNumber;



/*!
 @property priceDoubleNumber
 
 @brief A property that contains a price.  It is a double that is wrapped in an NSNumber.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.  In order to better preserve decimal precision, see priceDecimalNumber.  priceDecimalNumber and priceDoubleNumber share the same key when sent to the server.  If both are set, the value within priceDecimalNumber will win.
 */
@property (strong, nonatomic, nullable) NSNumber *priceDoubleNumber;



/*!
 @property quantityDoubleNumber
 
 @brief A property that contains a quantity.  It is a double that is wrapped in an NSNumber.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSNumber *quantityDoubleNumber;



/*!
 @property ratingValueDoubleNumber
 
 @brief A property that contains a rating value.  It is a double that is wrapped in an NSNumber.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSNumber *ratingValueDoubleNumber;



/*!
 @property receiptIdString
 
 @brief A property that contains a receipt id.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *receiptIdString;



/*!
 @property referralFromString
 
 @brief A property that contains where something was referred from.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *referralFromString;



/*!
 @property registrationMethodString
 
 @brief A property that contains a registration method.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *registrationMethodString;



/*!
 @property resultsString
 
 @brief A property that contains results.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *resultsString;



/*!
 @property scoreString
 
 @brief A property that contains a score.  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *scoreString;



/*!
 @property searchTermString
 
 @brief A property that contains a search term (or terms).  It is a string that can contain any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *searchTermString;



/*!
 @property serviceLocallyBool
 
 @brief A boolean which when set will cause the event to be serviced locally when sent.
 
 @discussion When serviced locally the response for the request is provided locally and the request is post the event to the server is never made.  In every other respect the effects are the same.  AdNetwork conversion information is still calculated.  
 */
@property BOOL serviceLocallyBool;



/*!
 @property sourceString
 
 @brief A property that contains a source string.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *sourceString;



/*!
 @property spatialXDoubleNumber
 
 @brief A property that contains a spatial coordinate’s x value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.  One such application is to record the occurrence of events in 3D Gamespace.
 */
@property (strong, nonatomic, nullable) NSNumber *spatialXDoubleNumber;



/*!
 @property spatialYDoubleNumber
 
 @brief A property that contains a spatial coordinate’s y value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.  One such application is to record the occurrence of events in 3D Gamespace.
 */
@property (strong, nonatomic, nullable) NSNumber *spatialYDoubleNumber;



/*!
 @property spatialZDoubleNumber
 
 @brief A property that contains a spatial coordinate’s z value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.  One such application is to record the occurrence of events in 3D Gamespace.
 */
@property (strong, nonatomic, nullable) NSNumber *spatialZDoubleNumber;



/*!
 @property startDate
 
 @brief A property that contains a start date.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSDate *startDate;



/*!
 @property startDateString
 
 @brief A property that contains a start date.  "startDateString" is a string that can be used as an alternate to "startDate" when a specific display format is desired.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *startDateString;



/*!
 @property successString
 
 @brief A property that contains a success string.  It is a string that can be any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *successString;



/*!
 @property userIdString
 
 @brief A property that contains a user id.  It is a string that can be any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *userIdString;



/*!
 @property uriString
 
 @brief A property that contains a URI (or URL) string.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *uriString;



/*!
 @property userNameString
 
 @brief A property that contains a username.  This is intended to be used to store an account-syle username, as opposed to a user's name (compare "nameString").  It is a string that can be any alphanumeric value.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *userNameString;



/*!
 @property validatedString
 
 @brief A property that contains a validated string.  It is a string that is intended to contain a boolean-like value, such as "true" or "false".  It can also contain other  custom values such as "partial".
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
 */
@property (strong, nonatomic, nullable) NSString *validatedString;



#pragma mark - GENERAL



/*!
 @method - eventNameString
 
 @brief Internal.  A method that returns a string representation of the name of the event.  Marked internal beginning in v3.5.1.  This method is scheduled to become internal in v4.0 of this SDK.
 */
- (nonnull NSString *)eventNameString;



/*!
 @method - send
 
 @brief Sends the event using the default KVAEventSenderProvider.
 
 @discussion The default is the shared instance of class KVATracker.
 */
- (void)send;



/*!
 @method - sendWithSenderArray:
 
 @brief Sends the event with a specifed array of KVAEventSenderProvider(s).
 */
- (void)sendWithSenderArray:(nullable NSArray<KVAEventSenderProvider,  KVADispatchQueueDelegate> *)senderArray;



@end



#endif



