//
//  M2ViewController.m
//  m2048
//
//  Created by Danqing on 3/16/14.
//  Copyright (c) 2014 Danqing. All rights reserved.
//

#import "M2ViewController.h"

#import "M2Scene.h"
#import "M2GameManager.h"
#import "M2ScoreView.h"
#import "M2Overlay.h"
#import "M2GridView.h"

#import <Skillz/Skillz.h>

@implementation M2ViewController {
  IBOutlet UIButton *_forfeitButton;
  IBOutlet UILabel *_targetScore;
  IBOutlet UILabel *_subtitle;
  IBOutlet M2ScoreView *_scoreView;
  IBOutlet M2ScoreView *_bestView;
  
  M2Scene *_scene;
  
  IBOutlet M2Overlay *_overlay;
  IBOutlet UIImageView *_overlayBackground;
}

- (void)viewDidLoad
{
    [super viewDidLoad];

    [self updateState];

    _bestView.score.text = [NSString stringWithFormat:@"%ld", (long)[Settings integerForKey:@"Best Score"]];

    _forfeitButton.layer.cornerRadius = [GSTATE cornerRadius];
    _forfeitButton.layer.masksToBounds = YES;

    _overlay.hidden = YES;
    _overlayBackground.hidden = YES;

    // Configure the view.
    SKView * skView = (SKView *)self.view;

    // Create and configure the scene.
    M2Scene * scene = [M2Scene sceneWithSize:skView.bounds.size];
    scene.scaleMode = SKSceneScaleModeAspectFill;

    // Present the scene.
    [skView presentScene:scene];
    [self updateScore:0];

    _scene = scene;
    _scene.controller = self;
    
    [[Skillz skillzInstance] launchSkillz];
}


- (void)updateState
{
  [_scoreView updateAppearance];
  [_bestView updateAppearance];
  
  _forfeitButton.backgroundColor = [GSTATE buttonColor];
  _forfeitButton.titleLabel.font = [UIFont fontWithName:[GSTATE boldFontName] size:14];
   
  _targetScore.textColor = [GSTATE buttonColor];
  
  long target = [GSTATE valueForLevel:GSTATE.winningLevel];
  
  if (target > 100000) {
    _targetScore.font = [UIFont fontWithName:[GSTATE boldFontName] size:34];
  } else if (target < 10000) {
    _targetScore.font = [UIFont fontWithName:[GSTATE boldFontName] size:42];
  } else {
    _targetScore.font = [UIFont fontWithName:[GSTATE boldFontName] size:40];
  }
  
  _targetScore.text = [NSString stringWithFormat:@"%ld", target];
  
  _subtitle.textColor = [GSTATE buttonColor];
  _subtitle.font = [UIFont fontWithName:[GSTATE regularFontName] size:14];
  _subtitle.text = [NSString stringWithFormat:@"Join the numbers to get to %ld!", target];
  
  _overlay.message.font = [UIFont fontWithName:[GSTATE boldFontName] size:36];
  _overlay.keepPlaying.titleLabel.font = [UIFont fontWithName:[GSTATE boldFontName] size:17];
  _overlay.restartGame.titleLabel.font = [UIFont fontWithName:[GSTATE boldFontName] size:17];
  
  _overlay.message.textColor = [GSTATE buttonColor];
  [_overlay.keepPlaying setTitleColor:[GSTATE buttonColor] forState:UIControlStateNormal];
  [_overlay.restartGame setTitleColor:[GSTATE buttonColor] forState:UIControlStateNormal];
}


- (void)updateScore:(NSInteger)score
{
    _scoreView.score.text = [NSString stringWithFormat:@"%ld", (long)score];
    if ([Settings integerForKey:@"Best Score"] < score) {
        [Settings setInteger:score forKey:@"Best Score"];
        _bestView.score.text = [NSString stringWithFormat:@"%ld", (long)score];
    }
    
    if ([[Skillz skillzInstance] tournamentIsInProgress]) {
        [[Skillz skillzInstance] updatePlayersCurrentScore:@(score)];
    }
}


- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
{
  // Pause Sprite Kit. Otherwise the dismissal of the modal view would lag.
  ((SKView *)self.view).paused = YES;
}


- (IBAction)forfeitMatch:(id)sender
{
    [self hideOverlay];
    [self updateScore:0];
    
    if (![[Skillz skillzInstance] tournamentIsInProgress]) {
        [self->_scene startNewGame];
        return;
    }
    
    UIAlertController *abortAlertController = [UIAlertController alertControllerWithTitle:@"Forfeit Match?"
                                                                 message:@"A match is in progress. Are you sure you want to forfeit the match?"
                                                                 preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction *yesAction = [UIAlertAction actionWithTitle:@"Yes"
                                              style:UIAlertActionStyleDefault
                                              handler:^(UIAlertAction* action)
    {
        [[Skillz skillzInstance] notifyPlayerAbortWithCompletion:^(void)
        {
            NSLog(@"The user forfeited the match.");
        }];
    }];
    
    UIAlertAction *noAction = [UIAlertAction actionWithTitle:@"No" style:UIAlertActionStyleCancel handler:^(UIAlertAction* action)
    {
        NSLog(@"Forfeit canceled, continuing the current match.");
    }];
    
    [abortAlertController addAction:yesAction];
    [abortAlertController addAction:noAction];
    [self presentViewController:abortAlertController animated:YES completion:nil];
}


- (IBAction)keepPlaying:(id)sender
{
  [self hideOverlay];
}


- (IBAction)done:(UIStoryboardSegue *)segue
{
  ((SKView *)self.view).paused = NO;
  if (GSTATE.needRefresh) {
    [GSTATE loadGlobalState];
    [self updateState];
    [self updateScore:0];
    [_scene startNewGame];
  }
}

- (void)startNewGame
{
    [_scene startNewGame];
}

- (void)endGame:(BOOL)won
{
  _overlay.hidden = NO;
  _overlay.alpha = 0;
  _overlayBackground.hidden = NO;
  _overlayBackground.alpha = 0;
  
  if (!won) {
    _overlay.keepPlaying.hidden = YES;
    _overlay.message.text = @"Game Over";
  } else {
    _overlay.keepPlaying.hidden = NO;
    _overlay.message.text = @"You Win!";
  }
  
  // Fake the overlay background as a mask on the board.
  _overlayBackground.image = [M2GridView gridImageWithOverlay];
  
  // Center the overlay in the board.
  CGFloat verticalOffset = [[UIScreen mainScreen] bounds].size.height - GSTATE.verticalOffset;
  NSInteger side = GSTATE.dimension * (GSTATE.tileSize + GSTATE.borderWidth) + GSTATE.borderWidth;
  _overlay.center = CGPointMake(GSTATE.horizontalOffset + side / 2, verticalOffset - side / 2);
  
  [UIView animateWithDuration:0.5 delay:1.5 options:UIViewAnimationOptionCurveEaseInOut animations:^{
      self->_overlay.alpha = 1;
      self->_overlayBackground.alpha = 1;
  } completion:^(BOOL finished) {
    // Freeze the current game.
    ((SKView *)self.view).paused = YES;
      
      if ([[Skillz skillzInstance] tournamentIsInProgress]) {
          NSInteger matchId = [[[Skillz skillzInstance] getMatchInfo] id];
          
          [[Skillz skillzInstance] displayTournamentResultsWithScore:@([self->_scoreView.score.text integerValue])
                                                         withMatchId:@(matchId)
                                                      withCompletion:^(void)
          {
              NSLog(@"The match has ended, and the user's final score was reported.");
          }];
      }
  }];
}


- (void)hideOverlay
{
  ((SKView *)self.view).paused = NO;
  if (!_overlay.hidden) {
    [UIView animateWithDuration:0.5 animations:^{
        self->_overlay.alpha = 0;
        self->_overlayBackground.alpha = 0;
    } completion:^(BOOL finished) {
        self->_overlay.hidden = YES;
        self->_overlayBackground.hidden = YES;
    }];
  }
}


- (void)didReceiveMemoryWarning
{
  [super didReceiveMemoryWarning];
  // Release any cached data, images, etc that aren't in use.
}

@end
