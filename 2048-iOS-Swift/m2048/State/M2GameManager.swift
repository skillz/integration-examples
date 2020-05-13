//
//  M2GameManageer.swift
//  m2048
//
//  Created by Jerry Lin on 5/11/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import Foundation
import QuartzCore

enum M2Direction: Int {
    case up
    case left
    case down
    case right
}

/**
 * Helper function that checks the termination condition of either counting up or down.
 *
 * @param value The current i value.
 * @param countUp If YES, we are counting up.
 * @param upper The upper bound of i.
 * @param lower The lower bound of i.
 * @return YES if the counting is still in progress. NO if it should terminate.
 */
func iterate(value: Int, countUp: Bool, upper: Int, lower: Int) -> Bool {
    return countUp ? value < upper : value > lower
}

class M2GameManager : NSObject {
    /** True if game over. */
    private var _over: Bool

    /** True if won game. */
    private var _won: Bool

    /** True if user chooses to keep playing after winning. */
    private var _keepPlaying: Bool

    /** The current score. */
    private var _score: Int

    /** The points earned by the user in the current round. */
    private var _pendingScore: Int

    /** The grid on which everything happens. */
    private var _grid: M2Grid?

    /** The display link to add tiles after removing all existing tiles. */
    private var _addTileDisplayLink: CADisplayLink
    
    override init() {
        self._over = false
        self._won = false
        self._keepPlaying = false
        self._score = 0
        self._pendingScore = 0
        self._grid = nil
        self._addTileDisplayLink = CADisplayLink()
        
        super.init()
    }

    func startNewSession(withScene: M2Scene) {
        if _grid != nil {
            _grid?.removeAllTiles(animated: false)
        }
        if _grid == nil || _grid?.dimension != GSTATE.dimension {
            _grid = M2Grid(dimension: GSTATE.dimension)
            _grid?.scene = withScene;
        }

        withScene.loadBoard(withGrid: _grid!)

        // Set the initial state for the game.
        _score = 0
        _over = false
        _won = false
        _keepPlaying = false

        // Existing tile removal is async and happens in the next screen refresh, so we'd wait a bit.
        _addTileDisplayLink = CADisplayLink(target: self, selector: #selector(addTwoRandomTiles))
        _addTileDisplayLink.add(to: RunLoop.current, forMode: RunLoop.Mode.default)
    }

    @objc func addTwoRandomTiles() {
        // If the scene only has one child (the board), we can proceed with adding new tiles
        // since all old ones are removed. After adding new tiles, remove the displaylink.
        if _grid?.scene?.children.count ?? 0 <= 1 {
            _grid?.insertTileAtRandomAvailablePosition(withDelay: false)
            _grid?.insertTileAtRandomAvailablePosition(withDelay: false)
            _addTileDisplayLink.invalidate()
        }
    }

    func move(toDirection: M2Direction) {
        var tile: M2Tile? = nil

        // Remember that the coordinate system of SpriteKit is the reverse of that of UIKit.
        let reverse = toDirection == M2Direction.up || toDirection == M2Direction.right
        let unit: Int = reverse ? 1 : -1;

        if toDirection == M2Direction.up || toDirection == M2Direction.down {
            _grid?.forEach(reverseOrder: reverse) { position in
                tile = self._grid?.tileAt(position: position)
                if tile != nil {
                    // Find farthest position to move to.
                    var target = position.x;
                    
                    var i = position.x + unit
                    while iterate(value: i, countUp: reverse, upper: self._grid!.dimension, lower: -1) {
                        let t: M2Tile? = self._grid?.tileAt(position: M2Position(x: i, y: position.y))
                        // Empty cell; we can move at least to here.
                        if (t == nil) {
                            target = i
                        } else {
                            // Try to merge to the tile in the cell.
                            var level = 0;

                            if GSTATE.gameType == M2GameType.PowerOf3 {
                                let further = M2Position(x: i + unit, y: position.y);
                                let ft = self._grid?.tileAt(position: further)
                                if ft != nil {
                                    level = tile!.merge3(toTile: t, andTile: ft!)
                                }
                            } else {
                                level = tile!.merge(toTile: t)
                            }

                            if level != 0 {
                                target = position.x;
                                self._pendingScore = GSTATE.valueFor(level: level)
                            }

                            break;
                        }

                        i += unit
                    }

                    // The current tile is movable.
                    if (target != position.x) {
                        tile!.move(toCell: (self._grid?.cellAt(position: M2Position(x: target, y: position.y)))!)
                        self._pendingScore = self._pendingScore + 1
                    }
                }
            }
        } else {
            self._grid?.forEach(reverseOrder: reverse) { position in
                tile = self._grid?.tileAt(position: position)
                if tile != nil {
                    var target = position.y;
                    var i = position.y + unit
                    while iterate(value: i, countUp: reverse, upper: self._grid!.dimension, lower: -1) {
                        var t = self._grid?.tileAt(position: M2Position(x: position.x, y: i))

                        if (t == nil) {
                            target = i
                        } else {
                            var level = 0;

                            if GSTATE.gameType == M2GameType.PowerOf3 {
                                var further = M2Position(x: position.x, y: i + unit);
                                var ft = self._grid?.tileAt(position: further)
                              if ft != nil {
                                level = tile!.merge3(toTile: t, andTile: ft!)
                              }
                            } else {
                                level = tile!.merge(toTile: t)
                            }

                            if level != 0 {
                                target = position.y;
                                self._pendingScore = GSTATE.valueFor(level: level)
                            }

                            break;
                        }

                        i += unit
                    }
                    
                    // The current tile is movable.
                    if target != position.y {
                        tile!.move(toCell: (self._grid?.cellAt(position: M2Position(x: position.x, y: target)))!)
                        self._pendingScore = self._pendingScore + 1
                    }
              }
            }
        }
          
        // Cannot move to the given direction. Abort.
        if _pendingScore == 0 {
            return
        }
          
        // Commit tile movements.
        self._grid?.forEach(reverseOrder: reverse) { position in
            tile = self._grid?.tileAt(position: position)
            if tile != nil {
                tile?.commitPendingActions()
                if tile?.level ?? -1 >= GSTATE.winningLevel() {
                    self._won = true
                }
            }
        }
          
        // Increment score.
        self.materializePendingScore()

          // Check post-move status.
        if !_keepPlaying && _won {
            // We set `keepPlaying` to YES. If the user decides not to keep playing,
            // we will be starting a new game, so the current state is no longer relevant.
            _keepPlaying = true;
            self._grid?.scene?.controller?.endGame(won: true)
        }
            
        // Add one more tile to the grid.
        self._grid?.insertTileAtRandomAvailablePosition(withDelay: true)
        if GSTATE.dimension == 5 && GSTATE.gameType == M2GameType.PowerOf2 {
            _grid?.insertTileAtRandomAvailablePosition(withDelay: true)
        }

        if !self.movesAvailable() {
            _grid?.scene?.controller?.endGame(won: false)
        }
    }

    func materializePendingScore() {
        _score += _pendingScore;
        _pendingScore = 0;
        _grid?.scene?.controller?.update(score: _score)
    }

    /**
     * Whether there are moves available.
     *
     * A move is available if either there is an empty cell, or there are adjacent matching cells.
     * The check for matching cells is more expensive, so it is only performed when there is no
     * available cell.
     *
     * @return YES if there are moves available.
     */
    func movesAvailable() -> Bool {
        return _grid?.hasAvailableCells() ?? false || self.adjacentMatchesAvailable()
    }

    /**
     * Whether there are adjacent matches available.
     *
     * An adjacent match is present when two cells that share an edge can be merged. We do not
     * consider cases in which two mergable cells are separated by some empty cells, as that should
     * be covered by the `cellsAvailable` function.
     *
     * @return YES if there are adjacent matches available.
     */
    func adjacentMatchesAvailable() -> Bool {
        let to = _grid?.dimension ?? 0
        for i in 0..<to {
            for j in 0..<to {
                // Due to symmetry, we only need to check for tiles to the right and down.
                let tile = _grid?.tileAt(position: M2Position(x: i, y: j))

                // Continue with next iteration if the tile does not exist. Note that this means that
                // the cell is empty. For our current usage, it will never happen. It is only in place
                // in case we want to use this function by itself.
                if tile == nil {
                    continue;
                }
                
                if GSTATE.gameType == M2GameType.PowerOf3 {
                    if ((tile!.canMerge(withTile: _grid?.tileAt(position: M2Position(x: i + 1, y: j))) &&
                         tile!.canMerge(withTile: _grid?.tileAt(position: M2Position(x: i + 2, y: j)))) ||
                        (tile!.canMerge(withTile: _grid?.tileAt(position: M2Position(x: i, y: j + 1))) &&
                         tile!.canMerge(withTile: _grid?.tileAt(position: M2Position(x: i, y: j + 2))))) {
                      return true;
                    }
                } else {
                    if (tile!.canMerge(withTile: _grid?.tileAt(position: M2Position(x: i + 1, y: j))) ||
                        tile!.canMerge(withTile: _grid?.tileAt(position: M2Position(x: i, y: j + 1)))) {
                      return true;
                }
              }
            }
        }

        // Nothing is found.
        return false;
    }
}
