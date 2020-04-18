using UnityEditor;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Create ScriptableObject Asset", "Create a ScriptableObject instance and save it.", StepCategories.AssetsCategory)]
    public class CreateScriptableObjectStep : SimpleStep
    {
        [InputField(tooltip: "The type of the ScriptableObject to create, as the name of the type.", required: true)]
        public string ClassName;
        
        [InputField(tooltip: "Path where to save the created scriptable object")]
        public string AssetPath;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var instance = ScriptableObject.CreateInstance(ClassName);
            AssetDatabase.CreateAsset(instance, PathHelpers.ConvertToAssetsPath(AssetPath));
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return true;
        }
    }
}