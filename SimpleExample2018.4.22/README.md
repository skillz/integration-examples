# Simple Example Game Using Unity Version 2018.4.22

This repository contains an simple example cross-platform Unity game that integrates the Skillz SDK. It is intended to supplement the [documentation](https://cdn.skillz.com/doc/developer/unity/install_unity_sdk/) that walks users through the process of integrating a cross-platform Unity game.

## Build Environment

This project is integrated the Skillz SDK version 25.0.22. Check the [Downloads](https://developers.skillz.com/downloads) page for the latest version, and the instructions are located [here](https://docs.skillz.com/docs/installing-skillz-unity/)

The project was built on Unity 2018.4.22.

If you are experiencing trouble, please email integrations@skillz.com with a detailed description of the issue you are encountering.

### Build Instructions iOS

1. From the Unity menu select `File > Build Settings...`
2. In the dialog find the `Platform` section and click `iOS`
3. In the lower right on the dialog click the `Switch Platform` button - wait for Unity to complete switching platforms
4. In the lower right on the dialog click the `Build` button, and select a location to save the exported project
5. Just before the export completes, you will be prompted to select the location of the Skillz SDK for iOS navigate and select `<SKILLZ_SDK_ROOT>/Skillz.framework`
6. From the location of where you exported the project open the Xcode project `Unity-iPhone.xcodeproj`
7. From XCode select valid device - simulators are not supported
8. Update the Unity target `Signing and Capabilities > Signing` section with your correct Apple developer account info
9. Build and run the project on your device

### Build Instructions Android

1. From the Unity menu select `File > Build Settings...`
2. In the dialog find the `Platform` section and click `Android`
3. In the lower right on the dialog click the `Switch Platform` button
4. In the lower left on the dialog click the `Player Settings...` button
5. From the `Inspector` on the right side, on the `Android icon` tab expand the `Other Settings`
6. Set the following values
    - `Scripting Backend` to `IL2CPP`
    - `Target Architectures` check boxes `ARMv7` and `ARM64`

7. Back over to the Build Settings. In the lower right on the dialog click the `Build` button, and select a location to save the exported project
8. Startup Android Studio and import the save project, then build and run on a device
