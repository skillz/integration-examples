//
//  Skillz+Unity.mm
//  SkillzSDK-iOS
//
//  Copyright (c) 2015 Skillz. All rights reserved.
//

#import <CoreImage/CoreImage.h>

#import <Skillz/Skillz.h>
#include <string>
#import <Metal/Metal.h>

#include "UnityInterface.h"
#import "UnityAppController.h"
#import "UnityAppController+Rendering.h"
#import "CVTextureCache.h"
#import "DisplayManager.h"
#import "CoreImage/CoreImage.h"

@class SkillzSDKDelegate;

extern "C" int UnitySelectedRenderingAPI();

@interface UnitySkillzSDKDelegate : NSObject< SkillzDelegate, SkillzSyncDelegate>

@property SkillzOrientation orientation;
@property (nonatomic) NSString *matchRules;
@property (nonatomic) NSString *matchInfo;

/*
 The following are stored C# function pointers for Skillz submit score callback methods
 */
@property (nonatomic) void (* onSubmitScoreSuccessFunc)();
@property (nonatomic) void (* onSubmitScoreFailureFunc)(const char*);

/*
 The following are stored C# function pointers for Skillz Progression callback methods
 */
@property (nonatomic) void (* onProgressionGetSuccessFunc)(int, const char*);
@property (nonatomic) void (* onProgressionGetFailureFunc)(int, const char*);
@property (nonatomic) void (* onProgressionUpdateSuccessFunc)(int);
@property (nonatomic) void (* onProgressionUpdateFailureFunc)(int, const char*);

/*
 The following are stored C# function pointers for SkillzSyncPlayDelegate methods
 */

@property (nonatomic) void (* onCurrentPlayerHasReconnectedFunc)();
@property (nonatomic) void (* onCurrentPlayerHasLostConnectionFunc)();
@property (nonatomic) void (* onCurrentPlayerHasLeftMatchFunc)();

@property (nonatomic) void (* onOpponentHasReconnectedFunc)(uint64_t);
@property (nonatomic) void (* onOpponentHasLostConnectionFunc)(uint64_t);
@property (nonatomic) void (* onOpponentHasLeftMatchFunc)(uint64_t);

@property (nonatomic) void (* didReceiveDataFunc)(const unsigned char *, uint64_t);
@property (nonatomic) void (* onMatchCompletedFunc)();

@end

static void PauseApp()
{
    UnitySetPlayerFocus(0);
    UnityPause(true);
}

static void PauseAppWithDelay()
{
    double delayInSeconds = 1.0;
    dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, (int64_t)(delayInSeconds * NSEC_PER_SEC));
    dispatch_after(popTime, dispatch_get_main_queue(), ^(void){
        PauseApp();
    });
}

static void ResumeApp()
{
    UnityPause(false);
    UnitySetPlayerFocus(1);
    
    [GetAppController().rootView layoutSubviews];
    [GetAppController() applicationDidBecomeActive:[UIApplication sharedApplication]];
}

@interface Skillz (Unity)

+ (void)sendMessageToUnityObject:(NSString *)objName
                   callingMethod:(NSString *)methodName
                withParamMessage:(NSString *)msg;

+ (BOOL)supportsUnityRecording;
+ (CIImage*)getFrame;
@end

@interface SKZRenderDelegate : RenderPluginArrayDelegate

-(id)init;
-(void)startLaunching:(NSNotification*)notification;

@property (nonatomic) BOOL hasMetalRendering;
@property (atomic) CIImage* frameImage;
@property (atomic) BOOL needsFrameImage;

@end

@implementation SKZRenderDelegate

- (id)init
{
    id result = [super init];
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(startLaunching:)
                                                 name:UIApplicationDidFinishLaunchingNotification
                                               object:nil];
    return result;
}

- (CIImage*)createImage
{
    CIImage* image = nil;
    CIImage* frameImage = self.frameImage;
    if (frameImage) {
        image = [[frameImage imageBySettingAlphaOneInExtent:self.frameImage.extent]
                 imageByApplyingOrientation:kCGImagePropertyOrientationDownMirrored];
        self.frameImage = nil;
    }
    self.needsFrameImage = YES;
    return image;
}

-(void)startLaunching:(NSNotification*)notification
{
    UnityAppController* appDelegate = [UIApplication sharedApplication].delegate;
    id renderDelegate = appDelegate.renderDelegate;
    if (UnitySelectedRenderingAPI() == apiMetal) {
        appDelegate.renderDelegate = self;
        if (renderDelegate) {
            self.delegateArray = @[renderDelegate];
        }
        
        self.hasMetalRendering = YES;
    }
}

- (void)onBeforeMainDisplaySurfaceRecreate:(struct RenderingSurfaceParams*)params
{
    [super onBeforeMainDisplaySurfaceRecreate:params];
    if (self.hasMetalRendering) {
        if (params->renderH > 0 && params->renderW > 0) {
            params->useCVTextureCache = YES;
        }
    }
    self.frameImage = nil;
    self.needsFrameImage = false;
}

-(void)onFrameResolved
{
    if (self.needsFrameImage) {
        self.needsFrameImage = NO;
        if (self.frameImage == nil) {
            [UnityCurrentMTLCommandBuffer() addCompletedHandler:^(MTLCommandBufferRef buffer){
                UnityDisplaySurfaceMTL* surface = (UnityDisplaySurfaceMTL*) GetMainDisplaySurface();
                
                if (surface) {
                    id<MTLTexture> metalTexture = GetMetalTextureFromCVTextureCache(surface->cvTextureCacheTexture);
             
                    self.frameImage = [CIImage imageWithMTLTexture:metalTexture options:nil];
                }
            }];
        }
    }
}
@end

static SKZRenderDelegate *sRenderDelgate = [[SKZRenderDelegate alloc] init];

@interface SKZAppDelegate : NSProxy  <UIApplicationDelegate>

@property (strong, nonatomic) id target;

- (void)forwardInvocation:(NSInvocation *)invocation;
- (NSMethodSignature *)methodSignatureForSelector:(SEL)sel;
- (void)installDelegate;
- (void)uninstallDelegate;

@end

@implementation SKZAppDelegate

- (void)forwardInvocation:(NSInvocation *)invocation
{
    [invocation invokeWithTarget:self.target];
}

- (NSMethodSignature *)methodSignatureForSelector:(SEL)sel
{
    return [self.target methodSignatureForSelector:sel];
}

- (void)installDelegate
{
    if ([UIApplication sharedApplication].delegate != self) {
        self.target = [UIApplication sharedApplication].delegate;
        [UIApplication sharedApplication].delegate = self;
    }
}

- (void)uninstallDelegate
{
    if ([UIApplication sharedApplication].delegate == self) {
        [UIApplication sharedApplication].delegate = self.target;
    }
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    if ([[Skillz skillzInstance] tournamentIsInProgress]) {
        [self.target applicationWillEnterForeground:application];
    }
}

- (void)applicationDidBecomeActive:(UIApplication*)application
{
    UnityPause(false);
    
    if ([[Skillz skillzInstance] tournamentIsInProgress]) {
        [self.target applicationDidBecomeActive:application];
    }
}

- (void)applicationWillResignActive:(UIApplication*)application
{
    UnityPause(true);
    
    if ([[Skillz skillzInstance] tournamentIsInProgress]) {
        [self.target applicationWillResignActive:application];
    }
}

@end

static SKZAppDelegate* sSKZAppDelegate;

@implementation UnitySkillzSDKDelegate

NSString *unitySkillzDelegateName = @"SkillzDelegate";
BOOL isReportingScore = false;

- (void)tournamentWillBegin:(NSDictionary *)gameParameters
              withMatchInfo:(SKZMatchInfo *)matchInfo
{
    dispatch_async(dispatch_get_main_queue(), ^{
        ResumeApp();
        isReportingScore = false;
        // Send message to Unity object to call C# method
        // SkillzDelegate.skillzTournamentWillBegin, implemented by publisher
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wundeclared-selector"
        NSString *json = [matchInfo performSelector:@selector(JSONStringRepresentation:)
                                         withObject:gameParameters];
#pragma GCC diagnostic pop
        
        [self updateCurrentMatchParams:gameParameters];
        
        // There are two sendMessageToUnityObject calls here, one of them will silently fail and should not cause a crash.
        // This sends a message to call the Unity iOS only skillzTournamentWillBegin method
        [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                           callingMethod:@"skillzTournamentWillBegin"
                        withParamMessage:json];
        
        // This sends a message to call the Cross Platform OnMatchWillBegin method
        [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                           callingMethod:@"OnMatchWillBegin"
                        withParamMessage:json];
        self.matchInfo = json;
        
    });
}

- (void)updateCurrentMatchParams:(NSDictionary *)gameParameters
{
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:gameParameters
                                                       options:0
                                                         error:&error];
    
    if (jsonData) {
        NSString *JSONString = [[NSString alloc] initWithBytes:[jsonData bytes]
                                                        length:[jsonData length]
                                                      encoding:NSUTF8StringEncoding];
        self.matchRules = JSONString;
    }
}

- (void)skillzWillExit
{
    ResumeApp();
    if (sSKZAppDelegate) {
        [sSKZAppDelegate uninstallDelegate];
    }
    sSKZAppDelegate = nil;
    
    // Send message to Unity object to call C# method
    // SkillzDelegate.skillzWillExit, implemented by publisher
    [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                       callingMethod:@"skillzWillExit"
                    withParamMessage:@""];
    
    // This sends a message to call the Cross Platform OnSkillzWillExit method
    [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                       callingMethod:@"OnSkillzWillExit"
                    withParamMessage:@""];
}

- (void)skillzWillLaunch
{
    if (sSKZAppDelegate) {
        // If an SKZAppDelegate already exists, uninstall it first to prevent a segfault
        [sSKZAppDelegate uninstallDelegate];
    }
    sSKZAppDelegate = [SKZAppDelegate alloc];
    [sSKZAppDelegate installDelegate];
    
    // Send message to Unity object to call C# method
    // SkillzDelegate.skillzWillLaunch, implemented by publisher
    [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                       callingMethod:@"skillzWillLaunch"
                    withParamMessage:@""];
}

- (void)skillzHasFinishedLaunching
{
    // Send message to Unity object to call C# method
    // SkillzDelegate.skillzLaunchHasCompleted, implemented by publisher
    [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                       callingMethod:@"skillzLaunchHasCompleted"
                    withParamMessage:@""];
}

- (void)onProgressionRoomEnter
{
    ResumeApp();
        
    // This sends a message to call the Cross Platform OnProgressionRoomEnter method
    [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                       callingMethod:@"OnProgressionRoomEnter"
                    withParamMessage:@""];
}

#pragma mark
#pragma mark Sync Delegate
#pragma mark

- (void)onOpponentHasReconnected:(SKZSyncPlayerId)playerId
{
    uint64_t playerIdLong = (uint64_t)playerId;
    if (self.onOpponentHasReconnectedFunc) {
        self.onOpponentHasReconnectedFunc(playerIdLong);
    }
}

- (void)onOpponentHasLostConnection:(SKZSyncPlayerId)playerId
{
    uint64_t playerIdLong = (uint64_t)playerId;
    if (self.onOpponentHasLostConnectionFunc) {
        self.onOpponentHasLostConnectionFunc(playerIdLong);
    }
}

- (void)onOpponentHasLeftMatch:(SKZSyncPlayerId)playerId
{
    uint64_t playerIdLong = (uint64_t)playerId;
    if (self.onOpponentHasLeftMatchFunc) {
        self.onOpponentHasLeftMatchFunc(playerIdLong);
    }
}

- (void)onCurrentPlayerHasReconnected
{
    if (self.onCurrentPlayerHasReconnectedFunc) {
        self.onCurrentPlayerHasReconnectedFunc();
    }
}

- (void)onCurrentPlayerHasLostConnection
{
    if (self.onCurrentPlayerHasLostConnectionFunc) {
        self.onCurrentPlayerHasLostConnectionFunc();
    }
}

- (void)onCurrentPlayerHasLeftMatch
{
    if (self.onCurrentPlayerHasLeftMatchFunc) {
        self.onCurrentPlayerHasLeftMatchFunc();
    }
}

- (void)onDidReceiveData:(NSData * _Nonnull)value
{
    uint64_t lengthShort = (uint64_t)value.length;
    if (self.didReceiveDataFunc) {
        self.didReceiveDataFunc((const unsigned char *)value.bytes, lengthShort);
    }
}

- (void)onMatchCompleted
{
    if (self.onMatchCompletedFunc) {
        self.onMatchCompletedFunc();
    }
}

@end

@implementation Skillz (Unity)

void UnitySendMessage(const char *obj, const char *method, const char *msg);

+ (void)sendMessageToUnityObject:(NSString *)objName
                   callingMethod:(NSString *)methodName
                withParamMessage:(NSString *)msg {
    UnitySendMessage([objName cStringUsingEncoding:NSUTF8StringEncoding],
                     [methodName cStringUsingEncoding:NSUTF8StringEncoding],
                     [msg cStringUsingEncoding:NSUTF8StringEncoding]);
}

+ (CIImage*)getFrame
{
    return [sRenderDelgate createImage];
}

+ (BOOL)supportsUnityRecording
{
    return UnitySelectedRenderingAPI() == apiMetal;
}

#pragma mark Sync Play Delegate

- (void)onCurrentPlayerHasReconnected
{
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasReconnectedFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasReconnectedFunc();
    }
}

- (void)onCurrentPlayerHasLostConnection
{
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasLostConnectionFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasLostConnectionFunc();
    }
}

- (void)onCurrentPlayerHasLeftMatch
{
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasLeftMatchFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasLeftMatchFunc();
    }
}

- (void)onOpponentHasReconnected:(SKZSyncPlayerId)playerId
{
    uint64_t playerIdShort = (uint64_t)playerId;
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasReconnectedFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasReconnectedFunc(playerIdShort);
    }
}

- (void)onOpponentHasLostConnection:(SKZSyncPlayerId)playerId
{
    uint64_t playerIdShort = (uint64_t)playerId;
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasLostConnectionFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasLostConnectionFunc(playerIdShort);
    }
}

- (void)onOpponentHasLeftMatch:(SKZSyncPlayerId)playerId
{
    uint64_t playerIdShort = (uint64_t)playerId;
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasLeftMatchFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasLeftMatchFunc(playerIdShort);
    }
}

- (void)onMatchCompleted
{
    if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onMatchCompletedFunc) {
        ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onMatchCompletedFunc();
    }
}

@end

#pragma mark C-Style wrapper

//################################################################################
//#####
//##### C-style wrapper methods that will be accessed
//##### by the Unity C# wrapper for the iOS Skillz SDK
//#####
//################################################################################

// C-style wrapper for
extern "C" void _addMetadataForMatchInProgress(const char *metadataJson, const bool forMatchInProgress) {
    NSString *jsonString = [[NSString alloc] initWithUTF8String:metadataJson];
    NSError *err = nil;
    NSDictionary *metadata = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding]
                                                             options:0
                                                               error:&err];
    [[Skillz skillzInstance] addMetadata:metadata forMatchInProgress:forMatchInProgress];
}

// C-style wrapper for getRandomNumber so that it can be accessed by Unity in C#
extern "C" int _getRandomNumber()
{
    return (int) [Skillz getRandomNumber];
}

// C-style wrapper for getRandNumberWithMin:andMax so that it can be accessed by Unity in C#
extern "C" int _getRandomNumberWithMinAndMax(int min, int max)
{
    return (int) [Skillz getRandomNumberWithMin:min andMax:max];
}

// C-style wrapper for skillzInitForGameId:AndEnvironment: so that it can be accessed by Unity in C#
extern "C" void _skillzInitForGameIdAndEnvironment(const char *gameId, const char *environment)
{
    NSString *gameIdString = [[NSString alloc] initWithUTF8String:gameId];
    NSString *environmentString = [[NSString alloc] initWithUTF8String:environment];
    SkillzEnvironment skillzEnvironment;
    
    // Initialize the game in either sandbox or production based on the input
    if ([environmentString isEqualToString:@"SkillzSandbox"]) {
        skillzEnvironment = SkillzSandbox;
    } else if ([environmentString isEqualToString:@"SkillzProduction"]) {
        skillzEnvironment = SkillzProduction;
    } else {
        // If the input environment is not SkillzSandbox or SkillzProduction, throw an error
        NSString *exceptionReason = [@"Invalid value for environment: " stringByAppendingString:environmentString];
        NSException *badEnvironmentException = [NSException exceptionWithName:@"InvalidArgumentException"
                                                                       reason:exceptionReason
                                                                     userInfo:nil];
        [badEnvironmentException raise];
    }
    
    [[Skillz skillzInstance] setGameHasSyncBot:NO];
    [[Skillz skillzInstance] initWithGameId:gameIdString
                                forDelegate:[[UnitySkillzSDKDelegate alloc] init]
                            withEnvironment:skillzEnvironment
                                  allowExit:YES];
}

// C-style wrapper for exposing a user's data to Unity in C#
extern "C" const char *_player()
{
    
    SKZPlayer *player = [Skillz player];
    if (player) {
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wundeclared-selector"
        NSString *json = [[Skillz player] performSelector:@selector(JSONStringRepresentation)];
#pragma GCC diagnostic pop
        
        if (json) {
            return [json UTF8String];
        } else {
            return [@"" UTF8String];
        }
    } else {
        return [@"" UTF8String];
    }
}

// C-style wrapper for checking whether a tournament is in progress so that it can be accessed by Unity in C#
extern "C" int _tournamentIsInProgress()
{
    if ([[Skillz skillzInstance] tournamentIsInProgress]) {
        return 1;
    } else {
        return 0;
    }
}

// C-style wrapper for SDKShortVersion so that it can be accessed by Unity in C#
extern "C" const char *_SDKShortVersion()
{
    return [[Skillz SDKShortVersion] UTF8String];
}

// C-style wrapper for showSDKVersionInfo so that it can be accessed by Unity in C#
extern "C" void _showSDKVersionInfo()
{
    [Skillz showSDKVersionInfo];
}

//Launches basic Skillz implementation for single match play.
extern "C" void _launchSkillz()
{
    dispatch_async(dispatch_get_main_queue(), ^{
        PauseApp();
        [[Skillz skillzInstance] launchSkillz];
    });
}

// C-style wrapper for displayTournamentResultsWithScore so that it can be accessed by Unity in C#
extern "C" void _displayTournamentResultsWithScore(int score)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            [[Skillz skillzInstance] displayTournamentResultsWithScore:@(score)
                                                        withCompletion:^{
                isReportingScore = false;
            }];
        }
    });
}

// C-style wrapper for displayTournamentResultsWithFloatScore so that it can be accessed by Unity in C#
extern "C" void _displayTournamentResultsWithFloatScore(float score)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            [[Skillz skillzInstance] displayTournamentResultsWithScore:@(score)
                                                        withCompletion:^{
                isReportingScore = false;
            }];
        }
    });
}

extern "C" void _displayTournamentResultsWithStringScore(const char *score)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            NSNumberFormatter *f = [[NSNumberFormatter alloc] init];
            f.numberStyle = NSNumberFormatterDecimalStyle;
            NSNumber *playerScore = [f numberFromString:@(score)];
            
            [[Skillz skillzInstance] displayTournamentResultsWithScore:playerScore
                                                        withCompletion:^{
                isReportingScore = false;
            }];
        }
    });
}

// C-style wrapper for displayBotTournamentResultsWithScores so that it can be access by Unity in C#
extern "C" void _displayBotTournamentResultsWithScores(int playerScore, int botScore)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            [[Skillz skillzInstance] displayResultsForBotMatchWithPlayerScore:@(playerScore)
                                                                     botScore:@(botScore)
                                                                   completion:^{
                isReportingScore = false;
            }];
        }
    });
}

// C-style wrapper for displayBotTournamentResultsWithFloatScores so that it can be access by Unity in C#
extern "C" void _displayBotTournamentResultsWithFloatScores(float playerScore, float botScore)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            [[Skillz skillzInstance] displayResultsForBotMatchWithPlayerScore:@(playerScore)
                                                                     botScore:@(botScore)
                                                                   completion:^{
                isReportingScore = false;
            }];
        }
    });
}

// C-style wrapper for displayBotTournamentResultsWithStringScores so that it can be access by Unity in C#
extern "C" void _displayBotTournamentResultsWithStringScores(const char *playerScore, const char *botScore)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            NSNumberFormatter *f = [[NSNumberFormatter alloc] init];
            f.numberStyle = NSNumberFormatterDecimalStyle;
            NSNumber *playerScoreNumber = [f numberFromString:@(playerScore)];
            NSNumber *botScoreNumber = [f numberFromString:@(playerScore)];
            
            [[Skillz skillzInstance] displayResultsForBotMatchWithPlayerScore:playerScoreNumber
                                                                     botScore:botScoreNumber
                                                                   completion:^{
                isReportingScore = false;
            }];
        }
    });
}

// C-style wrapper for updatePlayersCurrentScore: so that is can be accessed by Unity in C#
extern "C" void _updatePlayersCurrentScore(float score)
{
    [[Skillz skillzInstance] updatePlayersCurrentScore:@(score)];
}

extern "C" void _updatePlayersCurrentStringScore(const char *score)
{
    NSNumberFormatter *f = [[NSNumberFormatter alloc] init];
    f.numberStyle = NSNumberFormatterDecimalStyle;
    NSNumber *playerScore = [f numberFromString:@(score)];
    [[Skillz skillzInstance] updatePlayersCurrentScore:playerScore];
}

extern "C" void _updatePlayersCurrentIntScore(int score)
{
    [[Skillz skillzInstance] updatePlayersCurrentScore:@(score)];
}

// C-style wrapper for notifyPlayerAbortWithCompletion so that it can be accessed by Unity in C#
extern "C" void _notifyPlayerAbortWithCompletion()
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            [[Skillz skillzInstance] notifyPlayerAbortWithCompletion:^() {
                // Send message to Unity object to call C# method
                // SkillzDelegate.skillzWithPlayerAbort, implemented by publisher
                isReportingScore = false;
                [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                                   callingMethod:@"skillzWithPlayerAbort"
                                withParamMessage:@""];
            }];
            
        }
    });
}

// C-style wrapper for _notifyPlayerAbortWithBotScore so that it can be accessed by Unity in C#
extern "C" void _notifyPlayerAbortWithBotScore(int botScore)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            [[Skillz skillzInstance] notifyPlayerAbortForBotMatchWithBotScore:@(botScore)
                                                                   completion:^() {
                // Send message to Unity object to call C# method
                // SkillzDelegate.skillzWithPlayerAbort, implemented by publisher
                isReportingScore = false;
                [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                                   callingMethod:@"skillzWithPlayerAbort"
                                withParamMessage:@""];
            }];
            
        }
    });
}

// C-style wrapper for _notifyPlayerAbortWithFloatBotScore so that it can be accessed by Unity in C#
extern "C" void _notifyPlayerAbortWithFloatBotScore(float botScore)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            [[Skillz skillzInstance] notifyPlayerAbortForBotMatchWithBotScore:@(botScore)
                                                                   completion:^() {
                // Send message to Unity object to call C# method
                // SkillzDelegate.skillzWithPlayerAbort, implemented by publisher
                isReportingScore = false;
                [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                                   callingMethod:@"skillzWithPlayerAbort"
                                withParamMessage:@""];
            }];
            
        }
    });
}

// C-style wrapper for _notifyPlayerAbortWithStringBotScore so that it can be accessed by Unity in C#
extern "C" void _notifyPlayerAbortWithStringBotScore(const char *botScore)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (!isReportingScore) {
            isReportingScore = true;
            PauseAppWithDelay();
            
            NSNumberFormatter *f = [[NSNumberFormatter alloc] init];
            f.numberStyle = NSNumberFormatterDecimalStyle;
            NSNumber *botScoreNumber = [f numberFromString:@(botScore)];
            
            [[Skillz skillzInstance] notifyPlayerAbortForBotMatchWithBotScore:botScoreNumber
                                                                   completion:^() {
                // Send message to Unity object to call C# method
                // SkillzDelegate.skillzWithPlayerAbort, implemented by publisher
                isReportingScore = false;
                [Skillz sendMessageToUnityObject:unitySkillzDelegateName
                                   callingMethod:@"skillzWithPlayerAbort"
                                withParamMessage:@""];
            }];
            
        }
    });
}

// C-style wrapper for submitScore so that it can be accessed by Unity in C#
extern "C" void _submitScore(int score)
{
    [[Skillz skillzInstance] submitScore:@(score)
                             withSuccess:^(void) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc();
            }
        });
    } withFailure:^(NSString *errorMessage) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc([errorMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        });
    }];
}


// C-style wrapper for submitStringScore so that it can be accessed by Unity in C#
extern "C" void _submitStringScore(char *score)
{
    NSNumberFormatter *f = [[NSNumberFormatter alloc] init];
    f.numberStyle = NSNumberFormatterDecimalStyle;
    NSNumber *numberScore = [f numberFromString:@(score)];

    [[Skillz skillzInstance] submitScore:numberScore
                             withSuccess:^(void) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc();
            }
        });
    }  withFailure:^(NSString *errorMessage) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc([errorMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        });
    }];
}

// C-style wrapper for submitFloatScore so that it can be accessed by Unity in C#
extern "C" void _submitFloatScore(float score)
{
    [[Skillz skillzInstance] submitScore:@(score)
                             withSuccess:^(void) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc();
            }
        });
    }  withFailure:^(NSString *errorMessage) {
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc([errorMessage cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        });
    }];
}

// C-style wrapper for endReplayRecording so that it can be accessed by Unity in C#
extern "C" bool _endReplay()
{
    return [[Skillz skillzInstance] endReplayRecording];
}

// C-style wrapper for returnToSkillz so that it can be accessed by Unity in C#
extern "C" bool _returnToSkillz()
{
    return (bool) [[Skillz skillzInstance] returnToSkillzWithCompletion:^(void) {
        return;
    }];
}

// C-style wrapper for getRandomFloat so that it can be accessed by Unity in C#
extern "C" float _getRandomFloat() {
    return (float) [Skillz getRandomFloat];
}

// C-style wrapper for getMatchRules so that it can be accessed by Unity in C#
extern "C" const char *_getMatchRules() {
    return [((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).matchRules UTF8String];
}

extern "C" const char *_getMatchInfo() {
    return [((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).matchInfo UTF8String];
}

#pragma mark SubmitScore Callback Setup

extern "C" void _assignOnSubmitScoreSuccessCallback(void *funcPtr)
{
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreSuccessFunc = reinterpret_cast<void(*)(void)>(funcPtr);
}

extern "C" void _assignOnSubmitScoreFailureCallback(void *funcPtr)
{
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onSubmitScoreFailureFunc = reinterpret_cast<void(*)(const char*)>(funcPtr);
}


#pragma mark
#pragma mark Audio Integration
#pragma mark

extern "C" void _setSkillzBackgroundMusic(const char *fileName)
{
    NSString* fileNameString = [NSString stringWithUTF8String:fileName];
    [[Skillz skillzInstance] setBackgroundMusicFile:fileNameString];
}

extern "C" void _setSkillzMusicVolume(float volume)
{
    [[Skillz skillzInstance] updateSkillzMusicVolume:volume];
}

extern "C" void _setSFXVolume(float volume)
{
    [[Skillz skillzInstance] setSFXVolume:volume];
}

extern "C" float _getSFXVolume()
{
    float sfxVolume = (float)[[Skillz skillzInstance] getSFXVolume];
    return sfxVolume;
}

extern "C" float _getSkillzMusicVolume()
{
    float skillzMusicVolume = (float)[[Skillz skillzInstance] getBackgroundMusicVolume];
    return skillzMusicVolume;
}

#pragma mark
#pragma mark  Progression API
#pragma mark


extern "C" void _getProgressionUserData(int requestID, const char* progressionNamespace, const char* keysJSON)
{
    NSString *jsonString = [[NSString alloc] initWithUTF8String:keysJSON];
    NSString *namespaceString= [[NSString alloc] initWithUTF8String:progressionNamespace];
    
    NSError *toObjError;
    NSData* keysJSONData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSMutableArray *keysArray = [NSJSONSerialization JSONObjectWithData:keysJSONData
                                                                options:NSJSONReadingMutableContainers
                                                                  error:&toObjError];
    
    
    [[Skillz skillzInstance] getUserDataForNamespace:namespaceString
                                            withKeys:keysArray
                                         withSuccess:^(NSDictionary * userData){
        
        // We have to convert the data into a JSON string to get it across the Unity <> iOS bridge
        // But NSJSONSerialization does not parse NSDate objects. So we're removing the
        // date_cached field from this data before sending to Unity. Android doesn't provide
        // this data to Unity, so it's fine to remove it here as well.
        NSMutableDictionary* filteredUserData = [[NSMutableDictionary alloc] init];
        for (id key in userData) {
            id item = [userData objectForKey:key];
            
            if ([item objectForKey:@"date_cached"] != nil) {
                NSMutableDictionary* mutableItem = [item mutableCopy];
                [mutableItem removeObjectForKey:@"date_cached"];
                [filteredUserData setObject:mutableItem forKey:key];
            } else {
                [filteredUserData setObject:item forKey:key];
            }
        }
        
        // Convert data into a JSON, then invoke the delegate method
        NSError *toJSONError;
        NSData *data = [NSJSONSerialization dataWithJSONObject:filteredUserData
                                                       options:0
                                                         error:&toJSONError];
        NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionGetSuccessFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionGetSuccessFunc(requestID, [dataString cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        });
    } withFailure:^(NSString * errorString){
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionGetFailureFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionGetFailureFunc(requestID, [errorString cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        });
    }];
}

extern "C" void _updateProgressionUserData(int requestID, const char* progressionNamespace, const char* updatesJSON)
{
    NSString *jsonString = [[NSString alloc] initWithUTF8String:updatesJSON ];
    NSString *namespaceString = [[NSString alloc] initWithUTF8String:progressionNamespace];
    
    NSError *error;
    NSData* keysJSONData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSMutableDictionary *keysArray = [NSJSONSerialization JSONObjectWithData:keysJSONData
                                                                     options:NSJSONReadingMutableContainers
                                                                       error:&error];
    
    
    [[Skillz skillzInstance] updateUserDataForNamespace:namespaceString
                                           withUserData:keysArray
                                            withSuccess:^(NSDictionary * userData){
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionUpdateSuccessFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionUpdateSuccessFunc(requestID);
            }
        });
    } withFailure:^(NSString * errorString){
        dispatch_async(dispatch_get_main_queue(), ^{
            if (((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionUpdateFailureFunc) {
                ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionUpdateFailureFunc(requestID, [errorString cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        });
    }];
}

#pragma mark Progression Callback Setup

extern "C" void _assignOnProgressionGetSuccess(void *funcPtr)
{
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionGetSuccessFunc = reinterpret_cast<void(*)(int, const char*)>(funcPtr);
}

extern "C" void _assignOnProgressionGetFailure(void *funcPtr)
{
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionGetFailureFunc = reinterpret_cast<void(*)(int, const char*)>(funcPtr);
}
        
extern "C" void _assignOnProgressionUpdateSuccess(void *funcPtr)
{
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionUpdateSuccessFunc = reinterpret_cast<void(*)(int)>(funcPtr);
}

extern "C" void _assignOnProgressionUpdateFailure(void *funcPtr)
{
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onProgressionUpdateFailureFunc = reinterpret_cast<void(*)(int, const char*)>(funcPtr);
}

#pragma mark
#pragma mark Sync API
#pragma mark

#pragma mark Sync Delegate Setup

extern "C" void _assignOnCurrentPlayerHasReconnectedFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasReconnectedFunc = reinterpret_cast<void(*)()>(function);
}

extern "C" void _assignOnCurrentPlayerHasLostConnectionFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasLostConnectionFunc = reinterpret_cast<void(*)()>(function);
}

extern "C" void _assignOnCurrentPlayerHasLeftMatchFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onCurrentPlayerHasLeftMatchFunc = reinterpret_cast<void(*)()>(function);
}

extern "C" void _assignOnOpponentHasReconnectedFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasReconnectedFunc = reinterpret_cast<void(*)(uint64_t)>(function);
}

extern "C" void _assignOnOpponentHasLostConnectionFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasLostConnectionFunc = reinterpret_cast<void(*)(uint64_t)>(function);
}

extern "C" void _assignOnOpponentHasLeftMatchFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onOpponentHasLeftMatchFunc = reinterpret_cast<void(*)(uint64_t)>(function);
}

extern "C" void _assignOnDidReceiveDataFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).didReceiveDataFunc = reinterpret_cast<void(*)(const unsigned char *, uint64_t)>(function);
}

extern "C" void _assignOnMatchCompletedFunc(void *function) {
    ((UnitySkillzSDKDelegate*)[Skillz skillzInstance].skillzDelegate).onMatchCompletedFunc = reinterpret_cast<void(*)()>(function);
}

#pragma mark Sync Play Methods

extern "C" void _sendData(const char * data, uint64_t length) {
    [[Skillz skillzInstance] sendData:[NSData dataWithBytes:data length:length]];
}

extern "C" bool _isMatchCompleted() {
    return [[Skillz skillzInstance] isMatchCompleted];
}

extern "C" int _connectedPlayerCount() {
    return (int)[[Skillz skillzInstance] getConnectedPlayerCount];
}

extern "C" uint64_t _currentPlayerId()
{
    return (uint64_t)[[Skillz skillzInstance] getCurrentPlayerId];
}

extern "C" uint64_t _currentOpponentPlayerId()
{
    return (uint64_t)[[Skillz skillzInstance] getCurrentOpponentPlayerId];
}

extern "C" uint64_t _getServerTime() {
    return [[Skillz skillzInstance] getServerTime];
}

extern "C" int _reconnectTimeLeftForPlayer(uint64_t playerId) {
    return (int)[[Skillz skillzInstance] getTimeLeftForReconnection:(SKZSyncPlayerId)playerId];
}

//################################################################################
//#####
//##### End of C wrappers
//#####
//################################################################################
