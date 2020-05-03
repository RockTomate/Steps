using System;
using System.Collections;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Print List", "Prints contents of a list, array and anything that implements \"IEnumerable\".")]
    public class PrintListStep : Step
    {
        /// <summary>
        /// The enumerable through which this will iterate through
        /// </summary>
        [NonSerialized]
        [InputField(tooltip: "Item list which contents will be printed out.", fieldMode: FieldMode.FormulaOnly, required: true)]
        public IEnumerable ItemList;

        [InputField("Type", "Type of message which will be logged.")]
        public LogTier MessageType = LogTier.Info;

        [InputField(tooltip: "If true, message will be printed into Unity's Console window")]
        public bool PrintUnityConsole = true;

        [InputField(tooltip: "If true, the logs will be saved immediately into a log file (along with pending logs) after this message has been printed out.")]
        public bool FlushImmediately = false;

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            if (ItemList.IsEmpty())
                yield break;

            foreach (var item in ItemList)
            {
                Print(item);
                yield return null;
            }

            IsSuccess = true;
        }

        private void Print(object item)
        {
            var message = item.ToString();
            RockLog.WriteLine(this, MessageType, message);

            if (FlushImmediately)
                RockLog.FlushLogs();

            if (!PrintUnityConsole)
                return;

            switch (MessageType)
            {
                case LogTier.Info:

                    Debug.Log(message);

                    break;

                case LogTier.Warning:

                    Debug.LogWarning(message);

                    break;

                case LogTier.Error:

                    Debug.LogError(message);

                    break;
            }
        }
    }
}