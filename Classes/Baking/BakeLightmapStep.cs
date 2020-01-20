using System;
using System.Collections;
using UnityEditor;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Bake Lightmap", "Bakes a lightmap for a currently open scene", StepCategories.BakingCategory)]
    public class BakeLightmapStep : Step
    {
        private Lightmapping.GIWorkflowMode _initialWorkflowMode;

        [InputField(tooltip: "If true, clear the initial lightmap before baking a new one.")]
        public bool ClearFirst;

        [NonSerialized]
        [OutputField(tooltip: "File path to a generated lighting data asset")]
        public string LightingDataAssetPath;

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            Lightmapping.Cancel();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            _initialWorkflowMode = Lightmapping.giWorkflowMode;

            Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;

            if (ClearFirst)
                Lightmapping.Clear();

            IsSuccess = Lightmapping.BakeAsync();

            while (Lightmapping.isRunning)
                yield return null;

            Lightmapping.giWorkflowMode = _initialWorkflowMode;

            LightingDataAssetPath = AssetDatabase.GetAssetPath(Lightmapping.lightingDataAsset);
        }
    }
}