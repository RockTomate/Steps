using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Rename Directory", "Renames a specified directory", StepCategories.FileSystemCategory)]
    public class RenameDirectoryStep : SimpleStep
    {
        [InputField(tooltip: "Path to a directory that will be renamed", required: true)]
        public string DirectoryPath;

        [InputField(tooltip: "New name for the directory", required: true)]
        public string NewDirectoryName;

        [OutputField(tooltip: "Returns a path to a new directory")]
        public string NewDirectoryPath;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!Directory.Exists(DirectoryPath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            NewDirectoryPath = PathHelpers.Combine(PathHelpers.GetDirectoryName(DirectoryPath), NewDirectoryName);
            Directory.Move(DirectoryPath, NewDirectoryPath);
            return true;
        }
    }
}