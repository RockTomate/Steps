using System.IO;
using Ionic.Zip;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Compress to ZIP", "Compresses directory files to a ZIP archive", StepCategories.ZipCategory)]
    public class CompressZipStep : SimpleStep
    {
        [InputField(tooltip: "Output file path of an exported ZIP file.", required: true)]
        public string OutputPath;

        [InputField(tooltip: "Directory that will be compressed into a ZIP.", required: true)]
        public string TargetDirectory;

        [InputField(tooltip: "If enabled, source files are placed into root folder inside of archive.\nRoot Folder will be named after the outputted ZIP file.")]
        public bool PutInDirectory;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            using (var zip = new ZipFile())
            {
                if (PutInDirectory)
                {
                    zip.AddDirectory(TargetDirectory, Path.GetFileNameWithoutExtension(OutputPath));
                }
                else
                {
                    zip.AddDirectory(TargetDirectory);
                }

                zip.Save(OutputPath);
            }

            return true;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Compressing..."; }
        }
    }
}