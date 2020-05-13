using UnityEngine;
using TMPro;
using Servers;
using System.Collections.Generic;
using SkillzSDK.Events;
using SkillzSDK.Dialogs;
using UnityEngine.SceneManagement;
using FlatBuffers;
using System;
using SyncServer;

public sealed class GameController : MonoBehaviour
{
    private const int MinMovePixelDelta = 5;
    private const float ActiveOpacity = 1f;
    private const float InactiveOpacity = 0.65f;

    private static readonly Color PlayerColor = Color.blue;
    private static readonly Color OpponentColor = new Color(1, 0.65f, 0, ActiveOpacity);

    [SerializeField]
    private SyncServerController syncServer;

    [SerializeField]
    private GameObject circlePrefab;

    [SerializeField]
    private GameObject playerPositionGameObject;

    private TextMeshProUGUI playerPositionText;

    private RectTransform borderRT;

    private WaitForOpponentDialog waitForOpponentDialog;

    private ModalDialog matchEndedDialog;

    private bool isDragging;

    private Dictionary<long, GameObject> circles;
    private Dictionary<long, Vector2> circlesPreviousPositions;
    private Dictionary<long, Vector2> circlesDesiredPositions;
    private float circleDeltaTimer = 0f;
    private float circleInterpolationProgress = 0f;
    private bool playerInitialized;

    private bool gameOverReceived;

    private int tickRate;

    private long gameTime;

    private void Awake()
    {
        waitForOpponentDialog = GetComponent<WaitForOpponentDialog>();
        matchEndedDialog = GetComponent<ModalDialog>();
        matchEndedDialog.ButtonPressed += OnMatchEndedDialogButtonPresed;
        isDragging = false;
        gameOverReceived = false;

        syncServer.ReceiveTimedOut += OnServerTimedOut;
    }

    // Start is called before the first frame update
    private void Start()
    {
        borderRT = GameObject.Find("GameSpaceRect").GetComponent<RectTransform>();

        Debug.Log("GameController Start");

        circles = new Dictionary<long, GameObject>();
        circlesPreviousPositions = new Dictionary<long, Vector2>();
        circlesDesiredPositions = new Dictionary<long, Vector2>();
        playerInitialized = false;

        playerPositionText = playerPositionGameObject.GetComponent<TextMeshProUGUI>();
        playerPositionText.text = string.Empty;

        waitForOpponentDialog.Show(DialogProperties.WaitingForOpponent);

        InvokeRepeating("KeepAlive", 0.0f, 3.0f); // send keep alive packet every 3 seconds
    }

    private void KeepAlive() {
        syncServer.SendKeepAlive();
    }

    public void QuitMatch()
    {
        syncServer.QuitMatch();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && playerInitialized)
        {
            // The game server is server authoritative. That is, it is the
            // "source of truth", so only update the UI based on the information
            // it sends from the GameStateUpdatePacket in response to PlayerUpdateStatePackets
            // it gets from each player.
            syncServer.SendPlayerPosition(ScreenToWorldPosition(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
        }

        // Process all packets that have been received by the server thus far
        byte[] data;
        while (syncServer.GetNextPacket(out data))
        {
            var packet = PacketFactory.BytesToPacket(data);
            var byteBuffer = new ByteBuffer(data);
            switch ((Opcode)packet.Opcode)
            {
                case Opcode.Success:
                    Debug.Log("Success packet received");
                    break;

                case Opcode.GameStateUpdate:
                    OnGameStateUpdated(GameStateUpdatePacket.GetRootAsGameStateUpdatePacket(byteBuffer));
                    break;

                case Opcode.MatchSuccess:
                    // All players have entered the match, the game can now be played.
                    // set the tickRate so that we can properly interpolate animations on the client side
                    tickRate = MatchSuccessPacket.GetRootAsMatchSuccessPacket(byteBuffer).TickRate;
                    Debug.Log("Match found, joining... Tick Rate: " + tickRate);
                    OnMatchReady();
                    break;

                case Opcode.GameOver:
                    OnGameOver(GameOverPacket.GetRootAsGameOverPacket(byteBuffer));
                    break;

                default:
                    Debug.Log($"Packet received with Opcode={(Opcode)packet.Opcode}");
                    break;
            }
        }
        
        // If we've initialized our players, lets start interpolating between circlesPreviousPositions and circlesDesiredPositions
        if (playerInitialized)
        {
            DrawAllCircles();
        }
    }

    // FixedUpdate runs at a specificed interval based on Unity Time settings and is useful for calculating time-based events
    private void FixedUpdate()
    {
        // we call this from FixedUpdate because we've changed Unity's Time Settings to step once per 0.005 seconds
        // updating the timer here ensures we have enough granularity and accurate timekeeping
        UpdateCircleTimer();
    }

    private void OnServerTimedOut(object sender, EventArgs args)
    {
        syncServer.ReceiveTimedOut -= OnServerTimedOut;
        HideWaitForOpponentDialog();
        matchEndedDialog.Show(DialogProperties.ServerTimedOut);
    }

    private void OnMatchReady()
    {
        HideWaitForOpponentDialog();
        InitializePlayerCircle();
    }

    private void OnGameStateUpdated(GameStateUpdatePacket gameStateUpdatePacket)
    {
        gameTime = gameStateUpdatePacket.GameTime;
        HideWaitForOpponentDialog();
        UpdateAllCircles(gameStateUpdatePacket);
        playerPositionText.text = MakeInfoText();
    }

    private void OnGameOver(GameOverPacket gameOverPacket)
    {
        Debug.Log("The match has ended.");

        HideWaitForOpponentDialog();

        gameOverReceived = true;
        syncServer.Disconnect();
        matchEndedDialog.Show(gameOverPacket.GameSuccessful ? DialogProperties.MatchOver : DialogProperties.MatchAborted);
    }

    private void OnMatchEndedDialogButtonPresed(object sender, DialogButtonPressedArgs args)
    {
        if (matchEndedDialog != null) {
            matchEndedDialog.ButtonPressed -= OnMatchEndedDialogButtonPresed;
        }
        SceneManager.LoadScene("StartScene");
    }

    private void HideWaitForOpponentDialog()
    {
        if (waitForOpponentDialog != null) {
            waitForOpponentDialog?.Hide();
        }
    }

    private string MakeInfoText()
    {
        if (!circles.ContainsKey(syncServer.UserId))
        {
            return string.Empty;
        }

        var playerCircle = circles[syncServer.UserId];
        var playerScreenPos = Camera.main.WorldToScreenPoint(playerCircle.GetComponent<Rigidbody2D>().position);
        return $"Match Timer: {gameTime}, You=({Mathf.RoundToInt(playerScreenPos.x)},{Mathf.RoundToInt(playerScreenPos.y)})";
    }

    private void InitializePlayerCircle()
    {
        InitializeCircle(PlayerColor, syncServer.UserId, worldPosition: null);
        playerInitialized = true;
    }

    private void InitializeCircle(Color color, long userId, Vector2? worldPosition)
    {
        Debug.Log($"Initializing circle for userId={userId}");

        var playerCircle = Instantiate(circlePrefab);

        // Set the circle color
        var spriteRenderer = playerCircle.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;

        // Set the circle text
        var textGameObject = playerCircle.transform.GetChild(0);
        var textMeshPro = textGameObject.GetComponent<TextMeshPro>();
        textMeshPro.text = userId == syncServer.UserId ? "You" : userId.ToString();

        // Set the circle's position
        playerCircle.GetComponent<Rigidbody2D>().position = worldPosition.HasValue
            ? worldPosition.Value
            : MakeRandomWorldPosition(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y);

        circles[userId] = playerCircle;
        circlesPreviousPositions[userId] = new Vector2(playerCircle.GetComponent<Rigidbody2D>().position.x, playerCircle.GetComponent<Rigidbody2D>().position.y);
    }

    private void UpdateCircleTimer()
    {
        // convert delta from seconds to milliseconds
        circleDeltaTimer += Time.deltaTime * 1000F;
    }
    
    private void DrawAllCircles()
    {
        // along the path from circlesPreviousPosition to circlesDesiredPosition, how far do we progress
        circleInterpolationProgress = circleDeltaTimer / (tickRate * 2); // then determine percent of interpolation

        // Vector2.Lerp between circlesPreviousPosition to circlesDesiredPosition to update the circle position smoothly
        foreach (KeyValuePair<long,GameObject> pair in circles)
        {
            if (circlesPreviousPositions.ContainsKey(pair.Key) && circlesDesiredPositions.ContainsKey(pair.Key))
            {
                pair.Value.GetComponent<Rigidbody2D>().position =
                    Vector2.Lerp(circlesPreviousPositions[pair.Key], circlesDesiredPositions[pair.Key], circleInterpolationProgress);
            }
        }
    }

    private void UpdateAllCircles(GameStateUpdatePacket gameStatePacket)
    {
        // reset the timer used for interpolating circle position
        circleDeltaTimer = 0;

        for (var iPlayer = 0; iPlayer < gameStatePacket.PlayersLength; iPlayer++)
        {
            var player = gameStatePacket.Players(iPlayer);
            if (!player.HasValue)
            {
                continue;
            }

            if (!circles.ContainsKey(player.Value.UserId))
            {
                InitializeCircle(
                    syncServer.UserId == player.Value.UserId ? PlayerColor : OpponentColor,
                    player.Value.UserId, new Vector2(player.Value.PosX, player.Value.PosY)
                );

                continue;
            }

            var circle = circles[player.Value.UserId];
            // We set the circlesPreviousPosition to the current circle location -- we want to interpolate from that point to the new desired location
            circlesPreviousPositions[player.Value.UserId] = new Vector2(circle.GetComponent<Rigidbody2D>().position.x, circle.GetComponent<Rigidbody2D>().position.y);
            // We set the circlesDesiredPosition to the x,y coordinates the user clicked on -- this is where our circle will interpolate towards
            circlesDesiredPositions[player.Value.UserId] = new Vector2(player.Value.PosX, player.Value.PosY);

            SetCircleOpacity(circle, player.Value.Active ? ActiveOpacity : InactiveOpacity);
        }
    }

    private Vector2 MakeRandomWorldPosition(float circleWorldWidth, float circleWorldHeight)
    {
        Debug.Log("Making random initial world position...");

        var worldCenter = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));

        return new Vector2(
            worldCenter.x + UnityEngine.Random.Range(-circleWorldWidth, circleWorldWidth),
            worldCenter.y + UnityEngine.Random.Range(-circleWorldHeight, circleWorldHeight)
        );
    }

    private Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    private void SetCirclePosition(GameObject circle, Vector3 worldPosition)
    {
        circle.GetComponent<Rigidbody2D>().position = worldPosition;
    }

    private void SetCircleOpacity(GameObject circle, float opacity)
    {
        var spriteRenderer = circle.GetComponent<SpriteRenderer>();
        var currColor = spriteRenderer.color;

        spriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, opacity);
    }
}