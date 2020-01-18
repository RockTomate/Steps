using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Open Scene", "Opens a target unity scene in editor.", StepCategories.ScenesCategory)]
    public class OpenSceneStep : SimpleStep
    {
        [InputField(tooltip: "Scene which will be opened", required: true)]
        public UnityScene Scene;

        [InputField(tooltip: "Specify how scene should be opened")]
        public OpenSceneMode SceneMode = OpenSceneMode.Single;

        [InputField(tooltip: "Whether or not to set the scene active when it's opened.")]
        public bool SetActive = false;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var loadedScene = EditorSceneManager.OpenScene(Scene.ScenePath, SceneMode);

            // check if loaded scene is valid
            if (!loadedScene.IsValid())
                return false;

            if (SetActive)
                SceneManager.SetActiveScene(Scene.Value);

            return true;
        }
    }
}