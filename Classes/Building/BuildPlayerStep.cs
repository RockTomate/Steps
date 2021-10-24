using System;
using System.IO;
using UnityEditor;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Build Player", "Builds a Project to an executable.", StepCategories.BuildingCategory)]
    public class BuildPlayerStep : SimpleStep
    {
        private const string BuildReportCategory = "Build Report";
        private const string DefaultFileName = "output";

        [InputField(tooltip: "The path where the application will be built.\n" +
                             "If given without extension, then this will act as a folder directory and will rely on " +
                             "a \'File Name\' field instead.", required: true)]
        public string OutputPath;

        [InputField(tooltip: "Name of the output file. This option will be used depending your \"Build Target\". Do not include file extension.")]
        public string FileName = DefaultFileName;

        [InputField(tooltip: "The scenes to be included in the build. If empty, the currently open scene will be built. Paths are relative to the project folder (Assets/MyLevels/MyScene.unity)")]
        public string[] ScenePaths = new string[0];

        [InputField(tooltip: "The path to an manifest file describing all of the asset bundles used in the build")]
        public string AssetBundleManifestPath;

        [InputField(tooltip: "The platform to build")]
        public BuildTarget BuildTarget = BuildTarget.NoTarget;

        [InputField]
        public bool DevelopmentBuild = false;

        [InputField]
        public bool AllowDebugging = false;

        [InputField]
        public bool AutoRun = false;

        [InputField]
        public bool BuildScriptsOnly = false;

        [InputField]
        public bool BuildAdditionalStreamedScenes = false;

        [InputField]
        public bool ForceEnableAssertions = false;

        [InputField("Don't Compress Asset Bundle")]
        public bool DontCompressAssetBundles = false;

#if UNITY_2018_1_OR_NEWER
        [InputField(tooltip: "If enabled, manually cancelling the build via dialogue popup will consider the entire Step " +
                             "as \"failed\", stopping the entire Job in the process (enabled by default)\n" +
                             "If disabled, the step will be considered \"successful\" and the Job will continue on.")]
        public bool ManualCancelFailsStep = true;
#endif

#if UNITY_2018_1_OR_NEWER
        [NonSerialized]
        [OutputField(tooltip: "Result of the build", category: BuildReportCategory)]
        public BuildResult BuildResult;

        [NonSerialized]
        [OutputField(tooltip: "Time when build has started", category: BuildReportCategory)]
        public DateTime BuildStart;

        [NonSerialized]
        [OutputField(tooltip: "Time when build has ended", category: BuildReportCategory)]
        public DateTime BuildEnd;

        [NonSerialized]
        [OutputField(tooltip: "The GUID of the build", category: BuildReportCategory)]
        public GUID BuildGuidId;

        [NonSerialized]
        [OutputField(tooltip: "Number of errors occurred during the build", category: BuildReportCategory)]
        public int BuildErrorCount;

        [NonSerialized]
        [OutputField(tooltip: "Number of warnings occurred during the build", category: BuildReportCategory)]
        public int BuildWarningsCount;

        [NonSerialized]
        [OutputField(tooltip: "Total size of the build", category: BuildReportCategory)]
        public ulong BuildSize;

        [NonSerialized]
        [OutputField(name: "Build Time", tooltip: "Time taken (in milliseconds) for the build to complete", category: BuildReportCategory)]
        public double BuildTimeMs;
#else
        [NonSerialized]
        [OutputField(tooltip: "Report generated after the build.", category: BuildReportCategory)]
        public string BuildReport;
#endif

        private static string GetBuildLocationPath(BuildTarget buildTarget,
            BuildTargetGroup buildTargetGroup,
            BuildOptions options,
            string outputFolder,
            string outputFileName)
        {
            var fileExtension = buildTarget.GetExtension(buildTargetGroup, options);
            var fileName = outputFileName.IsNullOrWhiteSpace() ? DefaultFileName : outputFileName;

            // check if the output folder is actually a file path (for backwards compatibility)
            if (Path.HasExtension(outputFolder))
                return outputFolder;

            // if retrieved file extension is empty then that means the output is a folder
            return !fileExtension.IsNullOrWhiteSpace()
                ? PathHelpers.Combine(outputFolder, string.Format("{0}.{1}", fileName, fileExtension))
                : outputFolder;
        }

        private BuildOptions CreateBuildOptions()
        {
            var options = BuildOptions.None;

            if (DevelopmentBuild)
                options |= BuildOptions.Development;

            if (AllowDebugging)
                options |= BuildOptions.AllowDebugging;

            if (AutoRun)
                options |= BuildOptions.AutoRunPlayer;

            if (BuildScriptsOnly)
                options |= BuildOptions.BuildScriptsOnly;

            if (BuildAdditionalStreamedScenes)
                options |= BuildOptions.BuildAdditionalStreamedScenes;

            if (ForceEnableAssertions)
                options |= BuildOptions.ForceEnableAssertions;

            if (DontCompressAssetBundles)
                options |= BuildOptions.UncompressedAssetBundle;

            return options;
        }

        protected override bool OnValidate()
        {
            if (!BuildTarget.IsBuildTargetSupported())
            {
                RockLog.WriteLine(LogTier.Error,
                    string.Format("Build target \"{0}\" is not supported by this Unity engine. " +
                                  "This might be because module for this build target hasn't been installed.", BuildTarget));
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(BuildTarget);
            var buildOptions = CreateBuildOptions();

            var buildPlayerOptions = new BuildPlayerOptions
            {
                options = buildOptions,
                target = BuildTarget,
                scenes = ScenePaths,
                assetBundleManifestPath = AssetBundleManifestPath,
                targetGroup = buildTargetGroup,
                locationPathName = GetBuildLocationPath(BuildTarget, buildTargetGroup, buildOptions, OutputPath, FileName)
            };

            // make a build
#if UNITY_2018_1_OR_NEWER
            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var buildSummary = buildReport.summary;

            BuildResult = buildSummary.result;
            BuildStart = buildSummary.buildStartedAt;
            BuildEnd = buildSummary.buildEndedAt;
            BuildGuidId = buildSummary.guid;
            BuildErrorCount = buildSummary.totalErrors;
            BuildWarningsCount = buildSummary.totalWarnings;
            BuildSize = buildSummary.totalSize;
            BuildTimeMs = buildSummary.totalTime.TotalMilliseconds;

            if (ManualCancelFailsStep && buildReport.summary.result == BuildResult.Cancelled)
                return false;

            return buildReport.summary.result == BuildResult.Succeeded;
#else
            BuildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            return BuildReport == "True";
#endif
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Building..."; }
        }
    }
}