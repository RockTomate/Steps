using System;
using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Build Player", "Builds a Project to an executable.", StepCategories.BuildingCategory)]
    public class BuildPlayerStep : SimpleStep
    {
        private const string BuildReportCategory = "Build Report";

        [InputField(tooltip: "The path where the application will be built", required: true)]
        public string OutputPath;

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
        public bool BuildScriptsOnly = false;

        [InputField]
        public bool BuildAdditionalStreamedScenes = false;

        [InputField]
        public bool ForceEnableAssertions = false;

        [InputField("Don't Compress Asset Bundle")]
        public bool DontCompressAssetBundles = false;

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
        [OutputField("Build Time", "Time taken (in milliseconds) for the build to complete", category: BuildReportCategory)]
        public double BuildTimeMs;
#else
        [NonSerialized]
        [OutputField(tooltip: "Report generated after the build.", category: BuildReportCategory)]
        public string BuildReport;
#endif

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            // set settings
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = BuildTarget;
            buildPlayerOptions.scenes = ScenePaths;
            buildPlayerOptions.locationPathName = OutputPath;
            buildPlayerOptions.assetBundleManifestPath = AssetBundleManifestPath;

#if UNITY_5_6_OR_NEWER
            buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(BuildTarget);
#endif

            // set build options
            var options = BuildOptions.None;

            if (DevelopmentBuild)
                options |= BuildOptions.Development;

            if (AllowDebugging)
                options |= BuildOptions.AllowDebugging;

            if (BuildScriptsOnly)
                options |= BuildOptions.BuildScriptsOnly;

            if (BuildAdditionalStreamedScenes)
                options |= BuildOptions.BuildAdditionalStreamedScenes;

            if (ForceEnableAssertions)
                options |= BuildOptions.ForceEnableAssertions;

            if (DontCompressAssetBundles)
                options |= BuildOptions.UncompressedAssetBundle;

            buildPlayerOptions.options = options;

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