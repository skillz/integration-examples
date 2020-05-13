using System;
using SkillzSDK.Events;
using UnityEngine;

namespace SkillzSDK.Dialogs
{
    /// <summary>
    /// A general-purpose modal dialog that has a title, message text, and a button.
    /// </summary>
    public class ModalDialog : MonoBehaviour
    {
        private const int MaxWidth = 640;
        private const int MaxHeight = 480;
        private const int Margin = 20;

        private const int ButtonWidth = 150;
        private const int ButtonHeight = 80;

        private const string ModalDialogSkinName = "Modal Dialog Skin";

        public event EventHandler<DialogButtonPressedArgs> ButtonPressed;

        private DialogProperties configuration;

        private Rect windowRect;
        private GUISkin windowSkin;

        public void Show(DialogProperties modalDialogConfig)
        {
            configuration = modalDialogConfig;
            Show();
        }

        public void Show()
        {
            enabled = true;
        }

        public void Hide()
        {
            enabled = false;
        }

        protected virtual void Awake()
        {
            enabled = false;
            windowSkin = (GUISkin)Resources.Load(ModalDialogSkinName);
        }

        private void OnGUI()
        {
            // TODO: We should upgrade to Unity 2017.2 or above to account for notched devices via `Screen.safeArea`.
            // For more info: https://connect.unity.com/p/updating-your-gui-for-the-iphone-x-and-other-notched-devices

            GUI.skin = windowSkin;

            var windowWidth = Mathf.Min(Screen.width - Margin, MaxWidth);
            var windowHeight = Mathf.Min(Screen.height - Margin, MaxHeight);

            windowRect = new Rect(
                0.5f * (Screen.width - windowWidth),
                0.5f * (Screen.height - windowHeight),
                windowWidth,
                windowHeight
            );

            GUI.ModalWindow(configuration.WindowId, windowRect, MakeDialogContent, configuration.Title);

            GUI.skin = null;
        }

        private void MakeDialogContent(int windowBeingRenderedId)
        {
            if (windowBeingRenderedId != configuration.WindowId)
            {
                Debug.LogWarning(string.Format("Expected windowId of '{0}' but it was '{1}'", configuration.WindowId, windowBeingRenderedId));
                return;
            }

            LayoutMessage();
            LayoutButton();
        }

        private void LayoutMessage()
        {
            const int heightFudgeFactor = 200;

            var messageRect = new Rect(
                Margin,
                0.5f * (windowRect.height - heightFudgeFactor) - Margin,
                windowRect.width - 2 * Margin,
                heightFudgeFactor
            );

            GUILayout.BeginArea(messageRect);
            GUILayout.BeginVertical();

            var fontStyle = new GUIStyle
            {
                fontSize = 26,
                wordWrap = true
            };

            GUILayout.Label(configuration.Message, fontStyle);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void LayoutButton()
        {
            var buttonRect = new Rect(
                windowRect.width - 2 * Margin - ButtonWidth,
                windowRect.height - 2 * Margin - ButtonHeight,
                ButtonWidth,
                ButtonHeight
            );

            if (GUI.Button(buttonRect, configuration.ButtonText))
            {
                RaiseButtonPressed();

                enabled = false;
            }
        }

        private void RaiseButtonPressed()
        {
            ButtonPressed?.Invoke(this, new DialogButtonPressedArgs(configuration));
        }
    }
}
