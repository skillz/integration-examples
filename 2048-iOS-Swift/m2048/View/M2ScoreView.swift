//
//  M2ScoreView.swift
//  m2048
//
//  Created by Skillz on 5/11/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import UIKit

class M2ScoreView : UIView {

    @IBOutlet weak var title: UILabel?
    @IBOutlet weak var score: UILabel?

    override init(frame: CGRect) {
        super.init(frame: frame)
        self.commonInit()
    }

    required init(coder: NSCoder) {
        super.init(coder: coder)!
        self.commonInit()
    }

    private func commonInit() {
        self.layer.cornerRadius = CGFloat(GSTATE.cornerRadius)
        self.layer.masksToBounds = true
        self.backgroundColor = UIColor.green
    }

    func updateAppearance() {
        self.backgroundColor = GSTATE.scoreBoardColor()
        self.title?.font = UIFont(name: GSTATE.boldFontName(), size: 12)
        self.score?.font = UIFont(name: GSTATE.regularFontName(), size:16)
    }
}
