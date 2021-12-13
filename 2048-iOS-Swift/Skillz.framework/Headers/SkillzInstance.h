//
//  SkillzInstance.h
//  SkillzSDK-iOS
//
//  Copyright (c) 2014 Skillz. All rights reserved.
//  http://skillz.com/
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

@class SKZMatchInfo;
@class SKZPlayer;

/**
 * Skillz requires explicitly linking with the following Frameworks:
 *  1. libz
 *  2. libsqlite3
 *  3. libxml2
 *
 * Skillz requires enabling "Modules" (this auto-links all the required Frameworks):
 *
 *  Under "Apple LLVM 5.0 - Languages - Modules" in Build Settings, set
 *   - Enable Modules (C and Objective-C): YES
 *   - Link Frameworks Automatically: YES
 *
 * Skillz also requires the following linker flags:
 *  1. -ObjC
 *  2. -lc++
 *  3. -lz
 */


#pragma mark - Skillz SDK Enums

/**
 * NS_ENUM defining the different servers that Skillz can connect to.
 */
typedef NS_ENUM (NSInteger, SkillzEnvironment) {
    /** Used when connecting to the live production server. */
    SkillzProduction,
    /** Used when connecting to the test sandbox server. */
    SkillzSandbox
};

/**
 * NS_ENUM defining the orientations that Skillz can be launched in.
 */
typedef NS_ENUM (NSInteger, SkillzOrientation) {
    /** Used to launch Skillz in a portrait orientation. */
    SkillzPortrait,
    /** Used to launch Skillz in a landscape orientation. This will match the landscape orientation of your game. */
    SkillzLandscape
};


///----------------------------------------------------
///@name Skillz SDK Delegate Protocol
///----------------------------------------------------


#pragma mark - Skillz SDK Delegate Protocol

/**
 * SkillzBaseDelegate is the protocol used by applications to interface with Skillz.
 *
 * Do NOT define an object as conforming to SkillzBaseDelegate, rather your delegate should conform to SkillzDelegate
 *
 * Required methods in this protocol should be implemented in your application for basic Skillz functionality.
 */
@protocol SkillzBaseDelegate <NSObject>

@optional
/**
 * This method will be called when the Skillz SDK will exit. It will NOT be called when a Skillz Tournament is launched.
 */
- (void)skillzWillExit;

/**
 * This method will be called before the Skillz UI launches. Use this to clean up any state needed before you launch Skillz.
 */
- (void)skillzWillLaunch;

/**
 * This method will be called once the Skillz UI has finished displaying. Use this to clean up your view hierarchy.
 */
- (void)skillzHasFinishedLaunching;

@optional
/**
 * This method will be called when the Skillz SDK is launching into the progression room. You should use this method for
 *  constructing and displaying the progression experience for your game.
 */
- (void)onProgressionRoomEnter;

#pragma mark User Engagement

/**
 *  This method will be called when a user has successfully made a cash deposit with Skillz. You may use this method for recording
 *  and tracking deposits.
 *
 *  NOTE: This method will be called in both SANDBOX and PRODUCTION environments.
 *
 *  @param userId          The current logged in player's Skillz id.
 *  @param cashAmount      The amount of cash that the player deposited.
 *  @param bonusCashAmount The amount of bonus cash the player received with their deposit.
 *  @param promoAmount     The amount of cash that the player received from a promotional code.
 *  @param currencyCode    The currency code used for the transaction. (Follows ISO 4217)
 */
- (void)userDidDeposit:(NSString * _Nonnull)userId
            cashAmount:(NSNumber * _Nonnull)cashAmount
       bonusCashAmount:(NSNumber * _Nonnull)bonusCashAmount
           promoAmount:(NSNumber * _Nonnull)promoAmount
          currencyCode:(NSString * _Nonnull)currencyCode;

@end

/**
 * If you plan to support standard Skillz tournaments, your Skillz delegate should conform to this protocol and implement its
 * required methods.
 *
 * NOTE: Your Skillz Delegate may also conform to SkillzTurnBasedDelegate if you plan to support both play types.
 */
@protocol SkillzDelegate <SkillzBaseDelegate>

@required
/**
 * This method will be called when a typical Skillz tournament will launch. You should use this method for constructing a new game
 * based on the supplied argument.
 *
 * @param gameParameters dictionary contains the Game Parameters that were configured in the Skillz Developer Portal
 * @param matchInfo class contain data relevant to the current match
 */
- (void)tournamentWillBegin:(NSDictionary * _Nonnull)gameParameters
              withMatchInfo:(SKZMatchInfo * _Nonnull)matchInfo;

@optional


/**
 *  Deprecated, use tournamentWillBegin:withMatchInfo:
 *
 *  @param gameParameters parameters for your game.
 */
- (void)tournamentWillBegin:(NSDictionary * _Nonnull)gameParameters __attribute__ ((deprecated));

@end

///----------------------------------------------------
///@name Skillz SDK Interface
///----------------------------------------------------

#pragma mark - Skillz SDK Interface

__attribute__ ((visibility("default")))
NS_AVAILABLE_IOS(8_0)

/**
 * Main interface for the Skillz SDK
 */
@interface Skillz : NSObject


///------------------------------
///@name Skillz SDK Properties
///------------------------------

/**
 * Whether or not a Skillz match is currently in progress.
 */
@property (readonly, assign) BOOL tournamentIsInProgress;

/**
 * Whether or not a Skillz match is in progress against a sync gameplay onboarding bot.
 */
@property (readonly, assign) BOOL botMatchIsInProgress;

/**
 * The current SkillzBaseDelegate instance
 */
@property (readonly, strong) id<SkillzBaseDelegate> _Nullable skillzDelegate;


#pragma mark - Skillz SDK Functions

///----------------------------------------------------
///@name Skillz SDK Class Methods
///----------------------------------------------------

/**
 * Get a singleton reference to the Skillz SDK
 *
 * @return The singleton instance of the Skillz SDK object
 */
+ (Skillz * _Nonnull)skillzInstance;

/**
 * Returns a Dictionary of Game Parameters that you set in each tournament in Developer Portal.
 * 
 * You can set or edit these game parameters by clicking Tournaments > Edit in your Developer Portal (https://developers.skillz.com/dashboard)
 *
 * You can use these game parameters to provide a different user experience for each tournament that you have. 
 */
+ (NSDictionary * _Nonnull)getMatchRules;

/**
 * Returns a random integer supplied by the Skillz SDK to ensure fairness across competition games.
 *
 * The range of this function is [0, RAND_MAX) (0 is inclusive, RAND_MAX is exclusive)
 *
 * On iOS, RAND_MAX is 2,147,483,647
 *
 * Players in the same tournament will receive the same sequence of random numbers.
 */
+ (NSInteger)getRandomNumber;

/**
 * Returns a random floating point value supplied by the Skillz SDK to ensure fairness across competition games.
 *
 * The range of this function is [0, 1.0) (0 is inclusive, 1.0 is exclusive)
*
 * Players in the same tournament will receive the same sequence of random numbers.
 */
+ (float)getRandomFloat;

/**
 * Returns a random unsigned integer supplied by the Skillz SDK to ensure fairness across competition games.
 *
 * Number will be in the range [min,max) (min is inclusive, max is exclusive), and the probability will be evenly distributed 
 * amongst all the possible values.
 *
 * [Skillz skillzgetRandomNumberWithMin:2 andMax:10] will return one of the following numbers with equal probability: 
 * 2,3,4,5,6,7,8,9
 *
 * Players in the same tournament will receive the same sequence of random numbers.
 *
 * @param min  The minimum possible value returned, inclusive.
 * @param max  The maximum possible value returned, exclusive.
 */
+ (NSUInteger)getRandomNumberWithMin:(NSUInteger)min
                              andMax:(NSUInteger)max;

///----------------------------------------------------
///@name Skillz SDK Instance Methods
///----------------------------------------------------

/**
 * Initialize Skillz
 *
 * This method needs to be called within application:didFinishLaunchingWithOptions in your App Delegate.
 * Calling this method does not launch the Skillz experience, it only configures the Skillz SDK for your game

    Be sure that your app's Info.plist contains a "skillzSDK" dictionary with the correct configuration.
 
    Info.plist
 
    Info.plist Key      Type           Dictionary Key                 Type
    "skillzSDK"         Dictionary
                                      "environment"                   String  ("sandbox" | "production")
                                      "gameId"                        Number
                                      "allowExit"                     Boolean
                                      "orientation"  [optional]       String ("portrait | "landscape")
 
 * @param delegate         This delegate must implement all required methods of the SkillzBaseDelegate protocol
 *
 */
- (void)initWithDelegate:(id <SkillzBaseDelegate> _Nonnull)delegate;

/**
 * This method needs to be called within application:didFinishLaunchingWithOptions in your App Delegate.
 * Calling this method does not launch the Skillz experience, it only configures the Skillz SDK for your game
 *
 * @param gameId           Your game ID as given to you on the Skillz developer portal
 * @param delegate         This delegate must implement all required methods of the SkillzBaseDelegate protocol
 * @param environment      SkillzSandbox for sandbox testing or SkillzProduction for app store submission.
 * @param allowExit        Whether to allow the user to exit the Skillz experience
 *
 */
- (void)initWithGameId:(NSString * _Nonnull)gameId
           forDelegate:(id <SkillzBaseDelegate> _Nonnull)delegate
       withEnvironment:(SkillzEnvironment)environment
             allowExit:(BOOL)allowExit;

/**
 * Launch the Skillz Experience
 *
 * This function is what will actually render the Skillz experience on screen. This function will draw a ViewController onto your
 * view hierarchy.
 *
 * Because of this, do not call this method while attempting to draw another ViewController on screen as well.
 */
- (void)launchSkillz;

/**
 * If your game has a synchronous gameplay training bot, call this method immediately after calling
 * initWithGameId:forDelegate:withEnvironment:allowExit: with the value YES to inform Skillz that your build has a training bot implemented.
 *
 * Only call this if your game has synchronous gameplay with a custom server and you have implemented a sync training
 * bot to be used in onboarding the player to the Skillz Experience.
 *
 * @param hasSyncBot whether or not your game has implemented a synchronous training bot
 */
- (void)setGameHasSyncBot:(BOOL)hasSyncBot;

/**
 *  Use this method to fetch the match information of the current match.
 *
 *  @return A SKZMatchInfo containing information identifying the match and players within the match.
 */
- (SKZMatchInfo * _Nonnull)getMatchInfo;

/**
 * This method must be called each time the current player's score changes during a Skillz match.
 *
 * For example, in many games this method is called when the player scores points, when the player is penalized, and whenever a
 * time bonus is applied. It is OK for this method to
 * be called very often.
 *
 * If a continuous in-game score is displayed to the player, this method is generally called as often as that score display is
 * updated - usually by placing the updatePlayersCurrentScore call in the same place within the game loop.
 *
 * @param currentScoreForPlayer Current score value for the player
 */
- (void)updatePlayersCurrentScore:(NSNumber * _Nonnull)currentScoreForPlayer;

/**
 * This method must be called each time the current player's score changes during a Skillz match.
 *
 * For example, in many games this method is called when the player scores points, when the player is penalized, and whenever a
 * time bonus is applied. It is OK for this method to
 * be called very often.
 *
 * If a continuous in-game score is displayed to the player, this method is generally called as often as that score display is
 * updated - usually by placing the updatePlayersCurrentScore call in the same place within the game loop.
 *
 * @param currentScoreForPlayer Current score value for the player
 * @param matchId               Numeric value representing the current match's Id
 */
- (void)updatePlayersCurrentScore:(NSNumber * _Nonnull)currentScoreForPlayer
                      withMatchId:(NSNumber * _Nonnull)matchId;

/**
 * Call this function to report the player's score to Skillz. Ends the current tournament, and returns the user to the Skillz
 * experience.
 *
 * @param score            Numeric value representing the player's final score
 * @param completion       Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                         saving game data or removing the game from the view hierarchy.
 *
 * Note: If your game is resource intensive, you should attempt to release as much memory as possible prior to calling this method.
 */
- (void)displayTournamentResultsWithScore:(NSNumber * _Nonnull)score
                           withCompletion:(void (^_Nonnull)(void))completion;

/**
 * Call this function to report the player's score to Skillz. Ends the current tournament, and returns the user to the Skillz
 * experience.
 *
 * @param score            Numeric value representing the player's final score
 * @param matchId          Numeric value representing the current match's Id
 * @param completion       Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                         saving game data or removing the game from the view hierarchy.
 *
 * Note: If your game is resource intensive, you should attempt to release as much memory as possible prior to calling this method.
 */
- (void)displayTournamentResultsWithScore:(NSNumber * _Nonnull)score
                              withMatchId:(NSNumber * _Nonnull)matchId
                           withCompletion:(void (^_Nonnull)(void))completion;

/**
 * Call this function to report the player's score to Skillz if the current match is a synchronous match against a training bot. Ends the
 * current tournament, and returns the user to the Skillz experience.
 *
 * @param playerScore            Numeric value representing the player's final score
 * @param botScore            Numeric value representing the bot's final score
 * @param completion            Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                         saving game data or removing the game from the view hierarchy.
 *
 * Note: If your game is resource intensive, you should attempt to release as much memory as possible prior to calling this method.
 */
- (void)displayResultsForBotMatchWithPlayerScore:(NSNumber * _Nonnull)playerScore
                                        botScore:(NSNumber * _Nonnull)botScore
                                      completion:(void (^_Nonnull)(void))completion;

/**
 * Call this function when a player aborts a Skillz match in progress. Forfeits the match and brings the user back into the Skillz
 * experience.
 *
 * @param completion      Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                        saving game data or removing the game from the view hierarchy.
 *
 * Note: If your game is resource intensive, you should attempt to release as much memory as possible prior to calling this method.
 */
- (void)notifyPlayerAbortWithCompletion:(void (^_Nonnull)(void))completion;

/**
 * Call this function when a player aborts a Skillz match in progress. Forfeits the match and brings the user back into the Skillz
 * experience.
 * @param matchId         Numeric value representing the current match's Id
 * @param completion      Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                        saving game data or removing the game from the view hierarchy.
 *
 * Note: If your game is resource intensive, you should attempt to release as much memory as possible prior to calling this method.
 */
- (void)notifyPlayerAbortWithMatchId:(NSNumber * _Nonnull)matchId
                      WithCompletion:(void (^_Nonnull)(void))completion;

/**
 * Call this function when a player aborts a Skillz match in progress against a synchronous training bot. Forfeits the match and brings
 * the user back into the Skillz experience.
 *
 * @param botScore      Numeric value representing the bot's final score for the aborted match.
 * @param completion      Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                        saving game data or removing the game from the view hierarchy.
 *
 * Note: If your game is resource intensive, you should attempt to release as much memory as possible prior to calling this method.
 */
- (void)notifyPlayerAbortForBotMatchWithBotScore:(NSNumber * _Nullable)botScore
                                      completion:(void (^_Nonnull)(void))completion;

/**
 * Call this function to report the player's score to Skillz. This does not return the user to the Skillz experience. This should be used
 * in conjunction with the returnToSkillz function.
 *
 * @param score            Numeric value representing the player's final score
 * @param successCompletion       Completion will be called after the score is submitted successfully
 * @param failureCompletion       Completion will be called if the score submission fails with an error message
 */
- (void)submitScore:(NSNumber * _Nonnull)score
        withSuccess:(void (^_Nonnull)(void))successCompletion
        withFailure:(void (^_Nonnull)(NSString * _Nonnull errorMessage))failureCompletion;

/**
 * Call this function to end replay capturing after submitting the score to Skillz.
 *
 * This should be used in cases when your game displays a progression screen after the match that is not directly relevant
 * to the player's match. In that case, this method should be called after displaying the score results to the player (if your game
 * does this) but before displaying the progression screen.
 *
 * This function cannot be called before you call one of the submit score or abort game methods, and will return NO if you try.
 *
 * Replays will also be ended automatically when returning to Skillz, so if your game doesn't display a progression screen, you
 * can safely ignore calling this method.
 *
 * @return YES if the replay capture was successfully ended or if no replay was being recorded for this match, NO if replay could
 *         not be ended.
 */
- (BOOL)endReplayRecording;

/**
 * Call this function to return to the Skillz SDK. If the player is returning to Skillz from a match, you must have submitted a score
 * before this function is called.
 *
 * @param completion        Completion will be called on wrap up so that the developer can finish any ongoing processes, such as
 *                        saving game data or removing the game from the view hierarchy.
 * @return A boolean value representing whether or not control can be passed back to the SDK.
 *          A return value of false indicates that a score has not been reported.
 */
- (BOOL)returnToSkillzWithCompletion:(void (^_Nonnull)(void))completion;

/**
 * If your game plays its own background music that you'd like to play in the Skillz UI, set hasBackgroundMusic to YES to prevent 
 * the Skillz music from being played.
 *
 * @param hasBackgroundMusic       Whether or not your game uses background music.
 */
- (void)setGameHasBackgroundMusic:(BOOL)hasBackgroundMusic;

#pragma mark - Sklillz SDK Information

///----------------------------------------------------
///@name Skillz SDK Information
///----------------------------------------------------

/**
 * Get the version of the Skillz SDK
 *
 * @return The SDK Version
 */
+ (NSString * _Nonnull)SDKShortVersion;

/**
 * Display the long version of SDK Info in a UIAlertView
 */
+ (void)showSDKVersionInfo;

/**
 * This will return a value confirming whether or not the Skillz UI is currently presented
 *
 * @return True if Skillz UI is currently presented, otherwise false.
 */
+ (BOOL)isSkillzPresented;

/**
 * This will return the UIInterfaceOrientationMask set for Skillz, typically this is only used by our UnityViewControllerBase.
 *
 * @return UIInterfaceOrientationMask for Skillz
 */
+ (UIInterfaceOrientationMask)skillzOrientation;

/**
 * Get the current logged in player. Use this method if you need this
 * information outside of a tournament.
 *
 * @return SKZPlayer object that represent the current player. If there is no
 * player currently logged in, will return nil.
 */
+ (SKZPlayer * _Nullable)player;


/**
 *  DEPRECATED: Use the player method instead.
 */
+ (NSString * _Nullable)currentUserDisplayName __attribute__ ((deprecated));

/**
 *  Deprecated, use initWithGameId:forDelegate:withEnvironment:allowExit: instead.
 *
 *  @param gameId      Skillz ID for your game
 *  @param delegate    Delegate responsible for handling Skillz protocol call backs
 *  @param environment Environment to point the SDK to (Production or Sandbox)
 */
- (void)initWithGameId:(NSString * _Nonnull)gameId
           forDelegate:(id <SkillzBaseDelegate> _Nonnull)delegate
       withEnvironment:(SkillzEnvironment)environment __attribute__ ((deprecated));

@end

