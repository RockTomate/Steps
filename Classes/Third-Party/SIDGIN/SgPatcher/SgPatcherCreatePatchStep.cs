#if NET_STANDARD_2_0 || NET_4_6

using System.Collections;
using Google.Apis.Util;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Enums;
using SIDGIN.Patcher.Unity.Editors;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Create Patch", "Creates a patch", "\"SG Patcher\" Plugin")]
    public class SgPatcherCreatePatchStep : Step
    {
        [InputField(tooltip: "Whether a build should be invoked before creating a patch.")]
        public bool RunBuild = false;
        
        private SGPatcherUnityApi _sgPatcherApi = new SGPatcherUnityApi();

        private float _currentProgress = 0f;

        /// <inheritdoc />
        protected override void OnReset()
        {
            _currentProgress = 0f;
            base.OnReset();
        }

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            base.OnInterrupt();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            _sgPatcherApi.onProgressChanged += ProgressChanged;
            _sgPatcherApi.Patch();

            while (_currentProgress < 1f)
                yield return null;

            IsSuccess = true;
        }

        private void ProgressChanged(float arg1, string arg2)
        {
            _currentProgress = arg1;
        }
    }
}

#endif