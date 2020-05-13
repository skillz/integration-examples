using System;
using SkillzSDK.Dialogs;

namespace SkillzSDK.Events
{
    public sealed class DialogButtonPressedArgs : EventArgs
    {
        public DialogProperties DialogProperties
        {
            get;
        }

        public DialogButtonPressedArgs(DialogProperties dialogProperties)
        {
            DialogProperties = dialogProperties;
        }
    }
}