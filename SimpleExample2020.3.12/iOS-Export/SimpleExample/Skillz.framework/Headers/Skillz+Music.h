//
//  Skillz+Music.h
//  SkillzSDK-iOS
//
//  Created by Leo Leblanc on 10/1/18.
//  Copyright © 2018 Skillz. All rights reserved.
//

#import "SkillzInstance.h"

@interface Skillz (Music)

/**
* Call this function to set the volume of the background music the user will hear as part of the Skillz experience.
 
* @param volumeLevel        Float value representing your game's music volume as a float in the range from 0.0 to 1.0.
 
 * Note: This function should be called whenever a user changes your game's background music volume.

*/
-(void)setBackgroundMusicVolume:(CGFloat)volumeLevel;

/**
 * Call this function to get the volume of the background music the user has set while interfacing with Skillz.
 
 * @return      Float value representing Skillz background music volume as a float in the range from 0.0 to 1.0.
 
 * Note: This function's result should be used whenever your game's volume sliders initially load
 */
-(CGFloat)getBackgroundMusicVolume;

/**
 * Call this function to set the volume of the sound effects the user will hear as part of the Skillz experience.
 
 * @param volumeLevel        Float value representing your game's sound effects volume as a float in the range from 0.0 to 1.0.
 
 * Note: This function should be called whenever a user changes your game's sound effects volume.
 */
-(void)setSFXVolume:(CGFloat)volumeLevel;

/**
 * Call this function to get the volume of the sound effects the user has set while interfacing with Skillz.
 
 * @return         Float value representing Skillz sound effects volume as a float in the range from 0.0 to 1.0.
 
 * Note: This function's result should be used whenever your game's volume sliders initially load
 */

-(CGFloat)getSFXVolume;

/**
 * Call this function to set the background music the user will hear during the Skillz experience.
 
 * @param fileName     The name of the music file to be played, with its extension.
 
 * @return         Whether or not this file path was able to be located by the SDK.
 
 */
-(BOOL)setBackgroundMusicFile:(NSString*)fileName;

-(void)updateSkillzMusicVolume:(CGFloat)volumeLevel;
-(void)updateSFXVolume:(CGFloat)volumeLevel;
@end
