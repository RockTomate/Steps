using System;
using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Copy File", "Copies file to another destination", StepCategories.FileSystemCategory)]
    public class CopyFileStep : SimpleStep
    {
        [InputField(tooltip: "Path to the file which will be copied.", required: true)]
        public string SourceFilePath;

        [InputField(tooltip: "Destination file path.", required: true)]
        public string NewFileCopyPath;

        [InputField(tooltip: "If enabled, the destination copy will be overwritten if it already exists.")]
        public bool Overwrite = false;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!File.Exists(SourceFilePath))
                return false;

            if (!Overwrite && File.Exists(NewFileCopyPath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            try
            {
                if (Overwrite && File.Exists(NewFileCopyPath))
                    File.Delete(NewFileCopyPath);

                File.Copy(SourceFilePath, NewFileCopyPath);
            }
            catch (Exception ex)
            {
                RockLog.WriteLine(LogTier.Error, ex.Message);
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get
            {
                return string.Format("Copies \"{0}\" to \"{1}\"", SourceFilePath, NewFileCopyPath);
            }
        }
    }
}