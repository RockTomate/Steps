using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Create Directory", "Creates an empty directory", StepCategories.FileSystemCategory)]
    public class CreateDirectoryStep : SimpleStep
    {
        [InputField("Directory Path", "Path to a directory that will be created.", required: true)]
        public string NewDirectoryPath;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            if (Directory.Exists(NewDirectoryPath))
                return true;

            Directory.CreateDirectory(NewDirectoryPath);
            return true;
        }
    }
}