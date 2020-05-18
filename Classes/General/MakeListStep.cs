using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Make Text List", "Makes a list of string items.")]
    public class MakeListStep : SimpleStep
    {
        [InputField(required: true)]
        public string[] Items;

        [OutputField]
        public string[] ResultItems;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            ResultItems = Items;
            return true;
        }
    }
}