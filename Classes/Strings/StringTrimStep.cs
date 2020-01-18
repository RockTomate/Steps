using System;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Trim String", "Trims a String", StepCategories.StringCategory)]
    public class StringTrimStep : SimpleStep
    {
        [InputField(tooltip: "String to be trimmed", required: true)]
        public string String;

        [InputField(tooltip: "Characters that will be trimmed (whitespace by default)")]
        public string TrimChars;

        [InputField(tooltip: "How will a string be trimmed" +
                             "\nDefault - trims both beginning and trailing parts" +
                             "\nTrim Start - only beginning part will be trimmed" +
                             "\nTrim End - only ending part will be trimmed")]
        public TrimModeType TrimMode = TrimModeType.Default;

        [OutputField(tooltip: "Result string")]
        public string Output;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var trimChars = !string.IsNullOrEmpty(TrimChars)
                ? TrimChars.ToCharArray()
                : new[] { ' ' };

            switch (TrimMode)
            {
                case TrimModeType.Default:

                    Output = String.Trim(trimChars);
                    break;

                case TrimModeType.TrimStart:

                    Output = String.TrimStart(trimChars);
                    break;

                case TrimModeType.TrimEnd:

                    Output = String.TrimEnd(trimChars);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        [Serializable]
        public enum TrimModeType
        {
            Default,
            TrimStart,
            TrimEnd,
        }
    }
}