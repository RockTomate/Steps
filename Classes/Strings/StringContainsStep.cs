using System;
using System.Text.RegularExpressions;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("String Contains", "Checks if string contains a specified string/character", StepCategories.StringCategory)]
    public class StringContainsStep : SimpleStep
    {
        [InputField(tooltip: "String which will be searched", required: true)]
        public string String;

        [InputField(tooltip: "Text which will be searched. Serves as a pattern if 'Use Regex' option is enabled", required: true)]
        public string Text;

        [InputField(tooltip: "If true, then text will be searched based on regex")]
        public bool UseRegex;

        [InputField(tooltip: "Comparison mode used when comparing strings (not used if 'Use Regex' option is enabled)")]
        public StringComparison StringComparisonMode = StringComparison.CurrentCulture;

        [InputField(tooltip: "Regex Options (not used if 'Use Regex' option is disabled)")]
        public RegexOptions RegexOptions;

        [OutputField(tooltip: "Returns true if string does contain text")]
        public bool Result;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            if (UseRegex)
            {
                Result = Regex.IsMatch(String, Text, RegexOptions);
            }
            else
            {
                Result = String.Contains(Text, StringComparisonMode);
            }

            return true;
        }
    }
}