//
//  M2Theme.swift
//  m2048
//
//  Created by Skillz on 5/8/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import Foundation
import UIKit

func RGB(r: CGFloat, g: CGFloat, b: CGFloat) -> UIColor {
    return UIColor.init(red: r/255.0, green: g/255.0, blue: b/255.0, alpha: 1.0)
}

class M2Theme : NSObject {

    /** The background color of the board base. */
    class func boardColor() -> UIColor {
        fatalError("This method must be overridden")
    }

    /** The background color of the entire scene. */
    class func backgroundColor() -> UIColor {
        fatalError("This method must be overridden")
    }

    /** The background color of the score board. */
    class func scoreBoardColor() -> UIColor {
        fatalError("This method must be overridden")
    }

    /** The background color of the button. */
    class func buttonColor() -> UIColor {
        fatalError("This method must be overridden")
    }

    /** The name of the bold font. */
    class func boldFontName() -> String {
        fatalError("This method must be overridden")
    }

    /** The name of the regular font. */
    class func regularFontName() -> String {
        fatalError("This method must be overridden")
    }

    /**
     * The color for the given level. If level is greater than 15, return the color for Level 15.
     *
     * @param level The level of the tile.
     */
    class func colorFor(level: Int) -> UIColor {
        fatalError("This method must be overridden")
    }

    /**
     * The text color for the given level. If level is greater than 15, return the color for Level 15.
     *
     * @param level The level of the tile.
     */
    class func textColorFor(level: Int) -> UIColor {
        fatalError("This method must be overridden")
    }
    
    class func themeClassFor(type: Int) -> M2Theme.Type {
        switch (type) {
        case 1:
            return M2VibrantTheme.self
        case 2:
            return M2JoyfulTheme.self
        default:
            return M2DefaultTheme.self
        }
    }
}

class M2DefaultTheme : M2Theme {
    override class func colorFor(level: Int) -> UIColor {
        switch (level) {
        case 1:
            return RGB(r:238, g:228, b:218);
        case 2:
            return RGB(r:237, g:224, b:200);
        case 3:
            return RGB(r:242, g:177, b:121);
        case 4:
            return RGB(r:245, g:149, b:99);
        case 5:
            return RGB(r:246, g:124, b:95);
        case 6:
            return RGB(r:246, g:94, b:59);
        case 7:
            return RGB(r:237, g:207, b:114);
        case 8:
            return RGB(r:237, g:204, b:97);
        case 9:
            return RGB(r:237, g:200, b:80);
        case 10:
            return RGB(r:237, g:197, b:63);
        case 11:
            return RGB(r:237, g:194, b:46);
        case 12:
            return RGB(r:173, g:183, b:119);
        case 13:
            return RGB(r:170, g:183, b:102);
        case 14:
            return RGB(r:164, g:183, b:79);
        case 15:
            fallthrough
        default:
            return RGB(r:161, g:183, b:63);
        }
    }

    override class func textColorFor(level: Int) -> UIColor {
        switch (level) {
        case 1:
            fallthrough
        case 2:
            return RGB(r:118, g:109, b:100)
        default:
            return UIColor.white
        }
    }


    override class func backgroundColor() -> UIColor {
        return RGB(r:250, g:248, b:239)
    }


    override class func boardColor() -> UIColor {
        return RGB(r:204, g:192, b:179)
    }


    override class func scoreBoardColor() -> UIColor {
        return RGB(r:187, g:173, b:160)
    }


    override class func buttonColor() -> UIColor {
        return RGB(r:119, g:110, b:101)
    }


    override class func boldFontName() -> String {
      return "AvenirNext-DemiBold"
    }

    override class func regularFontName() -> String {
      return "AvenirNext-Regular"
    }
}

class M2VibrantTheme : M2Theme {
    override class func colorFor(level: Int) -> UIColor {
        switch (level) {
        case 1:
            return RGB(r:254, g:223, b:180);
        case 2:
            return RGB(r:254, g:183, b:143);
        case 3:
            return RGB(r:253, g:187, b:45);
        case 4:
            return RGB(r:253, g:157, b:40);
        case 5:
            return RGB(r:246, g:124, b:95);
        case 6:
            return RGB(r:217, g:70, b:119);
        case 7:
            return RGB(r:210, g:65, b:97);
        case 8:
            return RGB(r:207, g:50, b:90);
        case 9:
            return RGB(r:205, g:35, b:84);
        case 10:
            return RGB(r:200, g:30, b:78);
        case 11:
            return RGB(r:190, g:20, b:70);
        case 12:
            return RGB(r:254, g:233, b:78);
        case 13:
            return RGB(r:249, g:191, b:64);
        case 14:
            return RGB(r:247, g:167, b:56);
        case 15:
            fallthrough
        default:
            return RGB(r:244, g:138, b:48);
        }
    }

    override class func textColorFor(level: Int) -> UIColor {
        switch (level) {
        case 1:
            fallthrough
        case 2:
            return RGB(r:150, g:110, b:90);
        case 3:
            fallthrough
        case 4:
            fallthrough
        case 5:
            fallthrough
        case 6:
            fallthrough
        case 7:
            fallthrough
        case 8:
            fallthrough
        case 9:
            fallthrough
        case 10:
            fallthrough
        case 11:
            fallthrough
        default:
            return UIColor.white
        }
    }

    override class func backgroundColor() -> UIColor {
        return RGB(r:240, g:240, b:240)
    }

    override class func boardColor() -> UIColor {
        return RGB(r:240, g:240, b:240)
    }

    override class func scoreBoardColor() -> UIColor {
        return RGB(r:253, g:144, b:38)
    }

    override class func buttonColor() -> UIColor {
        return RGB(r:205, g:35, b:85)
    }

    override class func boldFontName() -> String
    {
      return "AvenirNext-DemiBold"
    }

    override class func regularFontName() -> String {
      return "AvenirNext-Regular"
    }
}

class M2JoyfulTheme : M2Theme {
    override class func colorFor(level: Int) -> UIColor {
        switch (level) {
        case 1:
            return RGB(r:236, g:243, b:251)
        case 2:
            return RGB(r:230, g:245, b:252)
        case 3:
            return RGB(r:95, g:131, b:157)
        case 4:
            return RGB(r:164, g:232, b:254)
        case 5:
            return RGB(r:226, g:246, b:209)
        case 6:
            return RGB(r:237, g:228, b:253)
        case 7:
            return RGB(r:254, g:224, b:235)
        case 8:
            return RGB(r:254, g:235, b:115)
        case 9:
            return RGB(r:255, g:249, b:136)
        case 10:
            return RGB(r:208, g:246, b:247)
        case 11:
            return RGB(r:251, g:244, b:236)
        case 12:
            return RGB(r:254, g:237, b:229)
        case 13:
            return RGB(r:205, g:247, b:235)
        case 14:
            return RGB(r:57, g:120, b:104)
        case 15:
            fallthrough
        default:
            return RGB(r:93, g:125, b:62)
        }
    }

    override class func textColorFor(level: Int) -> UIColor {
        switch (level) {
        case 1:
            return RGB(r:104, g:119, b:131)
        case 2:
            return RGB(r:70, g:128, b:161)
        case 3:
            return UIColor.white
        case 4:
            return RGB(r:64, g:173, b:246)
        case 5:
            return RGB(r:97, g:159, b:42)
        case 6:
            return RGB(r:124,g: 85, b:201)
        case 7:
            return RGB(r:223,g: 73, b:115)
        case 8:
            return RGB(r:244, g:111, b:41)
        case 9:
            return RGB(r:253, g:160, b:46)
        case 10:
            return RGB(r:30, g:160, b:158)
        case 11:
            return RGB(r:147, g:129, b:115)
        case 12:
            return RGB(r:162, g:93, b:60)
        case 13:
            return RGB(r:68, g:227, b:184)
        case 14:
            fallthrough
        case 15:
            fallthrough
        default:
            return UIColor.white
        }
    }

    override class func backgroundColor() -> UIColor {
        return RGB(r:255, g:254, b:237)
    }

    override class func boardColor() -> UIColor {
        return RGB(r:255, g:254, b:237)
    }

    override class func scoreBoardColor() -> UIColor {
        return RGB(r:243, g:168, b:40);
    }

    override class func buttonColor() -> UIColor {
        return RGB(r:242, g:79, b:46)
    }

    override class func boldFontName() -> String {
      return "AvenirNext-DemiBold"
    }

    override class func regularFontName() -> String {
      return "AvenirNext-Regular"
    }
}
