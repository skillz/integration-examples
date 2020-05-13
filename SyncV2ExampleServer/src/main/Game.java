package main;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.security.SecureRandom;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public final class Game {

    String matchId;
    private static final Random random = new SecureRandom();
    private static final int MIN_PLAYERS_PER_GAME = 2;


    private List<Player> players = new ArrayList<>(MIN_PLAYERS_PER_GAME);
    private boolean started = false;
    private boolean completed = false;
    private boolean aborted = false;
    private World world;
    private long gameTime = 0L;
    private long gameStartTime = 0L;


    private static final Logger logger = LogManager.getLogger(Game.class);

    public Game(World world, Player player, String matchId) {
        if (players.size() > 0) {
            logger.error("main.Game already had a registered player!");
            return;
        }
        if (players.contains(player)) {
            logger.error("This player is already registered for this game!");
            return;
        }
        this.world = world;
        this.matchId = matchId;
        registerPlayer(player);
    }

    public void registerPlayer(Player player) {
        player.setGame(this);
        players.add(player);
    }

    public void deregisterPlayer(Player player) {
        player.setGame(null);
        players.remove(player);
    }

    public boolean readyToStart() {
        return players.size() == MIN_PLAYERS_PER_GAME;
    }

    public boolean playerInGame(Player player) {
        return players.contains(player);
    }

    public void broadcast() {
        // If we're broadcasting then the game has started
        started = true;

        // update the game timer
        gameTime = System.currentTimeMillis() - gameStartTime;

        // Loop through all the players in the server
        for (Player player : players) {
            MessageHandler.ServerToClientMessage.GAME_STATE_UPDATE.write(player, players);
        }
    }

    public void sendGameOver(boolean gameSuccessful) {
        for (Player player : players) {
            logger.info("Sending Game Over to player: " + player.getUserId());

            // Send a final GameStateUpdate packet (to tell the client which of the other players disconnected in the case that we aborted the game)
            MessageHandler.ServerToClientMessage.GAME_STATE_UPDATE.write(player, players);
            // Send a GameOver packet to tell the clients that the game is still over
            MessageHandler.ServerToClientMessage.GAME_OVER.write(player, gameSuccessful);
            // Close the socket
            player.close();
        }
        reset();
    }

    private void reset() {
        for (Player player : players) {
            world.deregisterPlayer(player);
        }
        players.clear();
        aborted = false;
        completed = false;
    }

    public void startGameTimer() {
        gameStartTime = System.currentTimeMillis();
        gameTime = 0L;
    }

    public long getGameTime() { return gameTime; }

    public boolean isCompleted() {
        return completed;
    }
    
    public void setCompleted() {
        this.completed = true;
    }

    public boolean isAborted() {
        return aborted;
    }

    public void setAborted() {
        this.aborted = true;
    }

    public boolean started() {
        return started;
    }

    public String getMatchId() {
        return matchId;
    }

    public boolean isEmpty() {
        return players.isEmpty();
    }

    public List<Player> getPlayers() {
        return players;
    }
}
