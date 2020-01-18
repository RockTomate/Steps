using UnityEditor.SceneManagement;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    // The reason why we're deriving directly from Step class instead of SimpleStep, is to take advantage of editor coroutines and make waiting a non-blocking factor.
    [StepDescription("Close Scene", "Closes a scene from the editor.", StepCategories.ScenesCategory)]
    public class CloseSceneStep : SimpleStep
    {
        [InputField(tooltip: "Scene which will be unloaded", required: true)]
        public UnityScene Scene;

        [InputField(tooltip: "Whether or not should the scene be removed from the Hierarchy window.")]
        public bool Remove;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!Scene.Value.IsValid() || Scene.Value.isLoaded)
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            return EditorSceneManager.CloseScene(Scene.Value, Remove);
        }
    }
}