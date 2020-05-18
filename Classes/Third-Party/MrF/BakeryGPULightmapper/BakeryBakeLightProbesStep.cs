using System.Collections;
using HardCodeLab.RockTomate.Core.Attributes;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Bakery - Bake Light Probes", "Bakes light probes", "\"Bakery - GPU Lightmapper\" Plugin")]
    public class BakeryBakeLightProbesStep : Step
    {
        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            var bakery = ftRenderLightmap.instance != null
                ? ftRenderLightmap.instance
                : ScriptableObject.CreateInstance<ftRenderLightmap>();

            bakery.LoadRenderSettings();
            bakery.RenderLightProbesButton(false);
            while (ftRenderLightmap.bakeInProgress)
                yield return null;
            
            bakery.Close();
            IsSuccess = true;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Baking..."; }
        }
    }
}