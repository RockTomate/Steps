using System.Collections;
using UnityEditor;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Data;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Compute Occlusion Culling", "Computes static Occlusion Culling of a currently active Scene.", StepCategories.BakingCategory)]
    public class ComputeStaticOcclusionCullingStep : Step
    {
        [InputField(tooltip: "The size of the smallest object that will be used to hide other objects when doing occlusion culling." +
                             "Any objects smaller than this size will never cause objects occluded by them to be culled. " +
                             "For example, with a value of 5, all objects that are higher or wider than 5 meters will cause " +
                             "hidden objects behind them to be culled (not rendered, saving render time). " +
                             "Picking a good value for this property is a balance between occlusion accuracy and storage size for the occlusion data.")]
        public float SmallestOccluder = 5;

        [InputField(tooltip: "This value represents the smallest gap between geometry through which the camera is supposed to see. " +
                             "The value represents the diameter of an object that could fit through the hole. " +
                             "If your scene has very small cracks through which the camera should be able to see, " +
                             "the Smallest Hole value must be smaller than the narrowest dimension of the gap.")]
        public float SmallestHole = 0.25f;

        [InputField(tooltip: "Unity’s occlusion uses a data size optimization which reduces unnecessary details by testing backfaces. " +
                             "The default value of 100 is robust and never removes backfaces from the dataset. " +
                             "A value of 5 would aggressively reduce the data based on locations with visible backfaces. " +
                             "The idea is that typically, valid camera positions would not normally see many backfaces - for example, " +
                             "the view of the underside of a terrain\r\n, or the view from within a solid object that you should not be able to reach. " +
                             "With a threshold lower than 100, Unity will remove these areas from the dataset entirely, thereby reducing the data size for the occlusion. " +
                             "The value is clamped between 5 and 100.")]
        public float BackfaceThreshold;

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            StaticOcclusionCulling.Cancel();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            // store old settings for later restoration
            var tempSmallestHole = StaticOcclusionCulling.smallestHole;
            var tempSmallestOccluder = StaticOcclusionCulling.smallestOccluder;
            var tempBackfaceThreshold = StaticOcclusionCulling.backfaceThreshold;

            // assign new settings
            StaticOcclusionCulling.smallestHole = SmallestHole;
            StaticOcclusionCulling.smallestOccluder = SmallestOccluder;
            StaticOcclusionCulling.backfaceThreshold = Mathf.Clamp(BackfaceThreshold, 5, 100);

            IsSuccess = StaticOcclusionCulling.GenerateInBackground();

            while (StaticOcclusionCulling.isRunning)
                yield return null;

            // restore default values
            StaticOcclusionCulling.smallestHole = tempSmallestHole;
            StaticOcclusionCulling.smallestOccluder = tempSmallestOccluder;
            StaticOcclusionCulling.backfaceThreshold = tempBackfaceThreshold;
        }
    }
}