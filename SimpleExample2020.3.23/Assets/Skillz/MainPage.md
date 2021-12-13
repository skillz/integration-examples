# Skillz SDK Cross Platform Unity API
# Version 27.2.8

This is the CrossPlatform Unity wrapper. Refer to the [developer portal instructions](https://cdn.skillz.com/doc/developer/) for help with how to use these scripts. All functions that you will in your game use the SkillzCrossPlatform namespace. You will also have to implement two methods found inside the SkillzDelegate class.

## Skillz Method Calls

Messages *from* Skillz must be received by a special GameObject named "SkillzDelegate". The creation of this object is done through a menu option in the Unity editor (again, refer to the developer portal instructions).

* LaunchSkillz() -  This method call is used to launch the Skillz UI. This is usually called when Unity starts or it can also be hooked to a multiplayer button.
* IsMatchInProgress() -  This is a boolean to check whether or not you're in a Skillz match.
* GetMatchRules() - This returns a Hashtable of your game parameters that you set in Developer Portal. You can use these parameters to customize the gaming experience for each tournament.
* AbortMatch() - This method aborts a player out of a match and returns to the Skillz UI. This will show the player's score as "Aborted". A player who aborted loses to a player with a score of 0.
* AbortBotMatch() - This method aborts a player out of a match against a synchronous bot and returns to the Skillz UI. This will show the player's score as "Aborted" and submit the given score for the synchronous gameplay bot.
* UpdatePlayerCurrentScore() - This method should be called each time a player's score changes. This helps in preventing players cheat the system. This method can accept a string, a float, or an int as its score.
* ReportFinalScore() - (Deprecated) This method is used when the game ends to report the final score to our servers and returns the user back to the Skillz UI. This method can accept a string, a float, or an int as its score. This method has been replaced with DisplayTournamentResultsWithScore()
* DisplayTournamentResultsWithScore() - (Deprecated) This method is used when the game ends to report the final score to our servers and returns the user back to the Skillz UI. This method can accept a string, a float, or an int as its score.
* ReportFinalScoreForBotMatch() - This method is used when a game versus a synchronous gameplay bot ends to report the final scores to our servers and returns the user back to the Skillz UI. Thise should be given a score for both the player and the bot.
* SubmitScore() - This method is used when the game has finalized the player's score to report the score to our servers. It does not return the player to the Skillz experience and keeps the player in the developer controlled scene. This method can accept a string, a float, or an int as its score, as well as a pair of callbacks for if the score submission succeeds or fails.
* EndReplay() - This method can be used to end replay recording after a score has been submitted for a match. Use this if you are going to show a progression experience. Replays will be automatically ended by the SDK if you don't call this method. It returns true if the replay was stopped or no replay was recording, else it returns false.
* ReturnToSkillz() - This method returns the player to the Skillz SDK. It returns a boolean value indicating whether or not it can return the user, and will only return a user if a score has been submitted successfully.
* SDKVersionShort() - This method returns the current SDK version that the user is on.
* CurrentUserDisplayName() - This method returns the current user's display name.
* AddMetadataForMatchInProgress() - This method accepts a json string and an IsMatchInProgress boolean. This is used if you want to add meta data for your match.
* Player class - This returns an object where you can get the current player's Id, Avatar url, and Flag Url.
* Random class - This class is used to implement fairness in your matches. This will enable your game to return the same set of Random numbers in a match between 2 players. Developers will need to use this if they have variables that use random numbers that can affect gameplay.
