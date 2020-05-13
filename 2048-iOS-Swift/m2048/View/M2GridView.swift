//
//  M2GridView.swift
//  m2048
//
//  Created by Skillz on 5/8/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import UIKit

class M2GridView : UIView {
    override init(frame: CGRect) {
        super.init(frame: frame)
        self.backgroundColor = GSTATE.scoreBoardColor()
        self.layer.cornerRadius = CGFloat(GSTATE.cornerRadius)
        self.layer.masksToBounds = true;
    }

    convenience init()
    {
        let side = GSTATE.dimension * (GSTATE.tileSize() + GSTATE.borderWidth) + GSTATE.borderWidth
        let verticalOffset = Int(UIScreen.main.bounds.size.height) - GSTATE.verticalOffset;
        self.init(frame: CGRect(x: GSTATE.horizontalOffset, y: verticalOffset - side, width: side, height: side))
    }

    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }

    class func gridImage(withGrid: M2Grid) -> UIImage {
        let backgroundView = UIView(frame: UIScreen.main.bounds)
        backgroundView.backgroundColor = GSTATE.backgroundColor()

        let view = M2GridView()
        backgroundView.addSubview(view)

        withGrid.forEach(reverseOrder: false) { position in
            let layer = CALayer()
            let point = GSTATE.locationOf(position: position)

            var frame = layer.frame;
            frame.size = CGSize(width: GSTATE.tileSize(), height: GSTATE.tileSize())
            frame.origin = CGPoint(x: point.x, y: UIScreen.main.bounds.size.height - point.y - CGFloat(GSTATE.tileSize()))
            layer.frame = frame;

            layer.backgroundColor = GSTATE.boardColor().cgColor
            layer.cornerRadius = CGFloat(GSTATE.cornerRadius)
            layer.masksToBounds = true
            backgroundView.layer.addSublayer(layer)
        }

        return M2GridView.snapshot(withView: backgroundView)
    }

    class func gridImageWithOverlay() -> UIImage {
        let backgroundView = UIView(frame: UIScreen.main.bounds)
        backgroundView.backgroundColor = UIColor.clear
        backgroundView.isOpaque = false

        let view = M2GridView()
        view.backgroundColor = GSTATE.backgroundColor().withAlphaComponent(0.8)
        backgroundView.addSubview(view)

        return M2GridView.snapshot(withView: backgroundView)
    }

    class func snapshot(withView: UIView) -> UIImage {
        // This is a little hacky, but is probably the best generic way to do this.
        // [UIColor colorWithPatternImage] doesn't really work with SpriteKit, and we need
        // to take a retina-quality screenshot. But then in SpriteKit we need to shrink the
        // corresponding node back to scale 1.0 in order for it to display properly.
        UIGraphicsBeginImageContextWithOptions(withView.frame.size, withView.isOpaque, 0.0);
        withView.layer.render(in: UIGraphicsGetCurrentContext()!)
        let image = UIGraphicsGetImageFromCurrentImageContext()
        UIGraphicsEndImageContext();

        return image!
    }
}
