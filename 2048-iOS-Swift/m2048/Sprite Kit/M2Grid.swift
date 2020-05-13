//
//  M2Grid.swift
//  m2048
//
//  Created by Jerry Lin on 5/8/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import Foundation
import SpriteKit
import Skillz

class M2Grid : NSObject {
    /** The dimension of the grid. */
    var dimension: Int

    /** The scene in which the game happens. */
    var scene: M2Scene?
    
    var _grid: NSMutableArray

    /**
     * Initializes a new grid with the given dimension.
     *
     * @param dimension The desired dimension, i.e. # cells in a row or column.
     */
    init(dimension: Int) {
        // Set up the grid with all empty cells.
        _grid = NSMutableArray(capacity: dimension)
        
        for i in 0..<dimension {
            let array = NSMutableArray(capacity: dimension)
            
            for j in 0..<dimension {
                array.add(M2Cell(position: M2Position(x:i, y:j)))
            }

            _grid.add(array)
        }
        
        // Record the dimension of the grid.

        self.dimension = dimension

        super.init()
    }

    /**
     * Iterates over the grid and calls the block, which takes in the M2Position
     * of the current cell. Has the option to iterate in the reverse order.
     *
     * @param block The block to be applied to each cell position.
     * @param reverse If YES, iterate in the reverse order.
     */
    func forEach(reverseOrder: Bool, iteratorBlock: (M2Position) -> Void) {
        if !reverseOrder {
            let upper = self.dimension
            for i in 0..<upper {
                for j in 0..<upper {
                    iteratorBlock(M2Position(x:i, y:j));
                }
            }
        } else {
            for i in (0..<self.dimension).reversed() {
                for j in (0..<self.dimension).reversed() {
                    iteratorBlock(M2Position(x:i, y:j));
                }
            }
        }
    }

    /**
     * Returns the cell at the specified position.
     *
     * @param position The position we are interested in.
     * @return The cell at the position. If position out of bound, returns nil.
     */
    func cellAt(position: M2Position) -> M2Cell? {
        if position.x >= self.dimension || position.y >= self.dimension || position.x < 0 || position.y < 0 {
            return nil
        }
        return (_grid.object(at: position.x) as! NSMutableArray).object(at: position.y) as? M2Cell
    }

    /**
     * Returns the tile at the specified position.
     *
     * @param position The position we are interested in.
     * @return The tile at the position. If position out of bound or cell empty, returns nil.
     */
    func tileAt(position: M2Position) -> M2Tile? {
        let cell = self.cellAt(position: position)
        return cell != nil ? cell?.getTile() : nil;
    }

    /**
     * Whether there are any available cells in the grid.
     *
     * @return YES if there are at least one cell available.
     */
    func hasAvailableCells() -> Bool {
        return self.availableCells().count != 0
    }
    
    /**
     * Returns all available cells in an array.
     *
     * @return The array of all available cells. If no cell is available, returns empty array.
     */
    func availableCells() -> NSArray {
        let array = NSMutableArray(capacity: self.dimension * self.dimension)
        self.forEach(reverseOrder: false) { position in
            let cell = self.cellAt(position: position)
            if cell != nil && cell?.getTile() == nil {
                array.add(cell!)
            }
        }

        return array;
    }

    /**
     * Inserts a new tile at a randomly chosen position that's available.
     *
     * @param delay If YES, adds twice `animationDuration` long delay before the insertion.
     */
    func insertTileAtRandomAvailablePosition(withDelay: Bool) {
        let cell = self.randomAvailableCell()
        if cell != nil {
            let tile = M2Tile.insertNewTile(toCell: cell!)
            self.scene?.addChild(tile)

            let delayAction: SKAction = withDelay
                ? SKAction.wait(forDuration: GSTATE.animationDuration * 3)
                : SKAction.wait(forDuration: 0)
            
            let move: SKAction = SKAction.move(by: CGVector(dx: -GSTATE.tileSize() / 2, dy: -GSTATE.tileSize() / 2), duration: GSTATE.animationDuration)
            let scale: SKAction = SKAction.scale(to: 1.0, duration: GSTATE.animationDuration)
            tile.run(SKAction.sequence([delayAction, SKAction.group([move, scale])]))
        }
    }

    /**
     * Returns a randomly chosen cell that's available.
     *
     * @return A randomly chosen available cell, or nil if no cell is available.
     */
    func randomAvailableCell() -> M2Cell? {
        let availableCells = self.availableCells()
        if availableCells.count != 0 {
            let randomIndex: UInt = Skillz.skillzInstance().tournamentIsInProgress
                ? Skillz.getRandomNumber(withMin: 0, andMax: UInt(availableCells.count))
                : UInt(arc4random_uniform(UInt32(UInt(availableCells.count))))

            return availableCells.object(at: Int(randomIndex)) as? M2Cell
        }
        return nil;
    }

    /**
     * Removes all tiles in the grid from the scene.
     *
     * @param animated If YES, animate the removal.
     */
    func removeAllTiles(animated: Bool) {
        self.forEach(reverseOrder: false) { position in
            let tile = self.tileAt(position: position)
            if tile != nil {
                tile?.remove(animated: animated)
            }
        }
    }
}
