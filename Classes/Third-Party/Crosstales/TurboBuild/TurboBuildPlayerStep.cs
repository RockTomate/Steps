// Check if Crosstales' Turbo Builder Pro asset is present
#if CT_TPB

using UnityEditor;
using Crosstales.TPB;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Turbo - Build Player", "Builds a player", "\"Turbo\" Plugins")]
    public class TurboBuildPlayerStep : SimpleStep
    {
        [InputField(tooltip: "The platform to build")]
        public BuildTarget BuildTarget = BuildTarget.NoTarget;

        [InputField(tooltip: "Directory where all builds will be exported to", required: true)]
        public string ExportDirectory;

        [InputField(tooltip: "Name of the build (if applicable).")]
        public string FileName = string.Empty;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var fileName = FileName;
            var exportDir = ExportDirectory;

            if (string.IsNullOrEmpty(fileName))
                fileName = null;

            // ensure that export directory has a forward slash
            if (!exportDir.EndsWith("\\") || !exportDir.EndsWith("/"))
                exportDir += "/";

            return Builder.Build(BuildTarget, exportDir, fileName);
        }
    }
}

#endif