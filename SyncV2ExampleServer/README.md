

# Server Authoritative Sync Server

## An Important Note!
It is imperative that the your game logic follows a server-authoritative model.

An authoritative game server 'owns' the game state to protect against unauthorized operations by a client (e.g. cheating/hacking)

For example, in an authoritative game server model, the client reports information such as *"I've clicked the buy button"*, and the server processes
that input and tells the client to add an item to their inventory. This is opposed to a client-authoritative model where the client would
tell the server *"I bought an item and now its in my inventory"*. The player cannot take any action that is not directed
and verified by the server.

In a more complicated example, imagine we're building a racing game and we receive input from the player saying
*"I'm pressing the gas button and steering straight ahead"*. The server would be responsible for looking up the players position, ensuring
that the players' car has room to move forward (e.g. running a collision check to see it's not hitting a wall), and
updating the position of the player's car object in memory.

On the next `broadcast()` loop, we would build an update packet that contains the location of all of the cars on the server, and
send that to the client for rendering.

In this model, it is **impossible** for the client to tell the server that its car is in a specific position, preventing
cheating and stopping a hacker from forcing their car to move faster than it should, or passing through walls, etc.
All the player can do is report what it would like to happen, and the server verifies and executes the game logic based on those inputs.

The example Unity client in this repo is authoritative in that the player's circles are only drawn in a new location
when told to update that location from the game server. I.e., we only update YOUR circle's position after your client has
asked the server to do so, rather than drawing it in the new location as soon as we detect a touch on the screen locally.

For more in-depth information regarding designing and developing scalable, authoritative game servers, take a look at these resources:
- [Fast-Paced Multiplayer (Part I): Client-Server Game Architecture](https://www.gabrielgambetta.com/client-server-game-architecture.html)
- [Development and Deployment of Multiplayer Online Games:from social games to MMOFPS, with stock exchanges in between](http://ithare.com/contents-of-development-and-deployment-of-massively-multiplayer-games-from-social-games-to-mmofps-with-stock-exchanges-in-between/)

## Quick Architecture Overview
This test sync server implements a very simple authoritative TLS Java server using Flatbuffers running on the Netty framework.

FlatBuffers are a lightweight serialization mechanism for sending data over the wire.

Please see this tutorial for more information on how to use Flatbuffers: https://google.github.io/flatbuffers/flatbuffers_guide_use_java_c-sharp.html

The Netty Java documentation is available here: https://netty.io/wiki/

## Before You Begin ##
These instructions assume you are working on a mac. Please ensure you have the following dependencies installed:
1. Install flatc
```brew install flatbuffers```
2. Install Amazon Java 11 Coretto SDK: https://docs.aws.amazon.com/corretto/latest/corretto-11-ug/downloads-list.html
3. Install Gradle
```brew install gradle```
4. Install IntelliJ 2019.3 or higher: https://www.jetbrains.com/idea/download/#section=mac

## IntelliJ Project Setup ##
This project uses Gradle (and Gradle Wrapper v6.0) to build and run. Ensure you have the Amazon Java 11 Coretto SDK intstalled and
open the project in IntelliJ version 2019.3 or higher.

Select the "Open Project" option and choose the `build.gradle` file in `integration-examples/SyncV2ExampleServer`

Once the gradle project has been indexed, open the Gradle Task list and double click "build".
Once a build has run once, create a new Run Configuration using the Jar Application template.
Select the newly built `integration-examples/SyncV2ExampleServer/build/libs/src-1.0-SNAPSHOT.jar`
and add the following Gradle Tasks to the "Run before Build" list in the following order:
1. clean
2. cleanCreateFlatBuffers
3. createFlatBuffers
4. build
5. copyTask
6. createFlatBuffersCSharp (for C# classes) or createFlatBuffersCPP (for C++ client header files)

Ensure "Allow Parallel Run" is unchecked in the top right.

This new run configuration will build and run the new jarfile as well as export the CSharp Flatbuffer source to "csharp_flatbuffer_gen.zip".
See the FlatBuffer documentation for information on generating sourcefiles in more langauges.

## Server Flow ##
This section will walk you through the process that occurs when a new client connects to the server and sends and receives
initial packets.

When a client opens a connection to the server over TCP, the first thing we do is establish an SSL stream using TLSv1.2.
This ensure that all data sent to or received from the server is encrypted, which is vital for ensuring fairness.

After the SSL stream has been opened, we send a CONNECT from the client. All of our packets in this example are built using
Flatbuffers, which is an efficient data serialization method. The Flatbuffer file is defined using a scheme file.

The schema file for the CONNECT packet looks like this:
```
include "Packet.fbs";

namespace SyncServer;

table ConnectPacket {
    opcode: short; // 1
    userId: long;
    matchId: string;
    externalToken: string;
}

root_type Packet;
```

The `table` type is used to define the main structure of our packet. You'll notice that we specify a `root_type` of `Packet`.
The `Packet` type is a `table` that contains only a `short` labeled `opcode`, and is used as the parent type for all
packets that we write. The opcode is used to determine what kind of packet
we are sending to/from the server. You can look at the logic in the `channelRead` method in `MessageHandler.java` to see how
we cast the incoming byte buffer to a generic `Packet` before checking the `opcode` and using the java ENUM `ClientToServerMessage` to call
the right handler method based on the `opcode`.

For example, when the CONNECT packet is sent, we determine the `opcode` is equal to `1` and then call the `public void handle(World world, Player player, Table packet)`
in the `CONNECT` ENUM in `ClientToServerMessage`, which casts the byte buffer to a `ConnectPacket` type, which then gives native
access the the data enclosed. This decoding of `byte[]` to `SomePacket` requires no copies or parsing and is one of
the huge benefits of Flatbuffers.

The flow is nearly the same for creating and sending packets from the server to the client, except that we define `write` functions, not
`handle` functions in an ENUM in `MessageHandler.java`.

For example, to send a `GAME_STATE_UPDATE` packet to the client, we call the `write` function defined in the `ServerToClientMessage` ENUM.
This ENUM contains write functions for each packet that can be sent to the client. Take a look at the `write` function
in the ENUM for `GAME_STATE_UPDATE`:

```java
public enum ServerToClientMessage {
        GAME_STATE_UPDATE(5) {
            @Override
            public void write(Player player, List<Player> players) { //
                // We create a new FlatBufferBuilder with an initial size of 16
                FlatBufferBuilder builder = new FlatBufferBuilder(16);

                // Here we are creating a Vector containing a PlayerData table for each player in the game
                GameStateUpdatePacket.startPlayersVector(builder, players.size());
                for (Player thePlayer : players) {
                    // Create a PlayerData object for each player
                    PlayerData.createPlayerData(builder, thePlayer.getUserId(), thePlayer.getPosition().x, thePlayer.getPosition().y, thePlayer.isActive());
                }
                int playersOffset = builder.endVector();

                // We call the static method "startGameStateUpdatePacket" on the GameStateUpdatePacket class
                // This tells the FlatBufferBuilder what kind of object we're creating
                GameStateUpdatePacket.startGameStateUpdatePacket(builder);

                // IMPORTANT: This is where we define the packet opcode -- this MUST MATCH the opcode defined in the handler function in MessageHandler
                GameStateUpdatePacket.addOpcode(builder, (short) GAME_STATE_UPDATE.opcode);

                // We add the list of PlayerData to the GameStateUpdatePacket buffer
                GameStateUpdatePacket.addPlayers(builder, playersOffset);

                // We add the current game time to the packet
                GameStateUpdatePacket.addGameTime(builder, player.getGame().getGameTime());

                // Finally we "end" the packet -- this function returns the an int that points to the root of our data
                // in the FlatBufferBuilder
                int gameStateUpdatePacketOffset = GameStateUpdatePacket.endGameStateUpdatePacket(builder);

                // We pass this root offset to the builder.finish function, which finishes creating the byte array
                // inside of hte FlatBufferBuilder object
                builder.finish(gameStateUpdatePacketOffset);

                // We write byte[] to the socket
                player.write(builder.sizedByteArray());
            }
        },
<snip>
```
Please see the [Flatbuffer Tutorial](https://google.github.io/flatbuffers/flatbuffers_guide_tutorial.html)
for an in-depth guide on how to use Flatbuffers with code examples in many different programming languages.

The code below is taken from `Game.java` and shows how the game loops through and broadcasts to all players:

```java
public void broadcast() {
    // If we're broadcasting then the game has started
    started = true;

    // Loop through all the players in the server
    for (Player player : players) {
        logger.info("Broadcast Game State");
        MessageHandler.ServerToClientMessage.GAME_STATE_UPDATE.write(player, players);
    }
}
```

This `broadcast()` function is called every `TICK_RATE` in milliseconds as defined in `Game.java`.
We create a `HashedWheelTimer` for each `Game` once `MIN_PLAYERS_PER_GAME` number of players have joined:
```java
 HashedWheelTimer pulse = new HashedWheelTimer();
    pulse.newTimeout(timeout -> {

        <snip>

        // If the game is readyToStart, that means we need to be broadcasting updates
        // game.broadcast sends a GameStateUpdatePacket to all clients
        if (game.readyToStart()) {
            game.broadcast();

            // tell the timer to pulse again at the specified tick rate
            pulse.newTimeout(timeout.task(), Game.TICK_RATE, TimeUnit.MILLISECONDS);
        }

        <snip>

    }, Game.TICK_RATE, TimeUnit.MILLISECONDS);
```

In order to add a new packet type, you create a Flatbuffer source `.fbs` file in the `main/flatbuffers` folder.
Running the `createFlatBuffers` Gradle task will compile the `.fbs` files to `.java` files in `generated/flatbuffers/SyncServer` folder.

You can then create your `ServerToClientMessage` / `ClientToServerMessage` ENUM entry and implement `handle` / `write` functions in
order to send/receive that packet over the socket.

Let's take a look at what happens when we receive a `PlayerUpdatePacket`, which contains new information about the state of the player:

```java
<snip>

PLAYER_UPDATE(4) {
            @Override
            public void handle(World world, Player player, Table packet) {
                PlayerUpdatePacket playerUpdatePacket = PlayerUpdatePacket.getRootAsPlayerUpdatePacket(packet.getByteBuffer());

                player.setPosition(playerUpdatePacket.posX(), playerUpdatePacket.posY());

                ServerToClientMessage.SUCCESS.write(player, true);
            }
        },

<snip>
```
First we convert the generic packet to our `PlayerUpdatePacket` type. Then we set access the `posX` and `posY` variables
in the packet, and call the `setPosition` function on the `player` object to update their information.

The `write` function for `GameStateUpdatePacket` loops through all of the players in the game, and builds an update
packet using the position data stored in that `player` object.

## main.Server.java (Main)
This is the Main file of the program that handles setting up Netty and deciding where to pass around connections

1. Creates the main.World object, which holds the state of all running matches
2. Initializes the SSL Context, which loads the certs from disk and informs Netty about them
3. Begins listening on the defined Port
4. Setups the Netty Pipeline so that when a user connects, we
    - Create a main.Player object that holds a reference to this user's socket
    - Add the main.SSLHandler to the beginning of this user's pipeline
    - Add the main.MessageHandler to the next step of this user's pipeline


 ## main.SSLHandler.java
 1. Handles initializing the Netty SSL context by loading the certs out of the packed jar.
 2. Defines functions for encrypt + decrypt using the loaded SSL context
 3. Defines utility function for generating the matchmaker key pair (only needed once)

 ## main.MessageHandler.java
 After the user has successfully connected over TLS, all incoming data is passed to the main.MessageHandler.
 This class handles all incoming data, ClientToServer packet processing, and ServerToClient packet creation/sending logic.

 This handler takes in input from the client, modifies the state of the server, and then responds back to the client indicating success or failure.

 Here I have defined a large Enum that represents each ClientToServerMessage that this server can handle:
 - CONNECT
 - PLAYER_UPDATE
 - ABORT
 - etc.

 We have also defined a large Enum that represents each ServerToClientMessage that the client can receive:
 - SUCCESS
 - GAME_STATE_UPDATE
 - GAME_OVER
 - etc.

 This Handler uses the Netty `channelRead` callback to read in incoming data and responds using `ctx.write`

 If this was a more complicated server, we could separate each packet into it's own class. For this test server, an enum is perfect to lay everything out in one searchable file.


When a main.Player connects and the main.Game is ready-to-start (discussed in main.Game.java), we start the broadcast Loop. This is done using a HashedWheelTimer that calls game.broadcast on a defined Tick (in this case it's set to 100ms).


 ## main.World.java
Singleton Object that only has 1 Job:
Manage a HashMap of all existing Games by registering or deregistering a given player to the proper main.Game based on matchID

 ## main.Game.java
 This Object represents a currently-running main.Game or Match.
 It holds the players that have currently joined this main.Game.
 It decides when the main.Game is ready to start (all players have joined)

 Most importantly, it broadcasts the internal state of the game to both players in the main.Game.

 It does this by looping through all players in the match. For each player in the match, it loops through every other player in the match and builds a GameStateUpdate packet with a list of PlayerData.
