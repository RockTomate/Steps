using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Stop Job", "Stops a currently running job.", StepCategories.ControlFlowCategory)]
    public class StopJobStep : SimpleStep
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            PostExecutionRequest = StepRequest.TerminateJob;
            return true;
        }
    }
}