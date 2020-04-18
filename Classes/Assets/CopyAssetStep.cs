using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Copy Asset", "Makes a copy of an Asset file", StepCategories.AssetsCategory)]
    public class CopyAssetStep : SimpleStep
    {
        [InputField(tooltip: "Path to a file that will be duplicated.", required: true)]
        public string AssetPath;

        [InputField(tooltip: "Path of a duplicated asset.", required: true)]
        public string NewAssetPath;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            return AssetDatabase.CopyAsset(PathHelpers.ConvertToAssetsPath(AssetPath), PathHelpers.ConvertToAssetsPath(NewAssetPath));
        }
    }
}