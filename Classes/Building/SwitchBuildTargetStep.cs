using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Switch Build Target", "Changes build target of the project", StepCategories.BuildingCategory)]
    public class SwitchBuildTargetStep : SimpleStep
    {
        [InputField]
        public BuildTarget BuildTarget = BuildTarget.NoTarget;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            #if UNITY_5_6_OR_NEWER
            return EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(BuildTarget), BuildTarget);
            #else
            return EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget);
            #endif
        }
    }
}