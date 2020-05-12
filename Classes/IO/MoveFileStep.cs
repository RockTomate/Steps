using System;
using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Move File", "Moves file from one place to another. Creates a destination directory if it doesn't exist.", StepCategories.FileSystemCategory)]
    public class MoveFileStep : SimpleStep
    {
        [InputField(tooltip: "Path to the file which will be moved.", required: true)]
        public string SourceFilePath;

        [InputField(tooltip: "New file path of the source file.", required: true)]
        public string DestinationFilePath;

        [InputField(tooltip: "If enabled, duplicate file in destination folder will be deleted. Otherwise, it will be skipped.")]
        public bool Overwrite = false;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!File.Exists(SourceFilePath))
            {
                RockLog.WriteLine(this, LogTier.Error, string.Format("Source file not found at: \"{0}\"", SourceFilePath));
                return false;
            }

            if (!Overwrite && File.Exists(DestinationFilePath))
            {
                RockLog.WriteLine(this, LogTier.Error, string.Format("File already exists at: \"{0}\"", DestinationFilePath));
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            try
            {
                bool destinationFileExists = File.Exists(DestinationFilePath);

                // delete the destination file path if it already exists, otherwise, consider this step finished
                if (Overwrite && destinationFileExists)
                {
                    File.Delete(DestinationFilePath);
                }
                else if (!Overwrite && destinationFileExists)
                {
                    return true;
                }

                File.Move(SourceFilePath, DestinationFilePath);
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
                return string.Format("Move \"{0}\" to \"{1}\"", SourceFilePath, DestinationFilePath);
            }
        }
    }
}