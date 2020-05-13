package main;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;
import java.time.format.DateTimeFormatter;
import java.util.Arrays;
import java.util.Base64;

import SyncServer.ConnectPacket;
import SyncServer.GameOverPacket;
import SyncServer.GameStateUpdatePacket;
import SyncServer.MatchSuccessPacket;
import SyncServer.Packet;
import SyncServer.PlayerData;
import SyncServer.PlayerUpdatePacket;
import SyncServer.SuccessPacket;
import SyncServer.Vec2;
import com.google.flatbuffers.FlatBufferBuilder;
import com.google.flatbuffers.Table;
import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.util.HashedWheelTimer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;


// This class takes in all data received over the socket after it has been decrypted by SSLHandler
// This is where packet handling logic (i.e. most server logic) lives
public final class MessageHandler extends ChannelInboundHandlerAdapter {

    private static final Logger logger = LogManager.getLogger(MessageHandler.class);

    private World world;
    private Player player;

    public MessageHandler(World world, Player player) {
        this.world = world;
        this.player = player;
    }

    // This enum is where we define the Server to Client packets
    // You must create an empty function in ServerToClientMessage that can be overridden by the Enum you define
    // the enum takes the opcode of the packet in its constructor
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

        SUCCESS(2) {
            @Override
            public void write(Player player, boolean success) {
                FlatBufferBuilder builder = new FlatBufferBuilder(2);

                ConnectPacket.startConnectPacket(builder);
                ConnectPacket.addOpcode(builder, (short) SUCCESS.opcode);
                builder.addBoolean((boolean) success);
                int connectPacketOffset = ConnectPacket.endConnectPacket(builder);
                builder.finish(connectPacketOffset);

                player.write(builder.sizedByteArray());
            }
        },

        MATCH_SUCCESS(6) {
            @Override
            public void write(Player player, String registeredMatchId, boolean matchStarting, boolean success) {
                FlatBufferBuilder builder = new FlatBufferBuilder(8);

                int registeredMatchIdOffset = builder.createString(registeredMatchId);

                MatchSuccessPacket.startMatchSuccessPacket(builder);
                MatchSuccessPacket.addOpcode(builder, (short) MATCH_SUCCESS.opcode);
                MatchSuccessPacket.addRegisteredMatchId(builder, registeredMatchIdOffset);
                MatchSuccessPacket.addMatchStarting(builder, matchStarting);
                MatchSuccessPacket.addSuccess(builder, success);
                MatchSuccessPacket.addTickRate(builder, Server.TICK_RATE);
                int successPacketOffset = MatchSuccessPacket.endMatchSuccessPacket(builder);
                builder.finish(successPacketOffset);

                player.write(builder.sizedByteArray());
            }
        },

        GAME_OVER(7) {
            @Override
            public void write(Player player, boolean gameSuccessful) {
                FlatBufferBuilder builder = new FlatBufferBuilder(2);

                GameOverPacket.startGameOverPacket(builder);
                GameOverPacket.addOpcode(builder, (short) GAME_OVER.opcode);
                GameOverPacket.addGameSuccessful(builder, gameSuccessful);
                int gameOverPacketOffset = GameOverPacket.endGameOverPacket(builder);
                builder.finish(gameOverPacketOffset);

                player.write(builder.sizedByteArray());
            }
        };

        public void write(Player player, long userId1, float x1, float y1, long userId2, float x2, float y2) {};
        public void write(Player player, boolean success) {};
        public void write(Player player, String registeredMatchId, boolean matchStarting, boolean success) {};
        public void write(Player player, long disconnectedUserId) {};
        public void write(Player player) {};
        public void write(Player player, List<Player> players) {};

        private int opcode;

        ServerToClientMessage(int opcode) {
            this.opcode = opcode;
        }

        private static Map<Integer, ServerToClientMessage> messages = new HashMap<>();

        public static ServerToClientMessage decode(int opcode) {
            return messages.get(opcode);
        }

        static {
            for (ServerToClientMessage message : ServerToClientMessage.values()) {
                messages.put(message.opcode, message);
            }
        }
    }


    // This enum is used to define packet handling logic
    private enum ClientToServerMessage {
        CONNECT(1) {
            @Override
            public void handle(World world, Player player, Table packet) {
                // We know that CONNECT(1) packets are ConnectPackets
                // so we use the ConnectPacket.getRootAsConnectPacket function to convert the generic Table
                // to the packet type that we need. Then we can directly access the data in the packet object.
                ConnectPacket connectPacket = ConnectPacket.getRootAsConnectPacket(packet.getByteBuffer());
                logger.info("token: " + connectPacket.externalToken());
                logger.info("matchId: " + connectPacket.matchId());
                logger.info("userId: " + connectPacket.userId());
                boolean isTokenFromMatchmaker = false;

//                // This is where we validate the externalSyncToken from Matchmaker service
//                // This token is first Base64 decoded before being decrypted by SSLHandler
//                try {
//                    String timestamp = SSLHandler.decrypt(Base64.getDecoder().decode((connectPacket.externalToken()).getBytes(StandardCharsets.UTF_8)));
//                    ZonedDateTime time = ZonedDateTime.parse(timestamp, DateTimeFormatter.ISO_ZONED_DATE_TIME);
//                    if (time != null) {
//                        // If the timestamp is within 3 minutes of current time, we validate the token
//                        if (ZonedDateTime.now(ZoneId.of("GMT")).isBefore(time.plusMinutes(3)) &&
//                            ZonedDateTime.now(ZoneId.of("GMT")).isAfter(time.minusMinutes(3))) {
//                            isTokenFromMatchmaker = true;
//                        }
//                    }

                    // temporary dummy token validation for development
                    if (connectPacket.externalToken().contains("dummytoken")) {
                        isTokenFromMatchmaker = true;
                    }

                    // If we've validated the token, we set the userId of the player object
                    if (isTokenFromMatchmaker) {
                        player.setUserId(connectPacket.userId());
                    }

                    // This is where we validate matchID and insert player into their game
                    if (connectPacket.matchId() != null && isTokenFromMatchmaker) {

                        // If no game exists for the player, we create one and set the player and matchID
                        if (player.getGame() == null) {
                            Game game = world.registerPlayer(player, connectPacket.matchId());

                            // If two players have connected, the game is readyToStart and we create a HashWheeledTimer
                            if (game.readyToStart()) {
                                game.startGameTimer();

                                // We create and send a "MatchSuccessPacket" to let each client know they successfully matched
                                for (Player thePlayer : game.getPlayers()) {
                                    logger.info("Writing MatchSuccessPacket to player");
                                    ServerToClientMessage.MATCH_SUCCESS.write(thePlayer, game.getMatchId(), game.readyToStart(), isTokenFromMatchmaker);
                                }

                                HashedWheelTimer pulse = new HashedWheelTimer();
                                pulse.newTimeout(timeout -> {

                                    // if the game is completed/aborted we end the game
                                    if (game.isCompleted() || game.isAborted()) {
                                        logger.info("Game over for matchId: " + game.getMatchId());
                                        game.sendGameOver(!game.isAborted());
                                        return;
                                    }

                                    // If the game is readyToStart, that means we need to be broadcasting updates
                                    // game.broadcast sends a GameStateUpdatePacket to all clients
                                    if (game.readyToStart()) {
                                        game.broadcast();

                                        // tell the timer to pulse again at the specified tick rate
                                        pulse.newTimeout(timeout.task(), Server.TICK_RATE, TimeUnit.MILLISECONDS);
                                    }

                                    // If we've had a client disconnect during the match then we end the match early
                                    if (!game.readyToStart() &&  game.started()) {
                                        game.sendGameOver(false);
                                    }
                                }, Server.TICK_RATE, TimeUnit.MILLISECONDS);
                            }
                        }
                    } else {
                        // We did not validate the externalSyncToken and as such return null
                        isTokenFromMatchmaker = false;
                        return;
                    }
//                } catch (NoSuchPaddingException | NoSuchAlgorithmException | BadPaddingException |
//                    InvalidKeyException | IllegalBlockSizeException | IllegalArgumentException e) {
//                    e.printStackTrace();
//                }

                // the value returned here is instantly sent back to the client
                ServerToClientMessage.SUCCESS.write(player, true);
            }
        },
        PLAYER_UPDATE(4) {
            @Override
            public void handle(World world, Player player, Table packet) {
                PlayerUpdatePacket playerUpdatePacket = PlayerUpdatePacket.getRootAsPlayerUpdatePacket(packet.getByteBuffer());

                player.setPosition(playerUpdatePacket.posX(), playerUpdatePacket.posY());

                ServerToClientMessage.SUCCESS.write(player, true);
            }
        },
        ABORT(3) {
            @Override
            public void handle(World world, Player player, Table packet) {
                player.getGame().setAborted();

                ServerToClientMessage.SUCCESS.write(player, true);
            }
        },
        KEEP_ALIVE(8) {
            @Override
            public void handle(World world, Player player, Table packet) {
                ServerToClientMessage.SUCCESS.write(player, true);
            }
        };

        private int opcode;

        public abstract void handle(World world, Player player, Table packet);

        ClientToServerMessage(int opcode) {
            this.opcode = opcode;
        }

        private static Map<Integer, ClientToServerMessage> messages = new HashMap<>();

        public static ClientToServerMessage decode(int opcode) {
            return messages.get(opcode);
        }

        static {
            for (ClientToServerMessage message : ClientToServerMessage.values()) {
                messages.put(message.opcode, message);
            }
        }
    }

    @Override
    public void channelRead(final ChannelHandlerContext ctx, final Object msg) throws Exception {
            // This function is called for every incoming read on a socket

            // First we convert the msg Object to
            ByteBuf byteBuf = (ByteBuf) msg;

            byte[] bytes;
            int offset;
            int length = byteBuf.readableBytes();

            // Not all ByteBufs have backing arrays, so we check first. if not, we create one
            if (byteBuf.hasArray()) {
                bytes = byteBuf.array();
                offset = byteBuf.arrayOffset();
            } else {
                bytes = new byte[length];
                byteBuf.getBytes(byteBuf.readerIndex(), bytes);
                offset = 0;
            }

            // We then warp the byte[] array in a ByteBuffer
            ByteBuffer bb = ByteBuffer.wrap(bytes);

            // And we create a generic Packet type from the bytes
            // this allows us to check the opcode of the buffer, as ALL packet Flatbuffers
            // have an opcode as the first element
            Packet packet = Packet.getRootAsPacket(bb);

            // This decode function uses the enums declared above to map the opcode to a handler function
            // The returned value here is a packet that will be sent to the player
            ClientToServerMessage message = ClientToServerMessage.decode(packet.opcode());

            if (message == null) {
                logger.info("NO MATCH FOR OPCODE: " + packet.opcode());
                return;
            }

            // Calling handle on the message calls the right handle function and processes the packet
            message.handle(world, player, packet);
    }

    @Override
    public void channelReadComplete(final ChannelHandlerContext ctx) throws Exception {
        ctx.flush();
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {
        player.setActive(false);
        cause.printStackTrace();
        ctx.close();
    }

    @Override
    public void channelUnregistered(ChannelHandlerContext ctx) throws Exception {
        world.deregisterPlayer(player);
    }
}
