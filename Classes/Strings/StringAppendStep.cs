using System.Text;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Append to String", "Appends a string to another string", StepCategories.StringCategory)]
    public class StringAppendStep : SimpleStep
    {
        [InputField(tooltip: "String to append to", required: true)]
        public string String;

        [InputField(tooltip: "Strings to append to the original string", required: true)]
        public string[] NewStrings;

        [InputField(tooltip: "Adds a newline before appending string")]
        public bool AddNewline;

        [OutputField(tooltip: "Result string")]
        public string Output;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var stringBuilder = new StringBuilder(String);

            foreach (var newString in NewStrings)
            {
                if (AddNewline)
                {
                    stringBuilder.AppendLine(newString);
                }
                else
                {
                    stringBuilder.Append(newString);
                }
            }

            Output = stringBuilder.ToString();

            return true;
        }
    }
}