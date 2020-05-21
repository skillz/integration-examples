# "2048" Example Game Showing Skillz iOS (Objective-C) Integration

This repository contains an example iOS game that integrates the Skillz SDK. It is intended to supplement the [documentation](https://cdn.skillz.com/doc/developer/ios_native/integrate_skillz_sdk/install_skillz_via_xcode/) that walks users through the process of integrating an iOS game.

All credits to the design of this game belongs to:
* Danqing Liu
* Scott Matthewman
* Sihao Lu

Original code: https://github.com/danqing/2048

Use governed by the MIT License.

## Build Environment
This project is integrated the Skillz SDK version 25.0.25. Check the [Downloads](https://developers.skillz.com/downloads) page for the latest version, and the instructions are located [here](https://docs.skillz.com/docs/installing-skillz-ios/)

The project was built on Xcode 11.

If you are experiencing trouble, please email integrations@skillz.com with a detailed description of the issue you are encountering.

### Build Instructions iOS
1. From the project root open the Xcode project `m2048.xcodeproj`
2. From XCode select valid device - simulators are not supported
3. Update the m2048 target `Signing and Capabilities > Signing` section with your correct Apple developer account info
4. Build and run the project on your device
