using System.IO;
using System.Linq;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Copy Directory", "Copies directory to a specified destination", StepCategories.FileSystemCategory)]
    public class CopyDirectoryStep : SimpleStep
    {
        [InputField(tooltip: "Path of a directory that will be copied.", required: true)]
        public string SourceDirectoryPath;

        [InputField(tooltip: "Path to a new directory.", required: true)]
        public string NewDirectoryPath;

        [InputField(tooltip: "If enabled, the destination copy will be overwritten if it already exists.")]
        public bool Overwrite = false;

        [InputField(tooltip: "Paths which will be excluded", category: "Filtering")]
        public string[] ExcludedExtensions;

        [InputField(tooltip: "Extensions that will be excluded (e.g. \".meta\")", category: "Filtering")]
        public string[] ExcludedPaths;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!Directory.Exists(SourceDirectoryPath))
                return false;

            if (!Overwrite && Directory.Exists(NewDirectoryPath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            Directory.CreateDirectory(NewDirectoryPath);

            var sourceFilePaths = Directory.GetFiles(SourceDirectoryPath, "*.*", SearchOption.AllDirectories).ToArray();

            bool excludeFilePaths = ExcludedPaths != null && ExcludedPaths.Length > 0;
            bool excludeFileExtensions = ExcludedExtensions != null && ExcludedExtensions.Length > 0;

            // copy files and create any missing sub-directories in process
            foreach (var path in sourceFilePaths)
            {
                var sourceFilePath = PathHelpers.FixSlashes(path);

                if (excludeFilePaths && ExcludedPaths.Contains(sourceFilePath))
                    continue;

                if (excludeFileExtensions && ExcludedExtensions.Any(ignoredExtension => Path.GetExtension(sourceFilePath) == ignoredExtension))
                    continue;

                var targetDir = Path.GetDirectoryName(sourceFilePath);
                if (targetDir.IsNullOrWhiteSpace())
                    continue;

                var destinationFilePath = sourceFilePath.Replace(SourceDirectoryPath, NewDirectoryPath);
                var destinationDirPath = Path.GetDirectoryName(destinationFilePath);
                if (destinationDirPath == null)
                    continue;

                Directory.CreateDirectory(destinationDirPath);
                File.Copy(sourceFilePath, destinationFilePath, Overwrite);
            }

            return true;
        }
    }
}