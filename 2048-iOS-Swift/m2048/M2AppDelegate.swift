//
//  M2AppDelegate.swift
//  m2048
//
//  Created by Skillz on 5/12/20.
//  Copyright © 2020 Danqing. All rights reserved.
//

import UIKit
import Skillz

@UIApplicationMain
class M2AppDelegate : UIResponder, UIApplicationDelegate, SkillzDelegate {
    var window: UIWindow?

    func tournamentWillBegin(_ gameParameters: [AnyHashable : Any], with matchInfo: SKZMatchInfo) {
        let viewController = self.window?.rootViewController as! M2ViewController
        viewController.startNewGame()
    }

    func skillzWillExit() {
        // Optional. This method is called when the Skillz UI is exiting,
        // usually via from the Skillz UI's sidebar menu.
    }

    func application(_: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey : Any]? = nil) -> Bool {
        Skillz.skillzInstance().initWithGameId("5465", for: self, with: SkillzEnvironment.sandbox, allowExit: false)
 
        return true;
    }

    func applicationWillResignActive(_: UIApplication) {
        // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
        // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
    }

    func applicationDidEnterBackground(_: UIApplication) {
        // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
        // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
    }

    func applicationWillEnterForeground(_: UIApplication) {
        // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
    }

    func applicationDidBecomeActive(_: UIApplication) {
        // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
    }

    func applicationWillTerminate(_: UIApplication) {
        // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
    }

    func application(_: UIApplication, supportedInterfaceOrientationsFor: UIWindow?) -> UIInterfaceOrientationMask {
        return UIInterfaceOrientationMask(rawValue: (UIInterfaceOrientationMask.portrait.rawValue | UIInterfaceOrientationMask.portraitUpsideDown.rawValue))
    }
}
