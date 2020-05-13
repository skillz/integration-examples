//
//  M2Scene.swift
//  m2048
//
//  Created by Skillz on 5/11/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

// The min distance in one direction for an effective swipe.
let EFFECTIVE_SWIPE_DISTANCE_THRESHOLD : Float = 20.0

// The max ratio between the translation in x and y directions
// to make a swipe valid. i.e. diagonal swipes are invalid.
let VALID_SWIPE_DIRECTION_THRESHOLD: Float = 2.0

import SpriteKit

class M2Scene : SKScene {
    var controller: M2ViewController?

    /** The game manager that controls all the logic of the game. */
    private var _manager: M2GameManager

    /**
     * Each swipe triggers at most one action, and we don't wait the swipe to complete
     * before triggering the action (otherwise the user may swipe a long way but nothing
     * happens). So after a swipe is done, we turn this flag to NO to prevent further
     * moves by the same swipe.
     */
    private var _hasPendingSwipe: Bool

    /** The current board node. */
    private var _board: SKSpriteNode?
    
    override init(size: CGSize) {
        controller = nil
        _manager = M2GameManager()
        _hasPendingSwipe = false

        super.init(size: size)
    }

    required init?(coder aDecoder: NSCoder) {
        controller = nil
        _manager = M2GameManager()
        _hasPendingSwipe = false

        super.init(coder: aDecoder)
    }

    func loadBoard(withGrid: M2Grid) {
      // Remove the current board if there is one.
        if _board != nil {
            _board!.removeFromParent()
        }

        let image: UIImage = M2GridView.gridImage(withGrid: withGrid)

        let backgroundTexture: SKTexture = SKTexture(cgImage: image.cgImage!)
        
        _board = SKSpriteNode(texture: backgroundTexture)
        _board!.setScale(1 / UIScreen.main.scale) //This solves the Scaling problem in 6Plus and 6S Plus
        _board!.position = CGPoint(x: self.frame.midX, y: self.frame.midY)
        
        self.addChild(_board!)
    }

    func startNewGame() {
        _manager.startNewSession(withScene: self);
    }

    // @TODO: It makes more sense to move these logic stuff to the view controller.

    override func didMove(to view: SKView) {
      if (view == self.view) {
        // Add swipe recognizer immediately after we move to this scene.
        let recognizer = UIPanGestureRecognizer(target: self, action: #selector(handle))
        self.view?.addGestureRecognizer(recognizer)
      } else {
        // If we are moving away, remove the gesture recognizer to prevent unwanted behaviors.
        for recognizer in self.view?.gestureRecognizers ?? [] {
            self.view?.removeGestureRecognizer(recognizer)
        }
      }
    }


    @objc func handle(swipe: UIPanGestureRecognizer) {
        if swipe.state == UIGestureRecognizer.State.began {
            _hasPendingSwipe = true
        } else if (swipe.state == UIGestureRecognizer.State.changed) {
            self.commit(translation: swipe.translation(in: self.view))
      }
    }

    func commit(translation: CGPoint) {
        if (!_hasPendingSwipe) {
            return
        }
      
        let absX: CGFloat = abs(translation.x);
        let absY: CGFloat = abs(translation.y);
      
        // Swipe too short. Don't do anything.
        if max(absX, absY) < CGFloat(EFFECTIVE_SWIPE_DISTANCE_THRESHOLD) {
            return;
        }
        
        // We only accept horizontal or vertical swipes, but not diagonal ones.
        if absX > absY * CGFloat(VALID_SWIPE_DIRECTION_THRESHOLD) {
            translation.x < 0
                ? _manager.move(toDirection: M2Direction.left)
                : _manager.move(toDirection: M2Direction.right)
        } else if absY > absX * CGFloat(VALID_SWIPE_DIRECTION_THRESHOLD) {
            translation.y < 0
                ? _manager.move(toDirection: M2Direction.up)
                : _manager.move(toDirection: M2Direction.down)
        }

        _hasPendingSwipe = false;
    }
}
