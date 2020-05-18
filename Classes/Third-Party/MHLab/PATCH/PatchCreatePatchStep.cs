#if NET_STANDARD_2_0 || NET_4_6

using System;
using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using MHLab.Patch.Core.Admin;
using MHLab.Patch.Core.Admin.Progresses;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Create Patch", "Creates a patch", "\"PATCH - Updating System\" Plugin")]
    public class PatchCreatePatchStep : Step
    {
        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            var adminBuildContext = new AdminBuildContext(new AdminSettings(), new Progress<BuilderProgress>(Handler));
            adminBuildContext.BuildVersion
            var buildBuilder = new BuildBuilder();
            yield break;
        }

        private void Handler(BuilderProgress obj)
        {
            throw new NotImplementedException();
        }
    }
}

#endif