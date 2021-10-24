using System;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Preferences;

namespace HardCodeLab.RockTomate.Steps
{
    [Serializable]
    [StepDescription("Print", "Prints a log message to a Unity Console window.")]
    public class PrintLogStep : SimpleStep
    {
        [InputField(tooltip: "Message that will be logged.")]
        public string Message;

        [InputField("Type", "Type of message which will be logged.")]
        public LogTier MessageType = LogTier.Info;

        [InputField(tooltip: "If true, message will be printed into RockTomate's Job Session Console window.")]
        public bool PrintToConsole;

        protected override bool OnStepStart()
        {
            switch (MessageType)
            {
                case LogTier.Info:

                    Debug.Log(Message);

                    break;

                case LogTier.Warning:

                    Debug.LogWarning(Message);

                    break;

                case LogTier.Error:

                    Debug.LogError(Message);

                    break;

                default:
                    return false;
            }

            if (!PrintToConsole || RTPreferences.Data.PrintToUnityConsole)
                return true;

            RockLog.WriteLine(this, MessageType, Message);
            RockLog.FlushLogs();

            return true;
        }

        protected override string Description
        {
            get { return string.Format("Print to console a message of type '{0}'", MessageType); }
        }
    }
}