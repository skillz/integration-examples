package main;

import io.netty.channel.ChannelDuplexHandler;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.timeout.IdleState;
import io.netty.handler.timeout.IdleStateEvent;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

// Handler should handle the IdleStateEvent triggered by IdleStateHandler.
public class TimeoutHandler extends ChannelDuplexHandler {

    private static final Logger logger = LogManager.getLogger(TimeoutHandler.class);

    private Player player;
    public TimeoutHandler(Player player) {
        this.player = player;
    }



    @Override
    public void userEventTriggered(ChannelHandlerContext ctx, Object evt) throws Exception {
        if (evt instanceof IdleStateEvent) {
            IdleStateEvent e = (IdleStateEvent) evt;
            if (e.state() == IdleState.READER_IDLE) {
                logger.error("Timeout on receive from: " + player.getUserId());
//                player.setActive(false);
//                player.getGame().setAborted();
                ctx.close();
            }
        }
    }
}

