using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Run External Job", "Runs Job from another Unity project", StepCategories.ExternalCategory)]
    public class RunExternalJobStep : Step
    {
        private Process _process;
        private float _currentTimeLeftMs;
        private DateTime _prevTimeSinceStartup;

        [InputField(tooltip: "Path to a unity executable that will be run in batch mode", required: true)]
        public string UnityExePath;

        [InputField(tooltip: "Path to a project which has a job that will be executed", required: true)]
        public string ProjectPath;

        [InputField(tooltip: "File path to a job that will be executed (relative to target project path)", required: true)]
        public string LocalJobPath;

        [InputField(tooltip: "Arguments that will be passed to a job execution (do not include dash)", category: "Arguments")]
        public string[] JobArguments;

        [InputField(tooltip: "Additional arguments for Unity executable (must contain dashes to work)", category: "Arguments")]
        public string[] OtherArguments;

        [InputField (category: "Executable Settings")]
        public bool UseShellExecute = false;

        [InputField(category: "Executable Settings")]
        public ProcessWindowStyle WindowStyle = ProcessWindowStyle.Normal;

        [InputField(tooltip: "How long to wait (in milliseconds) for the process to finish and exit. Value below 0 means it will wait indefinitely.", category: "Executable Settings")]
        public int ExitTimeout = -1;

        [InputField(tooltip: "Should the process be killed if the timeout period has been exceeded?", category: "Executable Settings")]
        public bool KillOnTimeout = false;

        [OutputField]
        public int ExitCode;

        /// <inheritdoc />
        protected override void OnReset()
        {
            _process = null;
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!File.Exists(UnityExePath))
                return false;

            if (!Directory.Exists(ProjectPath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            _process.Kill();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            var otherArguments = OtherArguments != null && OtherArguments.Length > 0
                ? string.Join(" ", OtherArguments)
                : string.Empty;

            var jobArguments = OtherArguments != null && JobArguments.Length > 0
                ? string.Join(" ", JobArguments)
                : string.Empty;

            var processArguments = string.Format("-batchmode {0} -projectPath \"{1}\" -executeMethod \"{2}\" \"{3}\" {4}",
                otherArguments, ProjectPath, CLI.ExternalMethodName, LocalJobPath, jobArguments);

            var startInfo = new ProcessStartInfo
            {
                FileName = UnityExePath,
                Arguments = processArguments,
                UseShellExecute = UseShellExecute,
                WindowStyle = WindowStyle,
            };

            _process = new Process
            {
                StartInfo = startInfo
            };

            RockLog.WriteLine(this, LogTier.Debug, string.Format("Running: \"{0}\" {1}", UnityExePath, processArguments));

            ResetTimeout();

            if (!_process.Start())
            {
                IsSuccess = false;
                yield break;
            }

            // continuously check if the process has exited or timeout has been reached
            while (!_process.HasExited && _currentTimeLeftMs > 0)
            {
                UpdateTimeoutTimeLeft();
                yield return null;
            }

            // if the timeout has been reached and the process hasn't exited
            if (!_process.HasExited && _currentTimeLeftMs <= 0)
            {
                if (KillOnTimeout)
                    _process.Kill();

                ExitCode = -1;
                IsSuccess = false;
                yield break;
            }

            ExitCode = _process.ExitCode;
            IsSuccess = ExitCode == 0;
        }

        /// <summary>
        /// Resets the timeout timer
        /// </summary>
        private void ResetTimeout()
        {
            _currentTimeLeftMs = ExitTimeout > 0
                ? ExitTimeout
                : int.MaxValue;

            _prevTimeSinceStartup = DateTime.Now;
        }

        /// <summary>
        /// Updates remaining time left for timeout
        /// </summary>
        private void UpdateTimeoutTimeLeft()
        {
            var deltaTime = (float)DateTime.Now.Subtract(_prevTimeSinceStartup).TotalMilliseconds;
            _prevTimeSinceStartup = DateTime.Now;

            _currentTimeLeftMs -= deltaTime;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Running"; }
        }
    }
}