using System;
using UnityEditor.SceneManagement;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [Serializable]
    [StepDescription("Save Scenes", "Saves all open unity scenes.", StepCategories.ScenesCategory)]
    public class SaveScenesStep : SimpleStep
    {
        protected override bool OnStepStart()
        {
            return EditorSceneManager.SaveOpenScenes();
        }
    }
}