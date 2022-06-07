using System.Collections;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Bakery - Bake Reflection Probes", "Bakes reflection probes", "\"Bakery - GPU Lightmapper\" Plugin")]
    public class BakeryBakeReflectionProbesStep : Step
    {
        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            var bakery = ftRenderLightmap.instance != null
                ? ftRenderLightmap.instance
                : ScriptableObject.CreateInstance<ftRenderLightmap>();

            bakery.LoadRenderSettings();
            bakery.RenderReflectionProbesButton(false);
            
            while (ftRenderLightmap.bakeInProgress)
                yield return null;

            try
            {
                bakery.Close();
            }
            catch
            {
                // ignored. for some reason this keeps getting thrown
            }

            IsSuccess = true;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Baking..."; }
        }
    }
}