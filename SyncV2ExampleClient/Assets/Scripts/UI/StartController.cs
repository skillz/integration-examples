using System;
using System.Collections;
using SkillzSDK.Dialogs;
using SkillzSDK.Events;
using Servers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class StartController : MonoBehaviour
{
    [SerializeField]
    public Button playButtton;

    [SerializeField]
    private ConnectArgs connectionArgs;

    [SerializeField]
    private SyncServerController serverController;

    private ModalDialog connectionDialog;

    private void Awake()
    {
        serverController.InvalidServer += OnInvalidServer;
        serverController.ServerHandshakeFailed += OnServerHandshakeFailed;
        serverController.Connected += OnConnected;
        serverController.CouldNotOpenSocket += OnCouldNotOpenSocket;
        serverController.CouldNotOpenSslStream += OnCouldNotOpenSslStream;
        serverController.UnknownError += OnUnknownError;

        connectionDialog = GetComponent<ModalDialog>();
        connectionDialog.ButtonPressed += OnDialogButtonPressed;
    }

    private void OnDestroy()
    {
        serverController.InvalidServer -= OnInvalidServer;
        serverController.ServerHandshakeFailed -= OnServerHandshakeFailed;
        serverController.Connected -= OnConnected;
        serverController.CouldNotOpenSocket -= OnCouldNotOpenSocket;
        serverController.CouldNotOpenSslStream -= OnCouldNotOpenSslStream;
        serverController.UnknownError -= OnUnknownError;

        connectionDialog.ButtonPressed -= OnDialogButtonPressed;
    }

    private void Start()
    {
        Debug.Log("StartController Start()");

        playButtton.onClick.AddListener(OnStartGame);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void OnStartGame()
    {
        StartCoroutine(ConnectToGameServer());
    }

    private IEnumerator ConnectToGameServer()
    {
        connectionDialog.Show(DialogProperties.Connecting);

        yield return new WaitForSeconds(3);

        serverController.Connect(connectionArgs);
    }

    private void OnInvalidServer(object sender, EventArgs args)
    {
        connectionDialog.Show(DialogProperties.InvalidGameServer);
    }

    private void OnServerHandshakeFailed(object sender, EventArgs args)
    {
        connectionDialog.Show(DialogProperties.HandshakeFailed);
    }

    private void OnConnected(object sender, EventArgs args)
    {
        SceneManager.LoadScene("RealTimeGame");
    }

    private void OnCouldNotOpenSocket(object sender, EventArgs args)
    {
        connectionDialog.Show(DialogProperties.CouldNotOpenSocket);
    }

    private void OnCouldNotOpenSslStream(object sender, EventArgs args)
    {
        connectionDialog.Show(DialogProperties.CouldNotOpenSslStream);
    }

    private void OnDialogButtonPressed(object sender, DialogButtonPressedArgs args)
    {
        serverController.Disconnect();
    }

    private void OnUnknownError(object sender, EventArgs args)
    {
        connectionDialog.Show(DialogProperties.UnknownError);
    }
}