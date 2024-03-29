﻿using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Run Executable", "Run any executable and pass arguments to it", StepCategories.ExternalCategory)]
    public class RunExecutableStep : Step
    {
        private Process _process;
        private float _currentTimeLeftMs;
        private DateTime _prevTimeSinceStartup;

        [InputField(tooltip: "File path or domain to an executable file.", required: true)]
        public string ExecutableFilePath = string.Empty;

        [InputField(tooltip: "Work directory of an executable")]
        public string WorkDirectory = string.Empty;

        [InputField(tooltip: "Executable arguments")]
        public string[] Arguments;

        [InputField(category: "Executable Settings")]
        public bool UseShellExecute = false;

        [InputField(category: "Executable Settings")]
        public ProcessWindowStyle WindowStyle = ProcessWindowStyle.Normal;

        [InputField(tooltip: "How long to wait (in milliseconds) for the process to finish and exit. Value below 0 means it will wait indefinitely.", category: "Executable Settings")]
        public int ExitTimeout = -1;

        [InputField(tooltip: "Should the process be killed if the timeout period has been exceeded?", category: "Executable Settings")]
        public bool KillOnTimeout = false;

        [OutputField(tooltip: "Exit code of the executable.")]
        public int ExitCode;

        [OutputField(tooltip: "Textual output of the running process (if any). Will be ignored if " + nameof(UseShellExecute) + " is enabled.")]
        public string StandardOutput = string.Empty;

        [OutputField(tooltip: "Error output of the running process (if any). Will be ignored if " + nameof(UseShellExecute) + " is enabled.")]
        public string ErrorOutput = string.Empty;

        /// <inheritdoc />
        protected override void OnReset()
        {
            _process = null;
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!File.Exists(ExecutableFilePath))
            {
                RockLog.WriteLine(this, LogTier.Error, string.Format("Executable not found at: \"{0}\"", ExecutableFilePath));
                return false;
            }

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
            var redirectOutput = !UseShellExecute;

            var startInfo = new ProcessStartInfo
            {
                FileName = ExecutableFilePath,
                Arguments = string.Join(" ", Arguments),
                UseShellExecute = UseShellExecute,
                WindowStyle = WindowStyle,
                WorkingDirectory = WorkDirectory,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput
            };

            _process = new Process
            {
                StartInfo = startInfo
            };

            RockLog.WriteLine(this, LogTier.Info, string.Format("Running: \"{0}\" {1}", ExecutableFilePath, startInfo.Arguments));

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
