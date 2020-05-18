using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete Paths", "Deletes the set of files or directories", StepCategories.FileSystemCategory)]
    public class DeletePathsStep : SimpleStep
    {
        [InputField(tooltip: "Paths which will be removed.", required: true)]
        public string[] Paths;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            foreach (var path in Paths)
            {
                if (Directory.Exists(path))
                {
                    PathHelpers.DeleteDirectory(path);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return true;
        }
    }
}