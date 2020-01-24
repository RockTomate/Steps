using System.IO;
using System.Linq;
using System.Collections.Generic;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Helpers;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Resolve Paths", "Gets file paths of files and directories and filters them out (if necessary)", StepCategories.FileSystemCategory)]
    public class ResolvePathsStep : SimpleStep
    {
        [InputField(tooltip: "Paths which will be included", required: true)]
        public string[] TargetPaths;

        [InputField(tooltip: "Paths which will be excluded", category: "Filtering")]
        public string[] ExcludedPaths;

        [InputField(tooltip: "File names that will be excluded. File extension must be included.", category: "Filtering")]
        public string[] ExcludedFileNames;

        [InputField(tooltip: "Extensions that will be excluded (e.g. \".meta\")", category: "Filtering")]
        public string[] ExcludedExtensions;

        [InputField(tooltip: "If true, result paths will be converted to be relative to this project's \"Assets\" directory.")]
        public bool ConvertToRelative;

        [OutputField(tooltip: "List of resolved paths.")]
        public List<string> ResolvedPaths;

        [OutputField(tooltip: "Number of paths which have been resolved.")]
        public int NumberOfResolvedPaths;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var filteredPaths = new List<string>(TargetPaths.Length);
            bool excludePaths = ExcludedPaths != null && ExcludedPaths.Length > 0;
            bool excludeFileNames = ExcludedFileNames != null && ExcludedFileNames.Length > 0;
            bool excludeFileExtensions = ExcludedExtensions != null && ExcludedExtensions.Length > 0;

            foreach (var path in TargetPaths)
            {
                var targetPath = PathHelpers.FixSlashes(path);

                // filter out excluded paths
                if (excludePaths && ExcludedPaths.Contains(targetPath))
                    continue;

                // filter out excluded file names
                if (excludeFileNames && ExcludedFileNames.Any(fileName => Path.GetFileName(targetPath) == fileName))
                    continue;

                // filter out excluded file extensions
                if (excludeFileExtensions && ExcludedExtensions.Any(fileExtension => Path.GetExtension(targetPath) == fileExtension))
                    continue;

                // convert path to be relative to the project directory if required
                if (ConvertToRelative)
                    targetPath = PathHelpers.ConvertToAssetsPath(targetPath);

                filteredPaths.Add(targetPath);
            }

            ResolvedPaths = filteredPaths;
            NumberOfResolvedPaths = ResolvedPaths.Count;

            return true;
        }
    }
}