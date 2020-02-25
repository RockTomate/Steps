using System;
using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Logging;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Move Directory", "Moves directory from one place to another. Creates a destination directory if it doesn't exist.", StepCategories.FileSystemCategory)]
    public class MoveDirectoryStep : SimpleStep
    {
        [InputField(tooltip: "Path to the directory which will be moved.", required: true)]
        public string SourceDirectoryPath;

        [InputField(tooltip: "Directory to where the directory will be moved to.", required: true)]
        public string DestinationDirectory;

        [InputField(tooltip: "If enabled, directories with duplicate contents will be overwritten. Otherwise, it will be skipped.")]
        public bool Overwrite = false;

        private string NewDirectoryPath
        {
            get { return PathHelpers.Combine(DestinationDirectory, PathHelpers.GetFileName(SourceDirectoryPath)); }
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (string.IsNullOrEmpty(SourceDirectoryPath) || string.IsNullOrEmpty(DestinationDirectory))
                return false;

            if (!Directory.Exists(SourceDirectoryPath))
                return false;

            return true;
        }

        protected override bool OnStepStart()
        {
            try
            {
                if (Directory.Exists(NewDirectoryPath))
                {
                    if (Overwrite)
                    {
                        Directory.Delete(NewDirectoryPath, true);
                    }
                    else
                    {
                        return true;
                    }
                }

                // create a destination folder if it doesn't exist
                if (!Directory.Exists(DestinationDirectory))
                    Directory.CreateDirectory(DestinationDirectory);

                Directory.Move(SourceDirectoryPath, NewDirectoryPath);
            }
            catch (Exception exception)
            {
                RockLog.LogException(exception);
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get
            {
                return string.Format("Move \"{0}\" to \"{1}\"", SourceDirectoryPath, DestinationDirectory);
            }
        }
    }
}