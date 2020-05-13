package main;

import java.util.HashMap;
import java.util.Map;

// This class stores a HashMap of games/matchIDs and handles de/registering players to Games
public class World {
    private Map<String, Game> games = new HashMap<>();

    public World() {

    }

    // We check if a Game exists for a given matchId
    // If not, we create a new game and register the player
    // If it does exist, we register the player
    public Game registerPlayer(Player player, String matchId) {
        Game game = games.get(matchId);
        if (game == null) {
            game = new Game(this, player, matchId);
            games.put(matchId, game);
        } else {
            game.registerPlayer(player);
        }
        return game;
    }

    public void deregisterPlayer(Player player) {
        Game game = player.getGame();
        if (game != null) {
            game.deregisterPlayer(player);

            if (game.isEmpty()) {
                games.remove(game.getMatchId());
            }
        }
    }
}
