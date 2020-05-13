package main;

import com.google.flatbuffers.Table;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelFutureListener;
import io.netty.channel.socket.SocketChannel;
import io.netty.util.CharsetUtil;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.nio.ByteBuffer;

// This player class is a simple object that holds all information relating to the player
// This example has a simple Position class used for storing player position on an x,y grid
// The write functions take a Flatbuffer (as a Table object) or a byte[] or ByteBuf and write out to the socket
public class Player {

    private long userId;
    private int score;
    private Game game;
    private Position position;
    private SocketChannel socket;
    private boolean active;

    private static final Logger logger = LogManager.getLogger(Player.class);

    public Player(SocketChannel socket) {
        this.score = 0;
        this.socket = socket;
        this.position = new Position();
        this.position.x = 0;
        this.position.y = 0;
        this.active = true;
    }

    public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
    }

    public Game getGame() {
        return game;
    }

    public void setGame(Game game) {
        this.game = game;
    }

    public void write(String message) {
        socket.writeAndFlush(Unpooled.copiedBuffer(message, CharsetUtil.UTF_8));
    }

    public void write(byte[] buffer) {
        // We wrap the byte array in an Unpooled buffer to send at once
        try {
            ChannelFuture writeFuture = socket.writeAndFlush(Unpooled.copiedBuffer(buffer));
        } catch (Exception e) {
            logger.info(e);
        }
    }

    public void write(ByteBuffer byteBuffer) {
        write(byteBuffer.array());
    }

    public void write(Table packet) {
        write(packet.getByteBuffer());
    }

    // Using this function we convert a ByteBuf to a byte array
    // Not all ByteBufs have backing arrays, so we first check
    // if the backing array doesn't exist, we create a new one
    public void write(ByteBuf byteBuf) {
        byte[] bytes;
        int offset;
        int length = byteBuf.readableBytes();

        if (byteBuf.hasArray()) {
            bytes = byteBuf.array();
            offset = byteBuf.arrayOffset();
        } else {
            bytes = new byte[length];
            byteBuf.getBytes(byteBuf.readerIndex(), bytes);
            offset = 0;
        }

        socket.writeAndFlush(bytes);
    }

    public Position getPosition() { return position; }

    public void setPosition(float x, float y) { this.position.x= x; this.position.y = y; }

    public long getUserId() {
        return userId;
    }

    public void setUserId(long userId) {
        this.userId = userId;
    }

    public void close() {
        this.active = false;
        socket.close();
    }

    public class Position {
        public float x = 0;
        public float y = 0;
    }

    public boolean isActive() {
        return active;
    }

    public void setActive(boolean active) {
        logger.info("Player " + this.getUserId() + " active status changed to: " + active);
        this.active = active;
    }

}
