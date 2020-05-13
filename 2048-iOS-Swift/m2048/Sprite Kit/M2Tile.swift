//
//  M2Tile.swift
//  m2048
//
//  Created by Jerry Lin on 5/7/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import SpriteKit
import Skillz

class M2Tile : SKShapeNode {

    /** The level of the tile. */
    var level: Int

    /** The cell this tile belongs to. */
    var cell: M2Cell
    
    /** The value of the tile, as some text. */
    private var _value: SKLabelNode
    
    /** Pending actions for the tile to execute. */
    private var _pendingActions: NSMutableArray

    /** Pending function to call after @p _pendingActions are executed. */
    private var _pendingBlock: () -> ()

    override init() {
        self.level = 0
        self.cell = M2Cell(position: M2Position(x: 0, y: 0))
        self._value = SKLabelNode()
        self._pendingActions = []
        self._pendingBlock = { }

        super.init()

        // Layout of the tile.
        let rect = CGRect(origin: CGPoint(x: 0, y: 0), size: CGSize(width: GSTATE.tileSize(), height: GSTATE.tileSize()));

        let rectPath = CGPath(roundedRect: rect, cornerWidth: CGFloat(GSTATE.cornerRadius), cornerHeight: CGFloat(GSTATE.cornerRadius), transform: nil)
        path = rectPath;
        lineWidth = 0;

        // Initiate pending actions queue.
        _pendingActions = NSMutableArray()

        // Set up value label.
        _value = SKLabelNode(fontNamed: GSTATE.boldFontName())
        _value.position = CGPoint(x: CGFloat(GSTATE.tileSize() / 2), y: CGFloat(GSTATE.tileSize() / 2))
        _value.horizontalAlignmentMode = SKLabelHorizontalAlignmentMode.center
        _value.verticalAlignmentMode = SKLabelVerticalAlignmentMode.center
        self.addChild(_value)

        // For Fibonacci game, which is way harder than 2048 IMO, 40 seems to be the easiest number.
        // 90 definitely won't work, as we need approximately equal number of 2 and 3 to make the
        // game remotely makes sense.
        let randomInt = Skillz.skillzInstance().tournamentIsInProgress
            ? Skillz.getRandomNumber(withMin: 0, andMax: 100)
            : UInt(arc4random_uniform(100))
          
        if GSTATE.gameType == M2GameType.Fibonacci {
            level = randomInt < 40 ? 1 : 2;
        }
        else {
            level = randomInt < 95 ? 1 : 2;
        };

        refreshValue()
    }
    
    required init?(coder aDecoder: NSCoder) {
        self.level = 0
        self.cell = M2Cell(position: M2Position(x: 0, y: 0))
        self._value = SKLabelNode()
        self._pendingActions = []
        self._pendingBlock = { }

        super.init(coder:aDecoder)
    }

    class func insertNewTile(toCell: M2Cell) -> M2Tile {
        let tile = M2Tile()

        // The initial position of the tile is at the center of its cell. This is so because when
        // scaling the tile, SpriteKit does so from the origin, not the center. So we have to scale
        // the tile while moving it back to its normal position to achieve the "pop out" effect.
        let origin: CGPoint = GSTATE.locationOf(position: toCell.position)
        tile.position = CGPoint(x: origin.x + CGFloat(GSTATE.tileSize()) / 2, y: origin.y + CGFloat(GSTATE.tileSize()) / 2);
        tile.setScale(0)

        toCell.set(tile: tile)
        return tile;
    }

    func removeFromParentCell() {
        // Check if the tile is still registered with its parent cell, and if so, remove it.
        // We don't really care about self.cell, because that is a weak pointer.
        if (self.cell.getTile() == self) {
            self.cell.set(tile: nil)
        }
    }

    func hasPendingMerge() -> Bool {
        // A move is only one action, so if there are more than one actions, there must be
        // a merge that needs to be committed. If things become more complicated, change
        // this to an explicit ivar or property.
        return _pendingActions.count > 1;
    }


    func commitPendingActions() {
        self.run(SKAction.sequence(_pendingActions as! [SKAction])) {
            self._pendingActions.removeAllObjects()
            self._pendingBlock()
            self._pendingBlock = { }
        }
    }


    func canMerge(withTile: M2Tile?) -> Bool {
        if (withTile == nil) {
            return false
        }
        return GSTATE.isLevel(level: self.level, mergeableWithLevel: withTile!.level)
    }

    func merge(toTile: M2Tile?) -> Int {
        // Cannot merge with thin air. Also cannot merge with tile that has a pending merge.
        // For the latter, imagine we have 4, 2, 2. If we move to the right, it will first
        // become 4, 4. Now we cannot merge the two 4's.
        if (toTile == nil || toTile!.hasPendingMerge()) {
            return 0
        }

        let newLevel: Int = GSTATE.merge(level: self.level, withLevel: toTile!.level)
        if (newLevel > 0) {
            // 1. Move self to the destination cell.
            self.move(toCell: toTile!.cell)

            // 2. Remove the tile in the destination cell.
            toTile!.removeWithDelay()

            // 3. Update value and pop.
            self.update(levelTo: newLevel)
            _pendingActions.add(self.pop())
        }
        return newLevel;
    }


    func merge3(toTile: M2Tile?, andTile: M2Tile) -> Int {
        if toTile == nil || toTile!.hasPendingMerge() || andTile.hasPendingMerge() {
            return 0
        }

        let newLevel = min(GSTATE.merge(level: self.level, withLevel: toTile!.level), GSTATE.merge(level: toTile!.level, withLevel: andTile.level))

        if (newLevel > 0) {
            // 1. Move self to the destination cell AND move the intermediate tile to there too.
            toTile!.move(toCell: andTile.cell)
            self.move(toCell: andTile.cell)

            // 2. Remove the tile in the destination cell.
            toTile!.removeWithDelay();
            andTile.removeWithDelay();

            // 3. Update value and pop.
            self.update(levelTo: newLevel)
            _pendingActions.add(self.pop())
        }
        return Int(newLevel);
    }

    func update(levelTo: Int) {
      self.level = levelTo
        _pendingActions.add(SKAction.run({
            self.refreshValue()
        }))
    }

    func refreshValue() {
        let value = GSTATE.valueFor(level: self.level)
        _value.text = String(format: "%ld", value)
        _value.fontColor = GSTATE.textColorFor(level: self.level)
        _value.fontSize = GSTATE.textSizeFor(value: value)

        self.fillColor = GSTATE.colorFor(level:self.level)
    }

    func move(toCell: M2Cell) {
        _pendingActions.add(SKAction.move(to: GSTATE.locationOf(position: toCell.position), duration: GSTATE.animationDuration))

        
        self.cell.set(tile: nil)
        toCell.set(tile: self)
    }

    func remove(animated:Bool) {
        if animated {
            _pendingActions.add(SKAction.scale(to: 0, duration: GSTATE.animationDuration))
        }

        _pendingActions.add(SKAction.removeFromParent())

        weak var weakSelf = self
        _pendingBlock = {
            weakSelf?.removeFromParentCell()
        }

        self.commitPendingActions()
    }

    func removeWithDelay() {
        let wait = SKAction.wait(forDuration: GSTATE.animationDuration)
        let remove = SKAction.removeFromParent()

        self.run(SKAction.sequence([wait, remove])) {
            self.removeFromParentCell()
        }
    }

    func pop() -> SKAction {
        let d = 0.15 * CGFloat(GSTATE.tileSize())
        let wait = SKAction.wait(forDuration: GSTATE.animationDuration / 3)
        let enlarge = SKAction.scale(to: 1.3, duration: GSTATE.animationDuration / 1.5)
        let move = SKAction.move(by: CGVector(dx: -d, dy: -d), duration: GSTATE.animationDuration / 1.5)
        let restore = SKAction.scale(to: 1, duration: GSTATE.animationDuration /  1.5)
        let moveBack = SKAction.move(by: CGVector(dx: d, dy: d), duration: GSTATE.animationDuration / 1.5)

        return SKAction.sequence([wait, SKAction.group([enlarge, move]), SKAction.group([restore, moveBack])])
    }
}
