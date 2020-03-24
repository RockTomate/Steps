// Check if Crosstales' Turbo Platform Switch asset is present
#if CT_TPS

using UnityEditor;
using Crosstales.TPS;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Turbo - Switch Platform", "Switches build target of the project", "\"Turbo\" Plugins")]
    public class TurboSwitchPlatformStep : SimpleStep
    {
        [InputField]
        public BuildTarget BuildTarget = BuildTarget.NoTarget;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            return Switcher.Switch(BuildTarget);
        }
    }
}

#endif