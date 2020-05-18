using System;
using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Copy Files", "Copies the files to another destination", StepCategories.FileSystemCategory)]
    public class CopyFilesStep : SimpleStep
    {
        [InputField(tooltip: "Path of the files that will be copied.", required: true)]
        public string[] Paths;

        [InputField(tooltip: "Path of the files that will be copied.", required: true)]
        public string BaseDirectory;

        [InputField(tooltip: "Path of the files that will be copied.", required: true)]
        public string DestinationDirectory;

        [InputField(tooltip: "If enabled, the destination copy will be overwritten if it already exists.")]
        public bool Overwrite = false;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            try
            {
                foreach (var path in Paths)
                {
                    var destination = path.Replace(BaseDirectory, DestinationDirectory);

                    if (Overwrite && File.Exists(destination))
                    {
                        File.Delete(destination);
                    }

                    File.Copy(path, destination, Overwrite);
                }
            }
            catch (Exception ex)
            {
                RockLog.LogException(this, ex);
                return false;
            }

            return true;
        }
    }
}