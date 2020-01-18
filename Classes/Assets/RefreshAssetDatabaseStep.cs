using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Refresh Asset Database", "Synchronously imports any changed assets into the project", StepCategories.AssetsCategory)]
    public class RefreshAssetDatabaseStep : SimpleStep
    {
        [InputField(tooltip: "User initiated asset import.")]
        public bool ForceUpdate = false;

        [InputField(tooltip: "When a folder is imported, import all its contents as well.")]
        public bool ImportRecursive = false;

        [InputField(tooltip: "Force a full reimport and download the assets from the cache server.")]
        public bool DownloadFromCacheServer = true;

        [InputField(tooltip: "Forces asset import as uncompressed for edition facilities.")]
        public bool ForceUncompressedImport = false;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var importOptions = ImportAssetOptions.Default | ImportAssetOptions.ForceSynchronousImport;

            if (ForceUpdate)
                importOptions |= ImportAssetOptions.ForceUpdate;

            if (ImportRecursive)
                importOptions |= ImportAssetOptions.ImportRecursive;

            if (!DownloadFromCacheServer)
                importOptions |= ImportAssetOptions.DontDownloadFromCacheServer;

            if (ForceUncompressedImport)
                importOptions |= ImportAssetOptions.ForceUncompressedImport;

            AssetDatabase.Refresh(importOptions);
            return true;
        }
    }
}