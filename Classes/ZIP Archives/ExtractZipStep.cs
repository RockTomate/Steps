using System.IO;
using Ionic.Zip;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Logging;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Extract ZIP", "Extracts contents of a ZIP archive file into target directory", StepCategories.ZipCategory)]
    public class ExtractZipStep : SimpleStep
    {
        [InputField(tooltip: "Path to a ZIP archive file that will be extracted.", required: true)]
        public string ZipPath;

        [InputField(tooltip: "Path to a directory which will hold extracted files.", required: true)]
        public string OutputDirectoryPath;

        [InputField(tooltip: "If true, extracted files will overwrite existing files in the Output Directory.")]
        public bool OverwriteFiles = false;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!File.Exists(ZipPath))
            {
                RockLog.WriteLine(this, LogTier.Error, string.Format("File at \"{0}\" does not exist.", ZipPath));
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            using (var zip = ZipFile.Read(ZipPath))
            {
                if (OverwriteFiles)
                {
                    zip.ExtractAll(OutputDirectoryPath, ExtractExistingFileAction.OverwriteSilently);
                }
                else
                {
                    zip.ExtractAll(OutputDirectoryPath);
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Extracting..."; }
        }
    }
}