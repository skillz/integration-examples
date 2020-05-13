using UnityEngine;

namespace SkillzSDK.Dialogs
{
    public sealed class DialogProperties : ScriptableObject
    {
        public static readonly DialogProperties Connecting = new DialogProperties(
            title: "Connecting...",
            message: "Connecting to the game server.",
            buttonText: "Cancel",
            windowId: 0
        );

        public static readonly DialogProperties InvalidGameServer = new DialogProperties(
            title: "Invalid Game Server",
            message: "The server does not appear to be a Sync v2. game server.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties CouldNotOpenSocket = new DialogProperties(
            title: "Could Not Open Socket",
            message: "Could not open a TCP socket.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties CouldNotOpenSslStream = new DialogProperties(
            title: "Could Not Open SSL Stream",
            message: "Could not open an SSL stream to the game server.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties HandshakeFailed = new DialogProperties(
            title: "Handshake Failed",
            message: "There was a handshake failure. This is a not a legitimate Sync v2. game server.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties WaitingForOpponent = new DialogProperties(
            title: "Waiting for Opponent",
            message: "Waiting for an opponent to join the game...",
            buttonText: "Disconnect",
            windowId: 0
        );

        public static readonly DialogProperties MatchOver = new DialogProperties(
            title: "Match Ended",
            message: "The match has completed successfully.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties MatchAborted = new DialogProperties(
            title: "Match Aborted",
            message: "The match was aborted because one or more players were disconnected.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties UnknownError = new DialogProperties(
            title: "Oops!",
            message: "An unknown error occurred. Check the logs for more details.",
            buttonText: "OK",
            windowId: 0
        );

        public static readonly DialogProperties ServerTimedOut = new DialogProperties(
            title: "Server Timed Out",
            message: "The connection was closed because there was a timeout waiting to receive data from the server.",
            buttonText: "OK",
            windowId: 0
        );

        public string Title
        {
            get;
        }

        public string Message
        {
            get;
        }

        public string ButtonText
        {
            get;
        }

        public int WindowId
        {
            get;
        }

        private DialogProperties(string title, string message, string buttonText, int windowId)
        {
            Title = title;
            Message = message;
            ButtonText = buttonText;
            WindowId = windowId;
        }
    }
}
