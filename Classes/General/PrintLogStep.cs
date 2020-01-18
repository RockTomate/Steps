using System;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [Serializable]
    [StepDescription("Print Log", "Logs a debug message.")]
    public class PrintLogStep : SimpleStep
    {
        [InputField(tooltip: "Message that will be logged.")]
        public string Message = string.Empty;

        [InputField("Type", "Type of message which will be logged.")]
        public LogTier MessageType = LogTier.Debug;

        [InputField(tooltip: "If true, message will be printed into Unity's Console window")]
        public bool PrintUnityConsole = true;

        [InputField(tooltip: "If true, the logs will be saved immediately into a log file (along with pending logs) after this message has been printed out.")]
        public bool FlushImmediately = false;

        protected override bool OnStepStart()
        {
            RockLog.WriteLine(MessageType, Message);

            if (FlushImmediately)
                RockLog.FlushLogs();

            if (!PrintUnityConsole)
                return true;

            switch (MessageType)
            {
                case LogTier.Debug:

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

            return true;
        }

        protected override string Description
        {
            get { return string.Format("Print to console a message of type '{0}'", MessageType); }
        }
    }
}