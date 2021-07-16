//
//  KochavaEvent.h
//  KochavaTracker
//
//  Created by John Bushnell on 9/13/16.
//  Copyright © 2017 Kochava, Inc. All rights reserved.
//



#if WHTLBL_REVEAL_TARGET == 1

#warning KochavaEvent.h: libKochavaTrackeriOS

#endif



#pragma mark - DEFINE



#define WHTLBLEventTypeEnum WHTLBL_CLASS(EventTypeEnum)

#define WHTLBLEventTypeEnumUndefined WHTLBL_CLASS(EventTypeEnumUndefined)

#define WHTLBLEventTypeEnumAddToCart WHTLBL_CLASS(EventTypeEnumAddToCart)

#define WHTLBLEventTypeEnumAddToWishList WHTLBL_CLASS(EventTypeEnumAddToWishList)

#define WHTLBLEventTypeEnumAchievement WHTLBL_CLASS(EventTypeEnumAchievement)

#define WHTLBLEventTypeEnumCheckoutStart WHTLBL_CLASS(EventTypeEnumCheckoutStart)

#define WHTLBLEventTypeEnumCustom WHTLBL_CLASS(EventTypeEnumCustom)

#define WHTLBLEventTypeEnumLevelComplete WHTLBL_CLASS(EventTypeEnumLevelComplete)

#define WHTLBLEventTypeEnumPurchase WHTLBL_CLASS(EventTypeEnumPurchase)

#define WHTLBLEventTypeEnumRating WHTLBL_CLASS(EventTypeEnumRating)

#define WHTLBLEventTypeEnumRegistrationComplete WHTLBL_CLASS(EventTypeEnumRegistrationComplete)

#define WHTLBLEventTypeEnumSearch WHTLBL_CLASS(EventTypeEnumSearch)

#define WHTLBLEventTypeEnumTutorialComplete WHTLBL_CLASS(EventTypeEnumTutorialComplete)

#define WHTLBLEventTypeEnumView WHTLBL_CLASS(EventTypeEnumView)

#define WHTLBLEventTypeEnumAdView WHTLBL_CLASS(EventTypeEnumAdView)

#define WHTLBLEventTypeEnumPushReceived WHTLBL_CLASS(EventTypeEnumPushReceived)

#define WHTLBLEventTypeEnumPushOpened WHTLBL_CLASS(EventTypeEnumPushOpened)



#define WHTLBLEvent WHTLBL_CLASS(Event)



#pragma mark - INTERFACE



/*!
 @class KochavaEvent
 
 @brief A class that stores standardized parameters for an event.
 
 @discussion This class is used to store and pass standardized parameters when sending a post-install event to the server.  The proper use of this class is to instantiate an object using the designated initializer, and then to assign values to each property that you wish to send.  You may then pass this object as a parameter to sendEvent.
 
 Inherits from: NSObject
 
 @author John Bushnell
 
 @copyright 2017 Kochava, Inc.
 */
@interface KochavaEvent : NSObject



#pragma mark - NS_ENUM
#pragma mark KochavaEventTypeEnum



typedef NS_ENUM(NSUInteger, KochavaEventTypeEnum)
{
    /*!
     @brief Undefined
     
     @discussion This is an enumerated value which signifies that an the event type has not been defined.
     */
    KochavaEventTypeEnumUndefined = 0,
    
    
    
    /*! 
     @brief Add to Cart
     
     @discussion This is an enumerated value which signifies that an item was added to a cart.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumAddToCart = 100,
    
    
    
    /*! 
     @brief Add to Wish List
     
     @discussion This is an enumerated value which signifies that an item was added to a wish list.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumAddToWishList = 101,
    
    
    
    /*! 
     @brief Achievement
     
     @discussion This is an enumerated value which signifies that an achievement was achieved.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumAchievement = 102,
    
    
    
    /*! 
     @brief Checkout Start
     
     @discussion This is an enumerated value which signifies that a checkout was started.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumCheckoutStart = 103,
    
    
    
    /*!
     @brief Custom
     
     @discussion This is an enumerated value which signifies that a customEventNameString will be supplied.
     */
    KochavaEventTypeEnumCustom = 1,
    
    
    
    /*!
     @brief Level Complete
     
     @discussion This is an enumerated value which signifies that a level was completed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumLevelComplete = 104,
    
    
    
    /*! 
     @brief Purchase
     
     @discussion This is an enumerated value which signifies that a purchase was completed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumPurchase = 105,
    
    
    
    /*! 
     @brief Rating
     
     @discussion This is an enumerated value which signifies that an item was rated.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumRating = 106,
    
    
    
    /*! 
     @brief Registration Complete
     
     @discussion This is an enumerated value which signifies that a registration was completed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumRegistrationComplete = 107,

    
    
    /*! 
     @brief Search
     
     @discussion This is an enumerated value which signifies that a search was performed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumSearch = 108,
    
    
    
    /*! 
     @brief Tutorial Complete
     
     @discussion This is an enumerated value which signifies that a tutorial was completed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumTutorialComplete = 109,

    
    
    /*! 
     @brief View
     
     @discussion This is an enumerated value which signifies that an item was viewed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumView = 110,
    
    
    
    /*!
     @brief Ad View
     
     @discussion This is an enumerated value which signifies that an ad was viewed.  You may use this in any equivalent circumstance.
     */
    KochavaEventTypeEnumAdView = 111,
    
    
    
    /*!
     @brief Push Received
     
     @discussion This is an enumerated value which signifies that a push notification was received.
     */
    KochavaEventTypeEnumPushReceived = 112,
    
    
    
    /*!
     @brief Push Opened
     
     @discussion This is an enumerated value which signifies that a push notification was opened.
     */
    KochavaEventTypeEnumPushOpened = 113,
};



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
 
 @discussion Standardized event names are automatically determined from the event type enum.  If an appropriate event type is not present, you may set the event type to KochavaEventTypeEnumCustom and the customEventNameString property to your custom event name string.  Event names do not need to be pre-registered.  They can be any alphanumeric string value which does not begin with an underscore.  NOTE: Prepending an event name with an underscore is a convention reserved for Kochava system events. (i.e. _INSTALL)
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
 @property priceDoubleNumber
 
 @brief A property that contains a price.  It is a double that is wrapped in an NSNumber.
 
 @discussion This field has a somewhat generic quality, in that it can contain whatever you consider to be fitting value.
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



#pragma mark - METHODS



/*!
 @method - serverEventNameString
 
 @brief A method that returns the event name which is suitable to send to the server (internal).
 */
- (nonnull NSString *)serverEventNameString;



/*!
 @method - serverEventInfoObject
 
 @brief A method that returns an info object which is suitable to send to the server (internal).
 */
- (nullable NSObject *)serverEventInfoObject;



#pragma mark - CLASS METHODS



/*!
 @method + eventWithEventTypeEnum:
 
 @brief Create a KochavaEvent object.
 
 @discussion The designated initializer.
 */
+ (nullable instancetype)eventWithEventTypeEnum:(KochavaEventTypeEnum)eventTypeEnum;



@end



