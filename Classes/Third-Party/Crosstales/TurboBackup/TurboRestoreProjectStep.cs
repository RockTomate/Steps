// Check if Crosstales' Turbo Backup asset is present
#if CT_TB

using Crosstales.TB;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Turbo - Restore Project", "Restores a project backup for this project", "\"Turbo\" Plugins")]
    public class TurboRestoreProjectStep : SimpleStep
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            return BAR.Restore();
        }
    }
}

#endif