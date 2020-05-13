//
//  M2Cell.swift
//  m2048
//
//  Created by Skillz on 5/7/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import Foundation

class M2Cell {
    var position: M2Position
    private var tile: M2Tile?
    
    init(position: M2Position, tile: M2Tile) {
        self.position = position
        self.tile = tile
    }
    
    init(position: M2Position) {
        self.position = position
        self.tile = nil
    }
    
    func getTile() -> M2Tile? {
        return self.tile
    }
    
    func set(tile: M2Tile?) {
        self.tile = tile
        if (tile != nil) {
            tile!.cell = self
        }
    }
}
