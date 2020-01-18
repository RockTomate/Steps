using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("String Replace", "Replaces a part of string with another string", StepCategories.StringCategory)]
    public class StringReplaceStep : SimpleStep
    {
        [InputField(tooltip: "Input string", required: true)]
        public string String;

        [InputField(tooltip: "A string to be replaced", required: true)]
        public string OldValue;

        [InputField(tooltip: "A string to replace all occurrences of the old value", required: true)]
        public string NewValue;

        [OutputField(tooltip: "Result string")]
        public string Output;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            Output = String.Replace(OldValue, NewValue);
            return true;
        }
    }
}