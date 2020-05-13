# Example Sync v2 Client App

## Overview

This is the source code for an example Unity app demonstrating how a game server would work for Sync v2. matches. This guide will walk you through running the game server and client app to play a simulated match, and explain key concepts that will be utilized for your own game server.

## Prerequisites

This was designed to work with the [example game server](../SyncV2ExampleServer/README.md). Please view its README on how to build and run it before continuing.

The app requires [Unity 2019.2](https://unity3d.com/get-unity/download/archive) in order to run. Finally, the commands listed in this README are unix/mac based. If you are on Windows, please run the corresponding DOS or Powershell commands.

Note: Proficiency in C# is a must in order to understand the code.

## Getting started

This section will detail how to run the client app, and assumes you have started the example game server.

### Configure the Client App

Before you run the app, please configure it to point to the IP address of your example game server instance. To do so, first open the Unity project and select the `Assets/Sync v2 Server/ConnectArgs` asset. Its properties pane will look similar to:

![ConnectArgs properties](./ConnectArgs.png)

* The `Url` field should be the IP address of the machine running the example server.
* The `Port` is `10140`, but be sure to enter a different value if you modify in the example server.
* The `Match Id` can be any value you choose.
* For `Match Token`, the example server accepts `dummytoken`. When you actually integrate with the Skillz, the SDK will provide an RSA encrypted token when a Sync v2 match is about to begin. Your actual game server (not the example server), needs to decrypt this token. It will contain a timestamp and if the time is older than a threshold of your choosing, your server will need to reject the client connection. This is to ensure that clients who connect came from an actual Skillz Match Maker server.

Afterwards, open the scene named `StartScene`. Press the Play button in the Unity editor. You should be able to connect to the game server. Note that you will see a timeout dialog after about 15 seconds because no opponent joined within that window of time.


### Playing a Match

To play a match, we recommend you export to Android Studio and/or Xcode, compile and run the app from two phones. Remember to point the game to the correct URL/IP address from Unity beforehand :-).

## Explanation of key game server concepts

This section will describe key concepts that will be utilized when interacting with a Sync v2 game server:

1. Connecting to the game server/entering a sync match
2. Sending messages to the server
3. Receiving messages from the server

### Essential code pieces

Before explaining the concepts for interacting with the game server, it is worth calling out the main code pieces that were made for this demo app.

First, there is the [SyncServerController](./Assets/Scripts/Servers/SyncServerController.cs) class. It represents the game server that you will connect to, and send/receive messages. The connection is maintained via a TCP socket with SSL used as the transport layer.

Second, the `./Assets/Scripts/Servers/Packets/` subfolder contains the definitions for the messaging protocol used between the client and game server.

Let's now dive into how interacting with the game server will work.

### Connecting to the game server/entering a sync match

Entering a sync match on the game server is closely coupled with connecting to the game server. For security reasons, the connection to the game server is secured via TLS/SSL. When connecting to the game server, there are two major steps involved.

First, the client will validate the server. There is the possibility that someone could attempt to spoof a game server and corrupt a match. To circumvent this, the game server will store a TLS/SSL Certificate provided from a trusted Certificate Authority. In turn, the client will store the certificate's public key string somewhere, and when connecting to the server, it will compare the public key that it *expects* to the one provided by the server it is connecting to. If the keys match then we know that the game server is legitimate; otherwise, the client should close the connection immediately.

This client app stores the server's public key in the `TLS Configuration` asset within the Unity editor.

The `SyncServerController` class verifies that the server being connected to is legit [here](./Assets/Scripts/Servers/SyncServerController.cs#L487).

This process is known as certificate pinning. Refer to [this article](https://www.owasp.org/index.php/Certificate_and_Public_Key_Pinning) for more information.

Second, the game server will validate the client. We want to ensure that legit players are entering the match. The client will send a message to enter a match, encapsulated by the [ConnectArgs](./Assets/Scripts/Servers/ConnectArgs.cs) class. This message will contain an encrypted token for the match. The token contains information identifying the player and match, including a date-timestamp. The server will decrypt the token and validate the data contained within. If either the date-timestamp is out-of-date, or the other information is invalid, the client is considered illegitimate; the server will immediately close the connection. Check out the [TryHandshake method](./Assets/Scripts/Servers/SyncServerController.cs#L402) to see how this message is sent.

If both server and client validation are a success, the client app will enter the match. Note: the server will not send game state update packets until both players have entered the match.

### Sending messages to the game server

Messages sent to the server are represented by some of the classes in the `./Assets/Scripts/Servers/Packets/` folder. These are enqueued to a Producer-Consumer [queue](./Assets/Scripts/Servers/SyncServerController.cs#L105) of type [SafeQueue](./Assets/Scripts/Collections/SafeQueue.cs) in order to prevent the UI thread from being blocked. Messages will be enqueued by the UI thread, while a dedicated send thread will dequeue messages and send them. See the [SendPacketsThread method](./Assets/Scripts/Servers/SyncServerController.cs#L302).

These messages are sent as [FlatBuffers](https://google.github.io/flatbuffers/). FlatBuffers are a lightweight serialization mechanism for sending data over the wire. Check out the excellent tutorial [here](https://google.github.io/flatbuffers/flatbuffers_guide_tutorial.html). More information on the client app's message protocol is below.

### Receiving messages from the game server

The [ReceivePacketsThread method](./Assets/Scripts/Servers/SyncServerController.cs#L236) will receive a generic packet from the server every TICK_RATE as a sort of "heartbeat". The TICK_RATE is configurable. Each packet received is enqueued into another Producer-Consumer [queue](./Assets/Scripts/Servers/SyncServerController.cs#L104). The client is expected to drain and process the packets in the queue as quickly and as often as possible. This client app does it on every frame update [here](./Assets/Scripts/GameController.cs#L100).

NOTE: If the client does not [receive](./Assets/Scripts/Servers/SyncServerController.cs#L453) a message within 15 seconds, the match is considered aborted as this implies there is a problem with the connection between the client and server.

See the description of the message protocol in section below.

### Client Interpolation (Smooth Animations)

Given that the server is updating the clients on a looping tick, if the client only renders updates on receipt of a `GameStateUpdatePacket`, then the player will see a stuttering visual. We want the game to visually update every frame, and interpolate between the data that we get from the server every tick. The server sends this `tickRate` as an int in milliseconds in the `MatchSuccessPacket` on game start.

Rather than immediately updating player position when a new packet is received, we first store the current position of a player as a `previousPosition` and store the new position as `desiredPosition`. Whenever we receive a `GameStateUpdatePacket` we repeat this process.

After changing the Unity Project's Time Settings (Edit -> Project Settings -> Time), the [FixedUpdate](./Assets/Scripts/GameController.cs#L148) method runs once every `0.005 seconds` (`5 milliseconds`) and keeps track of the time since the last `GameStateUpdatePacket` was received. This `deltaTimer` is divided by the `tickRate` (in ms) and gives us a progress percentage for interpolating the player position.

In the [DrawAllCircles](./Assets/Scripts/GameController.cs#L148) method, we calculate the interpolation progress and use `Vector2.Lerp` to linerally interpolate between the `previousPosition` and the `desiredPosition`. This is called from the [Update](./Assets/Scripts/GameController.cs#L87) method, which aligned with unity frame render udpates (this gives us smoother updating than if we set the position in `FixedUpdate`).

In essence, the player is always interpolating towards the `desiredPosition` at a rate specified by the time since last game update and server tick rate.

### Message Protocol

Data is packaged as FlatBuffers.

* The server will broadcast the game state each time it receives a `PlayerUpdatePacket` from a client.
* `GameStateUpdatePacket`s are sent to clients every update tick to provide the current game state. The tick rate is configurable.
* Once one client has sent an abort message, the server will broadcast a `GameOverPacket` and then each client closes the connection and returns to the start screen.

#### Server to Client Messages

* `GameStateUpdatePacket` - Sent to clients when the game state is updated. Clients should update the UI accordingly.
* `MatchSuccessPacket` - Sent after the required # of clients have connected to indicate that the match can begin.
* `GameOverPacket` - Sent when the game ends, and contains a property that indicates if the game ended successfully or was aborted due to one or more players being inactive.

#### Client-to-Server messages

* `SuccessPacket` - Sent immediately back to the client after the server has successfully received a message.
* `ConnectPacket` - Sent by a client after it has successfully connected to the server and gone through a successful handshake. This indicates that they are joining the match.
* `AbortPacket` - Sent by a client to indicate that they want to end the match successfully (meaning, there were no inactive users due to being disconnected for ~15 secs).
* `PlayerUpdatePacket` - Sent by a client each time their portion of the game state is updated.