using System.Collections;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Bakery - Bake Lightmap", "Bakes lightmap", "\"Bakery - GPU Lightmapper\" Plugin")]
    public class BakeryBakeLightmapStep : Step
    {
        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            var bakery = ftRenderLightmap.instance != null
                ? ftRenderLightmap.instance
                : ScriptableObject.CreateInstance<ftRenderLightmap>();

            bakery.LoadRenderSettings();
            bakery.RenderButton(false);
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