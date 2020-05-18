using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete Asset", "Deletes an Asset file", StepCategories.AssetsCategory)]
    public class DeleteAssetStep : SimpleStep
    {
        [InputField(tooltip: "Asset that will be deleted", required: true)]
        public string AssetPath;
        
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            return AssetDatabase.DeleteAsset(PathHelpers.ConvertToAssetsPath(AssetPath));
        }
    }
}