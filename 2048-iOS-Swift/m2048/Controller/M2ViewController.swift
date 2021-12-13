//
//  M2ViewController.swift
//  m2048
//
//  Created by Skillz on 5/12/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import UIKit
import SpriteKit
import Skillz

class M2ViewController : UIViewController {
    @IBOutlet var  _forfeitButton: UIButton?
    @IBOutlet var _targetScore: UILabel?
    @IBOutlet var _subtitle: UILabel?
    @IBOutlet var _scoreView: M2ScoreView?
    @IBOutlet var _bestView: M2ScoreView?

    @IBOutlet var  _overlay: M2Overlay?
    @IBOutlet var  _overlayBackground: UIImageView?
    
    var _scene: M2Scene?

    override init(nibName nibNameOrNil: String?, bundle nibBundleOrNil: Bundle?) {
        self._scene = nil
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }

    required init?(coder: NSCoder) {
        self._scene = nil
        super.init(coder: coder)
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.updateState()

        _bestView?.score?.text = String(format: "%ld", Settings.integer(forKey: "Best Score"))

        _forfeitButton?.layer.cornerRadius = CGFloat(GSTATE.cornerRadius)
        _forfeitButton?.layer.masksToBounds = true;

        _overlay?.isHidden = true;
        _overlayBackground?.isHidden = true;

        // Configure the view.
        let skView = self.view as! SKView

        // Create and configure the scene.
        let scene = M2Scene(size: skView.bounds.size)
        scene.scaleMode = SKSceneScaleMode.aspectFill

        // Present the scene.
        skView.presentScene(scene)
        self.update(score: 0)

        _scene = scene;
        _scene!.controller = self;
        
        Skillz.skillzInstance().launch()
    }

    func updateState() {
        _scoreView?.updateAppearance()
        _bestView?.updateAppearance()

        _forfeitButton?.backgroundColor = GSTATE.buttonColor()
        _forfeitButton?.titleLabel?.font = UIFont(name: GSTATE.boldFontName(), size: 14)

        _targetScore?.textColor = GSTATE.buttonColor()

        let target: CLong = GSTATE.valueFor(level: GSTATE.winningLevel())

        if target > 100000 {
            _targetScore?.font = UIFont(name: GSTATE.boldFontName(), size: 34)
        } else if (target < 10000) {
            _targetScore?.font = UIFont(name: GSTATE.boldFontName(), size: 42)
        } else {
            _targetScore?.font = UIFont(name: GSTATE.boldFontName(), size: 40)
        }

        _targetScore?.text = String(format: "%ld", target)

        _subtitle?.textColor = GSTATE.buttonColor()
        _subtitle?.font = UIFont(name: GSTATE.regularFontName(), size: 14)
        _subtitle?.text = String(format: "Join the numbers to get the %ld!", target)

        _overlay?.message?.font = UIFont(name: GSTATE.boldFontName(), size: 36)
        _overlay?.keepPlaying?.titleLabel?.font = UIFont(name: GSTATE.boldFontName(), size: 17)
        _overlay?.restartGame?.titleLabel?.font = UIFont(name: GSTATE.boldFontName(), size: 17)
        _overlay?.message?.textColor = GSTATE.buttonColor()
        _overlay?.message?.textColor = GSTATE.buttonColor()

        _overlay?.keepPlaying?.setTitleColor(GSTATE.buttonColor(), for: UIControl.State.normal)
        _overlay?.restartGame?.setTitleColor(GSTATE.buttonColor(), for: UIControl.State.normal)
    }

    func update(score: Int) {
        _scoreView?.score?.text = String(format: "%ld", score)
        if Settings.integer(forKey: "Best Score") < score {
            Settings.set(score, forKey: "Best Score")
            _bestView?.score?.text = String(format: "%ld", score)
        }

        if Skillz.skillzInstance().tournamentIsInProgress {
            Skillz.skillzInstance().updatePlayersCurrentScore(NSNumber(value: score))
        }
    }

    func prepare(forSegue: UIStoryboardSegue, sender: Any?) {
      // Pause Sprite Kit. Otherwise the dismissal of the modal view would lag.
        (self.view as! SKView).isPaused = true;
    }

    @IBAction func quitMatch(sender: Any) {
        self.hideOverlay()

        if !Skillz.skillzInstance().tournamentIsInProgress {
            self._scene!.startNewGame()
            return;
        }
        
        let abortAlertController = UIAlertController(title: "Quit Match?",
                                                     message: "A match is in progress. Are you sure you want to end the match?", preferredStyle: UIAlertController.Style.alert)

        let yesAction = UIAlertAction(title: "Yes", style: UIAlertAction.Style.default, handler: {
            action in self.endGame(won: false)
            NSLog("The user quit the match.")
            }
        )

        let noAction = UIAlertAction(title: "No", style: UIAlertAction.Style.cancel, handler: { action in
            NSLog("Exit canceled, continuing the current match")
        })

        abortAlertController.addAction(yesAction)
        abortAlertController.addAction(noAction)

        self.present(abortAlertController, animated: true, completion: nil)
    }

    @IBAction func keepPlaying(sender: Any) {
        self.hideOverlay()
    }

    @IBAction func done(segue: UIStoryboardSegue) {
        (self.view as! SKView).isPaused = false
        if GSTATE.needRefresh {
            GSTATE.loadGlobalState()
            self.updateState()
            self.update(score: 0)
            _scene!.startNewGame()
        }
    }

    func startNewGame() {
        self.hideOverlay()
        self.update(score: 0)
        _scene!.startNewGame()
    }

    func endGame(won: Bool) {
        _overlay?.isHidden = false
        _overlay?.alpha = 0
        _overlayBackground?.isHidden = false
        _overlayBackground?.alpha = 0;
      
        if !won {
            _overlay?.keepPlaying?.isHidden = true
            _overlay?.message?.text = "Game Over"
        } else {
            _overlay?.keepPlaying?.isHidden = false;
            _overlay?.message?.text = "You Win!"
        }
      
        // Fake the overlay background as a mask on the board.
        _overlayBackground?.image = M2GridView.gridImageWithOverlay()

        // Center the overlay in the board.
        let verticalOffset = UIScreen.main.bounds.size.height - CGFloat(GSTATE.verticalOffset)
        let side = GSTATE.dimension * (GSTATE.tileSize() + GSTATE.borderWidth) + GSTATE.borderWidth
        _overlay?.center = CGPoint(x: Int(CGFloat(GSTATE.horizontalOffset) + CGFloat(side / 2)), y: Int(verticalOffset - CGFloat(side / 2)));

        UIView.animate(withDuration: 0.5, delay: 1.5, options: UIView.AnimationOptions.curveEaseInOut, animations: {
            self._overlay?.alpha = 1
            self._overlayBackground?.alpha = 1
        }, completion: { isFinished in
            (self.view as! SKView).isPaused = true;
            if Skillz.skillzInstance().tournamentIsInProgress {
                Skillz.skillzInstance().submitScore(
                    NSNumber(value: Int((self._scoreView?.score?.text)!)!),
                    withSuccess: {
                        NSLog("Score Submit Success!")
                        
                        Skillz.skillzInstance().returnToSkillz() {
                            // make sure to clean things up if needed
                            NSLog("Returning to Skillz")
                            self.startNewGame()
                            self.update(score: 0)
                            self.hideOverlay()
                            self.view.removeFromSuperview()
                            self.removeFromParent()
                        }
                    },
                    withFailure: {_msg in
                        NSLog("Score Submit Failure: " + _msg)
                        let numberFormatter = NumberFormatter()
                        numberFormatter.numberStyle = NumberFormatter.Style.decimal
                        
                        // Fallback to get score in queue and send user to Skillz UI
                        Skillz.skillzInstance().displayTournamentResults(
                            withScore: numberFormatter.number(from: (self._scoreView?.score?.text)!)!,
                            withCompletion: {
                                NSLog("The match has ended, and the user's final score was reported")
                                self.update(score: 0)
                                self.hideOverlay()
                                self.startNewGame()
                                }
                            )
                    }
                )
            }
        })
    }

    func hideOverlay() {
        (self.view as! SKView).isPaused = false
        if !(_overlay?.isHidden ?? true) {
            UIView.animate(withDuration: 0.5, animations: {
                self._overlay?.alpha = 0;
                self._overlayBackground?.alpha = 0;
            }, completion: { isFinished in
                self._overlay?.isHidden = true
                self._overlayBackground?.isHidden = true
            })
        }
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
      // Release any cached data, images, etc that aren't in use.
    }
}
