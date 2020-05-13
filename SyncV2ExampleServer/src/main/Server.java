package main;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;
import io.netty.handler.timeout.IdleStateHandler;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.net.InetSocketAddress;

public final class Server {

    private static final int PORT = 10140;
    private static final String HOST = "0.0.0.0";
    private static final int TIMEOUT = 30; // in seconds
    public static final int TICK_RATE = 100; // in ms

    private static final Logger logger = LogManager.getLogger(Server.class);


    public static void main(final String[] args) {
        // Determine the name of our service by determining the name of the executable
        // In our ECS deployment, we rename the jar to follow "game-name-server" pattern
        String serviceName = new java.io.File(Server.class.getProtectionDomain()
            .getCodeSource()
            .getLocation()
            .getPath())
            .getName();
        System.setProperty("servicename", serviceName);

        logger.trace("Starting server...");
        try {
            // Instantiate the World object
            World world = new World();

            // Initialize our SSL context
            // This reads in the cert/key pair and
            if (!SSLHandler.initalizeContext()) {
                return;
            }

            // Start the server and pass it our World object
            listen(world);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    private static void listen(World world) throws InterruptedException {
        EventLoopGroup group = new NioEventLoopGroup();

        try {
            // Netty TCP server setup
            ServerBootstrap serverBootstrap = new ServerBootstrap();
            serverBootstrap.group(group);
            serverBootstrap.channel(NioServerSocketChannel.class);
            serverBootstrap.localAddress(new InetSocketAddress(HOST, PORT));
            serverBootstrap.childHandler(new ChannelInitializer<SocketChannel>() {
                // This function runs for every new connection to the server
                protected void initChannel(SocketChannel socketChannel) throws Exception {

                    // We've connected a new socket and are instantiating a new player object with that socket
                    Player player = new Player(socketChannel);

                    logger.info("Socket connected from: " + socketChannel.remoteAddress());

                    // We create a new channel pipeline and add our SSLContext and new MessageHandler
                    // This MessageHandler object handles and processes incoming packets
                    ChannelPipeline pipeline = socketChannel.pipeline();
                    pipeline.addFirst(SSLHandler.getContext().newHandler(socketChannel.alloc()));
                    pipeline.addLast(new IdleStateHandler(TIMEOUT, 0, 0));
                    pipeline.addLast(new TimeoutHandler(player));
                    pipeline.addLast(new MessageHandler(world, player));
                }
            });
            ChannelFuture channelFuture = serverBootstrap.bind().sync();
            logger.info("Server started successfully. Listening on " + HOST + ":" + PORT + " using TLS1.2!");
            channelFuture.channel().closeFuture().sync();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            group.shutdownGracefully().sync();
        }
    }

}
