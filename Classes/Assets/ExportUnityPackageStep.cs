using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Export UnityPackage", "Exports assets into a UnityPackage file.", StepCategories.AssetsCategory)]
    public class ExportUnityPackageStep : SimpleStep
    {
        [InputField(tooltip: "File path where .unitypackage file will be saved to.", required: true)]
        public string FilePath;

        [InputField(tooltip: "Paths to assets which will be included.", required: true)]
        public string[] AssetPaths;

        [InputField(tooltip: "Will recurse through any subdirectories listed and include all assets inside them.")]
        public bool ExportRecursively = true;

        [InputField(tooltip: "In addition to the assets paths listed, all dependent assets will be included as well.")]
        public bool IncludeDependencies = true;

        [InputField(tooltip: "The exported package will include all library assets, ie. the project settings located in the Library folder of the project.")]
        public bool IncludeLibraryAssets = false;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var exportOptions = ExportPackageOptions.Default;

            if (ExportRecursively)
                exportOptions |= ExportPackageOptions.Recurse;

            if (IncludeDependencies)
                exportOptions |= ExportPackageOptions.IncludeDependencies;

            if (IncludeLibraryAssets)
                exportOptions |= ExportPackageOptions.IncludeLibraryAssets;

            AssetDatabase.ExportPackage(AssetPaths, FilePath, exportOptions);
            return true;
        }
    }
}