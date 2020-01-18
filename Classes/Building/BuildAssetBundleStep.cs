using System;
using System.IO;
using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Build Asset Bundle", "Builds all asset bundles specified in the editor.", StepCategories.BuildingCategory)]
    public class BuildAssetBundleStep : SimpleStep
    {
        private const string AssetBundleOptionsCategory = "Options";

        [InputField(tooltip: "Output path for the AssetBundles.", required: true)]
        public string OutputPath;

        [InputField(tooltip: "Chosen target build platform.")]
        public BuildTarget BuildTarget = BuildTarget.NoTarget;

        [InputField(tooltip: "Compression type that will be used when building asset bundles." +
                             "\nUncompressed - Don't compress the data when creating the asset bundle." +
                             "\nChunk Based - Use chunk-based LZ4 compression when creating the AssetBundle.", category: AssetBundleOptionsCategory)]
        public CompressionType Compression = CompressionType.Uncompressed;

        [InputField(tooltip: "Append the hash to the assetBundle name.", category: AssetBundleOptionsCategory)]
        public bool AppendHash = false;

        [InputField(tooltip: "Force rebuild the assetBundles.", category: AssetBundleOptionsCategory)]
        public bool ForceRebuild = false;

        [InputField("Deterministic", "Builds an asset bundle using a hash for the id of the object stored in the asset bundle.", category: AssetBundleOptionsCategory)]
        public bool DeterministicAssetBundle = false;

        [InputField(tooltip: "Do a dry run build.", category: AssetBundleOptionsCategory)]
        public bool DryRunBuild = false;

        [InputField(tooltip: "Do not allow the build to succeed if any errors are reporting during it.", category: AssetBundleOptionsCategory)]
        public bool StrictMode = false;

        [InputField(tooltip: "Enable Asset Bundle LoadAsset by file name.", category: AssetBundleOptionsCategory)]
        public bool LoadAssetByFileName = true;

        [InputField(tooltip: "Enable Asset Bundle LoadAsset by file name with extension.", category: AssetBundleOptionsCategory)]
        public bool LoadAssetByFileNameWithExtension = true;

        [InputField(tooltip: "Whether or not to include type information within the AssetBundle.", category: AssetBundleOptionsCategory)]
        public bool EnableWriteTypeTree = true;

        [InputField(tooltip: "Ignore the type tree changes when doing the incremental build check.", category: AssetBundleOptionsCategory)]
        public bool IgnoreTypeTreeChanges = false;

        public enum CompressionType
        {
            Uncompressed,
            ChunkBased
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (File.Exists(OutputPath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            // if the directory doesn't exist, then create one
            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            var buildOptions = BuildAssetBundleOptions.None;

            if (AppendHash)
                buildOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;

            if (ForceRebuild)
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

            if (DeterministicAssetBundle)
                buildOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;

            if (DryRunBuild)
                buildOptions |= BuildAssetBundleOptions.DryRunBuild;

            if (StrictMode)
                buildOptions |= BuildAssetBundleOptions.StrictMode;

            if (!EnableWriteTypeTree)
                buildOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;

            if (IgnoreTypeTreeChanges)
                buildOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;

            if (!LoadAssetByFileName)
                buildOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;

            if (!LoadAssetByFileNameWithExtension)
                buildOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

            switch (Compression)
            {
                case CompressionType.Uncompressed:

                    buildOptions |= BuildAssetBundleOptions.ChunkBasedCompression;

                    break;
                case CompressionType.ChunkBased:

                    buildOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // build an asset bundle
            return BuildPipeline.BuildAssetBundles(OutputPath, buildOptions, BuildTarget);
        }
    }
}