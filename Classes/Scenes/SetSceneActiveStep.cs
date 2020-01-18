using UnityEngine.SceneManagement;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set Scene Active", "Sets a specified scene as active. Note that it must be loaded.", StepCategories.ScenesCategory)]
    public class SetSceneActiveStep : SimpleStep
    {
        [InputField(tooltip: "Scene which will be set as active", required: true)]
        public UnityScene Scene;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!Scene.Value.isLoaded)
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            return SceneManager.SetActiveScene(Scene.Value);
        }
    }
}