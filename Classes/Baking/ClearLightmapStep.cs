using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Clear Lightmap", "Clears Lightmap data", StepCategories.BakingCategory)]
    public class ClearLightmapStep : SimpleStep
    {
        [InputField(tooltip: "If true, clears the cache used by lightmaps, reflection probes and default reflection.")]
        public bool ClearDiskCache = true;

        [InputField(tooltip: "Remove the lighting data asset used by the current scene.")]
        public bool ClearLightingDataAsset = true;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            if (ClearDiskCache)
                Lightmapping.ClearDiskCache();

            if (ClearLightingDataAsset)
                Lightmapping.ClearLightingDataAsset();

            return true;
        }
    }
}