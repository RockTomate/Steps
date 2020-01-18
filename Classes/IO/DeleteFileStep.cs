using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete File", "Deletes a file", StepCategories.FileSystemCategory)]
    public class DeleteFileStep : SimpleStep
    {
        [InputField(tooltip: "Path of a file which will be deleted.", required: true)]
        public string FilePath;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            return true;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get
            {
                return string.Format("Delete the file at \"{0}\"", FilePath);
            }
        }
    }
}