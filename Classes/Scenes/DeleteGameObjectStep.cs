using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete GameObject", "Deletes a GameObject in a currently active Scene.", StepCategories.ScenesCategory)]
    public class DeleteGameObjectStep : SimpleStep
    {
        [InputField]
        public UnityGameObject Target;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            Object.Destroy(Target.Value);
            return true;
        }
    }
}