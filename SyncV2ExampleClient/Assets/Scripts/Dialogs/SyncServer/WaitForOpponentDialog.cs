using UnityEngine;
using SkillzSDK.Dialogs;
using Servers;

/// <summary>
/// Specialized dialog to display when waiting for an
/// opponent to enter a sync v2 match.
/// </summary>
public sealed class WaitForOpponentDialog : ModalDialog
{
    [SerializeField]
    private SyncServerController serverController;
}
