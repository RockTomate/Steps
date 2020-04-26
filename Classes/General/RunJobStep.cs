﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [Serializable]
    [StepDescription("Run Job", "Runs a local Job")]
    public class RunJobStep : Step
    {
        private const string JobFilePathTip = "Job that will be executed.";

        [SerializeField]
        private Job _targetJob;

        [InputField(tooltip: JobFilePathTip, required: true)]
        public Job TargetJob
        {
            get { return _targetJob; }
            set { _targetJob = value; }
        }

        /// <summary>
        /// Represents target job variables that will be modified.
        /// </summary>
        public Dictionary<string, BaseField> TargetJobVariables = new Dictionary<string, BaseField>();

        /// <summary>
        /// Represents variables that will be added to the target job.
        /// </summary>
        public Dictionary<string, BaseField> NewVariables = new Dictionary<string, BaseField>();

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (TargetJob == null)
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool EvaluateInputFieldValues(JobContext context)
        {
            if (!base.EvaluateInputFieldValues(context))
                return false;

            // evaluate values of new variables
            foreach (var newVariable in NewVariables)
            {
                newVariable.Value.GetValue(context);
            }

            // evaluate values of job variables
            foreach (var targetJobVariable in TargetJobVariables)
            {
                targetJobVariable.Value.GetValue(context);
            }

            return true;
        }

        protected override IEnumerator OnExecute(JobContext context)
        {
            if (TargetJob.Id.Equals(context.JobId))
            {
                RockLog.WriteLine(this, LogTier.Error, "A currently running job cannot run itself!");
                IsSuccess = false;
                yield break;
            }

            var subSession = JobSession.Create(TargetJob, false);
            subSession.RootContext.Parent = context;

            // modify existing job variables
            foreach (var keyValuePair in TargetJobVariables)
            {
                var customVariable = keyValuePair.Value;
                var jobVariable = subSession.RootContext[keyValuePair.Key];
                jobVariable.UseFormula = false;
                jobVariable.SetValue(customVariable.GetValue());
            }

            // add new variables
            foreach (var keyValuePair in NewVariables)
            {
                subSession.RootContext.Add(keyValuePair.Value);
            }

            subSession.Start();

            while (subSession.InProgress)
                yield return null;

            IsSuccess = subSession.IsSuccess;
        }
    }
}