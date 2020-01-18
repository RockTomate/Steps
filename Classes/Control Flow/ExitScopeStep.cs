using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Exit Scope", "Exits a current scope", StepCategories.ControlFlowCategory)]
    public class ExitScopeStep : SimpleStep
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            PostExecutionRequest = StepRequest.ExitScope;
            return true;
        }
    }
}