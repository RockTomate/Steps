using System;
using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Move File", "Moves file from one place to another. Creates a destination directory if it doesn't exist.", StepCategories.FileSystemCategory)]
    public class MoveFileStep : SimpleStep
    {
        [InputField(tooltip: "Path to the file which will be moved.", required: true)]
        public string SourceFilePath;

        [InputField(tooltip: "Directory to where the source file will be moved to.", required: true)]
        public string DestinationDirectory;

        [InputField(tooltip: "If enabled, duplicate file in destination folder will be deleted. Otherwise, it will be skipped.")]
        public bool Overwrite = false;

        private string NewFilePath
        {
            get { return PathHelpers.Combine(DestinationDirectory, PathHelpers.GetFileName(SourceFilePath)); }
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (string.IsNullOrEmpty(SourceFilePath) || string.IsNullOrEmpty(DestinationDirectory))
                return false;

            if (!File.Exists(SourceFilePath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            try
            {
                if (Overwrite && File.Exists(NewFilePath))
                    File.Delete(NewFilePath);

                // create a destination folder if it doesn't exist
                if (!Directory.Exists(DestinationDirectory))
                    Directory.CreateDirectory(DestinationDirectory);

                File.Move(SourceFilePath, NewFilePath);
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
                return string.Format("Move \"{0}\" to \"{1}\"", SourceFilePath, DestinationDirectory);
            }
        }
    }
}