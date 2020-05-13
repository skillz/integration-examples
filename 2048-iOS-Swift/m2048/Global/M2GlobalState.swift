//
//  M2GlobalState.swift
//  m2048
//
//  Created by Skillz on 5/7/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import Foundation
import UIKit

let GSTATE = M2GlobalState.instance
let Settings = UserDefaults.standard
let NotifCtr = NotificationCenter.default

let kGameType  = "Game Type"
let kTheme     = "Theme"
let kBoardSize = "Board Size"
let kBestScore = "Best Score"

enum M2GameType: Int {
    case Fibonacci = 2
    case PowerOf2 = 0
    case PowerOf3 = 1
}

class M2GlobalState : NSObject {
    static let instance: M2GlobalState = M2GlobalState()

    var dimension: Int
    var borderWidth: Int
    var cornerRadius: Int
    var horizontalOffset: Int
    var verticalOffset: Int
    var animationDuration: TimeInterval
    var gameType: M2GameType
    var theme: Int

    var needRefresh: Bool

    override init() {
        self.dimension = 0
        self.borderWidth = 0
        self.cornerRadius = 0
        self.animationDuration = 0
        self.gameType = M2GameType.PowerOf2
        self.horizontalOffset = 0
        self.verticalOffset = 0
        self.theme = 0
        self.needRefresh = false
        super.init()

        self.setupDefaultState()
        self.loadGlobalState()
    }
    
    func setupDefaultState() {
        let defaultValues = [kGameType: 0, kTheme: 0, kBoardSize: 1, kBestScore: 0]
        Settings.register(defaults:defaultValues);
    }

    /** Refreshes global state to reflect user choice. */
    func loadGlobalState() {
        self.dimension = Settings.integer(forKey:kBoardSize) + 3
        self.borderWidth = 5
        self.cornerRadius = 4
        self.animationDuration = 0.1
        self.gameType = M2GameType(rawValue: Settings.integer(forKey:kGameType))!
        self.horizontalOffset = self.getHorizontalOffset()
        self.verticalOffset = self.getVerticalOffset()
        self.theme = Settings.integer(forKey:kTheme)
        self.needRefresh = false
    }
    
    func tileSize() -> Int {
        return self.dimension <= 4 ? 66 : 56
    }

    func getHorizontalOffset() -> Int {
        let width: CGFloat = CGFloat(self.dimension) * CGFloat(self.tileSize() + self.borderWidth) + CGFloat(self.borderWidth)
        return Int((UIScreen.main.bounds.size.width - width) / 2)
    }

    func getVerticalOffset() -> Int {
        let height: CGFloat = CGFloat(self.dimension) * CGFloat(self.tileSize() + self.borderWidth) + CGFloat(self.borderWidth) + CGFloat(120)
        return Int((UIScreen.main.bounds.size.height - height) / 2);
    }

    func winningLevel() -> Int {
        if GSTATE.gameType == M2GameType.PowerOf3 {
            switch (self.dimension) {
            case 3:
                return 4;
            case 4:
                return 5;
            case 5:
                return 6;
            default:
                return 5;
            }
        }
      
        let level: Int = 11;
        if self.dimension == 3 {
            return level - 1
        }
        if self.dimension == 5 {
            return level + 2
        }
        return level;
    }

    /**
     * Whether the two levels can merge with each other to form another level.
     * This behavior is commutative.
     *
     * @param level1 The first level.
     * @param level2 The second level.
     * @return YES if the two levels are actionable with each other.
     */
    func isLevel(level: Int, mergeableWithLevel: Int) -> Bool {
        if self.gameType == M2GameType.Fibonacci {
            return abs(level - mergeableWithLevel) == 1
        }
        return level == mergeableWithLevel
    }

    /**
     * The resulting level of merging the two incoming levels.
     *
     * @param level1 The first level.
     * @param level2 The second level.
     * @return The resulting level, or 0 if the two levels are not actionable.
     */
    func merge(level: Int, withLevel: Int) -> Int {
        if !self.isLevel(level:level, mergeableWithLevel:withLevel) {
            return 0;
        }

        if self.gameType == M2GameType.Fibonacci {
          return level + 1 == withLevel ? withLevel + 1 : level + 1;
        }
        return level + 1;
    }

    /**
     * The numerical value of the specified level.
     *
     * @param level The level we are interested in.
     * @return The numerical value of the level.
     */
    func valueFor(level: Int) -> Int {
        if self.gameType == M2GameType.Fibonacci {
            var a = 1, b = 1;
            for _ in 0...level - 1 {
                let c = a + b;
                a = b;
                b = c;
            }
            return b;
        } else {
            var value = 1;
            let base = self.gameType == M2GameType.PowerOf2 ? 2 : 3;
            for _ in 0...level - 1 {
                value *= base;
            }
            return value;
        }
    }

    /**
     * The background color of the specified level.
     *
     * @param level The level we are interested in.
     * @return The color of the level.
     */
    func colorFor(level: Int) -> UIColor {
        return M2Theme.themeClassFor(type: self.theme).colorFor(level: level)
    }

    /**
     * The text color of the specified level.
     *
     * @param level The level we are interested in.
     * @return The color of the level.
     */
    func textColorFor(level: Int) -> UIColor {
        return M2Theme.themeClassFor(type: self.theme).textColorFor(level: level)
    }

    func backgroundColor() -> UIColor {
        return M2Theme.themeClassFor(type: self.theme).backgroundColor()
    }

    func boardColor() -> UIColor {
        return M2Theme.themeClassFor(type: self.theme).boardColor()
    }

    func scoreBoardColor() -> UIColor {
        return M2Theme.themeClassFor(type: self.theme).scoreBoardColor()
    }

    func buttonColor() -> UIColor {
        return M2Theme.themeClassFor(type: self.theme).buttonColor()
    }

    func boldFontName() -> String {
        return M2Theme.themeClassFor(type: self.theme).boldFontName()
    }

    func regularFontName() -> String {
        return M2Theme.themeClassFor(type: self.theme).regularFontName()
    }

    /**
     * The text size of the specified value.
     *
     * @param value The value we are interested in.
     * @return The text size of the value.
     */
    func textSizeFor(value: Int) -> CGFloat {
        let offset = self.dimension == 5 ? 2 : 0
        if (value < 100) {
          return CGFloat(32 - offset)
        } else if (value < 1000) {
          return CGFloat(28 - offset)
        } else if (value < 10000) {
          return CGFloat(24 - offset)
        } else if (value < 100000) {
          return CGFloat(20 - offset)
        } else if (value < 1000000) {
          return CGFloat(16 - offset)
        } else {
          return CGFloat(13 - offset)
        }
    }

    /**
     * The starting location of the position.
     *
     * @param position The position we are interested in.
     * @return The location in points, relative to the grid.
     */
    func locationOf(position: M2Position) -> CGPoint {
        
        return CGPoint(x:self.xLocationOf(position: position) + CGFloat(self.horizontalOffset),
                       y:self.yLocationOf(position: position) + CGFloat(self.verticalOffset))
    }

    /**
     * The starting x location of the position.
     *
     * @param position The position we are interested in.
     * @return The x location in points, relative to the grid.
     */
    func xLocationOf(position: M2Position) -> CGFloat {
        return CGFloat(position.y) * CGFloat(GSTATE.tileSize() + GSTATE.borderWidth) + CGFloat(GSTATE.borderWidth)
    }

    /**
     * The starting y location of the position.
     *
     * @param position The position we are interested in.
     * @return The y location in points, relative to the grid.
     */
    func yLocationOf(position: M2Position) -> CGFloat {
        return CGFloat(position.x) * CGFloat(GSTATE.tileSize() + GSTATE.borderWidth) + CGFloat(GSTATE.borderWidth)
    }
}
