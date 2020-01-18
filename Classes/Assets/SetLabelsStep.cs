using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set Labels", "Sets labels for assets, replacing old ones.", StepCategories.AssetsCategory)]
    public class SetLabelsStep : SimpleStep
    {
        [InputField(tooltip: "Path to assets to update labels", required: true)]
        public string[] AssetPaths;

        [InputField(tooltip: "New labels that will be set", required: true)]
        public string[] Labels;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var loadedAssets = new List<Object>();

            // load all assets
            foreach (var assetPath in AssetPaths)
            {
                loadedAssets.AddRange(AssetDatabase.LoadAllAssetsAtPath(assetPath));
            }

            // label loaded assets
            foreach (var loadedAsset in loadedAssets)
            {
                AssetDatabase.SetLabels(loadedAsset, Labels);
            }

            return true;
        }
    }
}